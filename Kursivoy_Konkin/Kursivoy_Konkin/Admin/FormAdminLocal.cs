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
            try
            {
                // Подключаемся БЕЗ базы данных
                MySqlConnection connection = new MySqlConnection(connect.conNoDb);
                connection.Open();
                MySqlCommand command = new MySqlCommand("CREATE DATABASE IF NOT EXISTS mydb", connection);
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        private void FillTables()
        {
            try
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
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        

       

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button1.Enabled = true;
            button4.Enabled = true;
        }

        private string[] ReadCsvWithEncodingDetection(string filePath)
        {
            // Сначала пробуем UTF-8
            var lines = File.ReadAllLines(filePath, Encoding.UTF8);

            // Проверяем, нет ли "кракозябр" в первой строке с данными
            if (lines.Length > 1 && ContainsGarbage(lines[1]))
            {
                // Если есть мусор — пробуем Windows-1251
                return File.ReadAllLines(filePath, Encoding.GetEncoding(1251));
            }

            return lines;
        }

        private bool ContainsGarbage(string text)
        {
            // Проверяем на наличие символов, которые появляются при неверной кодировке
            // "" и подобные артефакты
            return text.Contains("") ||
                   text.Contains("") ||
                   text.Count(c => c > 255 && c < 1024) > text.Length * 0.3;
        }

        private int ImportCSV(string csvFilePath, string tableName, MySqlConnection connection)
        {
            int res = 0;
            string[] readText = ReadCsvWithEncodingDetection(csvFilePath);
            string[] values;
            string query = "";

            foreach (string str in readText.Skip(1).ToArray())
            {
                values = str.Split(';');

                switch (tableName)
                {
                    case "clients":
                        query = $@"INSERT INTO clients (FullName_client, phone, Age, Status_client_ID_Status_client, LTV, Birthday, IsDeleted) 
                    VALUES ('{values[0]}', '{values[1]}', '{values[2]}', '{values[3]}', 
                            {(string.IsNullOrEmpty(values[4]) ? "NULL" : $"'{values[4]}'")} , 
                            {(string.IsNullOrEmpty(values[5]) ? "NULL" : $"'{values[5]}'")} , 
                            '{values[6]}')";
                        break;

                    case "contract":
                        query = $@"INSERT INTO contract (Name_contract, date_signing, END_DATE, Clients_ID_Client, worker_ID_worker, connection_contract_object_ID_object) 
                    VALUES ('{values[0]}', '{values[1]}', '{values[2]}', '{values[3]}', '{values[4]}', '{values[5]}')";
                        break;

                    case "object":
                        query = $@"INSERT INTO object (name_object, square, cost, building_dates, number_floors, parking_space, connection_contract_object_ID_object, photo, IsDeleted) 
                    VALUES ('{values[0]}', '{values[1]}', '{values[2]}', '{values[3]}', '{values[4]}', '{values[5]}', 
                            {(string.IsNullOrEmpty(values[6]) ? "NULL" : $"'{values[6]}'")} , 
                            {(string.IsNullOrEmpty(values[7]) ? "NULL" : $"'{values[7]}'")} , 
                            '{values[8]}')";
                        break;

                    case "role_worker":
                        query = $@"INSERT INTO role_worker (Role) 
                    VALUES ('{values[0]}')";
                        break;

                    case "status_client":
                        query = $@"INSERT INTO status_client (status, IsDeleted) 
                    VALUES ({(string.IsNullOrEmpty(values[0]) ? "NULL" : $"'{values[0]}'")} , '{values[1]}')";
                        break;

                    case "worker":
                        query = $@"INSERT INTO worker (ID_Clientsl, FIO, Age, phone, Role_worker_ID_Role, IsDeleted, photo, password) 
                    VALUES ('{values[0]}', 
                            {(string.IsNullOrEmpty(values[1]) ? "NULL" : $"'{values[1]}'")} , 
                            {(string.IsNullOrEmpty(values[2]) ? "NULL" : $"'{values[2]}'")} , 
                            '{values[3]}', '{values[4]}', '{values[5]}', 
                            {(string.IsNullOrEmpty(values[6]) ? "NULL" : $"'{values[6]}'")} , 
                            '{values[7]}')";
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

            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Вы действительно хотите создать резервную копию?", "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
            {
                string backup = "backup build " + DateTime.Now + ".sql";
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

                        StreamWriter streamWriter = new StreamWriter(filePath, false, new UTF8Encoding(true));

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
            try
            {
                DialogResult result = MessageBox.Show("Вы уверены, что хотите восстановить базу данных?", "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    // Правильное формирование пути
                    string pathFile = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\dumb and dll\db.sql"));

                    // Проверяем существование файла
                    if (!File.Exists(pathFile))
                    {
                        MessageBox.Show($"Файл не найден:\n{pathFile}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    string textFile = File.ReadAllText(pathFile);

                    // Подключаемся БЕЗ базы данных
                    
                    MySqlConnection mySqlConnection = new MySqlConnection(connect.conNoDb);
                    mySqlConnection.Open();

                    // Создаем базу
                    MySqlCommand createDbCommand = new MySqlCommand("CREATE DATABASE IF NOT EXISTS mydb", mySqlConnection);
                    createDbCommand.ExecuteNonQuery();

                    // Выбираем базу
                    MySqlCommand useDbCommand = new MySqlCommand("USE mydb", mySqlConnection);
                    useDbCommand.ExecuteNonQuery();

                    // Выполняем скрипт
                    MySqlCommand mySqlCommand = new MySqlCommand(textFile, mySqlConnection);
                    mySqlCommand.ExecuteNonQuery();

                    mySqlConnection.Close();

                    MessageBox.Show("База данных успешно восстановлена!", "Сообщение пользователю", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    comboBox1.Items.Clear();
                    FillTables();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
