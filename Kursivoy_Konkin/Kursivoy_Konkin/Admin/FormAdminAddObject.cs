using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Kursivoy_Konkin
{
    // Форма для добавления нового объекта недвижимости (доступна администратору)
    public partial class FormAdminAddObject : Form
    {
        // Конструктор формы
        public FormAdminAddObject()
        {
            InitializeComponent(); // Инициализация компонентов дизайнера
            SetupFormConstraints(); // Настройка ограничений ввода
            this.MinimizeBox = false; // Запрет на сворачивание
            this.MaximizeBox = false; // Запрет на разворачивание
            this.ControlBox = false; // Скрытие системных кнопок
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

        // Обработчик кнопки "Назад" (устаревший, дублируется)
        private void button2_Click(object sender, EventArgs e)
        {
            FormAdminObject form = new FormAdminObject(); // Создаем форму списка объектов
            this.Visible = false; // Скрываем текущую форму
            form.ShowDialog(); // Показываем форму списка
            this.Close(); // Закрываем текущую форму
        }

        // Метод для добавления нового объекта в БД
        private void AddObject()
        {
            try
            {
                // Преобразуем текстовые значения в числа
                double Square = Convert.ToDouble(txt_Square.Text);               // Площадь
                double count = Convert.ToDouble(txtCost.Text);                   // Стоимость
                double ParkingSpace = Convert.ToDouble(txtParkingSpace.Text);    // Площадь парковки
                double floatObject = Convert.ToDouble(txt_float.Text);           // Количество этажей
                double DateDay = Convert.ToDouble(txtDateDay.Text);              // Срок строительства
                string photoName = fileName; // Имя файла фото
                int isdeleted = 0; // Флаг удаления (0 - не удален)

                // Проверяем, выбрано ли изображение
                if (string.IsNullOrEmpty(fullPath) || !File.Exists(fullPath))
                {
                    MessageBox.Show("Пожалуйста, выберите изображение для объекта.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Создаем папку photo_object, если она не существует
                string photoDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "photo_object");
                if (!Directory.Exists(photoDirectory))
                {
                    Directory.CreateDirectory(photoDirectory); // Создаем папку
                }

                // Генерируем уникальное имя файла (используем оригинальное имя)
                string uniqueFileName = Path.GetFileName(fullPath);
                string destinationPath = Path.Combine(photoDirectory, uniqueFileName);

                // Копируем файл в папку photo_object (с перезаписью, если существует)
                File.Copy(fullPath, destinationPath, true);

                // Обновляем имя файла для сохранения в БД
                photoName = uniqueFileName;

                // SQL-запрос для вставки нового объекта
                string query = $@"INSERT INTO object
                           (square, cost, building_dates, number_floors, parking_space, photo) VALUES
                           (@Square,@cost,@building_dates,@number_floors,@parking_space,@photo)";

                try
                {
                    using (MySqlConnection conn = new MySqlConnection(connect.con))
                    {
                        conn.Open(); // Открываем соединение
                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            // Добавляем параметры со значениями
                            cmd.Parameters.Add("@Square", MySqlDbType.Decimal).Value = Square;
                            cmd.Parameters.Add("@cost", MySqlDbType.Decimal).Value = count;
                            cmd.Parameters.Add("@building_dates", MySqlDbType.Int32).Value = DateDay;
                            cmd.Parameters.Add("@number_floors", MySqlDbType.Int32).Value = floatObject;
                            cmd.Parameters.Add("@parking_space", MySqlDbType.Decimal).Value = ParkingSpace;
                            cmd.Parameters.Add("@photo", MySqlDbType.VarChar, 50).Value = photoName;

                            int rows = cmd.ExecuteNonQuery(); // Выполняем запрос на вставку
                            if (rows > 0) // Если вставка успешна
                            {
                                MessageBox.Show("Объект успешно добавлен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                // Очищаем поля формы
                                txt_Square.Clear();
                                txtCost.Clear();
                                txtParkingSpace.Clear();
                                txt_float.Clear();
                                txtDateDay.Clear();

                                // Сбрасываем изображение на заглушку
                                pictureBox1.Image = Properties.Resources.picture;

                                conn.Close(); // Закрываем соединение
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message); // Показываем сообщение об ошибке
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public string fileName; // Имя выбранного файла
        public string fullPath; // Полный путь к выбранному файлу

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
                        pictureBox1.Image = Image.FromFile(openFileDialog.FileName); // Отображаем фото
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

        // Обработчик кнопки "Сбросить фото"
        private void button3_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = Properties.Resources.picture; // Устанавливаем изображение-заглушку
        }

        // Обработчик кнопки "Добавить объект"
        private void buttonAddObject_Click(object sender, EventArgs e)
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
            else
            {
                AddObject(); // Вызываем метод добавления объекта
            }
        }

        // Обработчик кнопки "Назад" (основной)
        private void button2_Click_1(object sender, EventArgs e)
        {
            FormAdminObject f = new FormAdminObject(); // Создаем форму списка объектов
            this.Visible = false; // Скрываем текущую форму
            f.ShowDialog(); // Показываем форму списка
            this.Close(); // Закрываем текущую форму
        }

        // Обработчик загрузки формы
        private void FormAdminAddObject_Load(object sender, EventArgs e)
        {
            // Дублирование настроек окна (на всякий случай)
            this.MinimizeBox = false; // Запрет на сворачивание
            this.MaximizeBox = false; // Запрет на разворачивание
            this.ControlBox = false; // Скрытие системных кнопок
        }
    }
}