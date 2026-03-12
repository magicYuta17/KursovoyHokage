
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
using YourNamespace;

namespace Kursivoy_Konkin
{
    // Форма для просмотра контрактов (доступна руководителю)
    public partial class FormHeadViewContract : Form
    {
        // Конструктор формы
        public FormHeadViewContract()
        {
            InitializeComponent(); // Инициализация компонентов дизайнера
            this.Load += FormHeadViewContract_Load; // Подписка на событие загрузки формы
            this.MinimizeBox = false; // Запрет на сворачивание
            this.MaximizeBox = false; // Запрет на разворачивание
           
        }

        // Обработчик кнопки "Назад" (для навигации)
        private void button5_Click(object sender, EventArgs e)
        {
            FormManagerNavigation f = new FormManagerNavigation(); // Создаем форму навигации менеджера
            this.Visible = false; // Скрываем текущую форму
            f.ShowDialog(); // Показываем форму навигации
            this.Close(); // Закрываем текущую форму
        }

        // Метод для загрузки данных контрактов из БД
        private void LoadData()
        {
            try
            {
                dataGridView1.Columns.Clear(); // Очищаем колонки таблицы
                dataGridView1.AutoGenerateColumns = true; // Автоматическая генерация колонок

                // Сложный SQL-запрос с объединением нескольких таблиц (contract, object, clients, worker)
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

                LEFT JOIN object o 
                    ON o.connection_contract_object_idconnection_contract_object 
                     = c.connection_contract_object_idconnection_contract_object

                LEFT JOIN clients cl 
                    ON cl.ID_Client = c.Clients_ID_Client

                LEFT JOIN worker w 
                    ON w.ID_worker = c.worker_ID_worker;";

                using (var connection = new MySqlConnection(connect.con)) // Создаем подключение к БД
                using (var command = new MySqlCommand(query, connection)) // Создаем команду с запросом
                using (var adapter = new MySqlDataAdapter(command)) // Создаем адаптер для заполнения DataTable
                {
                    var table = new DataTable(); // Создаем таблицу данных
                    connection.Open(); // Открываем соединение
                    adapter.Fill(table); // Заполняем таблицу данными из БД

                    if (table.Rows.Count == 0) // Если данные не найдены
                    {
                        MessageBox.Show("Данные не найдены.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    // Привязываем данные к DataGridView
                    dataGridView1.DataSource = table;

                    // Скрываем служебные столбцы (ID и connection-поля), которые не должны отображаться пользователю
                    if (dataGridView1.Columns["ID_Contract"] != null)
                        dataGridView1.Columns["ID_Contract"].Visible = false;
                    if (dataGridView1.Columns["ID Клиента"] != null)
                        dataGridView1.Columns["ID Клиента"].Visible = false;
                    if (dataGridView1.Columns["ID Работника"] != null)
                        dataGridView1.Columns["ID Работника"].Visible = false;
                    if (dataGridView1.Columns["connection_contract_object_idconnection_contract_object"] != null)
                        dataGridView1.Columns["connection_contract_object_idconnection_contract_object"].Visible = false;

                    // Отключаем сортировку у всех колонок
                    foreach (DataGridViewColumn col in dataGridView1.Columns)
                        col.SortMode = DataGridViewColumnSortMode.NotSortable;

                    dataGridView1.RowTemplate.Height = 80; // Устанавливаем высоту строк
                    dataGridView1.ClearSelection(); // Снимаем выделение
                }
            }
            catch (Exception ex)
            {
                // Показываем сообщение об ошибке при загрузке
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обработчик загрузки формы (версия для FormManagerViewContract - возможно, устаревшая)
        private void FormManagerViewContract_Load(object sender, EventArgs e)
        {
            this.MinimizeBox = false;
            this.MaximizeBox = false;
       

            LoadData(); // Загружаем данные
        }

        // Обработчик пункта меню "Печать"
        private void печатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Проверяем, выбрана ли строка
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите контракт для печати.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                var selectedRow = dataGridView1.SelectedRows[0]; // Получаем выбранную строку
                var table = (DataTable)dataGridView1.DataSource; // Получаем исходную таблицу данных
                DataRow dataRow = table.Rows[selectedRow.Index]; // Получаем строку данных из DataTable

                // Получаем данные из выбранной строки
                int contractId = Convert.ToInt32(dataRow["ID_Contract"]);
                string namContract = dataRow["Наименование контракта"].ToString();
                string cost = dataRow["Стоимость"].ToString();
                string dateSigning = Convert.ToDateTime(dataRow["Дата подписи"]).ToString("dd.MM.yyyy"); // Форматируем дату
                string endDate = Convert.ToDateTime(dataRow["Дата окончание договора о строительстве"]).ToString("dd.MM.yyyy");
                string constrDates = dataRow["Сроки строительства"].ToString();
                int clientId = Convert.ToInt32(dataRow["ID Клиента"]);
                int workerId = Convert.ToInt32(dataRow["ID Работника"]);

                // Переменные для хранения ФИО клиента и работника
                string clientFio = "";
                string workerFio = "";

                // Дополнительные запросы для получения полных данных о клиенте и работнике
                using (var connection = new MySqlConnection(connect.con))
                {
                    connection.Open();

                    // Запрос клиента по ID
                    string cmdClient = "SELECT * FROM clients WHERE ID_Client = @id";
                    using (var cmd = new MySqlCommand(cmdClient, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", clientId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read()) // Если клиент найден
                            {
                                // Получаем ФИО клиента
                                clientFio = $"{reader["FullName_client"]}";
                            }
                        }
                    }

                    // Запрос работника (менеджера) по ID
                    string cmdWorker = "SELECT * FROM worker WHERE ID_worker = @id";
                    using (var cmd = new MySqlCommand(cmdWorker, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", workerId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read()) // Если работник найден
                            {
                                // Получаем ФИО работника
                                workerFio = $"{reader["FIO"]}";
                            }
                        }
                    }
                }

                // Путь к шаблону Word (шаблон должен лежать в папке docPrint проекта)
                string projectDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\")); // Путь к корню проекта
                string fileName = Path.Combine(projectDir, "docPrint", "contract.docx"); // Полный путь к файлу шаблона
                if (!File.Exists(fileName)) // Проверяем, существует ли файл
                {
                    MessageBox.Show($"Шаблон не найден:\n{fileName}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Создаем экземпляр приложения Word
                var word = new Microsoft.Office.Interop.Word.Application();
                word.Visible = false; // Запускаем Word в фоновом режиме

                try
                {
                    var wordDocument = word.Documents.Add(fileName); // Открываем шаблон документа

                    // Заменяем заглушки в документе на реальные данные
                    ReplaceWordStub("{date_signing}", dateSigning, wordDocument);
                    ReplaceWordStub("{END_DATE}", endDate, wordDocument);
                    ReplaceWordStub("{Clients_ID_Client}", clientFio, wordDocument);
                    ReplaceWordStub("{worker_ID_worker}", workerFio, wordDocument);
                    ReplaceWordStub("{Name_contract}", namContract, wordDocument);
                    ReplaceWordStub("{Cost}", cost, wordDocument);
                    ReplaceWordStub("{Construction_Dates}", constrDates, wordDocument);

                    word.Visible = true; // Делаем Word видимым, чтобы пользователь увидел результат
                }
                catch (Exception ex)
                {
                    word.Quit(); // Закрываем Word в случае ошибки
                    MessageBox.Show($"Ошибка при заполнении документа: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Вспомогательный метод замены заглушек в документе Word
        private void ReplaceWordStub(string stubToReplace, string text, Microsoft.Office.Interop.Word.Document wordDocument)
        {
            var range = wordDocument.Content; // Получаем весь контент документа
            range.Find.ClearFormatting(); // Очищаем настройки поиска
            range.Find.Execute(
                FindText: stubToReplace, // Что ищем
                ReplaceWith: text, // На что заменяем
                Replace: Microsoft.Office.Interop.Word.WdReplace.wdReplaceAll // Заменяем все вхождения
            );
        }

        // Обработчик клика мышью по DataGridView (для контекстного меню)
        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right) // Если нажата правая кнопка мыши
                {
                    var hitTest = dataGridView1.HitTest(e.X, e.Y); // Определяем, куда кликнули

                    // Открываем меню только если кликнули на строку, не на заголовок
                    if (hitTest.RowIndex >= 0)
                    {
                        // Выделяем строку по которой кликнули
                        dataGridView1.ClearSelection();
                        dataGridView1.Rows[hitTest.RowIndex].Selected = true;
                        dataGridView1.CurrentCell = dataGridView1.Rows[hitTest.RowIndex].Cells[0];
                    }
                }
            }
            catch (Exception ex) { } // Игнорируем ошибки (пустой catch)
        }

        // Обработчик пункта меню "Добавить" - открывает форму добавления контракта
        private void добавитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormManagerAddContract f = new FormManagerAddContract(); // Создаем форму добавления контракта
            this.Visible = false; // Скрываем текущую форму
            f.ShowDialog(); // Показываем форму добавления
            this.Close(); // Закрываем текущую форму
        }

        // Обработчик загрузки формы (основной)
        private void FormHeadViewContract_Load(object sender, EventArgs e)
        {
            LoadData(); // Загружаем данные при загрузке формы
        }

        // Обработчик кнопки "Назад" (альтернативный)
        private void button1_Click(object sender, EventArgs e)
        {
            FormHeadNavigation f = new FormHeadNavigation(); // Создаем форму навигации для руководителя
            this.Visible = false; // Скрываем текущую форму
            f.ShowDialog(); // Показываем форму навигации
            this.Close(); // Закрываем текущую форму
        }

        private void FormHeadViewContract_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true; //отменяем закрытие формы
            }
        }
    }
}
