using MySql.Data.MySqlClient; // Импорт MySQL клиента для подключения и выполнения запросов к базе данных
using System; // Основные системные типы
using System.Data; // Типы для работы с данными, DataTable и др.
using System.Drawing; // Для обработки изображений
using System.IO; // Для работы с файловой системой
using System.Windows.Forms; // Для работы с элементами Windows Forms

namespace Kursivoy_Konkin
{
    public partial class FormViewObject : Form
    {
        // Имя формы-источника (используется для возврата назад)
        private readonly string _callerFormName;

        // Конструктор формы с параметром для определения предыдущей формы
        public FormViewObject(string callerFormName = "FormManagerNavigation")
        {
            InitializeComponent(); // Инициализация компонентов формы
            _callerFormName = callerFormName; // Сохраняем имя формы-источника
            LoadData(); // Загружаем данные при создании формы
            this.Load += FormViewObject_Load; // Обработчик события загрузки формы
            this.MinimizeBox = false; // Отключение кнопки минимизации
            this.MaximizeBox = false; // Отключение кнопки максимизации
            this.ControlBox = false;  // Отключение всего системного блока
        }

        // Метод загрузки и отображения данных
        private void LoadData()
        {
            try
            {
                // Очистка всех колонок у DataGridView перед загрузкой новых данных
                dataGridView1.Columns.Clear();
                dataGridView1.AutoGenerateColumns = true; // Автоматическая генерация колонок из DataTable

                // SQL-запрос для получения информации об объектах
                string query = @"
            SELECT 
                ID_object,
                square AS 'Площадь',
                cost AS 'Стоимость',
                building_dates AS 'Дата постройки',
                number_floors AS 'Количество комнат',
                parking_space AS 'Площадь парковки',
                photo
            FROM object
            WHERE IsDeleted = 0;";

                // Создаем подключение, команду и адаптер, используя оператор using для автоматического закрытия ресурсов
                using (var connection = new MySqlConnection(connect.con))
                using (var command = new MySqlCommand(query, connection))
                using (var adapter = new MySqlDataAdapter(command))
                {
                    var table = new DataTable(); // Таблица для хранения результатов
                    connection.Open(); // Открываем соединение
                    adapter.Fill(table); // Заполняем таблицу данными из запроса

                    // Если данных нет, показываем сообщение и преждевременно завершаем
                    if (table.Rows.Count == 0)
                    {
                        MessageBox.Show("Данные не найдены.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    // Привязка данных к DataGridView
                    dataGridView1.DataSource = table;

                    // Скрываем колонку с ID объекта (чтобы пользователь её не видел)
                    if (dataGridView1.Columns["ID_object"] != null)
                        dataGridView1.Columns["ID_object"].Visible = false;

                    // Скрываем колонку "photo" — сырые данные (имена файлов)
                    if (dataGridView1.Columns["photo"] != null)
                        dataGridView1.Columns["photo"].Visible = false;

                    // Создаем колонку для отображения изображений
                    var imageColumn = new DataGridViewImageColumn
                    {
                        Name = "Фото", // Имя колонки
                        HeaderText = "Фото", // Заголовок
                        ImageLayout = DataGridViewImageCellLayout.Zoom, // Масштаб изображения
                        Width = 80, // Ширина колонки
                        SortMode = DataGridViewColumnSortMode.NotSortable // Отключение сортировки
                    };
                    dataGridView1.Columns.Add(imageColumn); // Добавляем колонку к таблице

                    // Папка, где хранятся изображения
                    string photoDirectory = Path.Combine(Application.StartupPath, "photo_object");

                    // Заглушка — изображение по умолчанию из ресурсов
                    Image placeholder = Properties.Resources.picture;

                    // Проходим по каждой строке и заполняем колонку "Фото" изображением
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.IsNewRow) continue; // Пропускаем новую пустую строку

                        // Получение имени файла фотографии из таблицы
                        object photoFileName = table.Rows[row.Index]["photo"];

                        // Проверка на null или пустое значение
                        if (photoFileName == null || photoFileName == DBNull.Value || string.IsNullOrWhiteSpace(photoFileName.ToString()))
                        {
                            // Если фото нет, ставим заглушку
                            row.Cells["Фото"].Value = placeholder;
                            continue;
                        }

                        try
                        {
                            // Полный путь к изображению
                            string photoPath = Path.Combine(photoDirectory, photoFileName.ToString());
                            System.Diagnostics.Debug.WriteLine($"Путь к фото: {photoPath}");

                            // Проверка: существует ли файл
                            if (File.Exists(photoPath))
                            {
                                using (var img = Image.FromFile(photoPath))
                                {
                                    // Создаем Bitmap из изображения для безопасности и дисплея
                                    row.Cells["Фото"].Value = new Bitmap(img);
                                }
                            }
                            else
                            {
                                // Если файл не найден, ставим заглушку
                                System.Diagnostics.Debug.WriteLine($"Файл не найден: {photoPath}");
                                row.Cells["Фото"].Value = placeholder;
                            }
                        }
                        catch (Exception ex)
                        {
                            // Логируем возможные ошибки при загрузке изображения
                            System.Diagnostics.Debug.WriteLine($"Ошибка загрузки изображения: {ex.Message}");
                            row.Cells["Фото"].Value = placeholder; // Заглушка при ошибке
                        }
                    }

                    // Отключаем сортировку у всех колонок, чтобы не было несогласованных сортировок
                    foreach (DataGridViewColumn col in dataGridView1.Columns)
                        col.SortMode = DataGridViewColumnSortMode.NotSortable;

                    // Устанавливаем высоту строк
                    dataGridView1.RowTemplate.Height = 80;

                    // Снимаем выделение со строк
                    dataGridView1.ClearSelection();
                }
            }
            catch (Exception ex)
            {
                // Обработка ошибок со всплывающим сообщением
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обработчик нажатия кнопки возврата
        private void button1_Click(object sender, EventArgs e)
        {
            Form previousForm = null; // Переменная для следующей формы

            // В зависимости от имени вызывающей формы создаем её экземпляр
            switch (_callerFormName)
            {
                case "FormManagerNavigation":
                    previousForm = new FormManagerNavigation(); // новая форма навигации
                    break;
                case "FormHeadNavigation":
                    previousForm = new FormHeadNavigation(); // другая форма навигации
                    break;
            }

            // Если форма определена, возвращаемся назад
            if (previousForm != null)
            {
                this.Visible = false; // скрываем текущую форму
                previousForm.ShowDialog(); // показываем новую форму модально
                this.Close(); // закрываем текущую
            }
        }

        //Логика при загрузке формы
        private void FormViewObject_Load(object sender, EventArgs e)
        {
            // Установка свойств формы
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            

            // Загружаем данные ещё раз (на всякий случай при загрузке)
            LoadData();
        }

        private void FormViewObject_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true; //отменяем закрытие формы
            }
        }
    }
}