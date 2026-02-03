using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kursivoy_Konkin
{
    public partial class FormAdminWorker : Form
    {
        private string ConnectionString = connect.con;
        private DataTable originalDataTable;

        public FormAdminWorker()
        {
            InitializeComponent();
            InitializeContextMenu();
        }

        private void FormAdminWorker_Load(object sender, EventArgs e)
        {
            FillTableData();
        }

        // =========================================================
        // ЗАПОЛНЕНИЕ ТАБЛИЦЫ СОТРУДНИКОВ
        // =========================================================
        private void FillTableData()
        {
            dataGridView1.Columns.Clear();

            string query = @"
                SELECT 
                    w.ID_worker,
                    w.FIO,
                    w.Age,
                    w.phone,
                    r.Role,
                    w.ID_Role,
                    c.FullName_client,
                    w.ID_Clientsl,
                    w.IsDeleted
                FROM mydb.worker w
                LEFT JOIN mydb.role_worker r ON w.ID_Role = r.ID_Role
                LEFT JOIN mydb.clients c ON w.ID_Clientsl = c.ID_Client
                WHERE w.IsDeleted = 0
                ORDER BY w.ID_worker";

            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                using (var command = new MySqlCommand(query, connection))
                using (var adapter = new MySqlDataAdapter(command))
                {
                    DataTable table = new DataTable();
                    connection.Open();
                    adapter.Fill(table);

                    originalDataTable = table.Copy();
                    dataGridView1.DataSource = table;

                    // =========================
                    // Заголовки колонок
                    // =========================
                    SetHeader("FIO", "ФИО");
                    SetHeader("Age", "Возраст");
                    SetHeader("phone", "Телефон");
                    SetHeader("Role", "Роль");
                    SetHeader("FullName_client", "Клиент");

                    // =========================
                    // Скрытые поля
                    // =========================
                    HideColumn("ID_worker");
                    HideColumn("ID_Role");
                    HideColumn("ID_Clientsl");
                    HideColumn("IsDeleted");

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

        // Вспомогательные методы для настройки таблицы
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

        // =========================================================
        // ИНИЦИАЛИЗАЦИЯ КОНТЕКСТНОГО МЕНЮ
        // =========================================================
        private void InitializeContextMenu()
        {
            contextMenuStrip1.Items.Clear();

            var editItem = new ToolStripMenuItem("Редактировать сотрудника") { Name = "EditWorker" };
            var deleteItem = new ToolStripMenuItem("Удалить сотрудника") { Name = "DeleteWorker" };

            contextMenuStrip1.Items.Add(editItem);
            contextMenuStrip1.Items.Add(deleteItem);

            // Подписываем обработчики
            editItem.Click -= EditWorker_Click;
            editItem.Click += EditWorker_Click;

            deleteItem.Click -= DeleteWorker_Click;
            deleteItem.Click += DeleteWorker_Click;

            // Привязываем контекстное меню к dataGridView1
            dataGridView1.ContextMenuStrip = contextMenuStrip1;
        }

        // =========================================================
        // ОБРАБОТЧИК КНОПКИ: ДОБАВИТЬ СОТРУДНИКА
        // =========================================================
        private void buttonAddWorker_Click(object sender, EventArgs e)
        {
           // var addForm = new FormAdminAddWorker();
           // var result = addForm.ShowDialog();
            //if (result == DialogResult.OK)
            //{
            //    FillTableData();
            //}
        }

        // =========================================================
        // ОБРАБОТЧИК ПКМ: РЕДАКТИРОВАТЬ СОТРУДНИКА
        // =========================================================
        private void EditWorker_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите строку для редактирования.", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var row = dataGridView1.SelectedRows[0];

            if (row.Cells["ID_worker"].Value == null || row.Cells["ID_worker"].Value == DBNull.Value)
            {
                MessageBox.Show("Не найден ID выбранного сотрудника.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int workerId;
            try
            {
                workerId = Convert.ToInt32(row.Cells["ID_worker"].Value);
            }
            catch
            {
                MessageBox.Show("Некорректный ID сотрудника.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var editForm = new FormAdminEditWorker();
            //editForm.LoadRolesCombo();
            //editForm.LoadClientsCombo();
            //editForm.LoadWorkerById(workerId);

            var result = editForm.ShowDialog();
            if (result == DialogResult.OK)
            {
                FillTableData();
            }
        }

        // =========================================================
        // ОБРАБОТЧИК ПКМ: УДАЛИТЬ СОТРУДНИКА (МЯГКОЕ УДАЛЕНИЕ)
        // =========================================================
        private void DeleteWorker_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите строку для удаления.", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var row = dataGridView1.SelectedRows[0];
            if (row.Cells["ID_worker"].Value == null || row.Cells["ID_worker"].Value == DBNull.Value)
            {
                MessageBox.Show("Не найден ID выбранного сотрудника.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int workerId;
            try
            {
                workerId = Convert.ToInt32(row.Cells["ID_worker"].Value);
            }
            catch
            {
                MessageBox.Show("Некорректный ID сотрудника.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string workerName = row.Cells["FIO"]?.Value?.ToString() ?? $"ID {workerId}";
            var confirm = MessageBox.Show(
                $"Скрыть сотрудника \"{workerName}\" (ID {workerId}) из списка? Запись останется в БД.",
                "Подтвердите действие",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionString))
                using (MySqlCommand cmd = new MySqlCommand(
                    "UPDATE mydb.worker SET IsDeleted = 1 WHERE ID_worker = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", workerId);
                    conn.Open();
                    int affected = cmd.ExecuteNonQuery();
                    if (affected > 0)
                    {
                        MessageBox.Show("Сотрудник скрыт из списка.", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        FillTableData();
                    }
                    else
                    {
                        MessageBox.Show("Сотрудник не найден или уже скрыт.", "Информация",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Ошибка БД при изменении: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выполнении: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // ОБРАБОТЧИК ПРАВОЙ КНОПКИ МЫШИ НА DATAGRIDVIEW
        // =========================================================
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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Можно использовать для дополнительных действий при клике на ячейку
        }
    }
}