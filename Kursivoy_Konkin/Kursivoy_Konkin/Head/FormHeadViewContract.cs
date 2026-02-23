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
    public partial class FormHeadViewContract : Form
    {
        public FormHeadViewContract()
        {
            InitializeComponent();
            this.Load += FormHeadViewContract_Load;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            FormManagerNavigation f = new FormManagerNavigation();
            this.Visible = false;
            f.ShowDialog();
            this.Close();
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

        private void FormHeadViewContract_Load(object sender, EventArgs e)
        {
            LoadData();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
