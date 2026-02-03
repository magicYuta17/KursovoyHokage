using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions; 
using System.Windows.Forms;
using MySql.Data.MySqlClient; 

namespace Kursivoy_Konkin
{
    public partial class FormManagerAddClient : Form
    {
        private string ConnectionString = connect.con;
        private string _selectedImagePath = string.Empty;

        public FormManagerAddClient()
        {
            InitializeComponent();
            SetupFormConstraints();
            LoadStatusCombo(); // Загрузка статусов при инициализации
        }
        // =========================================================
        // 1. НАСТРОЙКА ОГРАНИЧЕНИЙ (ТОЛЬКО РУССКИЙ, ЦИФРЫ И Т.Д.)
        // =========================================================
        private void SetupFormConstraints()
        {
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

            // Применяем валидацию для телефона
            TextBoxFilters.InputValidators.ApplyPhoneValidation(txtPhone);
            TextBoxFilters.InputValidators.ApplyNotEmptyValidation(txtPhone);
        }

        // --- Методы фильтрации клавиш ---
        private void InputOnlyRussian_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Разрешаем Backspace, Пробел и кириллицу
            if (char.IsControl(e.KeyChar) || char.IsWhiteSpace(e.KeyChar)) return;
            if ((e.KeyChar < 'А' || e.KeyChar > 'я') && e.KeyChar != 'ё' && e.KeyChar != 'Ё') e.Handled = true;
        }

