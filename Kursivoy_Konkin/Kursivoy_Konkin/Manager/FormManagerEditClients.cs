
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Kursivoy_Konkin
{
    public partial class FormManagerEditClients : Form
    {
        // Строка подключения к базе данных, получаемая из внешнего источника connect.con
        private string ConnectionString = connect.con;
        // Путь к выбранному изображению (в текущей реализации не используется)
        private string _selectedImagePath = string.Empty;
        // Хранит ID клиента, данные которого редактируются
        private int _clientId = 0; // хранит текущий ID клиента для сохранения

        // Конструктор формы редактирования клиента
        public FormManagerEditClients()
        {
            InitializeComponent();
            SetupFormConstraints();
            LoadStatusCombo();
            // Подписки на кнопки
            this.buttonEditClient.Click += buttonEditClient_Click;
            // Отключаем системные кнопки управления окном (свернуть, развернуть, закрыть)
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.ControlBox = false;
        }

        // Загружает список статусов в comboBox (можно вызывать перед LoadClientById)
        public void LoadStatusCombo()
        {
            try
            {
                // Создаем подключение к БД и выполняем запрос на получение всех статусов клиентов
                using (MySqlConnection conn = new MySqlConnection(ConnectionString))
                using (MySqlCommand cmd = new MySqlCommand("SELECT ID_Status_client, status FROM status_client ORDER BY ID_Status_client", conn))
                using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                {
                    // Заполняем DataTable результатами запроса
                    var dt = new System.Data.DataTable();
                    da.Fill(dt);
                    // Настраиваем отображение и значения ComboBox
                    comboBoxStatus.DisplayMember = "status";
                    comboBoxStatus.ValueMember = "ID_Status_client";
                    comboBoxStatus.DataSource = dt;
                    comboBoxStatus.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось загрузить статусы: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Загружает данные клиента по ID и заполняет контролы формы
        public void LoadClientById(int clientId)
        {
            _clientId = clientId; // запоминаем id для дальнейшего обновления

            // SQL запрос для получения данных клиента по ID
            string query = @"SELECT FullName_client, phone, Age, Status_client_ID_Status_client, LTV
                             FROM mydb.clients WHERE ID_Client = @id LIMIT 1;";

            try
            {
                // Создаем подключение и выполняем запрос с параметром ID клиента
                using (MySqlConnection conn = new MySqlConnection(ConnectionString))
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", clientId);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        // Проверяем, найден ли клиент
                        if (!reader.Read())
                        {
                            MessageBox.Show("Клиент не найден.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // Заполняем поля формы данными из результата запроса, обрабатывая возможные NULL значения
                        txtFullName_client.Text = reader["FullName_client"] == DBNull.Value ? string.Empty : reader["FullName_client"].ToString();
                        maskedTextBox1.Text = reader["phone"] == DBNull.Value ? string.Empty : reader["phone"].ToString();
                        txtAge.Text = reader["Age"] == DBNull.Value ? string.Empty : reader["Age"].ToString();
                        txtLTV.Text = reader["LTV"] == DBNull.Value ? string.Empty : reader["LTV"].ToString();

                        int statusId = reader["Status_client_ID_Status_client"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Status_client_ID_Status_client"]);

                        // Устанавливаем выбранный статус в ComboBox, если данные уже загружены
                        try
                        {
                            if (comboBoxStatus.DataSource != null)
                            {
                                comboBoxStatus.SelectedValue = statusId;
                            }
                        }
                        catch
                        {
                            // Если привязка не установлена — оставляем пустым
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных клиента: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обработчик кнопки "Сохранить редактирование"
        private void buttonEditClient_Click(object sender, EventArgs e)
        {
            // Проверяем все зарегистрированные поля на корректность заполнения
            if (!TextBoxFilters.InputValidators.ValidateAll(out Control[] invalidControls))
            {
                MessageBox.Show("Заполните все обязательные поля, отмеченные красным!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Проверка длины номера телефона после удаления пробелов
            string phone = Regex.Replace(maskedTextBox1.Text, @"\s+", ""); // Удаляем все пробелы и пустое пространство
            if (phone.Length != 16)
            {
                MessageBox.Show("Некорректный номер телефона. Поле должно содержать ровно 16 символов (включая форматирование).", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                maskedTextBox1.BackColor = Color.MistyRose; // Подсвечиваем поле
                return;
            }

            // --- Логика редактирования клиента ---
            // Проверка корректности возраста
            if (!int.TryParse(txtAge.Text, out int age))
            {
                MessageBox.Show("Некорректный возраст.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Проверка корректности LTV (заменяем точку на запятую для корректного парсинга decimal)
            if (!decimal.TryParse(txtLTV.Text.Replace('.', ','), out decimal ltv))
            {
                MessageBox.Show("Некорректный LTV.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Проверка выбора статуса
            if (comboBoxStatus.SelectedValue == null)
            {
                MessageBox.Show("Выберите статус клиента.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int statusId;

            try
            {
                // Получаем ID выбранного статуса
                statusId = Convert.ToInt32(comboBoxStatus.SelectedValue);
                if (statusId <= 0)
                {
                    MessageBox.Show("Неправильный ID статуса. Выберите корректный статус.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            catch
            {
                MessageBox.Show("Не удалось определить ID статуса. Проверьте привязку ComboBox.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DateTime birthday = dateTimePicker1.Value;
            UpdateClientInDb(age, ltv, statusId, birthday);

            // Возвращаемся на предыдущую форму
            if (Owner != null)
            {
                Owner.ShowDialog(); // Показываем предыдущую форму
            }
            this.Visible = false;
            this.Close(); // Закрываем текущую форму
        }

        // Метод обновления данных клиента в базе данных
        private void UpdateClientInDb(int age, decimal ltv, int statusId, DateTime birthday)
        {
            // SQL запрос на обновление всех полей клиента
            string updateQuery = @"
                UPDATE mydb.clients
                SET FullName_client = @FullName,
                    phone = @Phone,
                    Age = @Age,
                    Status_client_ID_Status_client = @IDStatus,
                    LTV = @LTV,
                    Birthday = @Birthday
                WHERE ID_Client = @IDClient;";

            try
            {
                // Создаем подключение и выполняем запрос с параметрами
                using (MySqlConnection conn = new MySqlConnection(ConnectionString))
                using (MySqlCommand cmd = new MySqlCommand(updateQuery, conn))
                {
                    // Добавляем все параметры с соответствующими типами данных
                    cmd.Parameters.Add("@FullName", MySqlDbType.VarChar, 100).Value = txtFullName_client.Text.Trim();
                    cmd.Parameters.Add("@Phone", MySqlDbType.VarChar, 50).Value = maskedTextBox1.Text.Trim();
                    cmd.Parameters.Add("@Age", MySqlDbType.Int32).Value = age;
                    cmd.Parameters.Add("@IDStatus", MySqlDbType.Int32).Value = statusId;
                    cmd.Parameters.Add("@LTV", MySqlDbType.Decimal).Value = ltv;
                    cmd.Parameters.Add("@Birthday", MySqlDbType.Date).Value = birthday;

                    cmd.Parameters.Add("@IDClient", MySqlDbType.Int32).Value = _clientId;

                    conn.Open();
                    int affected = cmd.ExecuteNonQuery();
                    // Проверяем, были ли изменены данные
                    if (affected > 0)
                    {
                        MessageBox.Show("Данные клиента сохранены.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Закрываем форму с результатом OK — вызывающая форма обновит таблицу
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Обновление не внесло изменений.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Ошибка БД при обновлении: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Настройка ограничений и валидации для элементов управления формы
        private void SetupFormConstraints()
        {
            // Удаляем валидацию с txtPhone
            // TextBoxFilters.InputValidators.ApplyPhoneValidation(txtPhone);
            // TextBoxFilters.InputValidators.ApplyNotEmptyValidation(txtPhone);

            // Применяем валидацию для текстовых полей
            // Ограничиваем ввод только русскими буквами в поле ФИО
            TextBoxFilters.InputValidators.ApplyRussianLettersOnly(txtFullName_client);
            TextBoxFilters.InputValidators.ApplyNotEmptyValidation(txtFullName_client);

            // Разрешаем ввод только цифр и десятичного разделителя в поле возраста
            TextBoxFilters.InputValidators.ApplyNumericWithDecimal(txtAge);
            TextBoxFilters.InputValidators.ApplyNotEmptyValidation(txtAge);

            // Разрешаем ввод только цифр и десятичного разделителя в поле LTV
            TextBoxFilters.InputValidators.ApplyNumericWithDecimal(txtLTV);
            TextBoxFilters.InputValidators.ApplyNotEmptyValidation(txtLTV);

            // Проверяем, что выбран статус
            TextBoxFilters.InputValidators.ApplyNotEmptyValidation(comboBoxStatus);

            // Устанавливаем ограничения для dateTimePicker1
            dateTimePicker1.MaxDate = DateTime.Now.AddDays(-1).AddYears(-18); // Вчерашняя дата - 18 лет (минимальный возраст 18 лет)
            dateTimePicker1.Value = dateTimePicker1.MaxDate; // Устанавливаем значение по умолчанию
        }

        // Обработчик некорректного ввода в маскированном поле телефона
        private void maskedTextBox1_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {
            buttonEditClient.Enabled = true;
        }

        // Обработчик кнопки возврата к списку клиентов
        private void button1_Click(object sender, EventArgs e)
        {
            FormViewClients formViewClients = new FormViewClients();
            this.Visible = false;
            formViewClients.ShowDialog();
            this.Close();
        }

        // Пустой обработчик загрузки формы (можно удалить, если не используется)
        private void FormManagerEditClients_Load(object sender, EventArgs e)
        {

        }
    }
}
