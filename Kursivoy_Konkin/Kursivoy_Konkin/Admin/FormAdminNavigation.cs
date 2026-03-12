using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kursivoy_Konkin
{
    // Форма навигации для администратора (главное меню администратора)
    public partial class FormAdminNavigation : Form
    {
        // Конструктор формы
        public FormAdminNavigation()
        {
            InitializeComponent(); // Инициализация компонентов дизайнера
            this.MinimizeBox = false; // Запрет на сворачивание окна
            this.MaximizeBox = false; // Запрет на разворачивание окна
        }

        // Обработчик события закрытия формы
        private void FormAdmin_FormClosing(object sender, FormClosingEventArgs e)
        {


            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true; //отменяем закрытие формы
            }
        }

        // Обработчик кнопки "Выход" (button5)
        private void button5_Click(object sender, EventArgs e)
        {
            FormAutorization formauto = new FormAutorization(); // Создаем форму авторизации
            this.Visible = false; // Скрываем текущую форму
            formauto.ShowDialog(); // Показываем форму авторизации (модально)
            this.Close(); // Закрываем текущую форму
        }

        // Обработчик кнопки "Сотрудники" (button1)
        private void button1_Click(object sender, EventArgs e)
        {
            FormAdminWorker formAdminWorkers = new FormAdminWorker(); // Создаем форму управления сотрудниками
            this.Visible = false; // Скрываем текущую форму
            formAdminWorkers.ShowDialog(); // Показываем форму сотрудников
            this.Close(); // Закрываем текущую форму
        }

        // Обработчик кнопки "Объекты" (buttonViewObject)
        private void buttonViewObject_Click(object sender, EventArgs e)
        {
            FormAdminObject formAdminObject = new FormAdminObject(); // Создаем форму управления объектами
            this.Visible = false; // Скрываем текущую форму
            formAdminObject.ShowDialog(); // Показываем форму объектов
            this.Close(); // Закрываем текущую форму
        }

        // Обработчик загрузки формы
        private void FormAdminNavigation_Load(object sender, EventArgs e)
        {
            // Дублирование настроек окна (на всякий случай, если они не применились в конструкторе)
            this.MinimizeBox = false; // Запрет на сворачивание
            this.MaximizeBox = false; // Запрет на разворачивание

        }
    }
}