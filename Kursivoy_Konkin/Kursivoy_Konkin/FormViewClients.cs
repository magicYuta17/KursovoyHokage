using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kursivoy_Konkin
{
    public partial class FormViewClients : Form
    {
        private DataTable originalDataTable; // Добавьте это поле

        public FormViewClients()
        {
            InitializeComponent();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        string com = @"SELECT c.*, s.status as StatusName 
                          FROM mydb.clients c 
                          LEFT JOIN mydb.status_client s ON c.Status_client_ID_Status_client = s.ID_Status_client;";

        private void FillTableData(string cmd = "")
        {
            dataGridView1.Columns.Clear();

            // ИЗМЕНЕННЫЙ ЗАПРОС С JOIN ДЛЯ ПОЛУЧЕНИЯ НАЗВАНИЙ СТАТУСОВ
            string com = @"SELECT c.*, s.status as StatusName 
                  FROM mydb.clients c 
                  LEFT JOIN mydb.status_client s ON c.Status_client_ID_Status_client = s.ID_Status_client;";

            MySqlConnection connection = new MySqlConnection(connect.con);
            connection.Open();
            MySqlCommand command = new MySqlCommand(com, connection);
            MySqlDataAdapter adapter = new MySqlDataAdapter(command);
            DataTable table = new DataTable();

            adapter.Fill(table);

            // Сохраняем оригинальные данные для фильтрации
            originalDataTable = table.Copy();

            dataGridView1.DataSource = table;

            dataGridView1.Columns["ID_contract"].HeaderText = "Контракт";
            dataGridView1.Columns["FullName_client"].HeaderText = "ФИО";
            dataGridView1.Columns["phone"].HeaderText = "Телефон";
            dataGridView1.Columns["Age"].HeaderText = "Возраст";
            dataGridView1.Columns["StatusName"].HeaderText = "Статус"; // Используем новый столбец с названиями
            dataGridView1.Columns["Qualified_lead"].HeaderText = "Квал лид";
            dataGridView1.Columns["LTV"].HeaderText = "LTV";

            dataGridView1.Columns["ID_Client"].Visible = false;
            dataGridView1.Columns["photo_clients"].Visible = false;
            dataGridView1.Columns["Status_client_ID_Status_client"].Visible = false; // Скрываем старый столбец с ID

            DataGridViewImageColumn imageColumn = new DataGridViewImageColumn();
            imageColumn.Name = "Фото";
            imageColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;

            dataGridView1.Columns.Add(imageColumn);
            dataGridView1.AllowUserToAddRows = false;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                string namee = row.Cells["photo_clients"].Value == null ? "picture.png" : row.Cells["photo_clients"].Value.ToString();

                if (namee == "")
                {
                    namee = "picture.png";
                }
                row.Cells["Фото"].Value = Image.FromFile(@"./img/picture.png");
            }

            connection.Close();
        }

        // ДОБАВЬТЕ ЭТУ ФУНКЦИЮ ДЛЯ ФИЛЬТРАЦИИ И СОРТИРОВКИ
        private void ApplyFiltersAndSorting()
        {
            if (originalDataTable == null) return;

            // Создаем копию данных для фильтрации
            DataTable filteredTable = originalDataTable.Clone();
            IEnumerable<DataRow> rows = originalDataTable.AsEnumerable();

            // Применяем поиск
            // Применяем поиск
            string searchText = textBox1.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(searchText))
            {
                rows = rows.Where(row =>
                {
                    string fullName = GetSafeString(row["FullName_client"]);
                    string phone = GetSafeString(row["phone"]);
                    string contract = GetSafeString(row["ID_contract"]);
                    string status = GetSafeString(row["StatusName"]); // Добавляем поиск по названию статуса

                    return fullName.ToLower().Contains(searchText) ||
                           phone.ToLower().Contains(searchText) ||
                           contract.ToLower().Contains(searchText) ||
                           status.ToLower().Contains(searchText);
                });
            }

            // Применяем фильтрацию
            string filter = comboBox3.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(filter) && filter != "Все клиенты")
            {
                switch (filter)
                {
                    case "LTV > 500000":
                        rows = rows.Where(row =>
                            GetSafeDecimal(row["LTV"]) > 500000);
                        break;
                    case "Активные клиенты":
                        // Фильтруем по названию статуса
                        rows = rows.Where(row =>
                            GetSafeString(row["StatusName"]).ToLower() == "В работе");
                        break;
                    case "Новые клиенты":
                        // Фильтруем по названию статуса
                        rows = rows.Where(row =>
                            GetSafeString(row["StatusName"]).ToLower() == "план");
                        break;
                    case "Закрепленные сотрудники":
                        // Адаптируйте это условие под вашу базу данных
                        rows = rows.Where(row =>
                            GetSafeBool(row["IsAttached"]) == true);
                        break;
                }
            }

            // Копируем отфильтрованные строки
            foreach (DataRow row in rows)
            {
                filteredTable.ImportRow(row);
            }

            // Применяем сортировку
            string sortBy = comboBox1.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(sortBy) && sortBy != "Без сортировки")
            {
                switch (sortBy)
                {
                    case "По ФИО":
                        filteredTable.DefaultView.Sort = "FullName_client ASC";
                        break;
                    case "По статусу":
                        filteredTable.DefaultView.Sort = "StatusName ASC"; // Сортируем по названию статуса
                        break;
                    case "По LTV (по убыванию)":
                        filteredTable.DefaultView.Sort = "LTV DESC";
                        break;
                    case "По LTV (по возрастанию)":
                        filteredTable.DefaultView.Sort = "LTV ASC";
                        break;
                }
                filteredTable = filteredTable.DefaultView.ToTable();
            }

            dataGridView1.DataSource = filteredTable;

            // Обновляем фото после фильтрации
            UpdatePhotosAfterFilter();
        }


        // ДОБАВЬТЕ ЭТУ ФУНКЦИЮ ДЛЯ ОБНОВЛЕНИЯ ФОТО
        private void UpdatePhotosAfterFilter()
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;

                string namee = row.Cells["photo_clients"].Value == null ? "picture.png" : row.Cells["photo_clients"].Value.ToString();

                if (string.IsNullOrEmpty(namee))
                {
                    namee = "picture.png";
                }

                try
                {
                    row.Cells["Фото"].Value = Image.FromFile(@"./img/picture.png");
                }
                catch
                {
                    // Обработка ошибки загрузки изображения
                }
            }
        }
        private string GetSafeString(object value)
        {
            if (value == null || value == DBNull.Value)
                return string.Empty;

            return value.ToString();
        }

        private decimal GetSafeDecimal(object value)
        {
            if (value == null || value == DBNull.Value)
                return 0;

            try
            {
                return Convert.ToDecimal(value);
            }
            catch
            {
                return 0;
            }
        }

        private bool GetSafeBool(object value)
        {
            if (value == null || value == DBNull.Value)
                return false;

            try
            {
                return Convert.ToBoolean(value);
            }
            catch
            {
                return false;
            }
        }

        private void FormManagerNavigation_Load(object sender, EventArgs e)
        {
            // ДОБАВЬТЕ ИНИЦИАЛИЗАЦИЮ КОМБОБОКСОВ
            comboBox1.Items.AddRange(new string[]
            {
                "Без сортировки",
                "По ФИО",
                "По статусу",
                "По LTV (по убыванию)",
                "По LTV (по возрастанию)"
            });
            comboBox1.SelectedIndex = 0;

            comboBox3.Items.AddRange(new string[]
            {
                "Все клиенты",
                "LTV > 500000",
                "Активные клиенты",
                "Новые клиенты",
                "Закрепленные сотрудники"
            });
            comboBox3.SelectedIndex = 0;

            FillTableData();
            dataGridView1.ContextMenuStrip = contextMenuStrip1;
            dataGridView1.MouseDown += dataGridView1_MouseDown;
        }

        // ДОБАВЬТЕ ОБРАБОТЧИКИ СОБЫТИЙ ДЛЯ КОНТРОЛОВ

     

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hit = dataGridView1.HitTest(e.X, e.Y);
                if (hit.RowIndex >= 0)
                {
                    dataGridView1.ClearSelection();
                    dataGridView1.Rows[hit.RowIndex].Selected = true;
                    dataGridView1.CurrentCell = dataGridView1.Rows[hit.RowIndex].Cells[hit.ColumnIndex];
                }
                else
                {
                    dataGridView1.ClearSelection();
                    contextMenuStrip1.Close();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Обновление данных
            FillTableData();
        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {
            ApplyFiltersAndSorting();

        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            ApplyFiltersAndSorting();
        }

        private void comboBox3_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            ApplyFiltersAndSorting();
        }
    }
}