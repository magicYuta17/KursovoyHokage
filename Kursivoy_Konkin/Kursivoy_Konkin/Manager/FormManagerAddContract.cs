
using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Kursivoy_Konkin;
using Kursivoy_Konkin.Manager;

namespace YourNamespace
{
    public partial class FormManagerAddContract : Form
    {
        // Строка подключения к БД, берется из внешнего источника (connect.con)
        private string connectionString = connect.con;

        // Переменные для хранения ID выбранных элементов (клиент, сотрудник, объект).
        // int? означает, что они могут быть null (ничего не выбрано)
        private int? selectedClientId = null;
        private int? selectedWorkerId = null;
        private int? selectedObjectId = null;

        // Конструктор формы. Вызывается при создании нового экземпляра
        public FormManagerAddContract()
        {
            InitializeComponent(); // Инициализация компонентов дизайнера
            SetupForm(); // Вызов пользовательского метода для настройки формы
            this.MinimizeBox = false; // Запрет на сворачивание окна
            this.MaximizeBox = false; // Запрет на разворачивание окна
            
        }

        #region Инициализация формы

        // Метод для первичной настройки элементов управления на форме
        private void SetupForm()
        {

            // Разворачивает форму на весь экран при запуске
            this.WindowState = FormWindowState.Maximized;
            // this.Size = new System.Drawing.Size(2000, 842); // Закомментированный код для ручной установки размера

            // Открывать по центру экрана
            this.StartPosition = FormStartPosition.CenterScreen;

            // Настройка календаря для даты подписания: минимальная дата - 1900 год, максимальная - сегодня
            dtpDateSigning.MinDate = new DateTime(1900, 1, 1); // или любая ранняя дата
            dtpDateSigning.MaxDate = DateTime.Today; // нельзя выбрать будущее
            dtpDateSigning.Value = DateTime.Today; // Установка текущей даты по умолчанию

            // Настройка календаря для даты окончания
            dtpEndDate.MinDate = DateTime.Today; // по умолчанию, пока объект не выбран
            dtpEndDate.MaxDate = new DateTime(2100, 1, 1); // Максимальная дата - 2100 год
            dtpEndDate.Value = DateTime.Today; // Установка текущей даты по умолчанию

            // Настройка DataGridView (таблиц)
            SetupDataGridViews();

            // Загрузка данных в таблицы
            LoadClients();
            LoadWorkers();
            LoadObjects();

            // Подписка на события поиска: при изменении текста в поисковой строке вызывается метод загрузки с фильтром
            txtSearchClient.TextChanged += (s, e) => LoadClients(txtSearchClient.Text);
            txtSearchWorker.TextChanged += (s, e) => LoadWorkers(txtSearchWorker.Text);
            txtSearchObject.TextChanged += (s, e) => LoadObjects(txtSearchObject.Text);

            // Подписка на события выбора строки в таблицах
            dgvClients.SelectionChanged += DgvClients_SelectionChanged;
            dgvWorkers.SelectionChanged += DgvWorkers_SelectionChanged;
            dgvObjects.SelectionChanged += DgvObjects_SelectionChanged;

            // Ограничение ввода текста (только русские буквы и цифры)
            txtSearchClient.KeyPress += RussianAndDigitsOnly_KeyPress;
            txtSearchWorker.KeyPress += RussianAndDigitsOnly_KeyPress;
            txtSearchObject.KeyPress += RussianAndDigitsOnly_KeyPress;
            txtContractName.KeyPress += RussianAndDigitsOnly_KeyPress;

            // Логика: дата окончания не может быть раньше даты подписания
            dtpDateSigning.ValueChanged += DtpDateSigning_ValueChanged;

            // Подписка на события кнопок
            this.btnAddContract.Click += new EventHandler(this.btnAddContract_Click);
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
        }

        // Метод для настройки внешнего вида и поведения всех таблиц (DataGridView)
        private void SetupDataGridViews()
        {
            // Цикл по массиву из трех таблиц
            foreach (DataGridView dgv in new[] { dgvClients, dgvWorkers, dgvObjects })
            {
                dgv.ReadOnly = true; // Запрет на редактирование ячеек
                dgv.MultiSelect = false; // Запрет на выделение нескольких строк
                dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // Выделение строки целиком
                dgv.AllowUserToAddRows = false; // Убрать строку для добавления новой записи
                dgv.AllowUserToDeleteRows = false; // Запретить удаление строк пользователем
                dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Автоматическая ширина колонок
                dgv.RowHeadersVisible = false; // Скрыть заголовки строк (левый серый столбец)
                dgv.BackgroundColor = System.Drawing.Color.White; // Установить белый фон
            }
        }
        #endregion

        #region Загрузка данных из БД

