using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Windows.Forms;

namespace Kursivoy_Konkin
{
    public partial class FormAdminEditObject : Form
    {
        private int objectId;
        private string currentPhoto;

        public string fileName;
        public string fullPath;

        public FormAdminEditObject(int id)
        {
            InitializeComponent();
            objectId = id;
            SetupFormConstraints();
            LoadObjectData();
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

        private void LoadObjectData()
        {
            try
            {
                string query = "SELECT square, cost, building_dates, number_floors, parking_space, photo FROM object WHERE ID_object = @id";
                using (MySqlConnection conn = new MySqlConnection(connect.con))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = objectId;

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txt_Square.Text = reader["square"].ToString();
                                txtCost.Text = reader["cost"].ToString();
                                txtDateDay.Text = reader["building_dates"].ToString();
                                txt_float.Text = reader["number_floors"].ToString();
                                txtParkingSpace.Text = reader["parking_space"].ToString();

                                currentPhoto = reader["photo"].ToString();

                                // Загружаем фото если есть
                                if (!string.IsNullOrEmpty(currentPhoto))
                                {
                                    string photoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "photo_object", currentPhoto);
                                    if (File.Exists(photoPath))
                                        pictureBox1.Image = new System.Drawing.Bitmap(photoPath);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Объект не найден.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                this.Close();
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

        private void UpdateObject()
        {
            try
            {
                double square = Convert.ToDouble(txt_Square.Text);
                double cost = Convert.ToDouble(txtCost.Text);
                double parkingSpace = Convert.ToDouble(txtParkingSpace.Text);
                double floors = Convert.ToDouble(txt_float.Text);
                double dateDay = Convert.ToDouble(txtDateDay.Text);

                // Если выбрали новое фото — копируем его
                string photoName = currentPhoto; // по умолчанию оставляем старое

                if (!string.IsNullOrEmpty(fullPath) && File.Exists(fullPath))
                {
                    string photoDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "photo_object");
                    if (!Directory.Exists(photoDirectory))
                        Directory.CreateDirectory(photoDirectory);

                    string uniqueFileName = Path.GetFileName(fullPath);
                    string destinationPath = Path.Combine(photoDirectory, uniqueFileName);
                    File.Copy(fullPath, destinationPath, true);

                    photoName = uniqueFileName;
                }

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
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@square", MySqlDbType.Decimal).Value = square;
                        cmd.Parameters.Add("@cost", MySqlDbType.Decimal).Value = cost;
                        cmd.Parameters.Add("@building_dates", MySqlDbType.Int32).Value = (int)dateDay;
                        cmd.Parameters.Add("@number_floors", MySqlDbType.Int32).Value = (int)floors;
                        cmd.Parameters.Add("@parking_space", MySqlDbType.Decimal).Value = parkingSpace;
                        cmd.Parameters.Add("@photo", MySqlDbType.VarChar, 50).Value = photoName;
                        cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = objectId;

                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
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
            if (string.IsNullOrWhiteSpace(txtCost.Text) ||
                string.IsNullOrWhiteSpace(txtDateDay.Text) ||
                string.IsNullOrWhiteSpace(txtParkingSpace.Text) ||
                string.IsNullOrWhiteSpace(txt_float.Text) ||
                string.IsNullOrWhiteSpace(txt_Square.Text))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            UpdateObject();
        }

        // Кнопка "Назад"
        private void buttonBack_Click(object sender, EventArgs e)
        {
            FormAdminObject form = new FormAdminObject();
            this.Visible = false;
            form.ShowDialog();
            this.Close();
        }

        // Кнопка "Выбрать фото"
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
                        pictureBox1.Image = new System.Drawing.Bitmap(openFileDialog.FileName);
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

        // Кнопка "Сбросить фото"
        private void buttonResetPhoto_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = Properties.Resources.picture;
            fullPath = null;
            fileName = null;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FormAdminObject f = new FormAdminObject();
            this.Visible = false;
            f.ShowDialog();
            this.Close();
        }

        

        private void button3_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = Properties.Resources.picture;
        }

        private void buttonAddPhoto_Click_1(object sender, EventArgs e)
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
                        pictureBox1.Image = new System.Drawing.Bitmap(openFileDialog.FileName);
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

        private void buttonAddObject_Click(object sender, EventArgs e)
        {
            UpdateObject();
        }
    }
}