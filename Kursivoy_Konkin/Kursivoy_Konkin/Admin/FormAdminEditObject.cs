
using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Windows.Forms;

namespace Kursivoy_Konkin
{
    // Форма для редактирования существующего объекта недвижимости (доступна администратору)
    public partial class FormAdminEditObject : Form
    {
        private int objectId; // ID редактируемого объекта
        private string currentPhoto; // Имя текущего файла фото

        public string fileName; // Имя выбранного файла
        public string fullPath; // Полный путь к выбранному файлу

        // Конструктор, принимает ID объекта для редактирования
        public FormAdminEditObject(int id)
        {
            InitializeComponent(); // Инициализация компонентов дизайнера
            objectId = id; // Сохраняем ID объекта
            SetupFormConstraints(); // Настройка ограничений ввода
            LoadObjectData(); // Загрузка данных объекта
            this.MinimizeBox = false; // Запрет на сворачивание
            this.MaximizeBox = false; // Запрет на разворачивание
        }

        // Метод для настройки ограничений ввода в полях
        private void SetupFormConstraints()
        {
            // Применяем валидацию "обязательное поле" для всех текстовых полей
            TextBoxFilters.InputValidators.ApplyNotEmptyValidation(txt_Square);
            TextBoxFilters.InputValidators.ApplyNotEmptyValidation(txtCost);
            TextBoxFilters.InputValidators.ApplyNotEmptyValidation(txt_float);
            TextBoxFilters.InputValidators.ApplyNotEmptyValidation(txtParkingSpace);
            TextBoxFilters.InputValidators.ApplyNotEmptyValidation(txtDateDay);

            // Применяем валидацию "только цифры с десятичным разделителем"
            TextBoxFilters.InputValidators.ApplyNumericWithDecimal(txt_Square);
            TextBoxFilters.InputValidators.ApplyNumericWithDecimal(txtParkingSpace);
            TextBoxFilters.InputValidators.ApplyNumericWithDecimal(txt_float);
            TextBoxFilters.InputValidators.ApplyNumericWithDecimal(txtCost);
            TextBoxFilters.InputValidators.ApplyNumericWithDecimal(txtDateDay);

            // Устанавливаем максимальную длину для полей (ограничение на количество символов)
            txt_Square.MaxLength = 4;       // Площадь (до 9999)
            txtCost.MaxLength = 12;         // Стоимость (до 999999999999)
            txt_float.MaxLength = 2;         // Количество этажей (до 99)
            txtParkingSpace.MaxLength = 4;   // Площадь парковки (до 9999)
            txtDateDay.MaxLength = 4;        // Срок строительства в днях (до 9999)
        }

