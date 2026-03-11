
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Kursivoy_Konkin
{
    // Форма для добавления нового клиента менеджером
    public partial class FormManagerAddClient : Form
    {
        // Строка подключения к базе данных из внешнего источника
        private string ConnectionString = connect.con;
        // Путь к выбранному изображению (фото клиента) - пока не используется
        private string _selectedImagePath = string.Empty;

        // Конструктор формы
        public FormManagerAddClient()
        {
            InitializeComponent(); // Инициализация компонентов дизайнера
            SetupFormConstraints(); // Настройка ограничений для полей ввода
            LoadStatusCombo(); // Загрузка статусов при инициализации
            this.MinimizeBox = false; // Запрет на сворачивание
            this.MaximizeBox = false; // Запрет на разворачивание
            this.ControlBox = false; // Скрытие системных кнопок
        }
        // =========================================================
        // 1. НАСТРОЙКА ОГРАНИЧЕНИЙ (ТОЛЬКО РУССКИЙ, ЦИФРЫ И Т.Д.)
        // =========================================================
        private void SetupFormConstraints()
        {
            // Удаляем валидацию с txtPhone (закомментировано)
            // TextBoxFilters.InputValidators.ApplyPhoneValidation(txtPhone);
            // TextBoxFilters.InputValidators.ApplyNotEmptyValidation(txtPhone);



            // Применяем валидацию: только русские буквы для ФИО
            TextBoxFilters.InputValidators.ApplyRussianLettersOnly(txtFullName_client);
            TextBoxFilters.InputValidators.ApplyNotEmptyValidation(txtFullName_client); // Обязательное поле

            // Только цифры для возраста (с возможностью десятичных)
            TextBoxFilters.InputValidators.ApplyNumericWithDecimal(txtAge);
            TextBoxFilters.InputValidators.ApplyNotEmptyValidation(txtAge); // Обязательное поле

            // Только цифры для LTV (с возможностью десятичных)
            TextBoxFilters.InputValidators.ApplyNumericWithDecimal(txtLTV);
            TextBoxFilters.InputValidators.ApplyNotEmptyValidation(txtLTV); // Обязательное поле

            // Обязательное поле для выпадающего списка статусов
            TextBoxFilters.InputValidators.ApplyNotEmptyValidation(comboBoxStatus);

            // Устанавливаем ограничения для dateTimePicker1 (дата рождения)
            dateTimePicker1.MaxDate = DateTime.Now.AddDays(-1).AddYears(-18); // Вчерашняя дата - 18 лет (клиенту должно быть >=18)
            dateTimePicker1.Value = dateTimePicker1.MaxDate; // Устанавливаем значение по умолчанию
        }

        // --- Методы фильтрации клавиш (альтернативная ручная валидация) ---
        // Только русские буквы
        private void InputOnlyRussian_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Разрешаем Backspace, Пробел и кириллицу
            if (char.IsControl(e.KeyChar) || char.IsWhiteSpace(e.KeyChar)) return;
            if ((e.KeyChar < 'А' || e.KeyChar > 'я') && e.KeyChar != 'ё' && e.KeyChar != 'Ё') e.Handled = true;
        }

        // Только цифры
        private void InputOnlyDigits_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true;
        }

        // Для ввода телефона (цифры и спецсимволы +-() )
        private void InputPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
                e.KeyChar != '+' && e.KeyChar != '-' && e.KeyChar != '(' && e.KeyChar != ')' && e.KeyChar != ' ') e.Handled = true;
        }

        // Для ввода десятичных чисел
        private void InputDecimal_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != ',' && e.KeyChar != '.') { e.Handled = true; return; }
            if (e.KeyChar == '.') e.KeyChar = ','; // Заменяем точку на запятую
            if (e.KeyChar == ',' && (sender as TextBox).Text.Contains(",")) e.Handled = true; // Только одна запятая
        }


        // Заглушка для кнопки добавления фото (не реализовано)
        private void buttonAddClientPhoto_Click(object sender, EventArgs e)
        {

        }

        // Заглушка для кнопки удаления фото (не реализовано)
        private void buttonDeleteClientPhoto_Click(object sender, EventArgs e)
        {

        }

        // =========================================================
        // 3. КНОПКА: ДОБАВИТЬ КЛИЕНТА В БД (ОСНОВНАЯ ЛОГИКА)
        // =========================================================


        // --- Вспомогательный метод: Проверка дубля клиента ---
        private bool IsClientDuplicate(string fullName, string phone)
        {
            // SQL-запрос для подсчета клиентов с таким же ФИО и телефоном (не удаленных)
            string query = "SELECT COUNT(*) FROM Clients WHERE FullName_client = @FullName AND phone = @Phone AND IsDeleted = 0";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open(); // Открываем соединение
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@FullName", fullName); // Передаем ФИО
                        cmd.Parameters.AddWithValue("@Phone", phone); // Передаем телефон
                        return Convert.ToInt64(cmd.ExecuteScalar()) > 0; // Возвращаем true, если найдены записи
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка подключения при проверке: " + ex.Message);
                return true; // Блокируем добавление, если БД не отвечает
            }
        }

        // --- Вспомогательный метод: Вставка клиента в БД ---
        private void InsertClientToDb(int age, decimal ltv, DateTime birthday)
        {
            // SQL-запрос для вставки нового клиента
            string query = @"
                INSERT INTO Clients 
                (FullName_client, phone, Age, Status_client_ID_Status_client,LTV, Birthday) 
                VALUES 
                (@FullName, @Phone, @Age, @IDStatus, @LTV, @Birthday)";

            // Проверим выбранный статус и получим его ID
            if (comboBoxStatus.SelectedValue == null)
            {
                MessageBox.Show("Выберите статус клиента.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int statusId;
            try
            {
                statusId = Convert.ToInt32(comboBoxStatus.SelectedValue); // Преобразуем ID статуса
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
                    conn.Open(); // Открываем соединение
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // Добавляем параметры со значениями из полей формы
                        cmd.Parameters.Add("@FullName", MySqlDbType.VarChar, 100).Value = txtFullName_client.Text.Trim();
                        cmd.Parameters.Add("@Phone", MySqlDbType.VarChar, 50).Value = maskedTextBox1.Text.Trim();
                        cmd.Parameters.Add("@Age", MySqlDbType.Int32).Value = age;
                        cmd.Parameters.Add("@IDStatus", MySqlDbType.Int32).Value = statusId;
                        cmd.Parameters.Add("@LTV", MySqlDbType.Decimal).Value = ltv;
                        cmd.Parameters.Add("@Birthday", MySqlDbType.Date).Value = birthday;

                        int rows = cmd.ExecuteNonQuery(); // Выполняем вставку
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

        // Очистка всех полей формы (не используется в текущей логике)
        private void ClearAllFields()
        {
            txtFullName_client.Clear();
            maskedTextBox1.Clear();
            txtAge.Clear();


            txtLTV.Clear();
            buttonDeleteClientPhoto_Click(null, null); // Сброс фото
        }




        // Пример загрузки выпадающего списка статусов
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
                    da.Fill(dt); // Заполняем DataTable данными
                    comboBoxStatus.DisplayMember = "status"; // Отображаемое поле
                    comboBoxStatus.ValueMember = "ID_Status_client"; // Скрытое значение (ID)
                    comboBoxStatus.DataSource = dt; // Устанавливаем источник данных
                    comboBoxStatus.SelectedIndex = -1; // Не выбирать по умолчанию
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось загрузить статусы: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        // Обработчик кнопки "Добавить"
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            // Перед добавлением данных в базу
            Control[] invalidControls;
            if (!TextBoxFilters.InputValidators.ValidateAll(out invalidControls))
            {
                // Если есть некорректные поля, показываем сообщение и выходим
                MessageBox.Show("Пожалуйста, заполните все поля корректно.", "Ошибка валидации", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Прерываем выполнение метода
            }

            // Проверка длины номера телефона
            string phone = Regex.Replace(maskedTextBox1.Text, @"\s+", ""); // Удаляем все пробелы
            if (phone.Length != 16)
            {
                MessageBox.Show("Некорректный номер телефона. Поле должно содержать ровно 16 символов (включая форматирование).", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                maskedTextBox1.BackColor = Color.MistyRose; // Подсвечиваем поле розовым
                return;
            }

            // --- Остальная логика ---
            // --- 1. Проверка заполненности ---
            if (string.IsNullOrWhiteSpace(txtFullName_client.Text) ||
                string.IsNullOrWhiteSpace(maskedTextBox1.Text) ||
                string.IsNullOrWhiteSpace(txtAge.Text) ||
                string.IsNullOrWhiteSpace(comboBoxStatus.Text) ||
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
            if (IsClientDuplicate(txtFullName_client.Text, maskedTextBox1.Text))
            {
                MessageBox.Show("Такой клиент уже существует!", "Дубликат", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            // --- 5. Вставка ---
            InsertClientToDb(age, ltv, dateTimePicker1.Value);
        }

        // Альтернативный обработчик кнопки добавления (дублирует логику buttonAdd_Click)
        private void buttonAddClient_Click(object sender, EventArgs e)
        {
            // Проверяем все зарегистрированные поля
            if (!TextBoxFilters.InputValidators.ValidateAll(out Control[] invalidControls))
            {
                MessageBox.Show("Заполните все обязательные поля, отмеченные красным!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Проверка длины номера телефона
            string phone = Regex.Replace(maskedTextBox1.Text, @"\s+", ""); // Удаляем все пробелы
            if (phone.Length != 16)
            {
                MessageBox.Show("Некорректный номер телефона. Поле должно содержать ровно 16 символов (включая форматирование).", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                maskedTextBox1.BackColor = Color.MistyRose; // Подсвечиваем поле
                return;
            }

            // --- Логика добавления клиента ---
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

            if (IsClientDuplicate(txtFullName_client.Text, maskedTextBox1.Text))
            {
                MessageBox.Show("Такой клиент уже существует!", "Дубликат", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            // Получаем дату из dateTimePicker1
            DateTime birthday = dateTimePicker1.Value;

            // Вставляем данные в БД
            InsertClientToDb(age, ltv, birthday);

            // Возвращаемся на предыдущую форму
            if (Owner != null)
            {
                Owner.Show(); // Показываем предыдущую форму
            }
            this.Close(); // Закрываем текущую форму
        }

        // Обработчик кнопки "Назад" (закрыть форму)
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close(); // Просто закрываем текущую форму
        }

        // Загрузчик формы (дублирует настройки из конструктора)
        private void FormManagerAddClient_Load(object sender, EventArgs e)
        {
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.ControlBox = false;
        }
    }
}
