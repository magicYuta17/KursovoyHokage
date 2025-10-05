using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kursivoy_Konkin
{
    public partial class FormViewClients : Form
    {
        public FormViewClients()
        {
            InitializeComponent();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void FillTableData(string cmd = "")
        {
            dataGridView1.Columns.Clear();
            string com = @"SELECT * FROM mydb.clients;";



            MySqlConnection connection = new MySqlConnection(connect.con);
            connection.Open();
            MySqlCommand command = new MySqlCommand(com, connection);
            MySqlDataAdapter adapter = new MySqlDataAdapter(command);
            DataTable table = new DataTable();

            adapter.Fill(table);
            dataGridView1.DataSource = table;

            dataGridView1.Columns["ID_contract"].HeaderText = "Контракт";
            dataGridView1.Columns["FullName_client"].HeaderText = "ФИО";
            dataGridView1.Columns["phone"].HeaderText = "Телефон";
            dataGridView1.Columns["Age"].HeaderText = "Возраст";
            dataGridView1.Columns["Status_client_ID_Status_client"].HeaderText = "Статус";
            dataGridView1.Columns["Qualified_lead"].HeaderText = "Квал лид";
            dataGridView1.Columns["LTV"].HeaderText = "LTV";

            dataGridView1.Columns["ID_Client"].Visible = false;
            dataGridView1.Columns["photo_clients"].Visible = false;
           


            DataGridViewImageColumn imageColumn = new DataGridViewImageColumn();
            imageColumn.Name = "Фото";
            imageColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;

            dataGridView1.Columns.Add(imageColumn);
            dataGridView1.AllowUserToAddRows = false;


            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                string namee = row.Cells["photo_clients"].Value == null ? "picture.png" : row.Cells["photo_clients"].Value.ToString();

                if (namee == "")
                {
                    namee = "picture.png";
                }
                row.Cells["Фото"].Value = Image.FromFile(@"./img/picture.png");
            }



            connection.Close();
        }

        private void FormManagerNavigation_Load(object sender, EventArgs e)
        {
            FillTableData(@"SELECT * FROM product");
            dataGridView1.ContextMenuStrip = contextMenuStrip1;
            dataGridView1.MouseDown += dataGridView1_MouseDown;
        }

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
                    // Можно не показывать контекстное меню, если не на строке
                    contextMenuStrip1.Close();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
