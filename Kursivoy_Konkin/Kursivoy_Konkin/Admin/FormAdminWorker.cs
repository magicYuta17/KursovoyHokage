using Kursivoy_Konkin;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Kursivoy_Konkin
{
    public partial class FormAdminWorker : Form
    {
        public FormAdminWorker()
        {
            InitializeComponent();
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.ControlBox = false;

            dgvWorkers.MouseClick += dgvWorkers_MouseClick;

        }

        private void LoadWorkers()
        {
            try
            {
                dgvWorkers.Columns.Clear();
                dgvWorkers.AutoGenerateColumns = true;

                string query = @"
            SELECT 
                w.ID_worker,
                w.FIO,
                c.FullName_client AS Клиент,
                w.phone          AS Телефон,
                w.Age            AS Возраст,
                r.Role           AS Роль,
                w.photo
            FROM worker w
            LEFT JOIN clients c ON w.ID_Clientsl = c.ID_Client
            LEFT JOIN role_worker r ON w.Role_worker_ID_Role = r.ID_Role
            WHERE w.IsDeleted = 0";

                using (MySqlConnection conn = new MySqlConnection(connect.con))
                {
                    conn.Open();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dgvWorkers.DataSource = dt;

                    // Скрываем служебные столбцы
                    dgvWorkers.Columns["ID_worker"].Visible = false;
                    dgvWorkers.Columns["photo"].Visible = false;

                    // Переименовываем заголовки
                    dgvWorkers.Columns["FIO"].HeaderText = "ФИО";
                    dgvWorkers.Columns["Клиент"].HeaderText = "Клиент";
                    dgvWorkers.Columns["Телефон"].HeaderText = "Телефон";
                    dgvWorkers.Columns["Возраст"].HeaderText = "Возраст";
                    dgvWorkers.Columns["Роль"].HeaderText = "Роль";

                    // Добавляем колонку для фото
                    var imageColumn = new DataGridViewImageColumn
                    {
                        Name = "Фото",
                        HeaderText = "Фото",
                        ImageLayout = DataGridViewImageCellLayout.Zoom,
                        Width = 80,
                        SortMode = DataGridViewColumnSortMode.NotSortable
                    };
                    dgvWorkers.Columns.Add(imageColumn);

                    string photoDirectory = Path.Combine(Application.StartupPath, "photo_worker");
                    Image placeholder = Properties.Resources.picture;

                    // Заполняем фото по каждой строке
                    foreach (DataGridViewRow row in dgvWorkers.Rows)
                    {
                        if (row.IsNewRow) continue;

                        object photoFileName = dt.Rows[row.Index]["photo"];

                        if (photoFileName == null || photoFileName == DBNull.Value
                            || string.IsNullOrWhiteSpace(photoFileName.ToString()))
                        {
                            row.Cells["Фото"].Value = placeholder;
                            continue;
                        }

                        try
                        {
                            string photoPath = Path.Combine(photoDirectory, photoFileName.ToString());
                            if (File.Exists(photoPath))
                            {
                                using (var img = Image.FromFile(photoPath))
                                    row.Cells["Фото"].Value = new Bitmap(img);
                            }
                            else
                            {
                                row.Cells["Фото"].Value = placeholder;
                            }
                        }
                        catch
                        {
                            row.Cells["Фото"].Value = placeholder;
                        }
                    }

                    // Настройки таблицы
                    dgvWorkers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dgvWorkers.Columns["Фото"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    dgvWorkers.Columns["Фото"].Width = 80;
                    dgvWorkers.RowTemplate.Height = 80;
                    dgvWorkers.ReadOnly = true;
                    dgvWorkers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    dgvWorkers.AllowUserToAddRows = false;

                    foreach (DataGridViewColumn col in dgvWorkers.Columns)
                        col.SortMode = DataGridViewColumnSortMode.NotSortable;

                    dgvWorkers.ClearSelection();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка загрузки", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ПКМ — показываем контекстное меню только если кликнули по строке
        private void dgvWorkers_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hit = dgvWorkers.HitTest(e.X, e.Y);
                if (hit.RowIndex >= 0)
                {
                    dgvWorkers.ClearSelection();
                    dgvWorkers.Rows[hit.RowIndex].Selected = true;
                    cmsWorker.Show(dgvWorkers, e.Location);
                }
            }
        }

        // Получаем ID выбранного сотрудника
        private int GetSelectedWorkerId()
        {
            if (dgvWorkers.SelectedRows.Count == 0) return -1;
            return Convert.ToInt32(dgvWorkers.SelectedRows[0].Cells["ID_worker"].Value);
        }

        

       

        private void btnBack_Click(object sender, EventArgs e)
        {
            FormAdminNavigation f = new FormAdminNavigation();
            this.Visible = false;
            f.ShowDialog();
            this.Close();
        }

        private void tsmiAdd_Click_1(object sender, EventArgs e)
        {
            FormAdminWorkerAdd form = new FormAdminWorkerAdd();
            this.Visible = false;
            form.ShowDialog();
            this.Visible = true;
            LoadWorkers(); // обновляем таблицу после возврата
        }

        private void tsmiEdit_Click_1(object sender, EventArgs e)
        {
            int id = GetSelectedWorkerId();
            if (id == -1)
            {
                MessageBox.Show("Выберите сотрудника для редактирования.",
                    "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            FormAdminWorkerEdit form = new FormAdminWorkerEdit(id);
            this.Visible = false;
            form.ShowDialog();
            this.Visible = true;
            LoadWorkers();
        }

        private void tsmiDelete_Click(object sender, EventArgs e)
        {
            int id = GetSelectedWorkerId();
            if (id == -1)
            {
                MessageBox.Show("Выберите сотрудника для удаления.",
                    "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                "Вы уверены, что хотите удалить этого сотрудника?",
                "Подтверждение удаления",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            try
            {
                string query = "UPDATE worker SET IsDeleted = 1 WHERE ID_worker = @id";
                using (MySqlConnection conn = new MySqlConnection(connect.con))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Сотрудник удалён.", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadWorkers();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка удаления",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormAdminWorker_Load(object sender, EventArgs e)
        {
            LoadWorkers();
        }
    }
}