        // Загрузка списка клиентов из базы данных с возможностью поиска
        private void LoadClients(string search = "")
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open(); // Открытие соединения
                    // SQL-запрос для выборки активных клиентов (IsDeleted = 0) с фильтром по ФИО или телефону
                    string query = @"SELECT 
                                        ID_Client AS 'ID',
                                        FullName_client AS 'ФИО',
                                        phone AS 'Телефон'
                                    FROM clients
                                    WHERE IsDeleted = 0
                                    AND (FullName_client LIKE @search 
                                        OR phone LIKE @search)";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@search", $"%{search}%"); // Добавление параметра поиска с маской

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd); // Адаптер для заполнения DataTable
                    DataTable dt = new DataTable();
                    adapter.Fill(dt); // Заполнение таблицы данными

                    dgvClients.DataSource = dt; // Установка источника данных для DataGridView

                    // Скрываем колонку ID (она нужна нам, но не показываем)
                    dgvClients.Columns["ID"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                // Показ сообщения об ошибке в случае исключения
                MessageBox.Show($"Ошибка загрузки клиентов: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Загрузка сотрудников (аналогично LoadClients)
        private void LoadWorkers(string search = "")
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    // Запрос на выборку активных сотрудников
                    string query = @"SELECT 
                                        w.ID_worker AS 'ID',
                                        w.FIO AS 'ФИО',
                                        w.phone AS 'Телефон'
                                    FROM worker w
                                    WHERE w.IsDeleted = 0
                                    AND (w.FIO LIKE @search 
                                        OR w.phone LIKE @search)";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@search", $"%{search}%");

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dgvWorkers.DataSource = dt;
                    dgvWorkers.Columns["ID"].Visible = false; // Скрываем ID
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки сотрудников: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Загрузка объектов недвижимости
        private void LoadObjects(string search = "")
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    // Запрос на выборку активных объектов с множеством характеристик
                    string query = @"SELECT 
                                ID_object           AS 'ID',
                                name_object         AS 'Название объекта',
                                square              AS 'Площадь (м²)',
                                number_floors       AS 'Кол-во комнат',
                                parking_space       AS 'Парковка (м²)',
                                cost                AS 'Стоимость (руб.)',
                                building_dates      AS 'Срок строительства (дней)'
                            FROM object
                            WHERE IsDeleted = 0
                            AND (name_object LIKE @search 
                                OR CAST(square AS CHAR) LIKE @search
                                OR CAST(number_floors AS CHAR) LIKE @search)";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@search", $"%{search}%");

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dgvObjects.DataSource = dt;
                    dgvObjects.Columns["ID"].Visible = false; // Скрываем ID
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки объектов: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region События выбора в DataGridView

        // Обработчик выбора строки в таблице клиентов
        private void DgvClients_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvClients.CurrentRow != null) // Если есть выбранная строка
            {
                selectedClientId = Convert.ToInt32(dgvClients.CurrentRow.Cells["ID"].Value); // Сохраняем ID клиента
                string fio = dgvClients.CurrentRow.Cells["ФИО"].Value?.ToString(); // Получаем ФИО

                // Обновляем правую панель с отображением выбранного клиента
                lblClientValue.Text = $"{fio} ✅";
                lblClientValue.ForeColor = System.Drawing.Color.Green; // Меняем цвет текста на зеленый
            }
        }

        // Обработчик выбора строки в таблице сотрудников
        private void DgvWorkers_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvWorkers.CurrentRow != null)
            {
                selectedWorkerId = Convert.ToInt32(dgvWorkers.CurrentRow.Cells["ID"].Value); // Сохраняем ID сотрудника
                string fio = dgvWorkers.CurrentRow.Cells["ФИО"].Value?.ToString(); // Получаем ФИО

                // Обновляем метку
                lblWorkerValue.Text = $"{fio} ✅";
                lblWorkerValue.ForeColor = System.Drawing.Color.Green;
            }
        }

        // Обработчик выбора строки в таблице объектов
        private void DgvObjects_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvObjects.CurrentRow != null)
            {
                selectedObjectId = Convert.ToInt32(dgvObjects.CurrentRow.Cells["ID"].Value); // Сохраняем ID объекта
                string name = dgvObjects.CurrentRow.Cells["Название объекта"].Value?.ToString(); // Получаем название

                // Обновляем метку
                lblObjectValue.Text = $"{name} ✅";
                lblObjectValue.ForeColor = System.Drawing.Color.Green;

                // Автоматически считаем дату окончания из поля building_dates (срок строительства)
                UpdateEndDateMin();
            }
        }
        #endregion

        #region Валидация
        // Метод для обновления минимальной даты окончания контракта на основе срока строительства объекта
        private void UpdateEndDateMin()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    // Запрос на получение срока строительства выбранного объекта
                    string query = "SELECT building_dates FROM object WHERE ID_object = @id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", selectedObjectId); // Передаем ID объекта

                    object result = cmd.ExecuteScalar(); // Выполняем запрос и получаем одно значение

                    if (result != null) // Если значение получено
                    {
                        int buildingDays = Convert.ToInt32(result); // Преобразуем в число (количество дней)

                        // Минимальная дата окончания = дата подписания + срок строительства
                        DateTime minEndDate = dtpDateSigning.Value.AddDays(buildingDays);

                        dtpEndDate.MinDate = minEndDate; // Устанавливаем минимальную дату в календаре
                        dtpEndDate.Value = minEndDate; // Устанавливаем значение по умолчанию
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка расчёта даты: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Метод для ограничения ввода: только русские буквы, цифры и пробел
        private void RussianAndDigitsOnly_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Разрешаем: русские буквы, цифры, пробел, backspace
            bool isRussian = (e.KeyChar >= 'а' && e.KeyChar <= 'я') ||
                             (e.KeyChar >= 'А' && e.KeyChar <= 'Я') ||
                              e.KeyChar == 'ё' || e.KeyChar == 'Ё';
            bool isDigit = char.IsDigit(e.KeyChar);
            bool isSpace = e.KeyChar == ' ';
            bool isBackspace = e.KeyChar == '\b'; // Клавиша удаления

            // Если символ не подходит ни под одно разрешение
            if (!isRussian && !isDigit && !isSpace && !isBackspace)
            {
                e.Handled = true; // Блокируем ввод (символ не будет добавлен)
            }
        }

        // Обработчик изменения даты подписания
        private void DtpDateSigning_ValueChanged(object sender, EventArgs e)
        {
            dtpEndDate.MinDate = dtpDateSigning.Value; // Минимальная дата окончания = дата подписания

            // Если дата окончания стала меньше даты подписания, исправляем это
            if (dtpEndDate.Value < dtpDateSigning.Value)
                dtpEndDate.Value = dtpDateSigning.Value;
        }

        // Проверка заполненности формы перед сохранением
        private bool ValidateForm()
        {
            if (selectedClientId == null)
            {
                MessageBox.Show("Выберите клиента!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (selectedWorkerId == null)
            {
                MessageBox.Show("Выберите сотрудника!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (selectedObjectId == null)
            {
                MessageBox.Show("Выберите объект!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtContractName.Text))
            {
                MessageBox.Show("Введите название контракта!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (dtpEndDate.Value < dtpDateSigning.Value)
            {
                MessageBox.Show("Дата окончания не может быть раньше даты подписания!",
                    "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true; // Если все проверки пройдены, возвращаем true
        }
        #endregion

        #region Кнопки

        // Обработчик кнопки "Добавить контракт"
        private void btnAddContract_Click(object sender, EventArgs e)
        {
            if (!ValidateForm()) return; // Если валидация не пройдена, выходим из метода

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open(); // Открываем соединение
                    // SQL-запрос для вставки нового контракта в таблицу
                    string query = @"INSERT INTO contract 
                                    (Name_contract, date_signing, END_DATE, 
                                     Clients_ID_Client, worker_ID_worker, 
                                     connection_contract_object_idconnection_contract_object)
                                    VALUES 
                                    (@name, @dateSigning, @endDate, 
                                     @clientId, @workerId, @objectId)";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    // Добавляем параметры со значениями из полей формы
                    cmd.Parameters.AddWithValue("@name", txtContractName.Text.Trim()); // Название контракта
                    cmd.Parameters.AddWithValue("@dateSigning", dtpDateSigning.Value.Date); // Только дата (без времени)
                    cmd.Parameters.AddWithValue("@endDate", dtpEndDate.Value.Date);
                    cmd.Parameters.AddWithValue("@clientId", selectedClientId); // ID клиента
                    cmd.Parameters.AddWithValue("@workerId", selectedWorkerId); // ID сотрудника
                    cmd.Parameters.AddWithValue("@objectId", selectedObjectId); // ID объекта

                    cmd.ExecuteNonQuery(); // Выполняем запрос на вставку

                    // Показываем сообщение об успехе
                    MessageBox.Show("✅ Контракт успешно добавлен!", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.Close(); // Закрываем текущую форму
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении контракта: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обработчик кнопки "Назад" (Отмена)
        private void btnCancel_Click(object sender, EventArgs e)
        {
            FormManagerViewContract f = new FormManagerViewContract(); // Создаем форму просмотра контрактов
            this.Visible = false; // Скрываем текущую форму
            f.ShowDialog(); // Открываем новую форму в модальном режиме
            this.Close(); // Закрываем текущую форму
        }


        #endregion

        private void FormManagerAddContract_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true; //отменяем закрытие формы
            }
        }
    }
}
