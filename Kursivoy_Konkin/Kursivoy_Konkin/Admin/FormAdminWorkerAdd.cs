
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace Kursivoy_Konkin
{
    // Форма для добавления нового сотрудника (доступна администратору)
    public partial class FormAdminWorkerAdd : Form
    {
        public string fileName; // Имя выбранного файла с фото
        public string fullPath; // Полный путь к выбранному файлу с фото

        // Конструктор формы
        public FormAdminWorkerAdd()
        {
            InitializeComponent(); // Инициализация компонентов дизайнера
            this.MinimizeBox = false; // Запрет на сворачивание
            this.MaximizeBox = false; // Запрет на разворачивание
            this.ControlBox = false; // Скрытие системных кнопок
            SetupConstraints(); // Настройка ограничений ввода
            LoadRoles(); // Загрузка списка ролей
            LoadClients(); // Загрузка списка клиентов
        }

        // Метод для настройки ограничений ввода в полях
        private void SetupConstraints()
        {
            // Только русские буквы и пробел для ФИО
            tbFIO.KeyPress += (s, e) =>
            {
                char c = e.KeyChar;
                // Разрешаем русские буквы (в обоих регистрах), пробел и backspace
                bool isRussian = (c >= 'а' && c <= 'я') || (c >= 'А' && c <= 'Я') || c == 'ё' || c == 'Ё';
                bool isAllowed = isRussian || c == ' ' || c == (char)Keys.Back;
                if (!isAllowed) e.Handled = true; // Блокируем ввод
            };

            // Только цифры для возраста
            tbAge.KeyPress += (s, e) =>
            {
                if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
                    e.Handled = true; // Блокируем ввод, если не цифра и не backspace
            };
            tbAge.MaxLength = 3; // Максимум 3 символа (до 999 лет)

            // Устанавливаем маску для телефона (формат +7(XXX)XXX-XX-XX)
            mtbPhone.Mask = "+7(000)000-00-00";

            // Поиск клиента по TextBox - подписка на событие изменения текста
            tbClientSearch.TextChanged += TbClientSearch_TextChanged;
        }

        // Метод для загрузки ролей из БД в выпадающий список
        private void LoadRoles()
        {
            try
            {
                string query = "SELECT ID_Role, Role FROM role_worker";
                using (MySqlConnection conn = new MySqlConnection(connect.con))
                {
                    conn.Open(); // Открываем соединение
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt); // Заполняем DataTable

                    // Привязываем данные к ComboBox
                    cbRole.DataSource = dt;
                    cbRole.DisplayMember = "Role"; // Отображаемое поле
                    cbRole.ValueMember = "ID_Role"; // Значение (ID)
                    cbRole.SelectedIndex = -1; // Ничего не выбрано по умолчанию
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка загрузки ролей",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Метод для загрузки клиентов из БД с возможностью поиска
        private void LoadClients(string search = "")
        {
            try
            {
                // Запрос на выборку активных клиентов с фильтром по ФИО
                string query = @"
                    SELECT ID_Client, FullName_client, phone 
                    FROM clients 
                    WHERE IsDeleted = 0 
                    AND FullName_client LIKE @search";

                using (MySqlConnection conn = new MySqlConnection(connect.con))
                {
                    conn.Open(); // Открываем соединение
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    // Добавляем параметр поиска с маской
                    adapter.SelectCommand.Parameters.Add("@search", MySqlDbType.VarChar).Value = $"%{search}%";

                    DataTable dt = new DataTable();
                    adapter.Fill(dt); // Заполняем DataTable

                    dgvClients.DataSource = dt; // Устанавливаем источник данных

                    // Скрываем колонку ID (она нужна для работы, но не должна отображаться)
                    if (dgvClients.Columns.Contains("ID_Client"))
                        dgvClients.Columns["ID_Client"].Visible = false;

                    // Устанавливаем заголовки колонок
                    dgvClients.Columns["FullName_client"].HeaderText = "ФИО клиента";
                    dgvClients.Columns["phone"].HeaderText = "Телефон";

                    // Настройка таблицы
                    dgvClients.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Автоматическая ширина
                    dgvClients.ReadOnly = true; // Только для чтения
                    dgvClients.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // Выделение всей строки
                    dgvClients.AllowUserToAddRows = false; // Запрет добавления строк
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка загрузки клиентов",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обработчик изменения текста поиска клиентов
        private void TbClientSearch_TextChanged(object sender, EventArgs e)
        {
            LoadClients(tbClientSearch.Text.Trim()); // Перезагружаем список с фильтром
        }

        // Устаревший обработчик кнопки "Отмена" (дублируется)
        private void btnCancel_Click(object sender, EventArgs e)
        {
            FormAdminWorker f = new FormAdminWorker(); // Создаем форму списка сотрудников
            this.Visible = false; // Скрываем текущую форму
            f.ShowDialog(); // Показываем форму списка
            this.Close(); // Закрываем текущую форму
        }

        // Обработчик кнопки "Удалить фото"
        private void buttonDeletePhoto_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = Properties.Resources.picture; // Устанавливаем изображение по умолчанию
            fullPath = null; // Сбрасываем путь
            fileName = null; // Сбрасываем имя файла
        }

        // Обработчик кнопки "Сохранить"
        private void btnSave_Click(object sender, EventArgs e)
        {
            // Валидация полей
            if (string.IsNullOrWhiteSpace(tbFIO.Text))
            {
                MessageBox.Show("Введите ФИО сотрудника.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(tbAge.Text))
            {
                MessageBox.Show("Введите возраст.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (mtbPhone.MaskCompleted == false) // Проверка полноты ввода номера
            {
                MessageBox.Show("Введите корректный номер телефона.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cbRole.SelectedIndex == -1) // Проверка выбора роли
            {
                MessageBox.Show("Выберите роль.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dgvClients.SelectedRows.Count == 0) // Проверка выбора клиента
            {
                MessageBox.Show("Выберите клиента из списка.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Получаем ID выбранного клиента из скрытой колонки
                int clientId = Convert.ToInt32(dgvClients.SelectedRows[0].Cells["ID_Client"].Value);
                int roleId = Convert.ToInt32(cbRole.SelectedValue); // Получаем ID роли
                int age = Convert.ToInt32(tbAge.Text); // Преобразуем возраст в число

                // Сохраняем фото если выбрано
                string photoName = null;
                if (!string.IsNullOrEmpty(fullPath) && File.Exists(fullPath)) // Если фото выбрано
                {
                    string photoDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "photo_worker");
                    if (!Directory.Exists(photoDir))
                        Directory.CreateDirectory(photoDir); // Создаем папку, если её нет

                    // Генерируем уникальное имя для файла, чтобы избежать конфликтов
                    string uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(fullPath);
                    File.Copy(fullPath, Path.Combine(photoDir, uniqueName), true); // Копируем файл
                    photoName = uniqueName; // Сохраняем имя для записи в БД
                }

                // SQL-запрос для вставки нового сотрудника
                string query = @"
                    INSERT INTO worker (FIO, ID_Clientsl, phone, Age, Role_worker_ID_Role, IsDeleted, photo)
                    VALUES (@fio, @clientId, @phone, @age, @roleId, 0, @photo)";

                using (MySqlConnection conn = new MySqlConnection(connect.con))
                {
                    conn.Open(); // Открываем соединение
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // Добавляем параметры со значениями из полей формы
                        cmd.Parameters.Add("@fio", MySqlDbType.VarChar, 100).Value = tbFIO.Text.Trim();
                        cmd.Parameters.Add("@clientId", MySqlDbType.VarChar, 45).Value = clientId.ToString();
                        cmd.Parameters.Add("@phone", MySqlDbType.VarChar, 45).Value = mtbPhone.Text;
                        cmd.Parameters.Add("@age", MySqlDbType.Int32).Value = age;
                        cmd.Parameters.Add("@roleId", MySqlDbType.Int32).Value = roleId;
                        cmd.Parameters.Add("@photo", MySqlDbType.VarChar, 100).Value =
                            (object)photoName ?? DBNull.Value; // Если фото нет, записываем NULL

                        cmd.ExecuteNonQuery(); // Выполняем запрос на вставку
                    }
                }

                MessageBox.Show("Сотрудник успешно добавлен!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close(); // Закрываем форму
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка сохранения",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обработчик кнопки "Добавить фото"
        private void buttonAddPhoto_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                // Фильтр для изображений
                openFileDialog.Filter = "Image Files (*.jpg; *.jpeg; *.png)|*.jpg;*.jpeg;*.png";
                openFileDialog.Title = "Выберите изображение";

                if (openFileDialog.ShowDialog() == DialogResult.OK) // Если файл выбран
                {
                    FileInfo fileInfo = new FileInfo(openFileDialog.FileName);

                    // Проверяем расширение и размер файла (до 2 МБ)
                    if ((fileInfo.Extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
                         fileInfo.Extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                         fileInfo.Extension.Equals(".png", StringComparison.OrdinalIgnoreCase)) &&
                        fileInfo.Length <= 2 * 1024 * 1024)
                    {
                        pictureBox1.Image = new System.Drawing.Bitmap(openFileDialog.FileName); // Отображаем фото
                        fileName = fileInfo.Name; // Сохраняем имя файла
                        fullPath = openFileDialog.FileName; // Сохраняем полный путь
                    }
                    else
                    {
                        MessageBox.Show("Выберите файл JPG или PNG размером не более 2 Мб.",
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // Обработчик кнопки "Отмена" (основной)
        private void btnCancel_Click_1(object sender, EventArgs e)
        {
            FormAdminWorker f = new FormAdminWorker(); // Создаем форму списка сотрудников
            this.Visible = false; // Скрываем текущую форму
            f.ShowDialog(); // Показываем форму списка
            this.Close(); // Закрываем текущую форму
        }
    }
}
