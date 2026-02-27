using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace Kursivoy_Konkin
{
    public partial class FormAdminWorker : Form
    {
        private string ConnectionString = connect.con;

        public FormAdminWorker()
        {
            InitializeComponent();
            LoadWorkerTable();
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.ControlBox = false;
        }

        private void LoadWorkerTable()
        {
            try
            {
                string query = @"
                    SELECT 
                        w.FIO AS 'ФИО сотрудника',
                        w.Age AS 'Возраст',
                        w.phone AS 'Телефон',
                        r.Role AS 'Роль',
                        c.FullName_client AS 'Закрепленный клиент'
                    FROM mydb.worker w
                    LEFT JOIN mydb.role_worker r ON w.Role_worker_ID_Role = r.ID_Role
                    LEFT JOIN mydb.clients c ON w.ID_Clientsl = c.ID_Client
                    WHERE w.IsDeleted = 0";

                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                using (MySqlCommand command = new MySqlCommand(query, connection))
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                {
                    DataTable table = new DataTable();
                    connection.Open();
                    adapter.Fill(table);

                    // Привязываем данные к dataGridView1
                    dataGridView1.DataSource = table;

                    // Настраиваем внешний вид таблицы
                    ConfigureDataGridView();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureDataGridView()
        {
            // Скрываем ненужные столбцы
            if (dataGridView1.Columns.Contains("ID_worker"))
                dataGridView1.Columns["ID_worker"].Visible = false;

            if (dataGridView1.Columns.Contains("IsDeleted"))
                dataGridView1.Columns["IsDeleted"].Visible = false;

            // Настраиваем внешний вид таблицы
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
            FormAdminNavigation adminNavigation = new FormAdminNavigation();
            this.Visible = false;
            adminNavigation.ShowDialog();
            this.Close();
        }

       
    }
}