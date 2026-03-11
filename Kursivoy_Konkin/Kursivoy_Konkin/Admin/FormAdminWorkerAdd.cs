using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace Kursivoy_Konkin
{
    public partial class FormAdminWorkerAdd : Form
    {
        public string fileName;
        public string fullPath;

        public FormAdminWorkerAdd()
        {
            InitializeComponent();
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.ControlBox = false;
            SetupConstraints();
            LoadRoles();
            LoadClients();
        }

        private void SetupConstraints()
        {
            // Только русские буквы и пробел для ФИО
            tbFIO.KeyPress += (s, e) =>
            {
                char c = e.KeyChar;
                bool isRussian = (c >= 'а' && c <= 'я') || (c >= 'А' && c <= 'Я') || c == 'ё' || c == 'Ё';
                bool isAllowed = isRussian || c == ' ' || c == (char)Keys.Back;
                if (!isAllowed) e.Handled = true;
            };

            // Только цифры для возраста
            tbAge.KeyPress += (s, e) =>
            {
                if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
                    e.Handled = true;
            };
            tbAge.MaxLength = 3;

            // Маска телефона
            mtbPhone.Mask = "+7(000)000-00-00";

            // Поиск клиента по TextBox
            tbClientSearch.TextChanged += TbClientSearch_TextChanged;
        }

        private void LoadRoles()
        {
            try
            {
                string query = "SELECT ID_Role, Role FROM role_worker";
                using (MySqlConnection conn = new MySqlConnection(connect.con))
                {
                    conn.Open();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    cbRole.DataSource = dt;
                    cbRole.DisplayMember = "Role";
                    cbRole.ValueMember = "ID_Role";
                    cbRole.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка загрузки ролей",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadClients(string search = "")
        {
            try
            {
                string query = @"
                    SELECT ID_Client, FullName_client, phone 
                    FROM clients 
                    WHERE IsDeleted = 0 
                    AND FullName_client LIKE @search";

                using (MySqlConnection conn = new MySqlConnection(connect.con))
                {
                    conn.Open();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    adapter.SelectCommand.Parameters.Add("@search", MySqlDbType.VarChar).Value = $"%{search}%";

                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dgvClients.DataSource = dt;

                    if (dgvClients.Columns.Contains("ID_Client"))
                        dgvClients.Columns["ID_Client"].Visible = false;

                    dgvClients.Columns["FullName_client"].HeaderText = "ФИО клиента";
                    dgvClients.Columns["phone"].HeaderText = "Телефон";

                    dgvClients.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dgvClients.ReadOnly = true;
                    dgvClients.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    dgvClients.AllowUserToAddRows = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка загрузки клиентов",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TbClientSearch_TextChanged(object sender, EventArgs e)
        {
            LoadClients(tbClientSearch.Text.Trim());
        }

       


        

        private void btnCancel_Click(object sender, EventArgs e)
        {
            FormAdminWorker f = new FormAdminWorker();
            this.Visible = false;
            f.ShowDialog();
            this.Close();
        }

        private void buttonDeletePhoto_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = Properties.Resources.picture;
            fullPath = null;
            fileName = null;
        }

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

            if (mtbPhone.MaskCompleted == false)
            {
                MessageBox.Show("Введите корректный номер телефона.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cbRole.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите роль.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dgvClients.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите клиента из списка.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int clientId = Convert.ToInt32(dgvClients.SelectedRows[0].Cells["ID_Client"].Value);
                int roleId = Convert.ToInt32(cbRole.SelectedValue);
                int age = Convert.ToInt32(tbAge.Text);

                // Сохраняем фото если выбрано
                string photoName = null;
                if (!string.IsNullOrEmpty(fullPath) && File.Exists(fullPath))
                {
                    string photoDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "photo_worker");
                    if (!Directory.Exists(photoDir))
                        Directory.CreateDirectory(photoDir);

                    string uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(fullPath);
                    File.Copy(fullPath, Path.Combine(photoDir, uniqueName), true);
                    photoName = uniqueName;
                }

                string query = @"
                    INSERT INTO worker (FIO, ID_Clientsl, phone, Age, Role_worker_ID_Role, IsDeleted, photo)
                    VALUES (@fio, @clientId, @phone, @age, @roleId, 0, @photo)";

                using (MySqlConnection conn = new MySqlConnection(connect.con))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@fio", MySqlDbType.VarChar, 100).Value = tbFIO.Text.Trim();
                        cmd.Parameters.Add("@clientId", MySqlDbType.VarChar, 45).Value = clientId.ToString();
                        cmd.Parameters.Add("@phone", MySqlDbType.VarChar, 45).Value = mtbPhone.Text;
                        cmd.Parameters.Add("@age", MySqlDbType.Int32).Value = age;
                        cmd.Parameters.Add("@roleId", MySqlDbType.Int32).Value = roleId;
                        cmd.Parameters.Add("@photo", MySqlDbType.VarChar, 100).Value =
                            (object)photoName ?? DBNull.Value;

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Сотрудник успешно добавлен!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка сохранения",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonAddPhoto_Click(object sender, EventArgs e)
        {
             // Кнопка "Добавить фото"
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
                        MessageBox.Show("Выберите файл JPG или PNG размером не более 2 Мб.",
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnCancel_Click_1(object sender, EventArgs e)
        {
            FormAdminWorker f = new FormAdminWorker();
            this.Visible = false;
            f.ShowDialog();
            this.Close();
        }
    }
}