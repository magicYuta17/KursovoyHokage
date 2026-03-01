using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Kursivoy_Konkin
{
    public partial class FormViewClients : Form
    {
        private DataTable originalDataTable;

        public FormViewClients()
        {
            InitializeComponent();
            InitializeContextMenu();
            InitializeSearchAndFilter();
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.ControlBox = false;
        }

        private void InitializeContextMenu()
        {
            // Создаем контекстное меню
            contextMenuStrip1 = new ContextMenuStrip();

            // Добавляем элемент "Добавить клиента"
            var menuItemAdd = new ToolStripMenuItem("Добавить клиента");
            menuItemAdd.Click += MenuItemAdd_Click;
            contextMenuStrip1.Items.Add(menuItemAdd);

            // Добавляем элемент "Редактировать"
            var menuItemEdit = new ToolStripMenuItem("Редактировать");
            menuItemEdit.Click += MenuItemEdit_Click;
            contextMenuStrip1.Items.Add(menuItemEdit);

            // Добавляем элемент "Удалить"
            var menuItemDelete = new ToolStripMenuItem("Удалить");
            menuItemDelete.Click += MenuItemDelete_Click;
            contextMenuStrip1.Items.Add(menuItemDelete);

            // Привязываем контекстное меню к dataGridView1
            dataGridView1.ContextMenuStrip = contextMenuStrip1;
        }

        private void MenuItemAdd_Click(object sender, EventArgs e)
        {
            // Открываем форму добавления клиента
            using (var addClientForm = new FormManagerAddClient())
            {
                if (addClientForm.ShowDialog() == DialogResult.OK)
                {
                    // Обновляем таблицу после добавления клиента
                    FillTableData();
                }
            }
        }

        private void MenuItemEdit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
    {
        // Получаем ID клиента из выбранной строки
        int clientId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID_Client"].Value);

        // Открываем форму редактирования клиента
        using (var editClientForm = new FormManagerEditClients())
        {
            editClientForm.LoadClientById(clientId);
            if (editClientForm.ShowDialog() == DialogResult.OK)
            {
                // Обновляем таблицу после редактирования клиента
                FillTableData();
            }
        }
    }
    else
    {
        MessageBox.Show("Выберите клиента для редактирования.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}

private void MenuItemDelete_Click(object sender, EventArgs e)
{
    if (dataGridView1.SelectedRows.Count > 0)
    {
        // Получаем ID клиента из выбранной строки
        int clientId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID_Client"].Value);

        // Подтверждение удаления
        var result = MessageBox.Show("Вы уверены, что хотите удалить клиента?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (result == DialogResult.Yes)
        {
            try
            {
                string query = "UPDATE mydb.clients SET IsDeleted = 1 WHERE ID_Client = @IDClient;";
                using (var connection = new MySqlConnection(connect.con))
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IDClient", clientId);
                    connection.Open();
                    command.ExecuteNonQuery();
                }

                // Обновляем таблицу после удаления клиента
                FillTableData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении клиента: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
    else
    {
        MessageBox.Show("Выведите клиента для удаления.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}

        private void FillTableData()
        {
            try
            {
                dataGridView1.Columns.Clear();

                string query = @"
                    SELECT 
                        c.ID_Client,
                        c.FullName_client AS 'ФИО',
                        c.phone AS 'Телефон',
                        c.Birthday AS 'Дата рождения',
                        TIMESTAMPDIFF(YEAR, c.Birthday, CURDATE()) AS 'Возраст',
                        s.status AS 'Статус',
                        c.LTV AS 'LTV'
                    FROM mydb.clients c
                    LEFT JOIN mydb.status_client s ON c.Status_client_ID_Status_client = s.ID_Status_client
                    WHERE c.IsDeleted = 0;";

                using (var connection = new MySqlConnection(connect.con))
                using (var command = new MySqlCommand(query, connection))
                using (var adapter = new MySqlDataAdapter(command))
                {
                    DataTable table = new DataTable();
                    connection.Open();
                    adapter.Fill(table);

                    if (table.Rows.Count == 0)
                    {
                        MessageBox.Show("Данные не найдены.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    originalDataTable = table.Copy();
                    dataGridView1.DataSource = table;

                    // Устанавливаем заголовки
                    SetHeader("ФИО", "ФИО");
                    SetHeader("Телефон", "Телефон");
                    SetHeader("Дата рождения", "Дата рождения");
                    SetHeader("Возраст", "Возраст");
                    SetHeader("Статус", "Статус");
                    SetHeader("LTV", "LTV");

                    // Скрываем ненужные столбцы
                    HideColumn("ID_Client");

                    dataGridView1.AllowUserToAddRows = false;
                    dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    dataGridView1.MultiSelect = false;
                    dataGridView1.ReadOnly = true;
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetHeader(string columnName, string headerText)
        {
            if (dataGridView1.Columns.Contains(columnName))
                dataGridView1.Columns[columnName].HeaderText = headerText;
        }

        private void HideColumn(string columnName)
        {
            if (dataGridView1.Columns.Contains(columnName))
                dataGridView1.Columns[columnName].Visible = false;
        }

        private void UpdateClientBirthdaysAndAges()
        {
            try
            {
                string query = @"
                    UPDATE mydb.clients
                    SET Age = TIMESTAMPDIFF(YEAR, Birthday, CURDATE())
                    WHERE Birthday IS NOT NULL;";

                using (var connection = new MySqlConnection(connect.con))
                using (var command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    int affectedRows = command.ExecuteNonQuery();
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hitTestInfo = dataGridView1.HitTest(e.X, e.Y);
                if (hitTestInfo.RowIndex >= 0)
                {
                    dataGridView1.ClearSelection();
                    dataGridView1.Rows[hitTestInfo.RowIndex].Selected = true;
                    contextMenuStrip1.Show(dataGridView1, e.Location);
                }
            }
        }

        private void FormViewClients_Load(object sender, EventArgs e)
        {
            // Загружаем данные в таблицу
            FillTableData();

            // Обновляем данные о возрасте клиентов
            UpdateClientBirthdaysAndAges();

            // Инициализация ComboBox
            comboBox1.Items.AddRange(new[] { "ФИО", "Статус", "LTV" });
            comboBox2.Items.AddRange(new[] { "Все", "Больше 500 000", "Меньше 1 000 000", "Больше 2 000 000" });

            // Загрузка статусов из базы данных
            LoadStatusesToComboBox3();
        }

        private void LoadStatusesToComboBox3()
        {
            try
            {
                string query = "SELECT DISTINCT status FROM mydb.status_client;";
                using (var connection = new MySqlConnection(connect.con))
                using (var command = new MySqlCommand(query, connection))
                using (var adapter = new MySqlDataAdapter(command))
                {
                    DataTable statusTable = new DataTable();
                    connection.Open();
                    adapter.Fill(statusTable);

                    foreach (DataRow row in statusTable.Rows)
                    {
                        comboBox3.Items.Add(row["status"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке статусов: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeSearchAndFilter()
        {
            textBox1.TextChanged += (s, e) => ApplyFilters();
            comboBox1.SelectedIndexChanged += (s, e) => ApplyFilters();
            comboBox3.SelectedIndexChanged += (s, e) => ApplyFilters();
            comboBox2.SelectedIndexChanged += (s, e) => ApplyFilters();
        }

        private void ApplyFilters()
        {
            if (originalDataTable == null) return;

            var filteredData = originalDataTable.AsEnumerable();

            // Поиск по тексту в TextBox1
            string searchText = textBox1.Text.ToLower();
            if (!string.IsNullOrEmpty(searchText))
            {
                filteredData = filteredData.Where(row =>
                    row["ФИО"].ToString().ToLower().Contains(searchText) ||
                    row["Телефон"].ToString().ToLower().Contains(searchText));
            }

            // Фильтрация по статусу (ComboBox3)
            if (comboBox3.SelectedItem != null && comboBox3.SelectedItem.ToString() != "Все")
            {
                string selectedStatus = comboBox3.SelectedItem.ToString();
                filteredData = filteredData.Where(row => row["Статус"].ToString() == selectedStatus);
            }

            // Фильтрация по LTV (ComboBox2)
            if (comboBox2.SelectedItem != null && comboBox2.SelectedItem.ToString() != "Все")
            {
                switch (comboBox2.SelectedItem.ToString())
                {
                    case "Больше 500 000":
                        filteredData = filteredData.Where(row => Convert.ToDecimal(row["LTV"]) > 500000);
                        break;
                    case "Меньше 1 000 000":
                        filteredData = filteredData.Where(row => Convert.ToDecimal(row["LTV"]) < 1000000);
                        break;
                    case "Больше 2 000 000":
                        filteredData = filteredData.Where(row => Convert.ToDecimal(row["LTV"]) > 2000000);
                        break;
                }
            }

            // Сортировка (ComboBox1)
            if (comboBox1.SelectedItem != null)
            {
                string sortColumn = comboBox1.SelectedItem.ToString();
                switch (sortColumn)
                {
                    case "ФИО":
                        filteredData = filteredData.OrderBy(row => row["ФИО"]);
                        break;
                    case "Статус":
                        filteredData = filteredData.OrderBy(row => row["Статус"]);
                        break;
                    case "LTV":
                        filteredData = filteredData.OrderBy(row => Convert.ToDecimal(row["LTV"]));
                        break;
                }
            }

            // Применение фильтров и сортировки
            if (filteredData.Any())
            {
                dataGridView1.DataSource = filteredData.CopyToDataTable();
            }
            else
            {
                dataGridView1.DataSource = originalDataTable.Clone(); // Пустая таблица
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FormManagerNavigation managerForm = new FormManagerNavigation();
            this.Visible = false;
            managerForm.ShowDialog();
            this.Close();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}