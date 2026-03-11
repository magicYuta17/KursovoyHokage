// Импорт необходимых библиотек для работы с базой данных, интерфейсом и файлами
using MySql.Data.MySqlClient; // Библиотека для работы с MySQL
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
using YourNamespace; // Убедитесь, что это правильное пространство имен

namespace Kursivoy_Konkin.Manager
{
    // Класс форма просмотра контрактов менеджером
    public partial class FormManagerViewContract : Form
    {
        // Конструктор формы
        public FormManagerViewContract()
        {
            InitializeComponent(); // Инициализация компонентов интерфейса
            this.Load += FormManagerViewContract_Load; // Обработка события загрузки формы
            this.MinimizeBox = false; // Запрет свертывания окна
            this.MaximizeBox = false; // Запрет развертывания окна
            this.ControlBox = false; // Отключение стандартных кнопок управления
        }

        // Обработка кнопки возврата к меню навигации
        private void button5_Click(object sender, EventArgs e)
        {
            FormManagerNavigation f = new FormManagerNavigation();
            this.Visible = false; // Скрытие текущей формы
            f.ShowDialog(); // Открытие формы навигации модально
            this.Close(); // Закрытие текущей формы после закрытия навигационной
        }

        // Инициализация контекстного меню (например, правый клик)
        private void InitializeContextMenu()
        {
            dataGridView1.ContextMenuStrip = contextMenuStrip1; // Привязка контекстного меню к DataGridView
        }

