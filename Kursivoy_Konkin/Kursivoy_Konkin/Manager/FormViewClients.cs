using MySql.Data.MySqlClient; // Подключение библиотеки для работы с MySQL
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Kursivoy_Konkin
{
    public partial class FormViewClients : Form
    {
        private DataTable originalDataTable; // Хранит исходные данные для фильтрации и поиска

        public FormViewClients()
        {
            InitializeComponent(); // Инициализация компонентов формы
            InitializeContextMenu(); // Создание контекстного меню
            InitializeSearchAndFilter(); // Установка поиска и фильтрации
            this.MinimizeBox = false; // Запрет сворачивания
            this.MaximizeBox = false; // Запрет разворачивания
            
        }

        private void InitializeContextMenu()
        {
            // Создаем контекстное меню
            contextMenuStrip1 = new ContextMenuStrip();

            // Добавляем пункт "Добавить клиента"
            var menuItemAdd = new ToolStripMenuItem("Добавить клиента");
            menuItemAdd.Click += MenuItemAdd_Click; // Обработка клика
            contextMenuStrip1.Items.Add(menuItemAdd);

            // Добавляем пункт "Редактировать"
            var menuItemEdit = new ToolStripMenuItem("Редактировать");
            menuItemEdit.Click += MenuItemEdit_Click;
            contextMenuStrip1.Items.Add(menuItemEdit);

            // Добавляем пункт "Удалить"
            var menuItemDelete = new ToolStripMenuItem("Удалить");
            menuItemDelete.Click += MenuItemDelete_Click;
            contextMenuStrip1.Items.Add(menuItemDelete);

            // Привязываем контекстное меню к DataGridView
            dataGridView1.ContextMenuStrip = contextMenuStrip1;
        }

        // Обработка добавления нового клиента
        private void MenuItemAdd_Click(object sender, EventArgs e)
        {
            // Открываем форму добавления клиента
            using (var addClientForm = new FormManagerAddClient())
            {
                this.Visible = false; // Скрываем текущую форму
                if (addClientForm.ShowDialog() == DialogResult.OK)
                {
                    // Обновляем таблицу после добавления нового клиента
                    FillTableData();
                }
            }
        }

        // Обработка редактирования выбранного клиента
        private void MenuItemEdit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Получаем ID клиента из выбранной строки
                int clientId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID_Client"].Value);

                // Открываем форму редактирования клиента
                using (var editClientForm = new FormManagerEditClients())
                {
                    this.Visible = false; // Скрываем текущую форму
                    // Загружаем данные клиента по ID
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

        // Обработка удаления клиента
        private void MenuItemDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Получаем ID клиента из выбранной строки
                int clientId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID_Client"].Value);

                // Запрашиваем подтверждение удаления
                var result = MessageBox.Show("Вы уверены, что хотите удалить клиента?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        // Выполняем мягкое удаление (установка IsDeleted = 1)
                        string query = "UPDATE mydb.clients SET IsDeleted = 1 WHERE ID_Client = @IDClient;";
                        using (var connection = new MySqlConnection(connect.con))
                        using (var command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@IDClient", clientId);
                            connection.Open();
                            command.ExecuteNonQuery();
                        }
                        // Обновляем таблицу после удаления
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

        // Заполняет таблицу данных
        private void FillTableData()
        {
            try
            {
                dataGridView1.Columns.Clear(); // Очищаем текущие столбцы

                // SQL-запрос для получения данных клиентов
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
                    adapter.Fill(table); // Заполняем DataTable данными

                    if (table.Rows.Count == 0)
                    {
                        MessageBox.Show("Данные не найдены.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    originalDataTable = table.Copy(); // Сохраняем оригинал данных для фильтров
                    dataGridView1.DataSource = table; // Устанавливаем источник данных для таблицы

                    // Устанавливаем читаемые заголовки колонок
                    SetHeader("ФИО", "ФИО");
                    SetHeader("Телефон", "Телефон");
                    SetHeader("Дата рождения", "Дата рождения");
                    SetHeader("Возраст", "Возраст");
                    SetHeader("Статус", "Статус");
                    SetHeader("LTV", "LTV");

                    // Скрываем ID_Client, так как он не нужен для отображения
                    HideColumn("ID_Client");

                    // Настраиваем DataGridView
                    dataGridView1.AllowUserToAddRows = false; // Запрещаем пользователю добавлять строки вручную
                    dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // Выделение всей строки
                    dataGridView1.MultiSelect = false; // Разрешаем выбор только одной строки
                    dataGridView1.ReadOnly = true; // Запрет редактирования
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Расширение колонок
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Устанавливает заголовки колонок таблицы
        private void SetHeader(string columnName, string headerText)
        {
            if (dataGridView1.Columns.Contains(columnName))
                dataGridView1.Columns[columnName].HeaderText = headerText;
        }

        // Скрывает указанные колонки таблицы
        private void HideColumn(string columnName)
        {
            if (dataGridView1.Columns.Contains(columnName))
                dataGridView1.Columns[columnName].Visible = false;
        }

        // Обновляет возраст клиентов на основе даты рождения
        private void UpdateClientBirthdaysAndAges()
        {
            try
            {
                // Обновляем возраст для всех клиентов
                string query = @"
                    UPDATE mydb.clients
                    SET Age = TIMESTAMPDIFF(YEAR, Birthday, CURDATE())
                    WHERE Birthday IS NOT NULL;";

                using (var connection = new MySqlConnection(connect.con))
                using (var command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    int affectedRows = command.ExecuteNonQuery(); // Количество обновленных строк

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обработка правого клика мыши для показа контекстного меню
        private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hitTestInfo = dataGridView1.HitTest(e.X, e.Y);
                if (hitTestInfo.RowIndex >= 0)
                {
                    // Выделение строки под курсором
                    dataGridView1.ClearSelection();
                    dataGridView1.Rows[hitTestInfo.RowIndex].Selected = true;
                    // Отображение контекстного меню
                    contextMenuStrip1.Show(dataGridView1, e.Location);
                }
            }
        }

        private void FormViewClients_Load(object sender, EventArgs e)
        {
            // Загружаем данные при запуске формы
            FillTableData();

            // Обновляем возраст клиентов
            UpdateClientBirthdaysAndAges();

            // Инициализация комбобоксов
            comboBox1.Items.AddRange(new[] { "ФИО", "Статус", "LTV" });
            comboBox2.Items.AddRange(new[] { "Все", "Больше 500 000", "Меньше 1 000 000", "Больше 2 000 000" });

            // Загружаем статусы для фильтрации
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

                    // Добавляем статусы в ComboBox
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

        // Настраиваем фильтры и поиск
        private void InitializeSearchAndFilter()
        {
            // Поиск по тексту (ФИО, телефон)
            textBox1.TextChanged += (s, e) => ApplyFilters();
            // Фильтр по статусу
            comboBox1.SelectedIndexChanged += (s, e) => ApplyFilters();
            // Фильтр по статусу клиента
            comboBox3.SelectedIndexChanged += (s, e) => ApplyFilters();
            // Фильтр по LTV
            comboBox2.SelectedIndexChanged += (s, e) => ApplyFilters();
        }

        // Применяет фильтры к данным по текущим настройкам
        private void ApplyFilters()
        {
            try
            {

           
            if (originalDataTable == null) return;

            var filteredData = originalDataTable.AsEnumerable();

            // Поиск по тексту (ФИО, телефон)
            string searchText = textBox1.Text.ToLower();
            if (!string.IsNullOrEmpty(searchText))
            {
                filteredData = filteredData.Where(row =>
                    row["ФИО"].ToString().ToLower().Contains(searchText) ||
                    row["Телефон"].ToString().ToLower().Contains(searchText));
            }

            // Фильтр по статусу
            if (comboBox3.SelectedItem != null && comboBox3.SelectedItem.ToString() != "Все")
            {
                string selectedStatus = comboBox3.SelectedItem.ToString();
                filteredData = filteredData.Where(row => row["Статус"].ToString() == selectedStatus);
            }

            // Фильтр по LTV
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

            // Сортировка по выбранному столбцу
            if (comboBox1.SelectedItem != null)
            {
                string sortColumn = comboBox1.SelectedItem.ToString();
                switch (sortColumn)
                {
                    case "ФИО":
                        filteredData = filteredData.OrderBy(row => row["ФИО"]);
                        break;
                        // Можно добавить остальные сортировки по другим колонкам
                }
            }

            // Обновляем DataGridView данными
            dataGridView1.DataSource = filteredData.CopyToDataTable();
            }
            catch (Exception ex)
            {
                
            }
        }

        private void FormViewClients_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true; //отменяем закрытие формы
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Создаем форму 
            FormManagerNavigation f = new FormManagerNavigation();
            this.Visible = false; // Скрываем текущую
            f.ShowDialog(); // Открываем как модальную
            this.Close(); // Закрываем текущую форму после выхода
        }
    }
}