// Использование пространства имен для доступа к классам и формам менеджера
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

// Объявление пространства имен проекта
namespace Kursivoy_Konkin
{
    // Класс формы навигации менеджера
    public partial class FormManagerNavigation : Form
    {
        // Конструктор формы
        public FormManagerNavigation()
        {
            InitializeComponent(); // Инициализация компонентов формы
            // Запрет свертывания, развертывания и закрытия окна через стандартные кнопки
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.ControlBox = false;
        }

        // Обработчик кнопки перехода к форме просмотра клиентов
        private void button1_Click(object sender, EventArgs e)
        {
            // Создаем форму просмотра клиентов
            FormViewClients f = new FormViewClients();
            this.Visible = false; // Скрываем текущую форму
            f.ShowDialog(); // Открываем форму как модальную
            this.Close(); // После закрытия формы возвращаемся и закрываем текущую
        }

        // Обработчик кнопки перехода к форме просмотра контрактов
        private void button2_Click(object sender, EventArgs e)
        {
            // Создаем форму просмотра контрактов
            FormManagerViewContract f = new FormManagerViewContract();
            this.Visible = false; // Скрываем текущую форму
            f.ShowDialog(); // Открываем модально
            this.Close(); // Закрываем текущую форму после завершения
        }

        // Обработчик кнопки перехода к форме просмотра объектов
        private void button3_Click_1(object sender, EventArgs e)
        {
            // Создаем форму просмотра объектов, передавая название формы для возврата
            FormViewObject f = new FormViewObject("FormManagerNavigation");
            this.Visible = false; // Скрываем текущую форму
            f.ShowDialog(); // Открываем модально
            this.Close(); // Закрываем текущую после закрытия
        }

        // Обработчик кнопки выхода и возврата к окну авторизации
        private void button5_Click(object sender, EventArgs e)
        {
            // Создаем форму авторизации
            FormAutorization f = new FormAutorization();
            this.Visible = false; // Скрываем текущую
            f.ShowDialog(); // Открываем как модальную
            this.Close(); // Закрываем текущую форму после выхода
        }

        // Обработка загрузки формы
        private void FormManagerNavigation_Load(object sender, EventArgs e)
        {
            // Отключение окна от минимизации, максимизации и стандартного закрытия
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.ControlBox = false;
        }
    }
}