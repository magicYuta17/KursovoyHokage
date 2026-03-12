using Kursivoy_Konkin;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Kursivoy_Konkin
{
    // Форма для просмотра и управления сотрудниками (доступна администратору)
    public partial class FormAdminWorker : Form
    {
        // Конструктор формы
        public FormAdminWorker()
        {
            InitializeComponent(); // Инициализация компонентов дизайнера
            this.MinimizeBox = false; // Запрет на сворачивание
            this.MaximizeBox = false; // Запрет на разворачивание

            // Подписка на событие клика мышью по DataGridView
            dgvWorkers.MouseClick += dgvWorkers_MouseClick;
        }

        // Метод для загрузки списка сотрудников из БД
        private void LoadWorkers()
        {
            try
            {
                dgvWorkers.Columns.Clear(); // Очищаем колонки
                dgvWorkers.AutoGenerateColumns = true; // Автогенерация колонок

                // SQL-запрос с JOIN для получения данных о сотрудниках, их клиентах и ролях
                string query = @"
            SELECT 
                w.ID_worker,
                w.FIO,
                c.FullName_client AS Клиент,
                w.phone          AS Телефон,
                w.Age            AS Возраст,
                r.Role           AS Роль,
                w.photo
            FROM worker w
            LEFT JOIN clients c ON w.ID_Clientsl = c.ID_Client
            LEFT JOIN role_worker r ON w.Role_worker_ID_Role = r.ID_Role
            WHERE w.IsDeleted = 0"; // Только не удаленные сотрудники

                using (MySqlConnection conn = new MySqlConnection(connect.con))
                {
                    conn.Open(); // Открываем соединение
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt); // Заполняем DataTable

                    dgvWorkers.DataSource = dt; // Устанавливаем источник данных

                    // Скрываем служебные столбцы (ID и photo - они нужны только для работы)
                    dgvWorkers.Columns["ID_worker"].Visible = false;
                    dgvWorkers.Columns["photo"].Visible = false;

                    // Переименовываем заголовки для лучшего отображения
                    dgvWorkers.Columns["FIO"].HeaderText = "ФИО";
                    dgvWorkers.Columns["Клиент"].HeaderText = "Клиент";
                    dgvWorkers.Columns["Телефон"].HeaderText = "Телефон";
                    dgvWorkers.Columns["Возраст"].HeaderText = "Возраст";
                    dgvWorkers.Columns["Роль"].HeaderText = "Роль";

                    // Добавляем колонку для отображения фото
                    var imageColumn = new DataGridViewImageColumn
                    {
                        Name = "Фото",
                        HeaderText = "Фото",
                        ImageLayout = DataGridViewImageCellLayout.Zoom, // Масштабирование изображения
                        Width = 80,
                        SortMode = DataGridViewColumnSortMode.NotSortable // Отключаем сортировку
                    };
                    dgvWorkers.Columns.Add(imageColumn);

                    // Путь к папке с фотографиями сотрудников
                    string photoDirectory = Path.Combine(Application.StartupPath, "photo_worker");
                    Image placeholder = Properties.Resources.picture; // Изображение-заглушка

                    // Заполняем фото по каждой строке
                    foreach (DataGridViewRow row in dgvWorkers.Rows)
                    {
                        if (row.IsNewRow) continue; // Пропускаем пустую строку для добавления

                        // Получаем имя файла фото из DataTable
                        object photoFileName = dt.Rows[row.Index]["photo"];

                        // Если фото нет - ставим заглушку
                        if (photoFileName == null || photoFileName == DBNull.Value
                            || string.IsNullOrWhiteSpace(photoFileName.ToString()))
                        {
                            row.Cells["Фото"].Value = placeholder;
                            continue;
                        }

                        try
                        {
                            // Формируем полный путь к файлу фото
                            string photoPath = Path.Combine(photoDirectory, photoFileName.ToString());
                            if (File.Exists(photoPath)) // Если файл существует
                            {
                                // Загружаем изображение (создаем копию, чтобы не блокировать файл)
                                using (var img = Image.FromFile(photoPath))
                                    row.Cells["Фото"].Value = new Bitmap(img);
                            }
                            else
                            {
                                row.Cells["Фото"].Value = placeholder; // Файл не найден - заглушка
                            }
                        }
                        catch
                        {
                            row.Cells["Фото"].Value = placeholder; // Ошибка загрузки - заглушка
                        }
                    }

                    // Настройки таблицы
                    dgvWorkers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Автоширина
                    dgvWorkers.Columns["Фото"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None; // Фиксированная ширина
                    dgvWorkers.Columns["Фото"].Width = 80; // Ширина колонки с фото
                    dgvWorkers.RowTemplate.Height = 80; // Высота строки под фото
                    dgvWorkers.ReadOnly = true; // Только для чтения
                    dgvWorkers.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // Выделение всей строки
                    dgvWorkers.AllowUserToAddRows = false; // Запрет добавления строк

                    // Отключаем сортировку для всех колонок
                    foreach (DataGridViewColumn col in dgvWorkers.Columns)
                        col.SortMode = DataGridViewColumnSortMode.NotSortable;

                    dgvWorkers.ClearSelection(); // Снимаем выделение
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка загрузки", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обработчик клика правой кнопкой мыши по DataGridView
        private void dgvWorkers_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) // Если нажата правая кнопка
            {
                var hit = dgvWorkers.HitTest(e.X, e.Y); // Определяем, куда кликнули
                if (hit.RowIndex >= 0) // Если кликнули на строку
                {
                    dgvWorkers.ClearSelection(); // Снимаем выделение
                    dgvWorkers.Rows[hit.RowIndex].Selected = true; // Выделяем строку
                    cmsWorker.Show(dgvWorkers, e.Location); // Показываем контекстное меню
                }
            }
        }

        // Метод для получения ID выбранного сотрудника
        private int GetSelectedWorkerId()
        {
            if (dgvWorkers.SelectedRows.Count == 0) return -1; // Если ничего не выбрано
            return Convert.ToInt32(dgvWorkers.SelectedRows[0].Cells["ID_worker"].Value); // Возвращаем ID
        }

        // Обработчик кнопки "Назад"
        private void btnBack_Click(object sender, EventArgs e)
        {
            FormAdminNavigation f = new FormAdminNavigation(); // Создаем форму навигации администратора
            this.Visible = false; // Скрываем текущую форму
            f.ShowDialog(); // Показываем форму навигации
            this.Close(); // Закрываем текущую форму
        }

        // Обработчик пункта меню "Добавить"
        private void tsmiAdd_Click_1(object sender, EventArgs e)
        {
            FormAdminWorkerAdd form = new FormAdminWorkerAdd(); // Создаем форму добавления сотрудника
            this.Visible = false; // Скрываем текущую форму
            form.ShowDialog(); // Показываем форму добавления
            this.Visible = true; // Возвращаем видимость текущей формы
            LoadWorkers(); // обновляем таблицу после возврата
        }

        // Обработчик пункта меню "Редактировать"
        private void tsmiEdit_Click_1(object sender, EventArgs e)
        {
            int id = GetSelectedWorkerId(); // Получаем ID выбранного сотрудника
            if (id == -1) // Если ничего не выбрано
            {
                MessageBox.Show("Выберите сотрудника для редактирования.",
                    "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            FormAdminWorkerEdit form = new FormAdminWorkerEdit(id); // Создаем форму редактирования с ID
            this.Visible = false; // Скрываем текущую форму
            form.ShowDialog(); // Показываем форму редактирования
            this.Visible = true; // Возвращаем видимость
            LoadWorkers(); // Обновляем таблицу
        }

        // Обработчик пункта меню "Удалить"
        private void tsmiDelete_Click(object sender, EventArgs e)
        {
            int id = GetSelectedWorkerId(); // Получаем ID выбранного сотрудника
            if (id == -1) // Если ничего не выбрано
            {
                MessageBox.Show("Выберите сотрудника для удаления.",
                    "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Запрашиваем подтверждение удаления
            var confirm = MessageBox.Show(
                "Вы уверены, что хотите удалить этого сотрудника?",
                "Подтверждение удаления",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return; // Если пользователь отказался

            try
            {
                // Мягкое удаление (установка флага IsDeleted = 1)
                string query = "UPDATE worker SET IsDeleted = 1 WHERE ID_worker = @id";
                using (MySqlConnection conn = new MySqlConnection(connect.con))
                {
                    conn.Open(); // Открываем соединение
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = id; // Передаем ID
                        cmd.ExecuteNonQuery(); // Выполняем запрос
                    }
                }

                MessageBox.Show("Сотрудник удалён.", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadWorkers(); // Обновляем таблицу
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка удаления",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обработчик загрузки формы
        private void FormAdminWorker_Load(object sender, EventArgs e)
        {
            LoadWorkers(); // Загружаем данные при загрузке формы
        }

        private void FormAdminWorker_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true; //отменяем закрытие формы
            }
        }
    }
}