        // Загрузка данных из базы данных
        private void LoadData()
        {
            try
            {
                dataGridView1.Columns.Clear(); // Очистка текущих столбцов
                dataGridView1.AutoGenerateColumns = true; // Автоматическое создание колонок из DataTable

                // SQL-запрос для получения данных контрактов и связанных таблиц
                string query = @"
                   SELECT 
                        c.ID_Contract,
                        c.Name_contract AS 'Наименование контракта',
                        o.cost AS 'Стоимость',
                        c.date_signing AS 'Дата подписи',
                        o.building_dates AS 'Сроки строительства',
                        -- Данные клиента
                        cl.ID_Client AS 'ID Клиента',
                        cl.FullName_client AS 'ФИО Клиента',
                        cl.phone AS 'Телефон клиента',
                        -- Данные работника
                        w.ID_worker AS 'ID Работника',
                        w.FIO AS 'ФИО Работника',
                        w.phone AS 'Телефон работника',
                        c.connection_contract_object_idconnection_contract_object,
                        c.END_DATE AS 'Дата окончание договора о строительстве'         
                    FROM contract c
                    LEFT JOIN object o ON o.connection_contract_object_idconnection_contract_object = c.connection_contract_object_idconnection_contract_object
                    LEFT JOIN clients cl ON cl.ID_Client = c.Clients_ID_Client
                    LEFT JOIN worker w ON w.ID_worker = c.worker_ID_worker;";

                // Создаем соединение, команду и адаптер для выполнения запроса
                using (var connection = new MySqlConnection(connect.con))
                using (var command = new MySqlCommand(query, connection))
                using (var adapter = new MySqlDataAdapter(command))
                {
                    var table = new DataTable();
                    connection.Open(); // Открываем соединение
                    adapter.Fill(table); // Заполняем таблицу данными из базы

                    // Если данных нет, показываем сообщение
                    if (table.Rows.Count == 0)
                    {
                        MessageBox.Show("Данные не найдены.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    // Устанавливаем DataTable как источник данных для DataGridView
                    dataGridView1.DataSource = table;

                    // Скрываем вспомогательные столбцы (ID)
                    if (dataGridView1.Columns["ID_Contract"] != null)
                        dataGridView1.Columns["ID_Contract"].Visible = false;
                    if (dataGridView1.Columns["ID Клиента"] != null)
                        dataGridView1.Columns["ID Клиента"].Visible = false;
                    if (dataGridView1.Columns["ID Работника"] != null)
                        dataGridView1.Columns["ID Работника"].Visible = false;
                    if (dataGridView1.Columns["connection_contract_object_idconnection_contract_object"] != null)
                        dataGridView1.Columns["connection_contract_object_idconnection_contract_object"].Visible = false;

                    // Отключение сортировки для всех колонок
                    foreach (DataGridViewColumn col in dataGridView1.Columns)
                        col.SortMode = DataGridViewColumnSortMode.NotSortable;

                    // Установка высоты строк
                    dataGridView1.RowTemplate.Height = 80;
                    dataGridView1.ClearSelection(); // Снимаем выделение
                }
            }
            catch (Exception ex)
            {
                // Обработка ошибок загрузки
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Метод, вызываемый при загрузке формы
        private void FormManagerViewContract_Load(object sender, EventArgs e)
        {
            // Окно не может сворачиваться или разворачиваться
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.ControlBox = false;

            LoadData(); // Загружаем данные
            InitializeContextMenu(); // Инициализируем меню
        }

        // Обработка клика по пункту печати контракта
        private void печатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Проверка, выбран ли контракт
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите контракт для печати.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                var selectedRow = dataGridView1.SelectedRows[0]; // Выбираем строку
                var table = (DataTable)dataGridView1.DataSource;
                DataRow dataRow = table.Rows[selectedRow.Index]; // Получаем данные строки

                // Извлечение данных из строки
                int contractId = Convert.ToInt32(dataRow["ID_Contract"]);
                string namContract = dataRow["Наименование контракта"].ToString();
                string cost = dataRow["Стоимость"].ToString();
                string dateSigning = Convert.ToDateTime(dataRow["Дата подписи"]).ToString("dd.MM.yyyy");
                string endDate = Convert.ToDateTime(dataRow["Дата окончание договора о строительстве"]).ToString("dd.MM.yyyy");
                string constrDates = dataRow["Сроки строительства"].ToString();
                int clientId = Convert.ToInt32(dataRow["ID Клиента"]);
                int workerId = Convert.ToInt32(dataRow["ID Работника"]);

                // Инициализация переменных для ФИО клиента и работника
                string clientFio = "";
                string workerFio = "";

                // Создание соединения для получения данных о клиентах и работниках
                using (var connection = new MySqlConnection(connect.con))
                {
                    connection.Open();

                    // Запрос данных клиента по ID
                    string cmdClient = "SELECT * FROM clients WHERE ID_Client = @id";
                    using (var cmd = new MySqlCommand(cmdClient, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", clientId); // Передача параметра
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Получение ФИО клиента
                                clientFio = $"{reader["FullName_client"]}";
                            }
                        }
                    }

                    // Запрос данных работника по ID
                    string cmdWorker = "SELECT * FROM worker WHERE ID_worker = @id";
                    using (var cmd = new MySqlCommand(cmdWorker, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", workerId); // Передача параметра
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Получение ФИО работника
                                workerFio = $"{reader["FIO"]}";
                            }
                        }
                    }
                }

                // Путь к шаблону Word (размести шаблон в папке bin\Debug\doc\contract.docx)
                string projectDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\")); // Получение базового каталога проекта
                string fileName = Path.Combine(projectDir, "docPrint", "contract.docx");
                if (!File.Exists(fileName))
                {
                    MessageBox.Show($"Шаблон не найден:\n{fileName}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Создание объекта Word для автоматизации
                var word = new Microsoft.Office.Interop.Word.Application();
                word.Visible = false; // Не показывать Word сразу

                try
                {
                    // Открываем шаблон документа
                    var wordDocument = word.Documents.Add(fileName);

                    // Замена заглушек в шаблоне документе
                    ReplaceWordStub("{date_signing}", dateSigning, wordDocument);
                    ReplaceWordStub("{END_DATE}", endDate, wordDocument);
                    ReplaceWordStub("{Clients_ID_Client}", clientFio, wordDocument);
                    ReplaceWordStub("{worker_ID_worker}", workerFio, wordDocument);
                    ReplaceWordStub("{Name_contract}", namContract, wordDocument);
                    ReplaceWordStub("{Cost}", cost, wordDocument);
                    ReplaceWordStub("{Construction_Dates}", constrDates, wordDocument);

                    word.Visible = true; // Сделать видимым, чтобы пользователь мог сохранить или распечатать
                }
                catch (Exception ex)
                {
                    word.Quit(); // Закрываем Word при ошибке
                    MessageBox.Show($"Ошибка при заполнении документа: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Метод для поиска и замены текста в документе Word
        private void ReplaceWordStub(string stubToReplace, string text, Microsoft.Office.Interop.Word.Document wordDocument)
        {
            var range = wordDocument.Content; // Область документа
            range.Find.ClearFormatting(); // Очистка форматирования поиска
            range.Find.Execute(
                FindText: stubToReplace, // Строка для поиска
                ReplaceWith: text, // Строка замены
                Replace: Microsoft.Office.Interop.Word.WdReplace.wdReplaceAll // Замена все вхождения
            );
        }

        // Обработка клика правой кнопкой мыши по DataGridView
        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    var hitTest = dataGridView1.HitTest(e.X, e.Y); // Тест клик на ячейке

                    // Если клик не на заголовке
                    if (hitTest.RowIndex >= 0)
                    {
                        // Выбираем строку по позиции клика
                        dataGridView1.ClearSelection();
                        dataGridView1.Rows[hitTest.RowIndex].Selected = true;
                        dataGridView1.CurrentCell = dataGridView1.Rows[hitTest.RowIndex].Cells[0];

                        // Показываем контекстное меню
                        contextMenuStrip1.Show(dataGridView1, e.Location);
                    }
                }
            }
            catch (Exception ex) { /* Игнорируем исключения в этом обработчике */ }
        }

        // Обработка пункта меню для добавления контракта
        private void добавитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormManagerAddContract f = new FormManagerAddContract();
            this.Visible = false; // Скрываем текущую форму
            f.ShowDialog(); // Открываем форму добавления контракта модально
            this.Close(); // Закрываем текущую после
        }
    }
}