        private void InputOnlyDigits_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true;
        }

        private void InputPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
                e.KeyChar != '+' && e.KeyChar != '-' && e.KeyChar != '(' && e.KeyChar != ')' && e.KeyChar != ' ') e.Handled = true;
        }

        private void InputDecimal_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != ',' && e.KeyChar != '.') { e.Handled = true; return; }
            if (e.KeyChar == '.') e.KeyChar = ','; // Заменяем точку на запятую
            if (e.KeyChar == ',' && (sender as TextBox).Text.Contains(",")) e.Handled = true; // Только одна запятая
        }

       
        private void buttonAddClientPhoto_Click(object sender, EventArgs e)
        {
           
        }

        
        private void buttonDeleteClientPhoto_Click(object sender, EventArgs e)
        {
           
        }

        // =========================================================
        // 3. КНОПКА: ДОБАВИТЬ КЛИЕНТА В БД
        // =========================================================
        private void buttonAddClient_Click(object sender, EventArgs e)
        {
            // Проверяем все зарегистрированные поля
            if (!TextBoxFilters.InputValidators.ValidateAll(out Control[] invalidControls))
            {
                MessageBox.Show("Заполните все обязательные поля, отмеченные красным!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Проверка номера телефона
            string phone = txtPhone.Text.Replace(" ", "");
            if (!(phone.StartsWith("+7") && phone.Length == 12) && !(phone.StartsWith("8") && phone.Length == 11))
            {
                MessageBox.Show("Некорректный номер телефона. Номер должен начинаться с '+7' или '8' и содержать правильное количество цифр.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPhone.BackColor = Color.MistyRose; // или другой цвет, который вы используете для ошибок
                return;
            }

            // --- Остальная логика ---
            // --- 1. Проверка заполненности ---
            if (string.IsNullOrWhiteSpace(txtFullName_client.Text) ||
                string.IsNullOrWhiteSpace(txtPhone.Text) ||
                string.IsNullOrWhiteSpace(txtAge.Text) ||
                string.IsNullOrWhiteSpace(comboBoxStatus.Text) ||
                string.IsNullOrWhiteSpace(txtQualified_lead.Text) ||               
                string.IsNullOrWhiteSpace(txtLTV.Text))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // --- 2. Финальная проверка на Русские буквы (защита от вставки Ctrl+V) ---
            if (!Regex.IsMatch(txtFullName_client.Text, @"^[а-яА-ЯёЁ\s]+$"))
            {
                MessageBox.Show("ФИО должно содержать только русские буквы!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // --- 3. Парсинг чисел ---
            if (!int.TryParse(txtAge.Text, out int age))
            {
                MessageBox.Show("Некорректный возраст.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
            }
            if (!decimal.TryParse(txtLTV.Text.Replace('.', ','), out decimal ltv))
            {
                MessageBox.Show("Некорректный LTV.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
            }
            

            // --- 4. Проверка на дубликаты в БД ---
            if (IsClientDuplicate(txtFullName_client.Text, txtPhone.Text))
            {
                MessageBox.Show("Такой клиент уже существует!", "Дубликат", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            // --- 5. Вставка ---
            InsertClientToDb(age, ltv);
        }

        // --- Вспомогательный метод: Проверка дубля ---
        private bool IsClientDuplicate(string fullName, string phone)
        {

            string query = "SELECT COUNT(*) FROM Clients WHERE FullName_client = @FullName AND phone = @Phone AND IsDeleted = 0";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@FullName", fullName);
                        cmd.Parameters.AddWithValue("@Phone", phone);
                        return Convert.ToInt64(cmd.ExecuteScalar()) > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка подключения при проверке: " + ex.Message);
                return true; // Блокируем, если БД не отвечает
            }
        }

        // --- Вспомогательный метод: SQL Insert ---
        private void InsertClientToDb(int age, decimal ltv)
        {
            string query = @"
                INSERT INTO Clients 
                (FullName_client, phone, Age, Status_client_ID_Status_client, Qualified_lead, LTV, photo_clients) 
                VALUES 
                (@FullName, @Phone, @Age, @IDStatus, @QLead, @LTV, @Photo)";

            // Проверим выбранный статус и получим int ID
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

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@FullName", MySqlDbType.VarChar, 100).Value = txtFullName_client.Text.Trim();
                        cmd.Parameters.Add("@Phone", MySqlDbType.VarChar, 50).Value = txtPhone.Text.Trim();
                        cmd.Parameters.Add("@Age", MySqlDbType.Int32).Value = age;
                        cmd.Parameters.Add("@IDStatus", MySqlDbType.Int32).Value = statusId;
                        cmd.Parameters.Add("@QLead", MySqlDbType.VarChar, 50).Value = txtQualified_lead.Text.Trim();
                        cmd.Parameters.Add("@LTV", MySqlDbType.Decimal).Value = ltv;

                        if (string.IsNullOrEmpty(_selectedImagePath))
                            cmd.Parameters.Add("@Photo", MySqlDbType.VarChar).Value = DBNull.Value;
                        else
                            cmd.Parameters.Add("@Photo", MySqlDbType.VarChar).Value = _selectedImagePath;

                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            MessageBox.Show("Клиент успешно добавлен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Закрываем форму с результатом OK — вызывающая форма обновит таблицу
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Ошибка БД: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearAllFields()
        {
            txtFullName_client.Clear();
            txtPhone.Clear();
            txtAge.Clear();
            
            txtQualified_lead.Clear();
            
            txtLTV.Clear();
            buttonDeleteClientPhoto_Click(null, null); // Сброс фото
        }


        // =========================================================
        // 2. КНОПКИ РАБОТЫ С ФОТО
        // =========================================================

        // КНОПКА: Добавить фото
        private void buttonAddClientPhoto_Click_1(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Картинки (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";
                openFileDialog.Title = "Выберите фото клиента";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    FileInfo fileInfo = new FileInfo(openFileDialog.FileName);

                    // Проверка размера (2 МБ)
                    if (fileInfo.Length > 2 * 1024 * 1024)
                    {
                        MessageBox.Show("Файл слишком большой! Выберите фото до 2 МБ.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    _selectedImagePath = openFileDialog.FileName;

                    // Очистка памяти перед загрузкой новой
                    if (pictureBox1.Image != null) pictureBox1.Image.Dispose();

                    try
                    {
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

        // КНОПКА: Удалить фото
        private void buttonDeleteClientPhoto_Click_1(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
            }
            _selectedImagePath = string.Empty;
        }

        private void buttonAddClient_Click_1(object sender, EventArgs e)
        {
            // Проверяем все зарегистрированные поля
            if (!TextBoxFilters.InputValidators.ValidateAll(out Control[] invalidControls))
            {
                MessageBox.Show("Заполните все обязательные поля, отмеченные красным!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            // --- 3. Проверка на дубликаты в БД ---
            if (IsClientDuplicate(txtFullName_client.Text, txtPhone.Text))
            {
                MessageBox.Show("Такой клиент уже существует!", "Дубликат", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            // --- 4. Вставка данных в БД ---
            InsertClientToDb(age, ltv);
        }

        // Пример загрузки ComboBox (выполнить один раз, например в конструкторе после InitializeComponent)
        private void LoadStatusCombo()
        {
            // Загружаем таблицу статусов в comboBox
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
                    comboBoxStatus.SelectedIndex = -1; // не выбирать по умолчанию
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось загрузить статусы: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

    

