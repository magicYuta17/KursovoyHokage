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
        // Строка подключения к БД
        private string connectionString = connect.con;

        // Переменные для хранения выбранных ID
        private int? selectedClientId = null;
        private int? selectedWorkerId = null;
        private int? selectedObjectId = null;

        public FormManagerAddContract()
        {
            InitializeComponent();
            SetupForm();
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.ControlBox = false;
        }

        #region Инициализация формы

        private void SetupForm()
        {


            this.WindowState = FormWindowState.Maximized;
            // this.Size = new System.Drawing.Size(2000, 842);

            // Открывать по центру экрана
            this.StartPosition = FormStartPosition.CenterScreen;

            dtpDateSigning.MinDate = new DateTime(1900, 1, 1); // или любая ранняя дата
            dtpDateSigning.MaxDate = DateTime.Today; // нельзя выбрать будущее
            dtpDateSigning.Value = DateTime.Today;

            dtpEndDate.MinDate = DateTime.Today; // по умолчанию, пока объект не выбран
            dtpEndDate.MaxDate = new DateTime(2100, 1, 1);
            dtpEndDate.Value = DateTime.Today;

            // Настройка DataGridView
            SetupDataGridViews();

            // Загрузка данных
            LoadClients();
            LoadWorkers();
            LoadObjects();

            // Подписка на события поиска
            txtSearchClient.TextChanged += (s, e) => LoadClients(txtSearchClient.Text);
            txtSearchWorker.TextChanged += (s, e) => LoadWorkers(txtSearchWorker.Text);
            txtSearchObject.TextChanged += (s, e) => LoadObjects(txtSearchObject.Text);

            // Подписка на события выбора в таблицах
            dgvClients.SelectionChanged += DgvClients_SelectionChanged;
            dgvWorkers.SelectionChanged += DgvWorkers_SelectionChanged;
            dgvObjects.SelectionChanged += DgvObjects_SelectionChanged;

            // Ограничение ввода текста
            txtSearchClient.KeyPress += RussianAndDigitsOnly_KeyPress;
            txtSearchWorker.KeyPress += RussianAndDigitsOnly_KeyPress;
            txtSearchObject.KeyPress += RussianAndDigitsOnly_KeyPress;
            txtContractName.KeyPress += RussianAndDigitsOnly_KeyPress;

            // Дата окончания не раньше даты подписания
            dtpDateSigning.ValueChanged += DtpDateSigning_ValueChanged;

            this.btnAddContract.Click += new EventHandler(this.btnAddContract_Click);
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
        }

        private void SetupDataGridViews()
        {
            // Общие настройки для всех DataGridView
            foreach (DataGridView dgv in new[] { dgvClients, dgvWorkers, dgvObjects })
            {
                dgv.ReadOnly = true;
                dgv.MultiSelect = false;
                dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgv.AllowUserToAddRows = false;
                dgv.AllowUserToDeleteRows = false;
                dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgv.RowHeadersVisible = false;
                dgv.BackgroundColor = System.Drawing.Color.White;
            }
        }
        #endregion

        #region Загрузка данных из БД

        // Загрузка клиентов
        private void LoadClients(string search = "")
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"SELECT 
                                        ID_Client AS 'ID',
                                        FullName_client AS 'ФИО',
                                        phone AS 'Телефон'
                                    FROM clients
                                    WHERE IsDeleted = 0
                                    AND (FullName_client LIKE @search 
                                        OR phone LIKE @search)";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@search", $"%{search}%");

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dgvClients.DataSource = dt;

                    // Скрываем колонку ID (она нужна нам, но не показываем)
                    dgvClients.Columns["ID"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки клиентов: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Загрузка сотрудников
        private void LoadWorkers(string search = "")
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
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
                    dgvWorkers.Columns["ID"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки сотрудников: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Загрузка объектов
        private void LoadObjects(string search = "")
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
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
                    dgvObjects.Columns["ID"].Visible = false;
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

        private void DgvClients_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvClients.CurrentRow != null)
            {
                selectedClientId = Convert.ToInt32(dgvClients.CurrentRow.Cells["ID"].Value);
                string fio = dgvClients.CurrentRow.Cells["ФИО"].Value?.ToString();

                // Обновляем правую панель
                lblClientValue.Text = $"{fio} ✅";
                lblClientValue.ForeColor = System.Drawing.Color.Green;
            }
        }

        private void DgvWorkers_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvWorkers.CurrentRow != null)
            {
                selectedWorkerId = Convert.ToInt32(dgvWorkers.CurrentRow.Cells["ID"].Value);
                string fio = dgvWorkers.CurrentRow.Cells["ФИО"].Value?.ToString();

                lblWorkerValue.Text = $"{fio} ✅";
                lblWorkerValue.ForeColor = System.Drawing.Color.Green;
            }
        }

        private void DgvObjects_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvObjects.CurrentRow != null)
            {
                selectedObjectId = Convert.ToInt32(dgvObjects.CurrentRow.Cells["ID"].Value);
                string name = dgvObjects.CurrentRow.Cells["Название объекта"].Value?.ToString();

                lblObjectValue.Text = $"{name} ✅";
                lblObjectValue.ForeColor = System.Drawing.Color.Green;

                // Автоматически считаем дату окончания из building_dates
                UpdateEndDateMin();
            }
        }
        #endregion

        #region Валидация
        private void UpdateEndDateMin()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT building_dates FROM object WHERE ID_object = @id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", selectedObjectId);

                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        int buildingDays = Convert.ToInt32(result);

                        // Минимальная дата окончания = дата подписания + срок строительства
                        DateTime minEndDate = dtpDateSigning.Value.AddDays(buildingDays);

                        dtpEndDate.MinDate = minEndDate;
                        dtpEndDate.Value = minEndDate;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка расчёта даты: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Только русские буквы, цифры и пробел
        private void RussianAndDigitsOnly_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Разрешаем: русские буквы, цифры, пробел, backspace
            bool isRussian = (e.KeyChar >= 'а' && e.KeyChar <= 'я') ||
                             (e.KeyChar >= 'А' && e.KeyChar <= 'Я') ||
                              e.KeyChar == 'ё' || e.KeyChar == 'Ё';
            bool isDigit = char.IsDigit(e.KeyChar);
            bool isSpace = e.KeyChar == ' ';
            bool isBackspace = e.KeyChar == '\b';

            if (!isRussian && !isDigit && !isSpace && !isBackspace)
            {
                e.Handled = true; // Блокируем ввод
            }
        }

        // Дата окончания не раньше даты подписания
        private void DtpDateSigning_ValueChanged(object sender, EventArgs e)
        {
            dtpEndDate.MinDate = dtpDateSigning.Value;

            if (dtpEndDate.Value < dtpDateSigning.Value)
                dtpEndDate.Value = dtpDateSigning.Value;
        }

        // Проверка заполненности формы
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
            return true;
        }
        #endregion

        #region Кнопки

        // Кнопка "Добавить контракт"
        private void btnAddContract_Click(object sender, EventArgs e)
        {
            if (!ValidateForm()) return;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"INSERT INTO contract 
                                    (Name_contract, date_signing, END_DATE, 
                                     Clients_ID_Client, worker_ID_worker, 
                                     connection_contract_object_idconnection_contract_object)
                                    VALUES 
                                    (@name, @dateSigning, @endDate, 
                                     @clientId, @workerId, @objectId)";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@name", txtContractName.Text.Trim());
                    cmd.Parameters.AddWithValue("@dateSigning", dtpDateSigning.Value.Date);
                    cmd.Parameters.AddWithValue("@endDate", dtpEndDate.Value.Date);
                    cmd.Parameters.AddWithValue("@clientId", selectedClientId);
                    cmd.Parameters.AddWithValue("@workerId", selectedWorkerId);
                    cmd.Parameters.AddWithValue("@objectId", selectedObjectId);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("✅ Контракт успешно добавлен!", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении контракта: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Кнопка "Назад"
        private void btnCancel_Click(object sender, EventArgs e)
        {
            FormManagerViewContract f = new FormManagerViewContract();
            this.Visible = false;
            f.ShowDialog();
            this.Close();
        }




        #endregion

        
    }
}