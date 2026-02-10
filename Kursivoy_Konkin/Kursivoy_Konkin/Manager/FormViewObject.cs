using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Kursivoy_Konkin
{
    public partial class FormViewObject : Form
    {
        public FormViewObject()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                dataGridView1.Columns.Clear();
                dataGridView1.AutoGenerateColumns = true;

                string query = @"
            SELECT 
                square AS 'Площадь',
                cost AS 'Стоимость',
                building_dates AS 'Дата постройки',
                number_floors AS 'Этажность',
                parking_space AS 'Парковка',
                photo AS 'PhotoBlob'
            FROM object;";

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

                    dataGridView1.DataSource = table;

                    // Скрываем технический столбец с BLOB
                    if (dataGridView1.Columns["PhotoBlob"] != null)
                        dataGridView1.Columns["PhotoBlob"].Visible = false;

                    // Размеры под фото
                    dataGridView1.AllowUserToAddRows = false;
                    dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                    dataGridView1.RowTemplate.Height = 80;

                    // Добавляем колонку-картинку (как в примере)
                    var imageColumn = new DataGridViewImageColumn
                    {
                        Name = "Фото",
                        HeaderText = "Фото",
                        ImageLayout = DataGridViewImageCellLayout.Zoom,
                        Width = 80,
                        SortMode = DataGridViewColumnSortMode.NotSortable
                    };
                    dataGridView1.Columns.Add(imageColumn);

                    // Заглушка из ресурсов
                    Image placeholder = Properties.Resources.picture;

                    // Заполняем колонку "Фото" из BLOB
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.IsNewRow) continue;

                        object v = row.Cells["PhotoBlob"].Value;

                        if (v == null || v == DBNull.Value)
                        {
                            row.Cells["Фото"].Value = placeholder;
                            continue;
                        }

                        try
                        {
                            byte[] bytes = (byte[])v;

                            // ВАЖНО: делаем копию картинки, чтобы не зависеть от потока
                            using (var ms = new System.IO.MemoryStream(bytes))
                            using (var img = Image.FromStream(ms))
                            {
                                row.Cells["Фото"].Value = new Bitmap(img);
                            }
                        }
                        catch
                        {
                            row.Cells["Фото"].Value = placeholder;
                        }
                    }

                    // (как в примере) отключаем сортировку у всех колонок
                    foreach (DataGridViewColumn col in dataGridView1.Columns)
                        col.SortMode = DataGridViewColumnSortMode.NotSortable;

                    dataGridView1.ClearSelection();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void button1_Click(object sender, EventArgs e)
        {
            FormManagerNavigation f = new FormManagerNavigation();
            this.Visible = false;
            f.ShowDialog();
            this.Close();
        }
    }
}
