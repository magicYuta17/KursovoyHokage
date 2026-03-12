using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Kursivoy_Konkin
{
    // Форма для просмотра и управления объектами недвижимости (доступна администратору)
    public partial class FormAdminObject : Form
    {
        // Конструктор формы
        public FormAdminObject()
        {
            InitializeComponent(); // Инициализация компонентов дизайнера
            this.Load += FormAdminObject_Load; // Подписка на событие загрузки формы
            this.MinimizeBox = false; // Запрет на сворачивание
            this.MaximizeBox = false; // Запрет на разворачивание
        }

        // Метод для инициализации контекстного меню
        private void InitializeContextMenu()
        {
            // Привязываем контекстное меню к DataGridView
            dataGridView1.ContextMenuStrip = contextMenuStrip1;
        }

        // Метод для загрузки данных объектов из БД
        private void LoadData()
        {
            try
            {
                dataGridView1.Columns.Clear(); // Очищаем колонки
                dataGridView1.AutoGenerateColumns = true; // Автогенерация колонок

                // SQL-запрос для получения данных об объектах (только не удаленные)
                string query = @"
            SELECT 
                ID_object,
                name_object AS `Наименование объекта`,
                square AS 'Площадь',
                cost AS 'Стоимость',
                building_dates AS 'Дата постройки',
                number_floors AS 'Количество комнат',
                parking_space AS 'Площадь парковки',
                photo
            FROM object
            WHERE IsDeleted = 0;"; // Только активные объекты

                using (var connection = new MySqlConnection(connect.con))
                using (var command = new MySqlCommand(query, connection))
                using (var adapter = new MySqlDataAdapter(command))
                {
                    var table = new DataTable(); // Создаем таблицу данных
                    connection.Open(); // Открываем соединение
                    adapter.Fill(table); // Заполняем таблицу

                    if (table.Rows.Count == 0) // Если данных нет
                    {
                        MessageBox.Show("Данные не найдены.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    // Привязываем данные к DataGridView
                    dataGridView1.DataSource = table;

                    // Скрываем столбец ID_object (служебная информация)
                    if (dataGridView1.Columns["ID_object"] != null)
                        dataGridView1.Columns["ID_object"].Visible = false;

                    // Скрываем колонку photo (сырые данные из БД, будем показывать изображения отдельно)
                    if (dataGridView1.Columns["photo"] != null)
                        dataGridView1.Columns["photo"].Visible = false;

                    // Добавляем колонку для отображения фото
                    var imageColumn = new DataGridViewImageColumn
                    {
                        Name = "Фото",
                        HeaderText = "Фото",
                        ImageLayout = DataGridViewImageCellLayout.Zoom, // Масштабирование изображения
                        Width = 80, // Ширина колонки
                        SortMode = DataGridViewColumnSortMode.NotSortable // Отключаем сортировку
                    };
                    dataGridView1.Columns.Add(imageColumn);

                    // Путь к папке с фотографиями объектов
                    string photoDirectory = Path.Combine(Application.StartupPath, "photo_object");

                    // Заглушка из ресурсов (на случай отсутствия фото)
                    Image placeholder = Properties.Resources.picture;

                    // Заполняем колонку "Фото" для каждой строки
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.IsNewRow) continue; // Пропускаем пустую строку для добавления

                        // Читаем имя файла напрямую из DataTable
                        object photoFileName = table.Rows[row.Index]["photo"];

                        // Если фото нет в БД - ставим заглушку
                        if (photoFileName == null || photoFileName == DBNull.Value || string.IsNullOrWhiteSpace(photoFileName.ToString()))
                        {
                            row.Cells["Фото"].Value = placeholder;
                            continue;
                        }

                        try
                        {
                            // Формируем полный путь к файлу фото
                            string photoPath = Path.Combine(photoDirectory, photoFileName.ToString());
                            System.Diagnostics.Debug.WriteLine($"Путь к фото: {photoPath}"); // Отладочный вывод

                            if (File.Exists(photoPath)) // Если файл существует
                            {
                                // Загружаем изображение и создаем его копию (чтобы не блокировать файл)
                                using (var img = Image.FromFile(photoPath))
                                {
                                    row.Cells["Фото"].Value = new Bitmap(img);
                                }
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine($"Файл не найден: {photoPath}"); // Отладочный вывод
                                row.Cells["Фото"].Value = placeholder; // Файл не найден - заглушка
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Ошибка загрузки изображения: {ex.Message}"); // Отладочный вывод
                            row.Cells["Фото"].Value = placeholder; // Ошибка загрузки - заглушка
                        }
                    }

                    // Отключаем сортировку у всех колонок
                    foreach (DataGridViewColumn col in dataGridView1.Columns)
                        col.SortMode = DataGridViewColumnSortMode.NotSortable;

                    dataGridView1.RowTemplate.Height = 80; // Высота строк под фото
                    dataGridView1.ClearSelection(); // Снимаем выделение
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обработчик кнопки "Назад"
        private void button5_Click(object sender, EventArgs e)
        {
            FormAdminNavigation f = new FormAdminNavigation(); // Создаем форму навигации администратора
            this.Visible = false; // Скрываем текущую форму
            f.ShowDialog(); // Показываем форму навигации
            this.Close(); // Закрываем текущую форму
        }

        // Обработчик загрузки формы
        private void FormAdminObject_Load(object sender, EventArgs e)
        {
            LoadData(); // Загружаем данные
            InitializeContextMenu(); // Инициализируем контекстное меню
        }

        // Обработчик клика мышью по DataGridView (для контекстного меню)
        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right) // Если нажата правая кнопка мыши
                {
                    var hitTest = dataGridView1.HitTest(e.X, e.Y); // Определяем, куда кликнули

                    // Открываем меню только если кликнули на строку, не на заголовок
                    if (hitTest.RowIndex >= 0)
                    {
                        // Выделяем строку по которой кликнули
                        dataGridView1.ClearSelection(); // Снимаем выделение
                        dataGridView1.Rows[hitTest.RowIndex].Selected = true; // Выделяем строку
                        dataGridView1.CurrentCell = dataGridView1.Rows[hitTest.RowIndex].Cells[0]; // Устанавливаем текущую ячейку

                        contextMenuStrip1.Show(dataGridView1, e.Location); // Показываем контекстное меню
                    }
                }
            }
            catch (Exception ex) { } // Игнорируем ошибки (пустой catch)
        }

        // Обработчик пункта меню "Добавить"
        private void добавитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAdminAddObject f = new FormAdminAddObject(); // Создаем форму добавления объекта
            this.Visible = false; // Скрываем текущую форму
            f.ShowDialog(); // Показываем форму добавления
            this.Close(); // Закрываем текущую форму
        }

        // Обработчик пункта меню "Редактировать"
        private void редактироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Проверяем, выбрана ли строка
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите объект для редактирования.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Получаем ID выбранного объекта
            int selectedId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID_object"].Value);
            FormAdminEditObject f = new FormAdminEditObject(selectedId); // Создаем форму редактирования с ID
            this.Visible = false; // Скрываем текущую форму
            f.ShowDialog(); // Показываем форму редактирования
            this.Close(); // Закрываем текущую форму
        }

        // Обработчик пункта меню "Удалить"
        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Проверяем, выбрана ли строка
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Получаем ID объекта из выбранной строки
                int objectId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID_object"].Value);

                // Запрашиваем подтверждение удаления
                var result = MessageBox.Show("Вы уверены, что хотите удалить объект?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes) // Если пользователь подтвердил
                {
                    try
                    {
                        // Мягкое удаление (установка флага IsDeleted = 1)
                        string query = "UPDATE mydb.object SET IsDeleted = 1 WHERE ID_object = @IDobject;";
                        using (var connection = new MySqlConnection(connect.con))
                        using (var command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@IDobject", objectId); // Передаем ID объекта
                            connection.Open(); // Открываем соединение
                            command.ExecuteNonQuery(); // Выполняем запрос
                        }

                        // Обновляем таблицу после удаления объекта
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении объекта: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите объект для удаления.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Заглушка для события CellContentClick (не используется)
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Метод оставлен пустым, так как требуется дизайнером формы
        }

        private void FormAdminObject_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true; //отменяем закрытие формы
            }
        }
    }
}