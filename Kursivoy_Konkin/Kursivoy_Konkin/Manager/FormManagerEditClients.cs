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
        private string ConnectionString = connect.con;
        private string _selectedImagePath = string.Empty;
        private int _clientId = 0; // хранит текущий ID клиента для сохранения

        public FormManagerEditClients()
        {
            InitializeComponent();
            SetupFormConstraints();
            // Подписки на кнопки
            this.buttonEditClient.Click += buttonEditClient_Click;
            this.buttonEditClientPhoto.Click += buttonEditClientPhoto_Click;
            this.buttonDeleteClientPhoto.Click += buttonDeleteClientPhoto_Click;
        }

        // Загружаем список статусов в comboBox (можно вызывать перед LoadClientById)
        public void LoadStatusCombo()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionString))
                using (MySqlCommand cmd = new MySqlCommand("SELECT ID_Status_client, status FROM status_client ORDER BY ID_Status_client", conn))
                using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                {
                    var dt = new System.Data.DataTable();
                    da.Fill(dt);
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

            string query = @"SELECT FullName_client, phone, Age, Status_client_ID_Status_client, Qualified_lead, LTV, photo_clients
                             FROM mydb.clients WHERE ID_Client = @id LIMIT 1;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionString))
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", clientId);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            MessageBox.Show("Клиент не найден.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        txtFullName_client.Text = reader["FullName_client"] == DBNull.Value ? string.Empty : reader["FullName_client"].ToString();
                        maskedTextBox1.Text = reader["phone"] == DBNull.Value ? string.Empty : reader["phone"].ToString();
                        txtAge.Text = reader["Age"] == DBNull.Value ? string.Empty : reader["Age"].ToString();
                        txtQualified_lead.Text = reader["Qualified_lead"] == DBNull.Value ? string.Empty : reader["Qualified_lead"].ToString();
                        txtLTV.Text = reader["LTV"] == DBNull.Value ? string.Empty : reader["LTV"].ToString();

                        int statusId = reader["Status_client_ID_Status_client"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Status_client_ID_Status_client"]);

                        // Устанавливаем DataSource ранее загруженного comboBoxStatus (если ещё не загружен, можно вызвать LoadStatusCombo перед этим методом)
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

                        // Фото
                        string photoPath = reader["photo_clients"] == DBNull.Value ? string.Empty : reader["photo_clients"].ToString();
                        _selectedImagePath = photoPath ?? string.Empty;

                        if (!string.IsNullOrWhiteSpace(_selectedImagePath) && File.Exists(_selectedImagePath))
                        {
                            try
                            {
                                if (pictureBox1.Image != null) { pictureBox1.Image.Dispose(); pictureBox1.Image = null; }
                                pictureBox1.Image = Image.FromFile(_selectedImagePath);
                                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            catch
                            {
                                // Игнорируем ошибки загрузки фото
                            }
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
            // Проверяем все зарегистрированные поля
            if (!TextBoxFilters.InputValidators.ValidateAll(out Control[] invalidControls))
            {
                MessageBox.Show("Заполните все обязательные поля, отмеченные красным!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Проверка длины номера телефона
            string phone = Regex.Replace(maskedTextBox1.Text, @"\s+", ""); // Удаляем все пробелы и пустое пространство
            if (phone.Length != 16)
            {
                MessageBox.Show("Некорректный номер телефона. Поле должно содержать ровно 16 символов (включая форматирование).", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                maskedTextBox1.BackColor = Color.MistyRose; // Подсвечиваем поле
                return;
            }

            // --- 2. Парсинг чисел ---
            if (!int.TryParse(txtAge.Text, out int age))
            {
                MessageBox.Show("Некорректный возраст.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtLTV.Text.Replace('.', ','), out decimal ltv))
            {
                MessageBox.Show("Некорректный LTV.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (comboBoxStatus.SelectedValue == null)
            {
                MessageBox.Show("Выберите статус клиента.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int statusId;
            try
            {
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

            // --- 4. Обновление данных в БД ---
            UpdateClientInDb(age, ltv, statusId);
        }

        private void UpdateClientInDb(int age, decimal ltv, int statusId)
        {
            string updateQuery = @"
                UPDATE mydb.clients
                SET FullName_client = @FullName,
                    phone = @Phone,
                    Age = @Age,
                    Status_client_ID_Status_client = @IDStatus,
                    Qualified_lead = @QLead,
                    LTV = @LTV,
                    photo_clients = @Photo
                WHERE ID_Client = @IDClient;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionString))
                using (MySqlCommand cmd = new MySqlCommand(updateQuery, conn))
                {
                    cmd.Parameters.Add("@FullName", MySqlDbType.VarChar, 100).Value = txtFullName_client.Text.Trim();
                    cmd.Parameters.Add("@Phone", MySqlDbType.VarChar, 50).Value = maskedTextBox1.Text.Trim();
                    cmd.Parameters.Add("@Age", MySqlDbType.Int32).Value = age;
                    cmd.Parameters.Add("@IDStatus", MySqlDbType.Int32).Value = statusId;
                    cmd.Parameters.Add("@QLead", MySqlDbType.VarChar, 50).Value = txtQualified_lead.Text.Trim();
                    cmd.Parameters.Add("@LTV", MySqlDbType.Decimal).Value = ltv;

                    if (string.IsNullOrEmpty(_selectedImagePath))
                        cmd.Parameters.Add("@Photo", MySqlDbType.VarChar).Value = DBNull.Value;
                    else
                        cmd.Parameters.Add("@Photo", MySqlDbType.VarChar).Value = _selectedImagePath;

                    cmd.Parameters.Add("@IDClient", MySqlDbType.Int32).Value = _clientId;

                    conn.Open();
                    int affected = cmd.ExecuteNonQuery();
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

        // Простые обработчики для кнопок работы с фото (если нужны)
        private void buttonEditClientPhoto_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Картинки (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";
                openFileDialog.Title = "Выберите фото клиента";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string path = openFileDialog.FileName;
                    FileInfo fi = new FileInfo(path);
                    if (fi.Length > 2 * 1024 * 1024)
                    {
                        MessageBox.Show("Файл слишком большой. Выберите до 2 МБ.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    _selectedImagePath = path;
                    try
                    {
                        if (pictureBox1.Image != null) { pictureBox1.Image.Dispose(); pictureBox1.Image = null; }
                        pictureBox1.Image = Image.FromFile(_selectedImagePath);
                        pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                    }
                    catch
                    {
                        MessageBox.Show("Не удалось загрузить изображение.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void buttonDeleteClientPhoto_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
            }
            _selectedImagePath = string.Empty;
        }

        private void SetupFormConstraints()
        {
            // Удаляем валидацию с txtPhone
            // TextBoxFilters.InputValidators.ApplyPhoneValidation(txtPhone);
            // TextBoxFilters.InputValidators.ApplyNotEmptyValidation(txtPhone);

            // Применяем валидацию для maskedTextBox1
            //TextBoxFilters.InputValidators.ApplyPhoneValidation(maskedTextBox1);
            //TextBoxFilters.InputValidators.ApplyNotEmptyValidation(maskedTextBox1);

            // Применяем валидацию для текстовых полей
            TextBoxFilters.InputValidators.ApplyRussianLettersOnly(txtFullName_client);
            TextBoxFilters.InputValidators.ApplyNotEmptyValidation(txtFullName_client);

            TextBoxFilters.InputValidators.ApplyNumericWithDecimal(txtAge);
            TextBoxFilters.InputValidators.ApplyNotEmptyValidation(txtAge);

            TextBoxFilters.InputValidators.ApplyNumericWithDecimal(txtLTV);
            TextBoxFilters.InputValidators.ApplyNotEmptyValidation(txtLTV);

            TextBoxFilters.InputValidators.ApplyNotEmptyValidation(comboBoxStatus);
            TextBoxFilters.InputValidators.ApplyRussianLettersOnly(txtQualified_lead);
            TextBoxFilters.InputValidators.ApplyNotEmptyValidation(txtQualified_lead);
        }

        private void maskedTextBox1_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {
            buttonEditClient.Enabled = true;
        }
    }
}
