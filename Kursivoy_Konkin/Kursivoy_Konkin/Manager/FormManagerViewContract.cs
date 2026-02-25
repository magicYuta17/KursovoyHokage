using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kursivoy_Konkin.Manager
{
    public partial class FormManagerViewContract : Form
    {
        public FormManagerViewContract()
        {
            InitializeComponent();
            this.Load += FormManagerViewContract_Load;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            FormManagerNavigation f = new FormManagerNavigation();
            this.Visible = false;
            f.ShowDialog();
            this.Close();
        }
        private void InitializeContextMenu()
        {

            dataGridView1.ContextMenuStrip = contextMenuStrip1;

        }
        private void LoadData()
        {
            try
            {
                dataGridView1.Columns.Clear();
                dataGridView1.AutoGenerateColumns = true;

                string query = @"
            SELECT 
                ID_Contract,
                Name_contract AS 'Наименование контракта',
                Cost AS 'Стоимость',
                date_signing AS 'Дата подписи',
                Construction_Dates AS 'Сроки строительства',
                Clients_ID_Client,
                worker_ID_worker,
                connection_contract_object_idconnection_contract_object,
                END_DATE AS 'Дата окончание договора о строительстве'         
            FROM contract;";

                using (var connection = new MySqlConnection(connect.con))
                using (var command = new MySqlCommand(query, connection))
                using (var adapter = new MySqlDataAdapter(command))
                {
                    var table = new DataTable();
                    connection.Open();
                    adapter.Fill(table);

                    if (table.Rows.Count == 0)
                    {
                        MessageBox.Show("Данные не найдены.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    // Привязываем данные к DataGridView
                    dataGridView1.DataSource = table;

                    // Скрываем столбец ID_object
                    if (dataGridView1.Columns["ID_Contract"] != null)
                        dataGridView1.Columns["ID_Contract"].Visible = false;
                    if (dataGridView1.Columns["Clients_ID_Client"] != null)
                        dataGridView1.Columns["Clients_ID_Client"].Visible = false;
                    if (dataGridView1.Columns["worker_ID_worker"] != null)
                        dataGridView1.Columns["worker_ID_worker"].Visible = false;
                    if (dataGridView1.Columns["connection_contract_object_idconnection_contract_object"] != null)
                        dataGridView1.Columns["connection_contract_object_idconnection_contract_object"].Visible = false;



                    // Отключаем сортировку у всех колонок
                    foreach (DataGridViewColumn col in dataGridView1.Columns)
                        col.SortMode = DataGridViewColumnSortMode.NotSortable;

                    dataGridView1.RowTemplate.Height = 80;
                    dataGridView1.ClearSelection();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormManagerViewContract_Load(object sender, EventArgs e)
        {
            LoadData();
            InitializeContextMenu();
        }

        private void печатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите контракт для печати.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                var selectedRow = dataGridView1.SelectedRows[0];
                var table = (DataTable)dataGridView1.DataSource;
                DataRow dataRow = table.Rows[selectedRow.Index];

                // Получаем данные из выбранной строки
                int contractId = Convert.ToInt32(dataRow["ID_Contract"]);
                string namContract = dataRow["Наименование контракта"].ToString();
                string cost = dataRow["Стоимость"].ToString();
                string dateSigning = Convert.ToDateTime(dataRow["Дата подписи"]).ToString("dd.MM.yyyy");
                string endDate = Convert.ToDateTime(dataRow["Дата окончание договора о строительстве"]).ToString("dd.MM.yyyy");
                string constrDates = dataRow["Сроки строительства"].ToString();
                int clientId = Convert.ToInt32(dataRow["Clients_ID_Client"]);
                int workerId = Convert.ToInt32(dataRow["worker_ID_worker"]);

                // Получаем ФИО клиента из таблицы clients
                string clientFio = "";
                string workerFio = "";

                using (var connection = new MySqlConnection(connect.con))
                {
                    connection.Open();

                    // Запрос клиента (замени поля на актуальные названия в твоей БД)
                    string cmdClient = "SELECT * FROM clients WHERE ID_Client = @id";
                    using (var cmd = new MySqlCommand(cmdClient, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", clientId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Замени на реальные названия колонок ФИО в таблице clients
                                clientFio = $"{reader["FullName_client"]}";
                            }
                        }
                    }

                    // Запрос менеджера (замени поля на актуальные названия в твоей БД)
                    string cmdWorker = "SELECT * FROM worker WHERE ID_worker = @id";
                    using (var cmd = new MySqlCommand(cmdWorker, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", workerId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Замени на реальные названия колонок ФИО в таблице worker
                                workerFio = $"{reader["FIO"]}";
                            }
                        }
                    }
                }

                // Путь к шаблону Word (положи шаблон в bin\Debug\doc\contract.docx)
                string projectDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\"));
                string fileName = Path.Combine(projectDir, "docPrint", "contract.docx");
                if (!File.Exists(fileName))
                {
                    MessageBox.Show($"Шаблон не найден:\n{fileName}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var word = new Microsoft.Office.Interop.Word.Application();
                word.Visible = false;

                try
                {
                    var wordDocument = word.Documents.Add(fileName);

                    // Заменяем заглушки в документе
                    ReplaceWordStub("{date_signing}", dateSigning, wordDocument);
                    ReplaceWordStub("{END_DATE}", endDate, wordDocument);
                    ReplaceWordStub("{Clients_ID_Client}", clientFio, wordDocument);
                    ReplaceWordStub("{worker_ID_worker}", workerFio, wordDocument);
                    ReplaceWordStub("{Name_contract}", namContract, wordDocument);
                    ReplaceWordStub("{Cost}", cost, wordDocument);
                    ReplaceWordStub("{Construction_Dates}", constrDates, wordDocument);

                    word.Visible = true;
                }
                catch (Exception ex)
                {
                    word.Quit();
                    MessageBox.Show($"Ошибка при заполнении документа: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Вспомогательный метод замены заглушек (как у подруги)
        private void ReplaceWordStub(string stubToReplace, string text, Microsoft.Office.Interop.Word.Document wordDocument)
        {
            var range = wordDocument.Content;
            range.Find.ClearFormatting();
            range.Find.Execute(
                FindText: stubToReplace,
                ReplaceWith: text,
                Replace: Microsoft.Office.Interop.Word.WdReplace.wdReplaceAll
            );
        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {

            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    var hitTest = dataGridView1.HitTest(e.X, e.Y);

                    // Открываем меню только если кликнули на строку, не на заголовок
                    if (hitTest.RowIndex >= 0)
                    {
                        // Выделяем строку по которой кликнули
                        dataGridView1.ClearSelection();
                        dataGridView1.Rows[hitTest.RowIndex].Selected = true;
                        dataGridView1.CurrentCell = dataGridView1.Rows[hitTest.RowIndex].Cells[0];

                        contextMenuStrip1.Show(dataGridView1, e.Location);
                    }
                }
            }
            catch (Exception ex) { }
        }
    }
}
