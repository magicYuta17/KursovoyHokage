
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Kursivoy_Konkin
{
    // Форма для просмотра и управления статусами клиентов (доступна руководителю)
    public partial class FormHeadViewStatus : Form
    {
        // Строка подключения к базе данных
        private readonly string ConnectionString = connect.con;
        // Контекстное меню для DataGridView
        private ContextMenuStrip _ctx;
        // Множество для хранения ID скрытых статусов (временно)
        private HashSet<int> _hiddenStatusIds = new HashSet<int>();

        // Конструктор формы
        public FormHeadViewStatus()
        {
            InitializeComponent(); // Инициализация компонентов дизайнера
            InitializeUi(); // Дополнительная инициализация интерфейса
            this.MinimizeBox = false; // Запрет на сворачивание
            this.MaximizeBox = false; // Запрет на разворачивание
          
        }

        // Метод для настройки пользовательского интерфейса
        private void InitializeUi()
        {
            // Настройка DataGridView
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // Выделение всей строки
            dataGridView1.MultiSelect = false; // Запрет множественного выбора
            dataGridView1.ReadOnly = true; // Только для чтения
            dataGridView1.AllowUserToAddRows = false; // Запрет добавления строк пользователем
            dataGridView1.MouseDown += DataGridView1_MouseDown; // Подписка на событие нажатия мыши

            // Создание контекстного меню
            _ctx = new ContextMenuStrip();

            // Создание пунктов меню
            var addItem = new ToolStripMenuItem("Добавить статус") { Name = "AddStatus" };
            var editItem = new ToolStripMenuItem("Редактировать") { Name = "EditStatus" };
            var deleteItem = new ToolStripMenuItem("Удалить статус") { Name = "DeleteStatus" };

            // Подписка на события кликов по пунктам меню
            addItem.Click += ButtonAddStatus_Click;
            editItem.Click += EditItem_Click;
            deleteItem.Click += DeleteItem_Click;

            // Добавление пунктов в контекстное меню
            _ctx.Items.Add(addItem);
            _ctx.Items.Add(editItem);
            _ctx.Items.Add(deleteItem);

            // Привязка контекстного меню к DataGridView
            dataGridView1.ContextMenuStrip = _ctx;

            // Подписка на событие загрузки формы
            Load += FormHeadViewStatus_Load;
        }

        // Обработчик загрузки формы
        private void FormHeadViewStatus_Load(object sender, EventArgs e)
        {
            // Дублирование настроек окна (на всякий случай)
            this.MinimizeBox = false;
            this.MaximizeBox = false;
           

            // Заполнение таблицы статусов
            FillStatusGrid();
        }

        // Метод для заполнения DataGridView списком статусов
        private void FillStatusGrid()
        {
            try
            {
                var dt = new DataTable(); // Создание таблицы данных
                using (var conn = new MySqlConnection(ConnectionString))
                using (var cmd = new MySqlCommand(
                    "SELECT ID_Status_client, status FROM mydb.status_client WHERE IsDeleted = 0 ORDER BY ID_Status_client", conn))
                using (var da = new MySqlDataAdapter(cmd))
                {
                    conn.Open(); // Открытие соединения
                    da.Fill(dt); // Заполнение DataTable данными из БД
                }

                // Если есть скрытые статусы, фильтруем их
                if (_hiddenStatusIds.Any())
                {
                    var rowsToKeep = dt.AsEnumerable()
                        .Where(r => !_hiddenStatusIds.Contains(Convert.ToInt32(r["ID_Status_client"])))
                        .CopyToDataTableOrEmpty(); // Оставляем только нескрытые статусы
                    dataGridView1.DataSource = rowsToKeep; // Устанавливаем отфильтрованные данные
                }
                else
                {
                    dataGridView1.DataSource = dt; // Устанавливаем все данные
                }

                // Скрываем колонку ID (она нужна для работы, но не должна отображаться)
                if (dataGridView1.Columns.Contains("ID_Status_client"))
                    dataGridView1.Columns["ID_Status_client"].Visible = false;

                // Настройка отображения колонки статуса
                if (dataGridView1.Columns.Contains("status"))
                {
                    dataGridView1.Columns["status"].HeaderText = "Статус"; // Заголовок колонки
                    dataGridView1.Columns["status"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; // Автоматическая ширина
                }
            }
            catch (Exception ex)
            {
                // Обработка ошибок при загрузке
                MessageBox.Show("Ошибка при загрузке статусов: " + ex.Message,
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обработчик кнопки "Добавить статус"
        private void ButtonAddStatus_Click(object sender, EventArgs e)
        {
            // Вызов диалогового окна для ввода названия статуса
            string newStatus = Prompt.ShowDialog("Введите название статуса:", "Добавить статус", "", allowAnyInput: true);
            if (newStatus == null) return; // Отмена
            newStatus = newStatus.Trim(); // Удаление лишних пробелов
            if (string.IsNullOrEmpty(newStatus))
            {
                MessageBox.Show("Название статуса не может быть пустым.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Добавление нового статуса в БД
                using (var conn = new MySqlConnection(ConnectionString))
                using (var cmd = new MySqlCommand(
                    "INSERT INTO mydb.status_client (status) VALUES (@status)", conn))
                {
                    cmd.Parameters.AddWithValue("@status", newStatus); // Передаем название статуса
                    conn.Open(); // Открываем соединение
                    cmd.ExecuteNonQuery(); // Выполняем запрос на вставку
                }
                FillStatusGrid(); // Обновляем таблицу
            }
            catch (MySqlException mex)
            {
                MessageBox.Show("Ошибка БД при добавлении статуса: " + mex.Message,
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении статуса: " + ex.Message,
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обработчик нажатия мыши на DataGridView (для контекстного меню)
        private void DataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) // Если нажата правая кнопка мыши
            {
                var hit = dataGridView1.HitTest(e.X, e.Y); // Определяем, куда кликнули
                if (hit.RowIndex >= 0) // Если кликнули на строку
                {
                    dataGridView1.ClearSelection(); // Снимаем выделение
                    var row = dataGridView1.Rows[hit.RowIndex]; // Получаем строку
                    row.Selected = true; // Выделяем строку
                    dataGridView1.CurrentCell = row.Cells[Math.Max(0, hit.ColumnIndex)]; // Устанавливаем текущую ячейку
                }
                else // Если кликнули на пустую область
                {
                    dataGridView1.ClearSelection(); // Снимаем выделение
                    _ctx.Close(); // Закрываем контекстное меню
                }
            }
        }

        // ★ ИЗМЕНЁННЫЙ МЕТОД — после переименования статуса обновляем его везде в БД ★
        private void EditItem_Click(object sender, EventArgs e)
        {
            // Проверка, выбрана ли строка
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите статус для редактирования.", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var row = dataGridView1.SelectedRows[0]; // Получаем выбранную строку
            if (row.Cells["ID_Status_client"].Value == null)
            {
                MessageBox.Show("Не найден идентификатор статуса.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int id = Convert.ToInt32(row.Cells["ID_Status_client"].Value); // Получаем ID статуса
            string current = row.Cells["status"].Value?.ToString() ?? string.Empty; // Получаем текущее название

            // Вызов диалога для редактирования
            string edited = Prompt.ShowDialog("Отредактируйте название статуса:",
                "Редактирование статуса", current, allowAnyInput: true);
            if (edited == null) return; // Отмена
            edited = edited.Trim(); // Удаляем пробелы
            if (string.IsNullOrEmpty(edited))
            {
                MessageBox.Show("Название статуса не может быть пустым.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Если ничего не менялось — выходим
            if (edited == current)
            {
                MessageBox.Show("Название статуса не изменилось.", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();

                    // Считаем, сколько клиентов используют этот статус
                    long usedCount = 0;
                    using (var cmdCount = new MySqlCommand(
                        "SELECT COUNT(*) FROM mydb.clients WHERE Status_client_ID_Status_client = @id", conn))
                    {
                        cmdCount.Parameters.AddWithValue("@id", id);
                        usedCount = Convert.ToInt64(cmdCount.ExecuteScalar());
                    }

                    // Спрашиваем подтверждение с информацией о затронутых клиентах
                    string confirmMsg = usedCount > 0
                        ? $"Статус \"{current}\" используется у {usedCount} клиента(ов).\n" +
                          $"Переименовать статус на \"{edited}\" везде в системе?"
                        : $"Переименовать статус \"{current}\" на \"{edited}\"?";

                    var confirm = MessageBox.Show(confirmMsg, "Подтвердите изменение",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (confirm != DialogResult.Yes) return;

                    // ШАГ 1: Обновляем название статуса в таблице status_client
                    using (var cmdUpdate = new MySqlCommand(
                        "UPDATE mydb.status_client SET status = @status WHERE ID_Status_client = @id", conn))
                    {
                        cmdUpdate.Parameters.AddWithValue("@status", edited);
                        cmdUpdate.Parameters.AddWithValue("@id", id);
                        cmdUpdate.ExecuteNonQuery();
                    }

                    // ШАГ 2: Так как клиенты связаны по ID (внешний ключ),
                    // то они автоматически видят новое название через JOIN.
                    // Но если в таблице clients хранится текстовая копия статуса —
                    // обновляем и её тоже:
                    // (раскомментируй, если в таблице clients есть текстовое поле статуса)
                    /*
                    using (var cmdClients = new MySqlCommand(
                        "UPDATE mydb.clients SET status_text = @status " +
                        "WHERE Status_client_ID_Status_client = @id", conn))
                    {
                        cmdClients.Parameters.AddWithValue("@status", edited);
                        cmdClients.Parameters.AddWithValue("@id", id);
                        cmdClients.ExecuteNonQuery();
                    }
                    */

                    // Итоговое сообщение
                    string resultMsg = usedCount > 0
                        ? $"Статус успешно переименован.\nОбновлено клиентов: {usedCount}."
                        : "Статус успешно переименован.";

                    MessageBox.Show(resultMsg, "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    FillStatusGrid(); // Обновляем таблицу
                }
            }
            catch (MySqlException mex)
            {
                MessageBox.Show("Ошибка БД при сохранении статуса: " + mex.Message,
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении статуса: " + ex.Message,
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обработчик удаления статуса
        private void DeleteItem_Click(object sender, EventArgs e)
        {
            // Проверка, выбрана ли строка
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите статус для удаления.", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var row = dataGridView1.SelectedRows[0]; // Получаем выбранную строку
            if (row.Cells["ID_Status_client"].Value == null)
            {
                MessageBox.Show("Не найден идентификатор статуса.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int id = Convert.ToInt32(row.Cells["ID_Status_client"].Value); // Получаем ID статуса
            string name = row.Cells["status"].Value?.ToString() ?? $"ID {id}"; // Получаем название

            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();

                    // Проверка, используется ли статус клиентами
                    long usedCount = 0;
                    using (var cmdCheck = new MySqlCommand(
                        "SELECT COUNT(*) FROM mydb.clients WHERE Status_client_ID_Status_client = @id AND IsDeleted = 0", conn))
                    {
                        cmdCheck.Parameters.AddWithValue("@id", id);
                        usedCount = Convert.ToInt64(cmdCheck.ExecuteScalar());
                    }

                    // Запрос подтверждения с информацией об использовании
                    string confirmMessage = usedCount > 0
                        ? $"Статус \"{name}\" используется у {usedCount} клиента(ов).\nВсё равно удалить статус?"
                        : $"Удалить статус \"{name}\"?";

                    var conf = MessageBox.Show(confirmMessage, "Подтвердите удаление",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (conf != DialogResult.Yes) return;

                    // Мягкое удаление (установка флага IsDeleted = 1 вместо физического удаления)
                    using (var cmdDel = new MySqlCommand(
                        "UPDATE mydb.status_client SET IsDeleted = 1 WHERE ID_Status_client = @id", conn))
                    {
                        cmdDel.Parameters.AddWithValue("@id", id);
                        int affected = cmdDel.ExecuteNonQuery(); // Выполняем запрос
                        if (affected > 0)
                        {
                            MessageBox.Show("Статус успешно удалён.", "Успех",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            FillStatusGrid(); // Обновляем таблицу
                        }
                        else
                        {
                            MessageBox.Show("Статус не найден или уже удалён.", "Информация",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (MySqlException mex)
            {
                MessageBox.Show("Ошибка БД при удалении статуса: " + mex.Message,
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при удалении статуса: " + ex.Message,
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Внутренний статический класс для отображения диалогового окна ввода
        private static class Prompt
        {
            public static string ShowDialog(string text, string caption,
                string defaultText, bool allowAnyInput = false)
            {
                using (Form prompt = new Form())
                {
                    // Настройка формы диалога
                    prompt.Width = 500;
                    prompt.Height = 170;
                    prompt.FormBorderStyle = FormBorderStyle.FixedDialog;
                    prompt.Text = caption;
                    prompt.StartPosition = FormStartPosition.CenterParent;
                    prompt.MinimizeBox = false;
                    prompt.MaximizeBox = false;
                    prompt.ShowInTaskbar = false;

                    // Создание метки с текстом
                    Label textLabel = new Label() { Left = 20, Top = 10, Width = 440, Text = text };
                    // Создание поля ввода
                    TextBox textBox = new TextBox() { Left = 20, Top = 40, Width = 440 };
                    textBox.Text = defaultText ?? string.Empty;
                    textBox.SelectAll(); // Выделение всего текста по умолчанию

                    // Кнопка OK
                    Button confirmation = new Button()
                    { Text = "OK", Left = 280, Width = 80, Top = 75, DialogResult = DialogResult.OK };
                    // Кнопка Отмена
                    Button cancel = new Button()
                    { Text = "Отмена", Left = 370, Width = 80, Top = 75, DialogResult = DialogResult.Cancel };

                    confirmation.Font = cancel.Font = new Font("Microsoft Sans Serif", 10);

                    // Добавление элементов управления на форму
                    prompt.Controls.Add(textBox);
                    prompt.Controls.Add(confirmation);
                    prompt.Controls.Add(cancel);
                    prompt.Controls.Add(textLabel);
                    prompt.AcceptButton = confirmation; // Нажатие Enter = OK
                    prompt.CancelButton = cancel; // Нажатие Esc = Отмена

                    var result = prompt.ShowDialog(); // Показываем диалог
                    return result == DialogResult.OK ? textBox.Text : null; // Возвращаем введенный текст или null
                }
            }
        }

        // Обработчик кнопки "Назад"
        private void button1_Click(object sender, EventArgs e)
        {
            FormHeadNavigation f = new FormHeadNavigation(); // Создаем форму навигации
            this.Visible = false; // Скрываем текущую форму
            f.ShowDialog(); // Показываем форму навигации
            this.Close(); // Закрываем текущую форму
        }

        private void FormHeadViewStatus_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true; //отменяем закрытие формы
            }
        }
    }

    // Вспомогательный класс-расширение для работы с DataTable
    internal static class DataTableExtensions
    {
        // Метод расширения для преобразования коллекции строк DataRow в DataTable
        public static DataTable CopyToDataTableOrEmpty(this IEnumerable<DataRow> rows)
        {
            var enumerable = rows as DataRow[] ?? rows.ToArray(); // Преобразуем в массив
            if (!enumerable.Any()) // Если коллекция пуста
                return new DataTable(); // Возвращаем пустую таблицу
            return enumerable.CopyToDataTable(); // Иначе создаем таблицу из строк
        }
    }
}
