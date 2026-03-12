using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using Kursivoy_Konkin.Admin;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using MySql.Data.MySqlClient;
using System.Threading;

namespace Kursivoy_Konkin
{
    public partial class FormAutorization : Form
    {
        private string connectionString = connect.con;
        public FormAutorization()
        {
            InitializeComponent();
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            


            captchaIMG.Visible = false;
            label3.Visible = false;
            textBoxCaptcha.Visible = false;

            buttonCheckCaptcha2.Visible = false;


            this.Width = 600;

            
        }

        private int lastCaptchaID = 0;
        int authAtt = 0;
        private bool isBlocked = false;
        bool captchaTrue = false;

        private string cptAnswer = "";

       

        private void Form1_Load(object sender, EventArgs e)
        {
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length < 3 || textBox2.Text.Length < 3)
            {
                MessageBox.Show("Логин или пароль слишком короткие", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string role = null;
            bool isValid = false;

            // 1. Проверка жёстко заданных учётных записей
            if (textBox1.Text == "admin" && textBox2.Text == "admin")
            {
                isValid = true;
                role = "1";   // роль администратора
            }
            else if (textBox1.Text == "manager" && textBox2.Text == "manager")
            {
                isValid = true;
                role = "2";   // роль менеджера
            }
            else if (textBox1.Text == "head" && textBox2.Text == "head")
            {
                isValid = true;
                role = "3";   // роль руководителя
            }
            else
            {
                // 2. Проверка в таблице worker (логин и пароль из БД)
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();
                        // Предполагаем, что в таблице worker есть поля FIO, password, role (тип INT)
                        string query = "SELECT Role_worker_ID_Role FROM worker WHERE FIO = @login AND password = @password";
                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@login", textBox2.Text);   // логин
                            cmd.Parameters.AddWithValue("@password", textBox1.Text); // пароль

                            object result = cmd.ExecuteScalar(); // получаем значение role
                            if (result != null)
                            {
                                isValid = true;
                                role = result.ToString(); // роль как строка ("1", "2", "3")
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка подключения к базе данных: " + ex.Message,
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // 3. Обработка результата проверки
            if (isValid)
            {
                // Если были неудачные попытки, проверяем капчу
                if (authAtt >= 1)
                {
                    if (!CheckCaptcha())
                    {
                        MessageBox.Show("Неверная капча! Форма заблокирована на 10 секунд",
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        BlockSystem();
                        return;
                    }
                }

                MessageBox.Show("Вход выполнен!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Открытие соответствующей формы в зависимости от роли
                switch (role)
                {
                    case "1":
                        FormAdminLocal adminForm = new FormAdminLocal();
                        this.Visible = false;
                        adminForm.ShowDialog();
                        this.Close();
                        break;
                    case "2":
                        FormManagerNavigation managerForm = new FormManagerNavigation();
                        this.Visible = false;
                        managerForm.ShowDialog();
                        this.Close();
                        break;
                    case "3":
                        FormHeadNavigation headForm = new FormHeadNavigation();
                        this.Visible = false;
                        headForm.ShowDialog();
                        this.Close();
                        break;
                    default:
                        MessageBox.Show("Неизвестная роль пользователя", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }

                // Сброс счётчика попыток и капчи
                authAtt = 0;
                captchaTrue = false;
                textBoxCaptcha.Clear();
            }
            else
            {
                // Неудачная попытка – вызываем метод обработки
                HandleFailedLogin();
            }
        }
        private bool CheckCaptcha()
        {
            if (textBoxCaptcha.Text == cptAnswer)
            {
                captchaTrue = true;
                return true;
            }
            else
            {
                captchaTrue = false;
                return false;
            }
        }

        private void HandleFailedLogin()
        {
            authAtt++;
            if (authAtt == 1)
            {
                MessageBox.Show("Логин или пароль введены неверно! Требуется ввод CAPTCHA.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Width = 1050;
                ShowCaptcha();
            }
            else // authAtt > 1
            {
                MessageBox.Show("Логин или пароль были введены неверно! \nСистема будет заблокирована на 10 секунд!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxCaptcha.Text = "";
                GenerateCaptcha();
                BlockSystem();
            }
        }


        private void BlockSystem()
        {
            isBlocked = true;
            SetControlsEnabled(false);
            Thread blockThread = new Thread(() =>
            {
                Thread.Sleep(10000);

                this.Invoke(new Action(() =>
                {
                    isBlocked = false;
                    SetControlsEnabled(true);
                    textBoxCaptcha.Text = "";
                    MessageBox.Show("Система разблокирована", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    textBox1.Text = "";
                    textBox2.Text = "";
                }));
            });

            blockThread.IsBackground = true;
            blockThread.Start();
        }

        private void SetControlsEnabled(bool enabled)
        {
            textBox1.Enabled = enabled;
            textBox2.Enabled = enabled;

            textBoxCaptcha.Enabled = enabled;
            button2.Enabled = enabled;
            buttonCheckCaptcha2.Enabled = enabled;
           
        }

        private void ShowCaptcha()
        {
            GenerateCaptcha();
            captchaIMG.Visible = true;
            label3.Visible = true;
            textBoxCaptcha.Visible = true;

            buttonCheckCaptcha2.Visible = true;
        }

        public void GenerateCaptcha()
        {
            Random r = new Random();

            int captID;

            do
            {
                captID = r.Next(1, 4);
            }
            while (captID == lastCaptchaID);

            lastCaptchaID = captID;

            string imgPath = "";

            switch (captID)
            {
                case 1:
                    imgPath = "1.png";
                    cptAnswer = "9874";
                    break;

                case 2:
                    imgPath = "2.png";
                    cptAnswer = "$%#!@MH";
                    break;

                case 3:
                    imgPath = "3.png";
                    cptAnswer = "F.f_31!";
                    break;
            }

            if (File.Exists(imgPath))
            {
                if (captchaIMG.Image != null)
                {
                    captchaIMG.Image.Dispose();
                    captchaIMG.Image = null;
                }

                captchaIMG.Image = Image.FromFile(imgPath);
            }
            else
            {

                MessageBox.Show($"Файл с изображением капчи не найден: {imgPath}");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Вы действительно хотите выйти?", "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {

                    Environment.Exit(0);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Вы действительно хотите выйти?", "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {

                    Environment.Exit(0);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void buttonCheckCaptcha2_Click(object sender, EventArgs e)
        {
            GenerateCaptcha();
        }

        private void FormAutorization_FormClosing(object sender, FormClosingEventArgs e)
        {
            var result = MessageBox.Show(
       "Вы действительно хотите выйти?",
        "Подтверждение выхода",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Question);

            if (result == DialogResult.No)
            {
                
                e.Cancel = true;
            }

            if(e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true; //отменяем закрытие формы
            }
        }
    }
}
