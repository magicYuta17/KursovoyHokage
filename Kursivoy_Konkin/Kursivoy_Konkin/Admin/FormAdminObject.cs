using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Kursivoy_Konkin
{
    public partial class FormAdminObject : Form
    {
        private string ConnectionString = connect.con;

        public FormAdminObject()
        {
            InitializeComponent();
            LoadObjectTable();
        }

        private void LoadObjectTable()
        {
            try
            {
                string query = "SELECT ID_object, square, cost, building_dates, number_floors, parking_space, connection_contract_object_idconnection_contract_object FROM object";

                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                using (MySqlCommand command = new MySqlCommand(query, connection))
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                {
                    DataTable table = new DataTable();
                    connection.Open();
                    adapter.Fill(table);

                    // Устанавливаем источник данных для dataGridView1
                    dataGridView1.DataSource = table;

                    // Устанавливаем заголовки колонок на русском языке
                    SetColumnHeaders();

                    // Скрываем ненужные колонки
                    HideColumns();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetColumnHeaders()
        {
            if (dataGridView1.Columns.Contains("ID_object"))
                dataGridView1.Columns["ID_object"].HeaderText = "ID объекта";

            if (dataGridView1.Columns.Contains("square"))
                dataGridView1.Columns["square"].HeaderText = "Площадь";

            if (dataGridView1.Columns.Contains("cost"))
                dataGridView1.Columns["cost"].HeaderText = "Стоимость";

            if (dataGridView1.Columns.Contains("building_dates"))
                dataGridView1.Columns["building_dates"].HeaderText = "Сроки строительства";

            if (dataGridView1.Columns.Contains("number_floors"))
                dataGridView1.Columns["number_floors"].HeaderText = "Количество этажей";

            if (dataGridView1.Columns.Contains("parking_space"))
                dataGridView1.Columns["parking_space"].HeaderText = "Парковочные места";

            if (dataGridView1.Columns.Contains("connection_contract_object_idconnection_contract_object"))
                dataGridView1.Columns["connection_contract_object_idconnection_contract_object"].HeaderText = "ID контракта";
        }

        private void HideColumns()
        {
            if (dataGridView1.Columns.Contains("ID_object"))
                dataGridView1.Columns["ID_object"].Visible = false;

            if (dataGridView1.Columns.Contains("connection_contract_object_idconnection_contract_object"))
                dataGridView1.Columns["connection_contract_object_idconnection_contract_object"].Visible = false;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            FormAdminNavigation f = new FormAdminNavigation();
            this.Visible = false;
            f.ShowDialog();
            this.Close();
        }
    }
}