        // Метод для загрузки данных объекта из БД
        private void LoadObjectData()
        {
            try
            {
                string query = "SELECT square, cost, building_dates, number_floors, parking_space, photo FROM object WHERE ID_object = @id";
                using (MySqlConnection conn = new MySqlConnection(connect.con))
                {
                    conn.Open(); // Открываем соединение
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = objectId; // Передаем ID объекта

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read()) // Если данные найдены
                            {
                                // Заполняем поля формы данными из БД
                                txt_Square.Text = reader["square"].ToString();           // Площадь
                                txtCost.Text = reader["cost"].ToString();                 // Стоимость
                                txtDateDay.Text = reader["building_dates"].ToString();    // Срок строительства
                                txt_float.Text = reader["number_floors"].ToString();      // Количество этажей
                                txtParkingSpace.Text = reader["parking_space"].ToString(); // Площадь парковки

                                // Сохраняем имя текущего фото
                                currentPhoto = reader["photo"].ToString();

                                // Загружаем фото, если оно есть
                                if (!string.IsNullOrEmpty(currentPhoto))
                                {
                                    string photoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "photo_object", currentPhoto);
                                    if (File.Exists(photoPath))
                                        pictureBox1.Image = new System.Drawing.Bitmap(photoPath); // Отображаем фото
                                }
                            }
                            else
                            {
                                MessageBox.Show("Объект не найден.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                this.Close(); // Закрываем форму, если объект не найден
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка загрузки", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Метод для обновления данных объекта в БД
        private void UpdateObject()
        {
            try
            {
                // Преобразуем текстовые значения в числа
                double square = Convert.ToDouble(txt_Square.Text);               // Площадь
                double cost = Convert.ToDouble(txtCost.Text);                     // Стоимость
                double parkingSpace = Convert.ToDouble(txtParkingSpace.Text);     // Площадь парковки
                double floors = Convert.ToDouble(txt_float.Text);                 // Количество этажей
                double dateDay = Convert.ToDouble(txtDateDay.Text);               // Срок строительства

                // Если выбрали новое фото — копируем его
                string photoName = currentPhoto; // по умолчанию оставляем старое

                if (!string.IsNullOrEmpty(fullPath) && File.Exists(fullPath)) // Если выбрано новое фото
                {
                    string photoDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "photo_object");
                    if (!Directory.Exists(photoDirectory))
                        Directory.CreateDirectory(photoDirectory); // Создаем папку, если её нет

                    // Используем оригинальное имя файла
                    string uniqueFileName = Path.GetFileName(fullPath);
                    string destinationPath = Path.Combine(photoDirectory, uniqueFileName);
                    File.Copy(fullPath, destinationPath, true); // Копируем файл (с перезаписью)

                    photoName = uniqueFileName; // Сохраняем новое имя
                }

                // SQL-запрос на обновление данных объекта
                string query = @"UPDATE object SET
                                    square          = @square,
                                    cost            = @cost,
                                    building_dates  = @building_dates,
                                    number_floors   = @number_floors,
                                    parking_space   = @parking_space,
                                    photo           = @photo
                                 WHERE ID_object = @id";

                using (MySqlConnection conn = new MySqlConnection(connect.con))
                {
                    conn.Open(); // Открываем соединение
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // Добавляем параметры со значениями
                        cmd.Parameters.Add("@square", MySqlDbType.Decimal).Value = square;
                        cmd.Parameters.Add("@cost", MySqlDbType.Decimal).Value = cost;
                        cmd.Parameters.Add("@building_dates", MySqlDbType.Int32).Value = (int)dateDay;
                        cmd.Parameters.Add("@number_floors", MySqlDbType.Int32).Value = (int)floors;
                        cmd.Parameters.Add("@parking_space", MySqlDbType.Decimal).Value = parkingSpace;
                        cmd.Parameters.Add("@photo", MySqlDbType.VarChar, 50).Value = photoName; // Имя файла фото
                        cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = objectId; // ID объекта

                        int rows = cmd.ExecuteNonQuery(); // Выполняем запрос
                        if (rows > 0) // Если обновление успешно
                        {
                            MessageBox.Show("Объект успешно обновлён!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Кнопка "Сохранить"
        private void buttonSave_Click(object sender, EventArgs e)
        {
            // Проверка заполненности всех полей
            if (string.IsNullOrWhiteSpace(txtCost.Text) ||
                string.IsNullOrWhiteSpace(txtDateDay.Text) ||
                string.IsNullOrWhiteSpace(txtParkingSpace.Text) ||
                string.IsNullOrWhiteSpace(txt_float.Text) ||
                string.IsNullOrWhiteSpace(txt_Square.Text))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            UpdateObject(); // Вызываем метод обновления
        }

        // Кнопка "Назад" (устаревшая, дублируется)
        private void buttonBack_Click(object sender, EventArgs e)
        {
            FormAdminObject form = new FormAdminObject(); // Создаем форму списка объектов
            this.Visible = false; // Скрываем текущую форму
            form.ShowDialog(); // Показываем форму списка
            this.Close(); // Закрываем текущую форму
        }

        // Кнопка "Выбрать фото" (устаревшая, дублируется)
        private void buttonAddPhoto_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files (*.jpg; *.jpeg; *.png)|*.jpg;*.jpeg;*.png";
                openFileDialog.Title = "Выберите изображение";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
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
                        MessageBox.Show("Выберите файл JPG или PNG размером не более 2 Мб.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // Кнопка "Сбросить фото"
        private void buttonResetPhoto_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = Properties.Resources.picture; // Устанавливаем изображение-заглушку
            fullPath = null; // Сбрасываем путь
            fileName = null; // Сбрасываем имя файла
        }

        // Кнопка "Назад" (основная)
        private void button2_Click(object sender, EventArgs e)
        {
            FormAdminObject f = new FormAdminObject(); // Создаем форму списка объектов
            this.Visible = false; // Скрываем текущую форму
            f.ShowDialog(); // Показываем форму списка
            this.Close(); // Закрываем текущую форму
        }

        // Кнопка "Сбросить фото" (альтернативная)
        private void button3_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = Properties.Resources.picture; // Устанавливаем изображение-заглушку
        }

        // Кнопка "Выбрать фото" (основная)
        private void buttonAddPhoto_Click_1(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files (*.jpg; *.jpeg; *.png)|*.jpg;*.jpeg;*.png";
                openFileDialog.Title = "Выберите изображение";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
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
                        MessageBox.Show("Выберите файл JPG или PNG размером не более 2 Мб.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // Кнопка "Сохранить" (альтернативная)
        private void buttonAddObject_Click(object sender, EventArgs e)
        {
            UpdateObject(); // Вызываем метод обновления
        }

        private void FormAdminEditObject_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true; //отменяем закрытие формы
            }
        }
    }
}
