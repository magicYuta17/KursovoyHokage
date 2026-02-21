using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Kursivoy_Konkin
{
    public partial class FormAdminObject : Form
    {
        public FormAdminObject()
        {
            InitializeComponent();
            this.Load += FormAdminObject_Load;
        }

        private void InitializeContextMenu()
        {

            dataGridView1.ContextMenuStrip = contextMenuStrip1;

        }


        private void LoadData()
        {
            try
            {
                dataGridView1.Columns.Clear();
                dataGridView1.AutoGenerateColumns = true;

                string query = @"
            SELECT 
                ID_object,
                square AS 'Площадь',
                cost AS 'Стоимость',
                building_dates AS 'Дата постройки',
                number_floors AS 'Количество комнат',
                parking_space AS 'Площадь парковки',
                photo
            FROM object
            WHERE IsDeleted = 0;";

                using (var connection = new MySqlConnection(connect.con))
                using (var command = new MySqlCommand(query, connection))
                using (var adapter = new MySqlDataAdapter(command))
                {
                    var table = new DataTable();
                    connection.Open();
                    adapter.Fill(table);

                    if (table.Rows.Count == 0)
                    {
                        MessageBox.Show("Данные не найдены.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    // Привязываем данные к DataGridView
                    dataGridView1.DataSource = table;

                    // Скрываем столбец ID_object
                    if (dataGridView1.Columns["ID_object"] != null)
                        dataGridView1.Columns["ID_object"].Visible = false;

                    // Скрываем колонку photo (raw данные из БД)
                    if (dataGridView1.Columns["photo"] != null)
                        dataGridView1.Columns["photo"].Visible = false;

                    // Добавляем колонку для отображения фото
                    var imageColumn = new DataGridViewImageColumn
                    {
                        Name = "Фото",
                        HeaderText = "Фото",
                        ImageLayout = DataGridViewImageCellLayout.Zoom,
                        Width = 80,
                        SortMode = DataGridViewColumnSortMode.NotSortable
                    };
                    dataGridView1.Columns.Add(imageColumn);

                    // Путь к папке с фотографиями
                    string photoDirectory = Path.Combine(Application.StartupPath, "photo_object");

                    // Заглушка из ресурсов
                    Image placeholder = Properties.Resources.picture;

                    // Заполняем колонку "Фото"
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.IsNewRow) continue;

                        // Читаем имя файла напрямую из DataTable
                        object photoFileName = table.Rows[row.Index]["photo"];

                        if (photoFileName == null || photoFileName == DBNull.Value || string.IsNullOrWhiteSpace(photoFileName.ToString()))
                        {
                            row.Cells["Фото"].Value = placeholder;
                            continue;
                        }

                        try
                        {
                            string photoPath = Path.Combine(photoDirectory, photoFileName.ToString());
                            System.Diagnostics.Debug.WriteLine($"Путь к фото: {photoPath}");

                            if (File.Exists(photoPath))
                            {
                                using (var img = Image.FromFile(photoPath))
                                {
                                    row.Cells["Фото"].Value = new Bitmap(img);
                                }
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine($"Файл не найден: {photoPath}");
                                row.Cells["Фото"].Value = placeholder;
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Ошибка загрузки изображения: {ex.Message}");
                            row.Cells["Фото"].Value = placeholder;
                        }
                    }

                    // Отключаем сортировку у всех колонок
                    foreach (DataGridViewColumn col in dataGridView1.Columns)
                        col.SortMode = DataGridViewColumnSortMode.NotSortable;

                    dataGridView1.RowTemplate.Height = 80;
                    dataGridView1.ClearSelection();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




        private void button5_Click(object sender, EventArgs e)
        {
            FormAdminNavigation f = new FormAdminNavigation();
            this.Visible = false;
            f.ShowDialog();
            this.Close();
        }

        private void FormAdminObject_Load(object sender, EventArgs e)
        {
            LoadData();
            InitializeContextMenu();
        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {

            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    var hitTest = dataGridView1.HitTest(e.X, e.Y);

                    // Открываем меню только если кликнули на строку, не на заголовок
                    if (hitTest.RowIndex >= 0)
                    {
                        // Выделяем строку по которой кликнули
                        dataGridView1.ClearSelection();
                        dataGridView1.Rows[hitTest.RowIndex].Selected = true;
                        dataGridView1.CurrentCell = dataGridView1.Rows[hitTest.RowIndex].Cells[0];

                        contextMenuStrip1.Show(dataGridView1, e.Location);
                    }
                }
            }
            catch(Exception ex) { }
        }

        private void добавитьToolStripMenuItem_Click(object sender, EventArgs e)
        {

            FormAdminAddObject f = new FormAdminAddObject();
            this.Visible = false;
            f.ShowDialog();
            this.Close();
        }

        private void редактироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Получаем ID объекта из выбранной строки
                int clientId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID_object"].Value);

                // Подтверждение удаления
                var result = MessageBox.Show("Вы уверены, что хотите удалить объект?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        string query = "UPDATE mydb.object SET IsDeleted = 1 WHERE ID_object = @IDobject;";
                        using (var connection = new MySqlConnection(connect.con))
                        using (var command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@IDobject", clientId);
                            connection.Open();
                            command.ExecuteNonQuery();
                        }

                        // Обновляем таблицу после удаления объекта
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении объекта: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выведите клиента для удаления.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
