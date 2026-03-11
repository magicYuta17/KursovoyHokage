
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace Kursivoy_Konkin
{
    // Форма для редактирования данных сотрудника (доступна администратору)
    public partial class FormAdminWorkerEdit : Form
    {
        private int workerId; // ID редактируемого сотрудника
        private string currentPhoto; // Имя текущего файла фото

        public string fileName; // Имя выбранного файла
        public string fullPath; // Полный путь к выбранному файлу

        // Конструктор, принимает ID сотрудника для редактирования
        public FormAdminWorkerEdit(int id)
        {
            InitializeComponent(); // Инициализация компонентов дизайнера
            workerId = id; // Сохраняем ID сотрудника
            this.MinimizeBox = false; // Запрет на сворачивание
            this.MaximizeBox = false; // Запрет на разворачивание
            this.ControlBox = false; // Скрытие системных кнопок
            SetupConstraints(); // Настройка ограничений ввода
            LoadRoles(); // Загрузка списка ролей
            LoadClients(); // Загрузка списка клиентов
            LoadWorkerData(); // загружаем данные ПОСЛЕ ролей и клиентов
        }

        // Метод для настройки ограничений ввода в полях
        private void SetupConstraints()
        {
            // Только русские буквы для ФИО
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

            // Устанавливаем маску для телефона
            mtbPhone.Mask = "+7(000)000-00-00";

            // Подписка на событие изменения текста поиска клиентов
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

                    // Скрываем колонку ID
                    if (dgvClients.Columns.Contains("ID_Client"))
                        dgvClients.Columns["ID_Client"].Visible = false;

                    // Устанавливаем заголовки
                    dgvClients.Columns["FullName_client"].HeaderText = "ФИО клиента";
                    dgvClients.Columns["phone"].HeaderText = "Телефон";

                    // Настройка таблицы
                    dgvClients.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
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
            LoadClients(tbClientSearch.Text.Trim()); // Перезагружаем с фильтром
        }

        // Метод для загрузки данных сотрудника из БД
        private void LoadWorkerData()
        {
            try
            {
                string query = @"
                    SELECT FIO, ID_Clientsl, phone, Age, Role_worker_ID_Role, photo 
                    FROM worker 
                    WHERE ID_worker = @id";

                using (MySqlConnection conn = new MySqlConnection(connect.con))
                {
                    conn.Open(); // Открываем соединение
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = workerId; // Передаем ID

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read()) // Если данные найдены
                            {
                                tbFIO.Text = reader["FIO"].ToString(); // ФИО
                                tbAge.Text = reader["Age"].ToString(); // Возраст

                                // Убираем маску перед заполнением телефона
                                mtbPhone.Text = reader["phone"].ToString();

                                // Устанавливаем роль
                                cbRole.SelectedValue = Convert.ToInt32(reader["Role_worker_ID_Role"]);

                                // Сохраняем текущее фото
                                currentPhoto = reader["photo"].ToString();

                                // Загружаем фото
                                if (!string.IsNullOrEmpty(currentPhoto))
                                {
                                    string photoPath = Path.Combine(
                                        AppDomain.CurrentDomain.BaseDirectory,
                                        "photo_worker", currentPhoto);

                                    if (File.Exists(photoPath))
                                        pictureBox1.Image = new System.Drawing.Bitmap(photoPath);
                                }

                                // Выделяем текущего клиента в dgvClients
                                int clientId = Convert.ToInt32(reader["ID_Clientsl"]);
                                SelectClientInGrid(clientId);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка загрузки данных",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Выделяем нужного клиента в таблице
        private void SelectClientInGrid(int clientId)
        {
            foreach (DataGridViewRow row in dgvClients.Rows)
            {
                if (Convert.ToInt32(row.Cells["ID_Client"].Value) == clientId)
                {
                    row.Selected = true; // Выделяем строку
                    dgvClients.FirstDisplayedScrollingRowIndex = row.Index; // Прокручиваем к ней
                    break;
                }
            }
        }

        // Обработчик кнопки "Отмена" (устаревший, дублируется)
        private void btnCancel_Click(object sender, EventArgs e)
        {
            FormAdminWorker f = new FormAdminWorker(); // Создаем форму списка сотрудников
            this.Visible = false; // Скрываем текущую форму
            f.ShowDialog(); // Показываем форму списка
            this.Close(); // Закрываем текущую форму
        }

        // Обработчик кнопки "Добавить фото"
        private void buttonAddPhoto_Click_1(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
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
                int clientId = Convert.ToInt32(dgvClients.SelectedRows[0].Cells["ID_Client"].Value); // ID клиента
                int roleId = Convert.ToInt32(cbRole.SelectedValue); // ID роли
                int age = Convert.ToInt32(tbAge.Text); // Возраст

                // Обрабатываем фото
                string photoName = currentPhoto; // по умолчанию — старое фото

                if (!string.IsNullOrEmpty(fullPath) && File.Exists(fullPath)) // Если выбрано новое фото
                {
                    string photoDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "photo_worker");
                    if (!Directory.Exists(photoDir))
                        Directory.CreateDirectory(photoDir); // Создаем папку, если её нет

                    // Генерируем уникальное имя для файла
                    string uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(fullPath);
                    File.Copy(fullPath, Path.Combine(photoDir, uniqueName), true); // Копируем файл
                    photoName = uniqueName; // Сохраняем новое имя
                }

                // SQL-запрос на обновление данных сотрудника
                string query = @"
                    UPDATE worker SET
                        FIO                  = @fio,
                        ID_Clientsl          = @clientId,
                        phone                = @phone,
                        Age                  = @age,
                        Role_worker_ID_Role  = @roleId,
                        photo                = @photo
                    WHERE ID_worker = @id";

                using (MySqlConnection conn = new MySqlConnection(connect.con))
                {
                    conn.Open(); // Открываем соединение
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // Добавляем параметры
                        cmd.Parameters.Add("@fio", MySqlDbType.VarChar, 100).Value = tbFIO.Text.Trim();
                        cmd.Parameters.Add("@clientId", MySqlDbType.VarChar, 45).Value = clientId.ToString();
                        cmd.Parameters.Add("@phone", MySqlDbType.VarChar, 45).Value = mtbPhone.Text;
                        cmd.Parameters.Add("@age", MySqlDbType.Int32).Value = age;
                        cmd.Parameters.Add("@roleId", MySqlDbType.Int32).Value = roleId;
                        cmd.Parameters.Add("@photo", MySqlDbType.VarChar, 100).Value =
                            (object)photoName ?? DBNull.Value; // Если фото нет, записываем NULL
                        cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = workerId;

                        cmd.ExecuteNonQuery(); // Выполняем запрос
                    }
                }

                MessageBox.Show("Данные сотрудника обновлены!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close(); // Закрываем форму
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка сохранения",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обработчик кнопки "Удалить фото"
        private void buttonDeletePhoto_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = Properties.Resources.picture; // Устанавливаем изображение по умолчанию
            fullPath = null; // Сбрасываем путь
            fileName = null; // Сбрасываем имя файла
            currentPhoto = null; // при сохранении запишется NULL в БД
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
