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
        private HashSet<int> _hiddenStatusIds = new HashSet<int>();

        public FormHeadViewStatus()
        {
            InitializeComponent();
            InitializeUi();
        }

        private void InitializeUi()
        {
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.MouseDown += DataGridView1_MouseDown;

            // Контекстное меню
            _ctx = new ContextMenuStrip();

            // 1. Добавили "Добавить статус" в контекстное меню
            var addItem = new ToolStripMenuItem("Добавить статус") { Name = "AddStatus" };
            var editItem = new ToolStripMenuItem("Редактировать") { Name = "EditStatus" };
            var deleteItem = new ToolStripMenuItem("Удалить статус") { Name = "DeleteStatus" };

            addItem.Click += ButtonAddStatus_Click;
            editItem.Click += EditItem_Click;
            deleteItem.Click += DeleteItem_Click;

            _ctx.Items.Add(addItem);
            _ctx.Items.Add(editItem);
            _ctx.Items.Add(deleteItem);

            dataGridView1.ContextMenuStrip = _ctx;

           

            Load += FormHeadViewStatus_Load;
        }

        private void FormHeadViewStatus_Load(object sender, EventArgs e)
        {
            FillStatusGrid();
        }

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

                if (_hiddenStatusIds.Any())
                {
                    var rowsToKeep = dt.AsEnumerable()
                        .Where(r => !_hiddenStatusIds.Contains(Convert.ToInt32(r["ID_Status_client"])))
                        .CopyToDataTableOrEmpty();
                    dataGridView1.DataSource = rowsToKeep;
                }
                else
                {
                    dataGridView1.DataSource = dt;
                }

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

        private void ButtonAddStatus_Click(object sender, EventArgs e)
        {
            // 2. Убрали ограничение на язык — теперь можно вводить что угодно
            string newStatus = Prompt.ShowDialog("Введите название статуса:", "Добавить статус", "", allowAnyInput: true);
            if (newStatus == null) return;
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

            // 2. allowAnyInput: true — снимаем ограничение на язык
            string edited = Prompt.ShowDialog("Отредактируйте название статуса:", "Редактирование статуса", current, allowAnyInput: true);
            if (edited == null) return;
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
                        FillStatusGrid();
                    else
                        MessageBox.Show("Статус не найден или не изменён.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении статуса: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

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

            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();

                    // 3. Проверяем сколько клиентов используют этот статус
                    long usedCount = 0;
                    using (var cmdCheck = new MySqlCommand(
                        "SELECT COUNT(*) FROM mydb.clients WHERE Status_client_ID_Status_client = @id AND IsDeleted = 0", conn))
                    {
                        cmdCheck.Parameters.AddWithValue("@id", id);
                        usedCount = Convert.ToInt64(cmdCheck.ExecuteScalar());
                    }

                    // 3. Формируем сообщение с количеством клиентов и спрашиваем подтверждение
                    string confirmMessage = usedCount > 0
                        ? $"Статус \"{name}\" используется у {usedCount} клиента(ов).\nВсё равно удалить статус?"
                        : $"Удалить статус \"{name}\"?";

                    var conf = MessageBox.Show(confirmMessage, "Подтвердите удаление",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (conf != DialogResult.Yes) return;

                    // 3. Мягкое удаление — только помечаем IsDeleted = 1, не удаляем из БД
                    using (var cmdDel = new MySqlCommand(
                        "UPDATE mydb.status_client SET IsDeleted = 1 WHERE ID_Status_client = @id", conn))
                    {
                        cmdDel.Parameters.AddWithValue("@id", id);
                        int affected = cmdDel.ExecuteNonQuery();
                        if (affected > 0)
                        {
                            MessageBox.Show("Статус успешно удалён.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show("Ошибка БД при удалении статуса: " + mex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при удалении статуса: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 2. Добавили параметр allowAnyInput — убирает ограничение на язык ввода
        private static class Prompt
        {
            public static string ShowDialog(string text, string caption, string defaultText, bool allowAnyInput = false)
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

                    // 2. Если allowAnyInput = true — не ограничиваем ввод никак
                    // Старый код ограничивал язык через ImeMode или KeyPress — убираем это полностью
                    if (!allowAnyInput)
                    {
                        textBox.KeyPress += (s, ev) =>
                        {
                            // Оставляем только если нужна старая логика ограничения
                        };
                    }

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
                    return result == DialogResult.OK ? textBox.Text : null;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FormHeadNavigation f = new FormHeadNavigation();
            this.Visible = false;
            f.ShowDialog();
            this.Close();
        }
    }

    internal static class DataTableExtensions
    {
        public static DataTable CopyToDataTableOrEmpty(this IEnumerable<DataRow> rows)
        {
            var enumerable = rows as DataRow[] ?? rows.ToArray();
            if (!enumerable.Any())
                return new DataTable();
            return enumerable.CopyToDataTable();
        }
    }
}