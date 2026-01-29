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
        LEFT JOIN mydb.status_client s ON c.Status_client_ID_Status_client = s.ID_Status_client
        WHERE c.IsDeleted = 0;";

        string comAttached = @"SELECT c.*, s.status as StatusName, w.FIO as EmployeeName
        FROM mydb.clients c
        LEFT JOIN mydb.status_client s ON c.Status_client_ID_Status_client = s.ID_Status_client
        LEFT JOIN mydb.worker w ON c.ID_Client = w.ID_Clientsl
        WHERE w.FIO IS NOT NULL AND c.IsDeleted = 0;";
        private void FillTableData(string filter = "")
        {
            dataGridView1.Columns.Clear();

            // По умолчанию показываем всех клиентов; если нужно — можно передать filter = "attached" для показа только закреплённых
            string query = string.IsNullOrEmpty(filter) ? com : comAttached;

            using (MySqlConnection connection = new MySqlConnection(connect.con))
            using (MySqlCommand command = new MySqlCommand(query, connection))
            using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
            {
                DataTable table = new DataTable();
                connection.Open();
                adapter.Fill(table);
                originalDataTable = table.Copy();
                dataGridView1.DataSource = table;

                // Настройка столбцов
                if (table.Columns.Contains("FullName_client")) dataGridView1.Columns["FullName_client"].HeaderText = "ФИО";
                if (table.Columns.Contains("phone")) dataGridView1.Columns["phone"].HeaderText = "Телефон";
                if (table.Columns.Contains("Age")) dataGridView1.Columns["Age"].HeaderText = "Возраст";
                if (table.Columns.Contains("StatusName")) dataGridView1.Columns["StatusName"].HeaderText = "Статус";
                if (table.Columns.Contains("Qualified_lead")) dataGridView1.Columns["Qualified_lead"].HeaderText = "Квал лид";
                if (table.Columns.Contains("LTV")) dataGridView1.Columns["LTV"].HeaderText = "LTV";
                if (table.Columns.Contains("EmployeeName")) dataGridView1.Columns["EmployeeName"].HeaderText = "Сотрудник";

                if (table.Columns.Contains("ID_Client")) dataGridView1.Columns["ID_Client"].Visible = false;
                if (table.Columns.Contains("photo_clients")) dataGridView1.Columns["photo_clients"].Visible = false;
                if (table.Columns.Contains("Status_client_ID_Status_client")) dataGridView1.Columns["Status_client_ID_Status_client"].Visible = false;

                // Скрываем флаг soft-delete, если он пришёл в выборке
                if (table.Columns.Contains("IsDeleted"))
                    dataGridView1.Columns["IsDeleted"].Visible = false;

                // Добавляем колонку с изображением (если ещё нет)
                if (!dataGridView1.Columns.Contains("Фото"))
                {
                    DataGridViewImageColumn imageColumn = new DataGridViewImageColumn
                    {
                        Name = "Фото",
                        ImageLayout = DataGridViewImageCellLayout.Zoom
                    };
                    dataGridView1.Columns.Add(imageColumn);
                }

                dataGridView1.AllowUserToAddRows = false;

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.IsNewRow) continue;

                    string namee = (table.Columns.Contains("photo_clients") && row.Cells["photo_clients"].Value != null)
                        ? row.Cells["photo_clients"].Value.ToString()
                        : "picture.png";

                    if (string.IsNullOrWhiteSpace(namee)) namee = "picture.png";

                    try
                    {
                        row.Cells["Фото"].Value = Image.FromFile(@"./img/picture.png");
                    }
                    catch
                    {
                        // Игнорируем ошибку загрузки изображения
                    }
                }
            }
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

                    string status = GetSafeString(row["StatusName"]);

                    return fullName.ToLower().Contains(searchText) ||
                           phone.ToLower().Contains(searchText) ||
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

            // Скрываем флаг soft-delete в фильтрованной таблице, если он есть
            if (filteredTable.Columns.Contains("IsDeleted") && dataGridView1.Columns.Contains("IsDeleted"))
                dataGridView1.Columns["IsDeleted"].Visible = false;

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
            // Инициализация комбобоксов
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

            // Загружаем данные в таблицу
            FillTableData();

            // Устанавливаем контекстное меню и обработчик мыши
            dataGridView1.ContextMenuStrip = contextMenuStrip1;
            dataGridView1.MouseDown += dataGridView1_MouseDown;

            // Гарантированно создаём пункты контекстного меню (если их нет в Designer или они были случайно удалены)
            contextMenuStrip1.Items.Clear();

            var addItem = new ToolStripMenuItem("Добавить") { Name = "AddUser" };
            var editItem = new ToolStripMenuItem("Редактировать") { Name = "EditUser" };
            var deleteItem = new ToolStripMenuItem("Удалить") { Name = "DeleteUser" };

            contextMenuStrip1.Items.Add(addItem);
            contextMenuStrip1.Items.Add(editItem);
            contextMenuStrip1.Items.Add(deleteItem);

            // Подписываем обработчики (без дублирования)
            addItem.Click -= AddUser_Click;
            addItem.Click += AddUser_Click;

            editItem.Click -= EditUser_Click;
            editItem.Click += EditUser_Click;

            deleteItem.Click -= DeleteUser_Click;
            deleteItem.Click += DeleteUser_Click;
        }

        private void AddUser_Click(object sender, EventArgs e)
        {
            var addForm = new FormManagerAddClient();
            var result = addForm.ShowDialog();
            if (result == DialogResult.OK)
            {
                FillTableData();
            }
        }

        private void EditUser_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите строку для редактирования.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var row = dataGridView1.SelectedRows[0];

            if (row.Cells["ID_Client"].Value == null || row.Cells["ID_Client"].Value == DBNull.Value)
            {
                MessageBox.Show("Не найден ID выбранного клиента.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int clientId;
            try
            {
                clientId = Convert.ToInt32(row.Cells["ID_Client"].Value);
            }
            catch
            {
                MessageBox.Show("Некорректный ID клиента.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var editForm = new FormManagerEditClients();
            editForm.LoadStatusCombo();
            editForm.LoadClientById(clientId);

            var result = editForm.ShowDialog();
            if (result == DialogResult.OK)
            {
                FillTableData();
            }
        }

        // Новый обработчик для удаления клиента из контекстного меню
        private void DeleteUser_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите строку для удаления.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var row = dataGridView1.SelectedRows[0];
            if (row.Cells["ID_Client"].Value == null || row.Cells["ID_Client"].Value == DBNull.Value)
            {
                MessageBox.Show("Не найден ID выбранного клиента.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int clientId;
            try
            {
                clientId = Convert.ToInt32(row.Cells["ID_Client"].Value);
            }
            catch
            {
                MessageBox.Show("Некорректный ID клиента.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string fullName = row.Cells["FullName_client"]?.Value?.ToString() ?? $"ID {clientId}";
            var confirm = MessageBox.Show($"Скрыть клиента \"{fullName}\" (ID {clientId}) из списка? Запись останется в БД.", "Подтвердите действие", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connect.con))
                using (MySqlCommand cmd = new MySqlCommand("UPDATE mydb.clients SET IsDeleted = 1 WHERE ID_Client = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", clientId);
                    conn.Open();
                    int affected = cmd.ExecuteNonQuery();
                    if (affected > 0)
                    {
                        MessageBox.Show("Клиент скрыт из списка.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        FillTableData();
                    }
                    else
                    {
                        MessageBox.Show("Клиент не найден или уже скрыт.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Ошибка БД при изменении: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выполнении: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            //final
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
            FormManagerNavigation f = new FormManagerNavigation();
            f.Show();
            this.Close();
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

        private void button3_Click(object sender, EventArgs e)
        {
            using (var addForm = new FormManagerAddClient())
            {
                var result = addForm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    // Если клиент успешно добавлен — перезагрузим данные в таблице
                    FillTableData();
                }
            }
        }
    }

    // Новый SQL-запрос с фильтрацией по IsDeleted = 0
   
}