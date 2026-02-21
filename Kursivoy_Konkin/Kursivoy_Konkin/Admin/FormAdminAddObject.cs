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
    public partial class FormAdminAddObject : Form
    {
        public FormAdminAddObject()
        {
            InitializeComponent();
            SetupFormConstraints();

        }


        private void SetupFormConstraints()
        {

            TextBoxFilters.InputValidators.ApplyNotEmptyValidation(txt_Square);
            TextBoxFilters.InputValidators.ApplyNotEmptyValidation(txtCost);
            TextBoxFilters.InputValidators.ApplyNotEmptyValidation(txt_float);
            TextBoxFilters.InputValidators.ApplyNotEmptyValidation(txtParkingSpace);
            TextBoxFilters.InputValidators.ApplyNotEmptyValidation(txtDateDay);


            TextBoxFilters.InputValidators.ApplyNumericWithDecimal(txt_Square);
            TextBoxFilters.InputValidators.ApplyNumericWithDecimal(txtParkingSpace);
            TextBoxFilters.InputValidators.ApplyNumericWithDecimal(txt_float);
            TextBoxFilters.InputValidators.ApplyNumericWithDecimal(txtCost);
            TextBoxFilters.InputValidators.ApplyNumericWithDecimal(txtDateDay);

            txt_Square.MaxLength = 4;
            txtCost.MaxLength = 12;
            txt_float.MaxLength = 2;
            txtParkingSpace.MaxLength = 4;
            txtDateDay.MaxLength = 4;




        }

        private void button2_Click(object sender, EventArgs e)
        {

            FormAdminObject form = new FormAdminObject();
            this.Visible = false;
            form.ShowDialog();
            this.Close();
        }
        private void AddObject()
        {
            try
            {
                double Square = Convert.ToDouble(txt_Square.Text);
                double count = Convert.ToDouble(txtCost.Text);
                double ParkingSpace = Convert.ToDouble(txtParkingSpace.Text);
                double floatObject = Convert.ToDouble(txt_float.Text);
                double DateDay = Convert.ToDouble(txtDateDay.Text);
                string photoName = fileName;
                int isdeleted = 0;

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
                    Directory.CreateDirectory(photoDirectory);
                }

                // Генерируем уникальное имя файла
                string uniqueFileName = Path.GetFileName(fullPath);
                string destinationPath = Path.Combine(photoDirectory, uniqueFileName);

                // Копируем файл в папку photo_object
                File.Copy(fullPath, destinationPath, true);

                // Обновляем имя файла для сохранения в БД
                photoName = uniqueFileName;

                string query = $@"INSERT INTO object
                           (square, cost, building_dates, number_floors, parking_space, photo) VALUES
                           (@Square,@cost,@building_dates,@number_floors,@parking_space,@photo)";

                try
                {
                    using (MySqlConnection conn = new MySqlConnection(connect.con))
                    {
                        conn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.Add("@Square", MySqlDbType.Decimal).Value = Square;
                            cmd.Parameters.Add("@cost", MySqlDbType.Decimal).Value = count;
                            cmd.Parameters.Add("@building_dates", MySqlDbType.Int32).Value = DateDay;
                            cmd.Parameters.Add("@number_floors", MySqlDbType.Int32).Value = floatObject;
                            cmd.Parameters.Add("@parking_space", MySqlDbType.Decimal).Value = ParkingSpace;
                            cmd.Parameters.Add("@photo", MySqlDbType.VarChar, 50).Value = photoName;

                            int rows = cmd.ExecuteNonQuery();
                            if (rows > 0)
                            {
                                MessageBox.Show("Объект успешно добавлен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                txt_Square.Clear();
                                txtCost.Clear();
                                txtParkingSpace.Clear();
                                txt_float.Clear();
                                txtDateDay.Clear();

                                pictureBox1.Image = Properties.Resources.picture;

                                conn.Close();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public string fileName;
        public string fullPath;
        private void buttonAddPhoto_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files (*.jpg; *.jpeg; *.png)|*.jpg;*.jpeg;*.png";
                openFileDialog.Title = "Выберите изображение";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    FileInfo fileInfo = new FileInfo(openFileDialog.FileName);

                    if ((fileInfo.Extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
                         fileInfo.Extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                         fileInfo.Extension.Equals(".png", StringComparison.OrdinalIgnoreCase)) &&
                        fileInfo.Length <= 2 * 1024 * 1024)
                    {
                        pictureBox1.Image = Image.FromFile(openFileDialog.FileName);
                        fileName = fileInfo.Name;
                        fullPath = openFileDialog.FileName;
                    }
                    else
                    {
                        MessageBox.Show("Выберите файл JPG или PNG размером не более 2 Мб.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = Properties.Resources.picture;
        }

        private void buttonAddObject_Click(object sender, EventArgs e)
        {
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
                AddObject();

            }
        }
    }
}
