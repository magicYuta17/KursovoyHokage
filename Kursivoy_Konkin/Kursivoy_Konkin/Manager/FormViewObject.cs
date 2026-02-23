using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Kursivoy_Konkin
{
    public partial class FormViewObject : Form
    {
        private readonly string _callerFormName;
        public FormViewObject(string callerFormName = "FormManagerNavigation")
        {
            InitializeComponent();
            _callerFormName = callerFormName;
            LoadData();
            this.Load += FormViewObject_Load;
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



        private void button1_Click(object sender, EventArgs e)
        {
            Form previousForm = null;

            switch (_callerFormName)
            {
                case "FormManagerNavigation":
                    previousForm = new FormManagerNavigation();
                    break;
                case "FormHeadNavigation":
                    previousForm = new FormHeadNavigation();
                    break;
            }

            if (previousForm != null)
            {
                this.Visible = false;
                previousForm.ShowDialog();
                this.Close();
            }
        }

        private void FormViewObject_Load(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}
