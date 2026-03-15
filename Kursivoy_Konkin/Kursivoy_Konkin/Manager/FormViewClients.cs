using Kursivoy_Konkin.Manager;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Kursivoy_Konkin
{
    public partial class FormViewClients : Form
    {
        private DataTable originalDataTable;

        private int currentPage = 1;
        private int pageSize = 20;
        private int totalRecords = 0;
        private int totalPages = 0;


        private bool isMasked = true;

        public FormViewClients()
        {
            InitializeComponent();
            InitializeContextMenu();
            InitializeSearchAndFilter();
            InitializePaginationEvents();


           
            this.MinimizeBox = false;
            this.MaximizeBox = false;
        }

        private void InitializePaginationEvents()
        {
            btnFirst.Click += BtnFirst_Click;
            btnPrev.Click += BtnPrev_Click;
            btnNext.Click += BtnNext_Click;
            btnLast.Click += BtnLast_Click;
            txtPageNumber.KeyPress += TxtPageNumber_KeyPress;
        }

        private void BtnFirst_Click(object sender, EventArgs e)
        {
            if (currentPage > 1) { currentPage = 1; ApplyFilters(); }
        }

        private void BtnPrev_Click(object sender, EventArgs e)
        {
            if (currentPage > 1) { currentPage--; ApplyFilters(); }
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            if (currentPage < totalPages) { currentPage++; ApplyFilters(); }
        }

        private void BtnLast_Click(object sender, EventArgs e)
        {
            if (currentPage < totalPages) { currentPage = totalPages; ApplyFilters(); }
        }

        private void TxtPageNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (int.TryParse(txtPageNumber.Text, out int pageNumber))
                {
                    if (pageNumber >= 1 && pageNumber <= totalPages)
                    {
                        currentPage = pageNumber;
                        ApplyFilters();
                    }
                    else
                    {
                        MessageBox.Show($"Введите номер страницы от 1 до {totalPages}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                e.Handled = true;
            }
        }

        private void UpdatePaginationInfo()
        {
            int displayedRecords = dataGridView1.Rows.Count;
            lblRecordsInfo.Text = $"Показано записей: {displayedRecords} из {totalRecords}";
            lblPageInfo.Text = $"Страница {currentPage} из {totalPages}";
            txtPageNumber.Text = currentPage.ToString();

            btnFirst.Enabled = currentPage > 1;
            btnPrev.Enabled = currentPage > 1;
            btnNext.Enabled = currentPage < totalPages;
            btnLast.Enabled = currentPage < totalPages;
        }

        private void InitializeContextMenu()
        {
            contextMenuStrip1 = new ContextMenuStrip();

            var menuItemView = new ToolStripMenuItem("Просмотреть запись");
            menuItemView.Click += MenuItemView_Click;
            contextMenuStrip1.Items.Add(menuItemView);

            var menuItemAdd = new ToolStripMenuItem("Добавить клиента");
            menuItemAdd.Click += MenuItemAdd_Click;
            contextMenuStrip1.Items.Add(menuItemAdd);

            var menuItemEdit = new ToolStripMenuItem("Редактировать");
            menuItemEdit.Click += MenuItemEdit_Click;
            contextMenuStrip1.Items.Add(menuItemEdit);

            var menuItemDelete = new ToolStripMenuItem("Удалить");
            menuItemDelete.Click += MenuItemDelete_Click;
            contextMenuStrip1.Items.Add(menuItemDelete);

            dataGridView1.ContextMenuStrip = contextMenuStrip1;
        }

        private void MenuItemView_Click(object sender, EventArgs e)
        {
            int clientId = GetSelectedClientId();
            if (clientId <= 0)
            {
                MessageBox.Show("Выберите клиента для просмотра.", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var viewForm = new FormViewClientDetails(clientId))
            {
                this.Visible = false;
                viewForm.ShowDialog();
                this.Close();
            }
        }
        
        private void MenuItemAdd_Click(object sender, EventArgs e)
        {
            using (var addClientForm = new FormManagerAddClient())
            {
                this.Visible = false;
                if (addClientForm.ShowDialog() == DialogResult.OK)
                {
                    currentPage = 1;
                    FillTableData();
                }
            }
        }

        private void MenuItemEdit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Берём ID из скрытой колонки originalDataTable через текущую страницу
                int clientId = GetSelectedClientId();
                if (clientId <= 0)
                {
                    MessageBox.Show("Не удалось определить ID клиента.", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                using (var editClientForm = new FormManagerEditClients())
                {
                    this.Visible = false;
                    editClientForm.LoadClientById(clientId);
                    if (editClientForm.ShowDialog() == DialogResult.OK)
                        FillTableData();
                }
            }
            else
            {
                MessageBox.Show("Выберите клиента для редактирования.", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void MenuItemDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int clientId = GetSelectedClientId();
                if (clientId <= 0)
                {
                    MessageBox.Show("Не удалось определить ID клиента.", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var result = MessageBox.Show("Вы уверены, что хотите удалить клиента?", "Подтверждение",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

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
                        FillTableData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении клиента: {ex.Message}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите клиента для удаления.", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Получает ID_Client выбранной строки из оригинальной таблицы по индексу на текущей странице
        /// </summary>
        private int GetSelectedClientId()
        {
            try
            {
                if (dataGridView1.SelectedRows.Count == 0) return -1;

                // DataSource содержит отображаемую (замаскированную) таблицу без ID_Client
                // Нам нужно найти оригинальную строку по ФИО + Телефону (они уникальны в паре)
                // Лучший способ — хранить ID в скрытой колонке отображаемой таблицы
                // Читаем из Tag строки
                var selectedRow = dataGridView1.SelectedRows[0];
                if (selectedRow.Tag != null && int.TryParse(selectedRow.Tag.ToString(), out int id))
                    return id;

                return -1;
            }
            catch
            {
                return -1;
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
                        MessageBox.Show("Данные не найдены.", "Информация",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    originalDataTable = table.Copy();
                    totalRecords = table.Rows.Count;
                    totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
                    currentPage = 1;

                    ApplyFilters();

                    dataGridView1.AllowUserToAddRows = false;
                    dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    dataGridView1.MultiSelect = false;
                    dataGridView1.ReadOnly = true;
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении данных: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            isMasked = true; // данные скрыты по умолчанию
            

            FillTableData();
            UpdateClientBirthdaysAndAges();
            comboBox1.Items.AddRange(new[] { "ФИО", "Статус", "LTV" });
            comboBox2.Items.AddRange(new[] { "Все", "Больше 500 000", "Меньше 1 000 000", "Больше 2 000 000" });
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
                    adapter.Fill(statusTable);
                    foreach (DataRow row in statusTable.Rows)
                        comboBox3.Items.Add(row["status"].ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке статусов: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeSearchAndFilter()
        {
            textBox1.TextChanged += (s, e) => { currentPage = 1; ApplyFilters(); };
            comboBox1.SelectedIndexChanged += (s, e) => { currentPage = 1; ApplyFilters(); };
            comboBox3.SelectedIndexChanged += (s, e) => { currentPage = 1; ApplyFilters(); };
            comboBox2.SelectedIndexChanged += (s, e) => { currentPage = 1; ApplyFilters(); };
        }

        private void ApplyFilters()
        {
            try
            {
                if (originalDataTable == null) return;

                var filteredData = originalDataTable.AsEnumerable();

                // Фильтр по тексту
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

                // Сортировка
                if (comboBox1.SelectedItem != null)
                {
                    switch (comboBox1.SelectedItem.ToString())
                    {
                        case "ФИО":
                            filteredData = filteredData.OrderBy(row => row["ФИО"]);
                            break;
                        case "Статус":
                            filteredData = filteredData.OrderBy(row => row["Статус"]);
                            break;
                        case "LTV":
                            filteredData = filteredData.OrderByDescending(row => Convert.ToDecimal(row["LTV"]));
                            break;
                    }
                }

                // Пагинация
                var filteredList = filteredData.ToList();
                int totalFilteredRecords = filteredList.Count;
                int totalPagesFiltered = (int)Math.Ceiling((double)totalFilteredRecords / pageSize);

                if (currentPage > totalPagesFiltered)
                    currentPage = totalPagesFiltered > 0 ? totalPagesFiltered : 1;

                if (filteredList.Count == 0)
                {
                    dataGridView1.DataSource = BuildDisplayTable(new List<DataRow>());
                    totalRecords = 0;
                    totalPages = 0;
                    currentPage = 1;
                }
                else
                {
                    var pagedRows = filteredList
                        .Skip((currentPage - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    // ✅ Строим отображаемую таблицу с учётом маски
                    dataGridView1.DataSource = BuildDisplayTable(pagedRows);

                    totalRecords = totalFilteredRecords;
                    totalPages = totalPagesFiltered;

                    // Сохраняем ID клиента в Tag каждой строки грида
                    for (int i = 0; i < pagedRows.Count && i < dataGridView1.Rows.Count; i++)
                        dataGridView1.Rows[i].Tag = pagedRows[i]["ID_Client"].ToString();
                }

                // Скрываем служебные колонки если вдруг попали
                HideColumn("ID_Client");

                UpdatePaginationInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при фильтрации: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void HideColumn(string columnName)
        {
            if (dataGridView1.Columns.Contains(columnName))
                dataGridView1.Columns[columnName].Visible = false;
        }

        // ─────────────────────────────────────────────────────────────
        //  КЛЮЧЕВОЙ МЕТОД: строит DataTable для отображения
        //  с маскировкой или без — в зависимости от isMasked
        // ─────────────────────────────────────────────────────────────
        private DataTable BuildDisplayTable(List<DataRow> rows)
        {
            var display = new DataTable();
            display.Columns.Add("ФИО", typeof(string));
            display.Columns.Add("Телефон", typeof(string));
            display.Columns.Add("Дата рождения", typeof(string));
            display.Columns.Add("Возраст", typeof(object));
            display.Columns.Add("Статус", typeof(string));
            display.Columns.Add("LTV", typeof(object));

            foreach (var row in rows)
            {
                string fio = row["ФИО"].ToString();
                string phone = row["Телефон"].ToString();
                string bday = row["Дата рождения"] == DBNull.Value
                                   ? ""
                                   : Convert.ToDateTime(row["Дата рождения"]).ToString("dd.MM.yyyy");
                object age = row["Возраст"];
                string status = row["Статус"].ToString();
                object ltv = row["LTV"];

                if (isMasked)
                {
                    display.Rows.Add(
                        MaskFio(fio),
                        MaskPhone(phone),
                        "**.**.**** ",
                        age,
                        status,
                        ltv);
                }
                else
                {
                    display.Rows.Add(fio, phone, bday, age, status, ltv);
                }
            }

            return display;
        }

        // ─────────────────────────────────────────
        //  МАСКИРОВКА
        // ─────────────────────────────────────────

        /// <summary>Маскирует каждое слово ФИО: первые 2-3 буквы + звёздочки</summary>
        private string MaskFio(string fio)
        {
            if (string.IsNullOrEmpty(fio)) return fio;
            var words = fio.Split(' ');
            var masked = words.Select(word =>
            {
                if (word.Length <= 2) return new string('*', word.Length);
                int visible = word.Length <= 4 ? 2 : 3;
                return word.Substring(0, visible) + new string('*', word.Length - visible);
            });
            return string.Join(" ", masked);
        }

        /// <summary>Маскирует телефон: показывает "+7 (XXX) ", остальное — звёздочки</summary>
        private string MaskPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone)) return phone;
            int visible = Math.Min(9, phone.Length);
            return phone.Substring(0, visible) + new string('*', phone.Length - visible);
        }


        private void FormViewClients_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
                e.Cancel = true;
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