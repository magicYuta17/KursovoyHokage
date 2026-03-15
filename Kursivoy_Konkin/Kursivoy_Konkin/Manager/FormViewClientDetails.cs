using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Kursivoy_Konkin.Manager
{
    
    public partial class FormViewClientDetails : Form
    {
        private int _clientId;
        public FormViewClientDetails(int clientId)
        {
            _clientId = clientId;
            InitializeComponent();
            LoadClientData();
        }

        private void FormViewClientDetails_Load(object sender, EventArgs e)
        {
            this.MinimizeBox = false;
            this.MaximizeBox = false;

        }

        private void FormViewClientDetails_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
                e.Cancel = true;
        }

        private void LoadClientData()
        {
            try
            {
                string query = @"
                    SELECT 
                        c.FullName_client,
                        c.phone,
                        c.Birthday,
                        TIMESTAMPDIFF(YEAR, c.Birthday, CURDATE()) AS Age,
                        s.status,
                        c.LTV
                    FROM mydb.clients c
                    LEFT JOIN mydb.status_client s 
                        ON c.Status_client_ID_Status_client = s.ID_Status_client
                    WHERE c.ID_Client = @ClientId AND c.IsDeleted = 0;";

                using (var connection = new MySqlConnection(connect.con))
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClientId", _clientId);
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            lblFioValue.Text = reader["FullName_client"].ToString();
                            lblPhoneValue2.Text = reader["phone"].ToString();
                            lblBdayValue.Text = reader["Birthday"] == DBNull.Value
                                                       ? "—"
                                                       : Convert.ToDateTime(reader["Birthday"]).ToString("dd.MM.yyyy");
                            lblAgeValue.Text = reader["Age"].ToString();
                            lblStatusValue.Text = reader["status"].ToString();
                            lblLtvValue.Text = $"{reader["LTV"]:N0} ₽";
                        }
                        else
                        {
                            MessageBox.Show("Клиент не найден.", "Информация",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных клиента: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        

        private void btnClose_Click_1(object sender, EventArgs e)
        {
            FormViewClients f = new FormViewClients();
            this.Visible = false;
            f.ShowDialog();
            this.Close();
        }
    }
}
