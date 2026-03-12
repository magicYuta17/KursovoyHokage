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
using MySql.Data.MySqlClient;

namespace Kursivoy_Konkin.Admin
{
    public partial class FormAdminLocal : Form
    {
        public FormAdminLocal()
        {
            InitializeComponent();
        }
        private void DbExists()
        {
            MySqlConnection connection = new MySqlConnection(connect.con);
            connection.Open();
            MySqlCommand command = new MySqlCommand("CREATE DATABASE  IF NOT EXISTS mydb", connection);
            command.ExecuteNonQuery();
            connection.Close();
        }
        private void FillTables()
        {
            MySqlConnection connection = new MySqlConnection(connect.con);
            connection.Open();
            MySqlCommand command = new MySqlCommand("SHOW TABLES", connection);
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                comboBox1.Items.Add(reader.GetValue(0).ToString());
            }
            connection.Close();
        }

        

       

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button1.Enabled = true;
            button4.Enabled = true;
        }

       

        private int ImportCSV(string csvFilePath, string tableName, MySqlConnection connection)
        {
            int res = 0;
            string[] readText = File.ReadAllLines(csvFilePath);
            string[] values;
            string query = "";

            foreach (string str in readText.Skip(1).ToArray())
            {
                values = str.Split(';');

                switch (tableName)
                {
                    case "role":
                        query = $@"INSERT INTO role (RoleName) 
                                    VALUES ('{values[0]}')";
                        break;

                    case "user":
                        query = $@"INSERT INTO user (UserSurname, UserName, UserPatronymic, UserLogin, UserPassword, UserRole) 
                                    VALUES ('{values[0]}', '{values[1]}', '{values[2]}', '{values[3]}', '{values[4]}', '{values[5]}')";
                        break;

                    case "client":
                        query = $@"INSERT INTO client (ClientSurname, ClientName, ClientPatronymic, ClientPhone, ClientBirthday, ClientRightEye, ClientLeftEye) 
                                    VALUES ('{values[0]}', '{values[1]}', '{values[2]}', '{values[3]}', '{values[4]}', '{values[5]}', '{values[6]}')";
                        break;

                    case "product":
                        query = $@"INSERT INTO product (ProductArticleNumber, ProductName, ProductUnit, ProductCost, ProductManufacturer, ProductSupplier, ProductQuantityInStock, ProductDescription, ProductCategory, ProductPhoto) 
                                    VALUES ('{values[0]}', '{values[1]}', '{values[2]}', '{values[3]}', '{values[4]}', '{values[5]}', '{values[6]}', '{values[7]}', '{values[8]}', '{values[9]}')";
                        break;

                    case "productcategory":
                        query = $@"INSERT INTO productcategory (ProductCategoryName) 
                                    VALUES ('{values[0]}')";
                        break;

                    case "supplier":
                        query = $@"INSERT INTO supplier (SupplierName, SupplierContactPerson, SupplierPhone, SupplierAddress, SupplierINN, SupplierKPP, SupplierPaymentAccount, SupplierBank, SupplierBIK) 
                                    VALUES ('{values[0]}','{values[1]}', '{values[2]}', '{values[3]}', '{values[4]}', '{values[5]}', '{values[6]}', '{values[7]}', '{values[8]}')";
                        break;

                    case "order":
                        query = $@"INSERT INTO supplier (OrderDate, ProductUser, OrderClient, OrderAmount) 
                                    VALUES ('{values[0]}', '{values[1]}', '{values[2]}', '{values[3]}')";
                        break;

                    case "orderproduct":
                        query = $@"INSERT INTO supplier (ProductArticleNumber, OrderCount) 
                                    VALUES ('{values[0]}', '{values[1]}')";
                        break;
                }
                MySqlCommand command = new MySqlCommand(query, connection);

                res += command.ExecuteNonQuery();
            }
            return res;
        }

      


       

        private void FormAdminLocal_Load(object sender, EventArgs e)
        {
            DbExists();
            FillTables();
            button1.Enabled = false;
            button4.Enabled = false;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Вы действительно хотите создать резервную копию?", "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
            {
                string backup = "backup optics " + DateTime.Now + ".sql";
                backup = backup.Replace(":", "-");
                string file = Directory.GetCurrentDirectory() + "\\backup\\" + backup;
                try
                {
                    Data.GetBackup(file);
                    MessageBox.Show($"Резервная копия успешно создана по пути: {file}", "Сообщение пользователю", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.SelectedIndex == -1)
                {
                    MessageBox.Show("Пожалуйста, заполните все поля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "CSV files (*.csv)|*.csv";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = saveFileDialog.FileName;
                        string tableName = comboBox1.SelectedItem.ToString();

                        MySqlConnection connection = new MySqlConnection(connect.con);
                        connection.Open();
                        MySqlCommand command = new MySqlCommand($"SELECT * FROM {tableName}", connection);
                        MySqlDataReader reader = command.ExecuteReader();
                        DataTable table = new DataTable();
                        table.Load(reader);

                        StreamWriter streamWriter = new StreamWriter(filePath, false);

                        for (int i = 0, len = table.Columns.Count - 1; i <= len; ++i)
                        {
                            if (i != len)
                                streamWriter.Write(table.Columns[i].ColumnName + ";");
                            else
                                streamWriter.Write(table.Columns[i].ColumnName);
                        }
                        streamWriter.Write("\n");

                        foreach (DataRow dataRow in table.Rows)
                        {
                            string r = String.Join(";", dataRow.ItemArray);
                            streamWriter.WriteLine(r);
                        }

                        streamWriter.Close();

                        connection.Close();

                        MessageBox.Show($"Данные успешно выгружены!", "Сообщение пользователю", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        connection.Close();

                        button1.Enabled = false;
                        button4.Enabled = false;
                        comboBox1.SelectedIndex = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Вы уверены, что хотите восстановить базу данных?", "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                MySqlConnection mySqlConnection = new MySqlConnection(connect.con);
                mySqlConnection.Open();

                string pathFile = Directory.GetCurrentDirectory() + @"\dumb and dll\last.sql";
                string textFile = File.ReadAllText(pathFile);
                MySqlCommand mySqlCommand = new MySqlCommand(textFile, mySqlConnection);
                mySqlCommand.ExecuteNonQuery();

                mySqlConnection.Close();

                MessageBox.Show("База данных успешно восстановлена!", "Сообщение пользователю", MessageBoxButtons.OK, MessageBoxIcon.Information);
                comboBox1.Items.Clear();
                FillTables();
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.SelectedIndex == -1)
                {
                    MessageBox.Show("Пожалуйста, заполните все поля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    OpenFileDialog openFileDialog1 = new OpenFileDialog();
                    openFileDialog1.Filter = "CSV files (*.csv)|*.csv";
                    if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        MySqlConnection mySqlConnection = new MySqlConnection(connect.con);
                        mySqlConnection.Open();

                        string filePath = openFileDialog1.FileName;
                        string tableName = comboBox1.SelectedItem.ToString();

                        int importRows = ImportCSV(filePath, tableName, mySqlConnection);
                        if (importRows != 0)
                        {
                            MessageBox.Show($"Успешно импортировано {importRows} записей в таблицу {tableName}!", "Сообщение пользователю", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        mySqlConnection.Close();

                        button1.Enabled = false;
                        button4.Enabled = false;
                        comboBox1.SelectedIndex = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Вы действительно хотите выйти?", "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
            {
                FormAutorization authorizationForm = new FormAutorization();
                this.Visible = false;
                authorizationForm.ShowDialog();
                this.Close();
            }
        }
    }

}
