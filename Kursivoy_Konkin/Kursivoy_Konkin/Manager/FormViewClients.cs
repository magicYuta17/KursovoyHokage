using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Kursivoy_Konkin
{
    public partial class FormViewClients : Form
    {
        private DataTable originalDataTable;
       

        public FormViewClients()
        {
            InitializeComponent();
            InitializeContextMenu();
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
        MessageBox.Show("Выберите клиента для удаления.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                        c.Qualified_lead AS 'Квал лид',
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
                    SetHeader("Квал лид", "Квалифицированный лид");
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
        }
    }
}