using Kursivoy_Konkin.Manager;
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
    // Форма навигации для руководителя (главное меню)
    public partial class FormHeadNavigation : Form
    {
        // Конструктор формы
        public FormHeadNavigation()
        {
            InitializeComponent(); // Инициализация компонентов дизайнера
            this.MinimizeBox = false; // Запрет на сворачивание окна
            this.MaximizeBox = false; // Запрет на разворачивание окна
            this.ControlBox = false; // Скрытие системных кнопок (свернуть/развернуть/закрыть)
        }

        // Обработчик кнопки "Выход" (button5)
        private void button5_Click(object sender, EventArgs e)
        {
            FormAutorization f = new FormAutorization(); // Создаем форму авторизации
            this.Visible = false; // Скрываем текущую форму
            f.ShowDialog(); // Показываем форму авторизации (модально)
            this.Close(); // Закрываем текущую форму
        }

        // Обработчик кнопки "Статусы" (button6)
        private void button6_Click(object sender, EventArgs e)
        {
            FormHeadViewStatus f = new FormHeadViewStatus(); // Создаем форму просмотра статусов
            this.Visible = false; // Скрываем текущую форму
            f.ShowDialog(); // Показываем форму статусов
            this.Close(); // Закрываем текущую форму
        }

        // Обработчик кнопки "Клиенты" (button1)
        private void button1_Click(object sender, EventArgs e)
        {
            FormHeadViewClients f = new FormHeadViewClients(); // Создаем форму просмотра клиентов
            this.Visible = false; // Скрываем текущую форму
            f.ShowDialog(); // Показываем форму клиентов
            this.Close(); // Закрываем текущую форму
        }

        // Обработчик кнопки "Объекты" (button3)
        private void button3_Click(object sender, EventArgs e)
        {
            // Создаем форму просмотра объектов, передавая имя текущей формы для возможности возврата
            FormViewObject f = new FormViewObject("FormHeadNavigation");
            this.Visible = false; // Скрываем текущую форму
            f.ShowDialog(); // Показываем форму объектов
            this.Close(); // Закрываем текущую форму
        }

        // Обработчик кнопки "Контракты" (button2)
        private void button2_Click(object sender, EventArgs e)
        {
            FormHeadViewContract f = new FormHeadViewContract(); // Создаем форму просмотра контрактов
            this.Visible = false; // Скрываем текущую форму
            f.ShowDialog(); // Показываем форму контрактов
            this.Close(); // Закрываем текущую форму
        }
    }
}