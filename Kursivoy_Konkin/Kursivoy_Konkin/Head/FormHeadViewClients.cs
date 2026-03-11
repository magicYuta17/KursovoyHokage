
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kursivoy_Konkin
{
    // Форма для просмотра списка клиентов (доступна руководителю)
    public partial class FormHeadViewClients : Form
    {
        // Хранит исходную таблицу данных для фильтрации и поиска
        private DataTable originalDataTable;

        // Конструктор формы
        public FormHeadViewClients()
        {
            InitializeComponent(); // Инициализация компонентов дизайнера
            InitializeContextMenu(); // Настройка контекстного меню
            InitializeSearchAndFilter(); // Настройка поиска и фильтрации
            this.MinimizeBox = false; // Запрет на сворачивание
            this.MaximizeBox = false; // Запрет на разворачивание
            this.ControlBox = false; // Скрытие системных кнопок
        }

        // Метод для инициализации контекстного меню
        private void InitializeContextMenu()
        {
            // Создаем контекстное меню
            contextMenuStrip1 = new ContextMenuStrip();

            // Создаем пункт меню "Печать отчета по LTV"
            var menuItemPrint = new ToolStripMenuItem("Печать отчета по LTV");
            menuItemPrint.Click += MenuItemPrint_Click; // Подписка на событие клика
            contextMenuStrip1.Items.Add(menuItemPrint); // Добавляем пункт в меню

            // Привязываем контекстное меню к dataGridView1
            dataGridView1.ContextMenuStrip = contextMenuStrip1;
        }

        // Обработчик пункта меню "Добавить" (закомментирован, но код сохранен)
        private void MenuItemAdd_Click(object sender, EventArgs e)
        {
            // Открываем форму добавления клиента
            using (var addClientForm = new FormManagerAddClient())
            {
                if (addClientForm.ShowDialog() == DialogResult.OK)
                {
                    // Обновляем таблицу после добавления клиента
                    FillTableData();
                }
            }
        }

        // Обработчик пункта меню "Печать отчета по LTV"
        private void MenuItemPrint_Click(object sender, EventArgs e)
        {
            // Проверяем, есть ли данные для печати
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("Нет данных для печати.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Формируем путь к шаблону Word для отчета
            string projectDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\"));
            string fileName = Path.Combine(projectDir, "docPrint", "printLTV.docx");
            if (!File.Exists(fileName)) // Проверяем существование файла
            {
                MessageBox.Show($"Шаблон не найден:\n{fileName}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Создаем экземпляр приложения Word
            var word = new Microsoft.Office.Interop.Word.Application();
            word.Visible = false; // Запускаем в фоновом режиме

            try
            {
                var wordDocument = word.Documents.Add(fileName); // Открываем шаблон

                // Заменяем заглушку с датой в шаблоне
                ReplaceWordStub("{Дата}", DateTime.Now.ToString("dd.MM.yyyy"), wordDocument);

                // Определяем столбцы, которые нужно исключить из отчета
                var excludedColumns = new[] { "Телефон", "Дата рождения", };

                // Получаем только видимые столбцы, исключая указанные
                var visibleColumns = dataGridView1.Columns
                    .Cast<DataGridViewColumn>()
                    .Where(c => c.Visible && !excludedColumns.Contains(c.HeaderText))
                    .ToList();

                int rowCount = dataGridView1.Rows.Count; // Количество строк
                int colCount = visibleColumns.Count; // Количество колонок

                // Переходим в конец документа
                var range = wordDocument.Content;
                range.Collapse(Microsoft.Office.Interop.Word.WdCollapseDirection.wdCollapseEnd);

                // Добавляем абзац перед таблицей
                range.InsertParagraphAfter();
                range.Collapse(Microsoft.Office.Interop.Word.WdCollapseDirection.wdCollapseEnd);

                // Создаём таблицу в Word (rowCount+1 для заголовка, colCount колонок)
                var wordTable = wordDocument.Tables.Add(range, rowCount + 1, colCount);
                wordTable.Borders.Enable = 1; // Включаем границы

                // Пытаемся применить стиль таблицы (если не найден - игнорируем)
                object tableStyle = "Сетка таблицы"; // русский Word
                try
                {
                    wordTable.set_Style(ref tableStyle);
                }
                catch
                {
                    // Если стиль не найден — просто оставляем границы без стиля
                }

                // Заполняем заголовки (первая строка таблицы)
                for (int col = 0; col < colCount; col++)
                {
                    var cell = wordTable.Cell(1, col + 1);
                    cell.Range.Text = visibleColumns[col].HeaderText; // Текст заголовка
                    cell.Range.Bold = 1; // Жирный шрифт
                    cell.Range.ParagraphFormat.Alignment = // Выравнивание по центру
                        Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                }

                // Заполняем строки данными из DataGridView
                for (int row = 0; row < rowCount; row++)
                {
                    for (int col = 0; col < colCount; col++)
                    {
                        // Получаем значение ячейки
                        var cellValue = dataGridView1.Rows[row].Cells[visibleColumns[col].Index].Value;
                        string text = cellValue != null && cellValue != DBNull.Value
                            ? cellValue.ToString()
                            : ""; // Пустая строка, если значение null

                        wordTable.Cell(row + 2, col + 1).Range.Text = text; // +2 т.к. первая строка - заголовок
                    }
                }

                // Устанавливаем размер шрифта для всех ячеек
                foreach (Microsoft.Office.Interop.Word.Row wordRow in wordTable.Rows)
                {
                    foreach (Microsoft.Office.Interop.Word.Cell cell in wordRow.Cells)
                    {
                        cell.Range.Font.Size = 9; // Размер шрифта 9
                    }
                }

                word.Visible = true; // Делаем Word видимым для пользователя
            }
            catch (Exception ex)
            {
                word.Quit(); // Закрываем Word в случае ошибки
                MessageBox.Show($"Ошибка при формировании отчёта: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Вспомогательный метод для замены заглушек в документе Word
        private void ReplaceWordStub(string stubToReplace, string text, Microsoft.Office.Interop.Word.Document wordDocument)
        {
            var range = wordDocument.Content; // Весь контент документа
            range.Find.ClearFormatting(); // Очищаем настройки поиска
            range.Find.Execute(
                FindText: stubToReplace, // Что ищем
                ReplaceWith: text, // На что заменяем
                Replace: Microsoft.Office.Interop.Word.WdReplace.wdReplaceAll // Заменяем все вхождения
            );
        }

        // Обработчик пункта меню "Редактировать"
        private void MenuItemEdit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0) // Проверяем, выбрана ли строка
            {
                // Получаем ID клиента из выбранной строки
                int clientId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID_Client"].Value);

                // Открываем форму редактирования клиента
                using (var editClientForm = new FormManagerEditClients())
                {
                    editClientForm.LoadClientById(clientId); // Загружаем данные клиента по ID
                    if (editClientForm.ShowDialog() == DialogResult.OK) // Если редактирование успешно
                    {
                        // Обновляем таблицу после редактирования клиента
                        FillTableData();
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите клиента для редактирования.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Обработчик пункта меню "Удалить"
        private void MenuItemDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0) // Проверяем, выбрана ли строка
            {
                // Получаем ID клиента из выбранной строки
                int clientId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID_Client"].Value);

                // Запрашиваем подтверждение удаления
                var result = MessageBox.Show("Вы уверены, что хотите удалить клиента?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes) // Если пользователь подтвердил
                {
                    try
                    {
                        // Мягкое удаление (установка флага IsDeleted = 1)
                        string query = "UPDATE mydb.clients SET IsDeleted = 1 WHERE ID_Client = @IDClient;";
                        using (var connection = new MySqlConnection(connect.con))
                        using (var command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@IDClient", clientId); // Передаем ID клиента
                            connection.Open(); // Открываем соединение
                            command.ExecuteNonQuery(); // Выполняем запрос
                        }

                        // Обновляем таблицу после удаления клиента
                        FillTableData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении клиента: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите клиента для удаления.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Метод для заполнения таблицы данными из БД
        private void FillTableData()
        {
            try
            {
                dataGridView1.Columns.Clear(); // Очищаем колонки

                // SQL-запрос с JOIN для получения полной информации о клиентах
                string query = @"
                   SELECT 
                        c.ID_Client,
                        c.FullName_client AS 'ФИО',
                        c.phone AS 'Телефон',
                        c.Birthday AS 'Дата рождения',
                        TIMESTAMPDIFF(YEAR, c.Birthday, CURDATE()) AS 'Возраст', -- Вычисляем возраст
                        s.status AS 'Статус',
                        c.LTV AS 'LTV',
                        w.FIO AS 'ФИО сотрудника'
                    FROM mydb.clients c
                    LEFT JOIN mydb.status_client s ON c.Status_client_ID_Status_client = s.ID_Status_client
                    LEFT JOIN mydb.worker w ON w.ID_Clientsl = c.ID_Client
                    WHERE c.IsDeleted = 0;"; // Только не удаленные клиенты

                using (var connection = new MySqlConnection(connect.con))
                using (var command = new MySqlCommand(query, connection))
                using (var adapter = new MySqlDataAdapter(command))
                {
                    DataTable table = new DataTable(); // Создаем таблицу данных
                    connection.Open(); // Открываем соединение
                    adapter.Fill(table); // Заполняем таблицу

                    if (table.Rows.Count == 0) // Если данных нет
                    {
                        MessageBox.Show("Данные не найдены.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    originalDataTable = table.Copy(); // Сохраняем копию исходных данных для фильтрации
                    dataGridView1.DataSource = table; // Устанавливаем источник данных

                    // Устанавливаем заголовки столбцов
                    SetHeader("ФИО", "ФИО");
                    SetHeader("Телефон", "Телефон");
                    SetHeader("Дата рождения", "Дата рождения");
                    SetHeader("Возраст", "Возраст");
                    SetHeader("Статус", "Статус");
                    SetHeader("LTV", "LTV");
                    SetHeader("ФИО сотрудника", "Сотрудник");

                    // Скрываем служебный столбец ID
                    HideColumn("ID_Client");

                    // Настройка DataGridView
                    dataGridView1.AllowUserToAddRows = false; // Запрет добавления строк
                    dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // Выделение всей строки
                    dataGridView1.MultiSelect = false; // Запрет множественного выбора
                    dataGridView1.ReadOnly = true; // Только для чтения
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Автоматическая ширина
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Вспомогательный метод для установки заголовка столбца
        private void SetHeader(string columnName, string headerText)
        {
            if (dataGridView1.Columns.Contains(columnName))
                dataGridView1.Columns[columnName].HeaderText = headerText;
        }

        // Вспомогательный метод для скрытия столбца
        private void HideColumn(string columnName)
        {
            if (dataGridView1.Columns.Contains(columnName))
                dataGridView1.Columns[columnName].Visible = false;
        }

        // Метод для обновления возраста клиентов в БД (на всякий случай)
        private void UpdateClientBirthdaysAndAges()
        {
            try
            {
                // Запрос для обновления возраста на основе даты рождения
                string query = @"
                    UPDATE mydb.clients
                    SET Age = TIMESTAMPDIFF(YEAR, Birthday, CURDATE())
                    WHERE Birthday IS NOT NULL;";

                using (var connection = new MySqlConnection(connect.con))
                using (var command = new MySqlCommand(query, connection))
                {
                    connection.Open(); // Открываем соединение
                    int affectedRows = command.ExecuteNonQuery(); // Выполняем обновление
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обработчик нажатия мыши на DataGridView (для контекстного меню)
        private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) // Если нажата правая кнопка
            {
                var hitTestInfo = dataGridView1.HitTest(e.X, e.Y); // Определяем, куда кликнули
                if (hitTestInfo.RowIndex >= 0) // Если кликнули на строку
                {
                    dataGridView1.ClearSelection(); // Снимаем выделение
                    dataGridView1.Rows[hitTestInfo.RowIndex].Selected = true; // Выделяем строку
                    contextMenuStrip1.Show(dataGridView1, e.Location); // Показываем контекстное меню
                }
            }
        }

        // Обработчик загрузки формы
        private void FormHeadViewClients_Load(object sender, EventArgs e)
        {
            FillTableData(); // Загружаем данные

            // Обновляем данные о возрасте клиентов
            UpdateClientBirthdaysAndAges();

            // Инициализация ComboBox для фильтрации и сортировки
            comboBox1.Items.AddRange(new[] { "ФИО", "Статус", "LTV" }); // Поля для сортировки
            comboBox2.Items.AddRange(new[] { "Все", "Больше 500 000", "Меньше 1 000 000", "Больше 2 000 000" }); // Фильтры по LTV

            // Загрузка статусов из базы данных для фильтрации
            LoadStatusesToComboBox3();
        }

        // Метод для загрузки статусов в ComboBox3
        private void LoadStatusesToComboBox3()
        {
            try
            {
                string query = "SELECT DISTINCT status FROM mydb.status_client;";
                using (var connection = new MySqlConnection(connect.con))
                using (var command = new MySqlCommand(query, connection))
                using (var adapter = new MySqlDataAdapter(command))
                {
                    DataTable statusTable = new DataTable();
                    connection.Open();
                    adapter.Fill(statusTable); // Заполняем таблицу статусами

                    // Добавляем каждый статус в ComboBox
                    foreach (DataRow row in statusTable.Rows)
                    {
                        comboBox3.Items.Add(row["status"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке статусов: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Метод для инициализации поиска и фильтрации (подписка на события)
        private void InitializeSearchAndFilter()
        {
            textBox1.TextChanged += (s, e) => ApplyFilters(); // Поиск по ФИО и телефону
            comboBox1.SelectedIndexChanged += (s, e) => ApplyFilters(); // Сортировка
            comboBox3.SelectedIndexChanged += (s, e) => ApplyFilters(); // Фильтр по статусу
            comboBox2.SelectedIndexChanged += (s, e) => ApplyFilters(); // Фильтр по LTV
            textBox2.TextChanged += (s, e) => ApplyFilters(); // Поиск по сотруднику
        }

        // Основной метод применения фильтров и поиска
        private void ApplyFilters()
        {
            if (originalDataTable == null) return; // Если исходных данных нет, выходим

            // Начинаем с исходной таблицы
            var filteredData = originalDataTable.AsEnumerable();

            // Поиск по тексту в TextBox1 (ФИО или телефон)
            string searchText = textBox1.Text.ToLower();
            if (!string.IsNullOrEmpty(searchText))
            {
                filteredData = filteredData.Where(row =>
                    row["ФИО"].ToString().ToLower().Contains(searchText) ||
                    row["Телефон"].ToString().ToLower().Contains(searchText));
            }

            // Поиск по сотруднику (TextBox2)
            string employeeSearchText = textBox2.Text.ToLower();
            if (!string.IsNullOrEmpty(employeeSearchText))
            {
                filteredData = filteredData.Where(row =>
                    row["ФИО сотрудника"] != DBNull.Value &&
                    row["ФИО сотрудника"].ToString().ToLower().Contains(employeeSearchText));
            }

            // Фильтрация по статусу (ComboBox3)
            if (comboBox3.SelectedItem != null && comboBox3.SelectedItem.ToString() != "Все")
            {
                string selectedStatus = comboBox3.SelectedItem.ToString();
                filteredData = filteredData.Where(row => row["Статус"].ToString() == selectedStatus);
            }

            // Фильтрация по LTV (ComboBox2)
            if (comboBox2.SelectedItem != null && comboBox2.SelectedItem.ToString() != "Все")
            {
                switch (comboBox2.SelectedItem.ToString())
                {
                    case "Больше 500 000":
                        filteredData = filteredData.Where(row => Convert.ToDecimal(row["LTV"]) > 500000);
                        break;
                    case "Меньше 1 000 000":
                        filteredData = filteredData.Where(row => Convert.ToDecimal(row["LTV"]) < 1000000);
                        break;
                    case "Больше 2 000 000":
                        filteredData = filteredData.Where(row => Convert.ToDecimal(row["LTV"]) > 2000000);
                        break;
                }
            }

            // Сортировка (ComboBox1)
            if (comboBox1.SelectedItem != null)
            {
                string sortColumn = comboBox1.SelectedItem.ToString();
                switch (sortColumn)
                {
                    case "ФИО":
                        filteredData = filteredData.OrderBy(row => row["ФИО"]); // По алфавиту
                        break;
                    case "Статус":
                        filteredData = filteredData.OrderBy(row => row["Статус"]); // По статусу
                        break;
                    case "LTV":
                        filteredData = filteredData.OrderBy(row => Convert.ToDecimal(row["LTV"])); // По возрастанию LTV
                        break;
                }
            }

            // Применение фильтров и сортировки
            if (filteredData.Any()) // Если есть данные после фильтрации
            {
                dataGridView1.DataSource = filteredData.CopyToDataTable(); // Показываем отфильтрованные данные
            }
            else // Если данных нет
            {
                dataGridView1.DataSource = originalDataTable.Clone(); // Пустая таблица
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
    }
}
