using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Kursivoy_Konkin
{
    public partial class FormHeadViewStatus : Form
    {
        private readonly string ConnectionString = connect.con;
        private ContextMenuStrip _ctx;
        private HashSet<int> _hiddenStatusIds = new HashSet<int>(); // скрытые только в UI

        public FormHeadViewStatus()
        {
            InitializeComponent();
            InitializeUi();
        }

        private void InitializeUi()
        {
            // Настроить DataGridView и контекстное меню
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.MouseDown += DataGridView1_MouseDown;

            // Контекстное меню
            _ctx = new ContextMenuStrip();
            var editItem = new ToolStripMenuItem("Редактировать") { Name = "EditStatus" };
            var deleteItem = new ToolStripMenuItem("Удалить статус") { Name = "DeleteStatus" };

            editItem.Click += EditItem_Click;
            deleteItem.Click += DeleteItem_Click;

            _ctx.Items.Add(editItem);
            _ctx.Items.Add(deleteItem);

            dataGridView1.ContextMenuStrip = _ctx;

            // Привязка кнопки добавления
           
            buttonAddStatus.Click += ButtonAddStatus_Click;

            // Загрузка данных
            Load += FormHeadViewStatus_Load;
        }

        private void FormHeadViewStatus_Load(object sender, EventArgs e)
        {
            FillStatusGrid();
        }

        // Загрузка статусов (отображаем только статус и скрываем ID)
        private void FillStatusGrid()
        {

          
                try
            {
                var dt = new DataTable();
                using (var conn = new MySqlConnection(ConnectionString))
                using (var cmd = new MySqlCommand("SELECT ID_Status_client, status FROM mydb.status_client WHERE IsDeleted = 0 ORDER BY ID_Status_client", conn))
                using (var da = new MySqlDataAdapter(cmd))
                {
                    conn.Open();
                    da.Fill(dt);
                }

                // Убираем скрытые статусы (только в UI)
                if (_hiddenStatusIds.Any())
                {
                    var rowsToKeep = dt.AsEnumerable().Where(r => !_hiddenStatusIds.Contains(Convert.ToInt32(r["ID_Status_client"]))).CopyToDataTableOrEmpty();
                    dataGridView1.DataSource = rowsToKeep;
                    if (dataGridView1.Columns.Contains("IsDeleted"))
                        dataGridView1.Columns["IsDeleted"].Visible = false;
                }
                else
                {
                    dataGridView1.DataSource = dt;

                }

                // Показываем только столбец status
                if (dataGridView1.Columns.Contains("ID_Status_client"))
                    dataGridView1.Columns["ID_Status_client"].Visible = false;

                if (dataGridView1.Columns.Contains("status"))
                {
                    dataGridView1.Columns["status"].HeaderText = "Статус";
                    dataGridView1.Columns["status"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке статусов: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Добавление нового статуса в БД
        private void ButtonAddStatus_Click(object sender, EventArgs e)
        {
            string newStatus = Prompt.ShowDialog("Введите название статуса:", "Добавить статус", "");
            if (newStatus == null) return; // Cancel
            newStatus = newStatus.Trim();
            if (string.IsNullOrEmpty(newStatus))
            {
                MessageBox.Show("Название статуса не может быть пустым.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                using (var cmd = new MySqlCommand("INSERT INTO mydb.status_client (status) VALUES (@status)", conn))
                {
                    cmd.Parameters.AddWithValue("@status", newStatus);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                FillStatusGrid();
            }
            catch (MySqlException mex)
            {
                MessageBox.Show("Ошибка БД при добавлении статуса: " + mex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении статуса: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Правый клик — выделить строку
        private void DataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hit = dataGridView1.HitTest(e.X, e.Y);
                if (hit.RowIndex >= 0)
                {
                    dataGridView1.ClearSelection();
                    var row = dataGridView1.Rows[hit.RowIndex];
                    row.Selected = true;
                    dataGridView1.CurrentCell = row.Cells[Math.Max(0, hit.ColumnIndex)];
                }
                else
                {
                    dataGridView1.ClearSelection();
                    _ctx.Close();
                }
            }
        }

        // Редактировать пункт контекстного меню
        private void EditItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите статус для редактирования.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var row = dataGridView1.SelectedRows[0];
            if (row.Cells["ID_Status_client"].Value == null)
            {
                MessageBox.Show("Не найден идентификатор статуса.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int id = Convert.ToInt32(row.Cells["ID_Status_client"].Value);
            string current = row.Cells["status"].Value?.ToString() ?? string.Empty;

            string edited = Prompt.ShowDialog("Отредактируйте название статуса:", "Редактирование статуса", current);
            if (edited == null) return; // Cancel
            edited = edited.Trim();
            if (string.IsNullOrEmpty(edited))
            {
                MessageBox.Show("Название статуса не может быть пустым.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                using (var cmd = new MySqlCommand("UPDATE mydb.status_client SET status = @status WHERE ID_Status_client = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@status", edited);
                    cmd.Parameters.AddWithValue("@id", id);
                    conn.Open();
                    int affected = cmd.ExecuteNonQuery();
                    if (affected > 0)
                    {
                        FillStatusGrid();
                    }
                    else
                    {
                        MessageBox.Show("Статус не найден или не изменён.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (MySqlException mex)
            {
                MessageBox.Show("Ошибка БД при сохранении статуса: " + mex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении статуса: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Удалить статус — только из UI (строка убирается из DataGridView), не удаляем запись из БД
        private void DeleteItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите статус для удаления.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var row = dataGridView1.SelectedRows[0];
            if (row.Cells["ID_Status_client"].Value == null)
            {
                MessageBox.Show("Не найден идентификатор статуса.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int id = Convert.ToInt32(row.Cells["ID_Status_client"].Value);
            string name = row.Cells["status"].Value?.ToString() ?? $"ID {id}";

            var conf = MessageBox.Show($"Удалить статус \"{name}\" из базы? Если этот статус используется у клиентов — удаление не получится.", 
                "Подтвердите удаление", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (conf != DialogResult.Yes) return;

            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                using (var cmdCheck = new MySqlCommand("SELECT COUNT(*) FROM mydb.clients WHERE Status_client_ID_Status_client = @id AND IsDeleted = 0", conn))
                {
                    cmdCheck.Parameters.AddWithValue("@id", id);
                    conn.Open();
                    long usedCount = Convert.ToInt64(cmdCheck.ExecuteScalar());

                    if (usedCount > 0)
                    {
                        MessageBox.Show($"Нельзя удалить статус — он используется у {usedCount} активного(ых) клиента(ов). Сначала замените/удалите привязки.", 
                            "Ошибка удаления", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Статус нигде не используется — можно удалить
                    using (var cmdDel = new MySqlCommand("DELETE FROM mydb.status_client WHERE ID_Status_client = @id", conn))
                    {
                        cmdDel.Parameters.AddWithValue("@id", id);
                        int affected = cmdDel.ExecuteNonQuery();
                        if (affected > 0)
                        {
                            MessageBox.Show("Статус успешно удалён из базы.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            FillStatusGrid();
                        }
                        else
                        {
                            MessageBox.Show("Статус не найден или уже удалён.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (MySqlException mex)
            {
                // на случай, если все-таки есть FK от других таблиц или другая ошибка
                if (mex.Number == 1451)
                {
                    MessageBox.Show("Невозможно удалить статус — существуют связанные записи в других таблицах.", "Ошибка удаления", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Ошибка БД при попытке удаления статуса: " + mex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при удалении статуса: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Вспомогательный Prompt диалог (простая реализация)
        private static class Prompt
        {
            public static string ShowDialog(string text, string caption, string defaultText)
            {
                using (Form prompt = new Form())
                {
                    prompt.Width = 500;
                    prompt.Height = 170;
                    prompt.FormBorderStyle = FormBorderStyle.FixedDialog;
                    prompt.Text = caption;
                    prompt.StartPosition = FormStartPosition.CenterParent;
                    prompt.MinimizeBox = false;
                    prompt.MaximizeBox = false;
                    prompt.ShowInTaskbar = false;

                    Label textLabel = new Label() { Left = 20, Top = 10, Width = 440, Text = text };
                    TextBox textBox = new TextBox() { Left = 20, Top = 40, Width = 440 };
                    textBox.Text = defaultText ?? string.Empty;
                    textBox.SelectAll();

                    Button confirmation = new Button() { Text = "OK", Left = 280, Width = 80, Top = 75, DialogResult = DialogResult.OK };
                    Button cancel = new Button() { Text = "Отмена", Left = 370, Width = 80, Top = 75, DialogResult = DialogResult.Cancel };
                    confirmation.Font = cancel.Font = new Font("Microsoft Sans Serif", 10);
                    prompt.Controls.Add(textBox);
                    prompt.Controls.Add(confirmation);
                    prompt.Controls.Add(cancel);
                    prompt.Controls.Add(textLabel);
                    prompt.AcceptButton = confirmation;
                    prompt.CancelButton = cancel;

                    var result = prompt.ShowDialog();
                    if (result == DialogResult.OK)
                        return textBox.Text;
                    return null;
                }
            }
        }
    }

    // Расширение для безопасного копирования выборки в DataTable, если фильтр пустой — возвращает пустую таблицу с той же схемой
    internal static class DataTableExtensions
    {
        public static DataTable CopyToDataTableOrEmpty(this IEnumerable<DataRow> rows)
        {
            var enumerable = rows as DataRow[] ?? rows.ToArray();
            if (!enumerable.Any())
            {
                // возвращаем пустую таблицу с той же структурой (если возможно)
                var first = enumerable.FirstOrDefault();
                if (first != null)
                {
                    return first.Table.Clone();
                }
                // если нет строк и не знаем схему, вернём пустую DataTable
                return new DataTable();
            }
            return enumerable.CopyToDataTable();
        }
    }
}
