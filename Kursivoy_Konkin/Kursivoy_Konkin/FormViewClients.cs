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
        // Новый SQL-запрос с INNER JOIN для закреплённых сотрудников
        string com = @"SELECT c.*, s.status as StatusName
    FROM mydb.clients c
    LEFT JOIN mydb.status_client s ON c.Status_client_ID_Status_client = s.ID_Status_client;";

        string comAttached = @"SELECT c.*, s.status as StatusName, w.FIO as EmployeeName
    FROM mydb.clients c
    LEFT JOIN mydb.status_client s ON c.Status_client_ID_Status_client = s.ID_Status_client
    LEFT JOIN mydb.worker w ON c.ID_Client = w.ID_Clientsl
    WHERE w.FIO IS NOT NULL;";
        private void FillTableData(string filter = "")
        {
            dataGridView1.Columns.Clear();

            // Всегда используем расширенный запрос с JOIN на работников
            string query = comAttached;

            MySqlConnection connection = new MySqlConnection(connect.con);
            connection.Open();
            MySqlCommand command = new MySqlCommand(query, connection);
            MySqlDataAdapter adapter = new MySqlDataAdapter(command);
            DataTable table = new DataTable();

            adapter.Fill(table);

            originalDataTable = table.Copy();

            dataGridView1.DataSource = table;

            // Настройка столбцов
            dataGridView1.Columns["ID_contract"].HeaderText = "Контракт";
            dataGridView1.Columns["FullName_client"].HeaderText = "ФИО";
            dataGridView1.Columns["phone"].HeaderText = "Телефон";
            dataGridView1.Columns["Age"].HeaderText = "Возраст";
            dataGridView1.Columns["StatusName"].HeaderText = "Статус";
            dataGridView1.Columns["Qualified_lead"].HeaderText = "Квал лид";
            dataGridView1.Columns["LTV"].HeaderText = "LTV";
            if (table.Columns.Contains("EmployeeName"))
                dataGridView1.Columns["EmployeeName"].HeaderText = "Сотрудник";

            dataGridView1.Columns["ID_Client"].Visible = false;
            dataGridView1.Columns["photo_clients"].Visible = false;
            dataGridView1.Columns["Status_client_ID_Status_client"].Visible = false;

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

            DataTable filteredTable = originalDataTable.Clone();
            IEnumerable<DataRow> rows = originalDataTable.AsEnumerable();

            // Поиск по клиенту (textBox1)
            string searchText = textBox1.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(searchText))
            {
                rows = rows.Where(row =>
                {
                    string fullName = GetSafeString(row["FullName_client"]);
                    string phone = GetSafeString(row["phone"]);
                    string contract = GetSafeString(row["ID_contract"]);
                    string status = GetSafeString(row["StatusName"]);

                    return fullName.ToLower().Contains(searchText) ||
                           phone.ToLower().Contains(searchText) ||
                           contract.ToLower().Contains(searchText) ||
                           status.ToLower().Contains(searchText);
                });
            }

            // Поиск по сотруднику (textBox2)
            string employeeSearch = textBox2.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(employeeSearch) && originalDataTable.Columns.Contains("EmployeeName"))
            {
                rows = rows.Where(row =>
                    GetSafeString(row["EmployeeName"]).ToLower().Contains(employeeSearch));
            }

            // Фильтрация по статусу и другим параметрам
            string filter = comboBox3.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(filter) && filter != "Все клиенты")
            {
                switch (filter)
                {
                    case "LTV > 500000":
                        rows = rows.Where(row => GetSafeDecimal(row["LTV"]) > 500000);
                        break;
                    case "Клиенты в работе":
                        rows = rows.Where(row => GetSafeString(row["StatusName"]).Trim() == "Идет строительство");
                        break;
                    case "Буфер клиентов":
                        rows = rows.Where(row => GetSafeString(row["StatusName"]).Trim() == "Буфер");
                        break;
                    case "Подписываем договор":
                        rows = rows.Where(row => GetSafeString(row["StatusName"]).Trim() == "Подписываем договор");
                        break;
                    case "Завершён":
                        rows = rows.Where(row => GetSafeString(row["StatusName"]).Trim() == "Завершён");
                        break;
                    case "Закрепленные сотрудники":
                        if (originalDataTable.Columns.Contains("EmployeeName"))
                            rows = rows.Where(row => !string.IsNullOrEmpty(GetSafeString(row["EmployeeName"])));
                        else
                            rows = Enumerable.Empty<DataRow>();
                        break;
                }
            }

            // Копируем отфильтрованные строки
            foreach (DataRow row in rows)
            {
                filteredTable.ImportRow(row);
            }

            // Сортировка
            string sortBy = comboBox1.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(sortBy) && sortBy != "Без сортировки")
            {
                switch (sortBy)
                {
                    case "По ФИО":
                        filteredTable.DefaultView.Sort = "FullName_client ASC";
                        break;
                    case "По статусу":
                        filteredTable.DefaultView.Sort = "StatusName ASC";
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
                "Клиенты в работе",
                "Буфер клиентов",
                "Подписываем договор",
                "Завершён",
              //  "Закрепленные сотрудники"
            });
            comboBox3.SelectedIndex = 0;

            FillTableData();
            dataGridView1.ContextMenuStrip = contextMenuStrip1;
            dataGridView1.MouseDown += dataGridView1_MouseDown;
        }

     

     

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

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            ApplyFiltersAndSorting();
        }
    }
}