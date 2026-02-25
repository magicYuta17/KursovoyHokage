using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
        public FormAutorization()
        {
            InitializeComponent();


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

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {



            if (textBox1.Text.Length < 3 || textBox2.Text.Length < 3)
            {
                MessageBox.Show("Логин или пароль слишком короткие", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool credentialsValid = (textBox1.Text == "admin" && textBox2.Text == "admin");
            bool credentialsValidManager = (textBox1.Text == "manager" && textBox2.Text == "manager");
            bool credentialsValidHead= (textBox1.Text == "head" && textBox2.Text == "head");

            if (credentialsValid)
            {
                if (authAtt >= 1)
                {
                    if (!CheckCaptcha())
                    {
                        MessageBox.Show("Неверная капча! Форма заблокирована на 10 секунд", "Ошибка",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                        BlockSystem();
                        return;
                    }
                }

                MessageBox.Show("Вход выполнен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                FormAdminNavigation f = new FormAdminNavigation();

                this.Hide();
                f.ShowDialog();
                this.Show();

                authAtt = 0;
                captchaTrue = false;
                textBoxCaptcha.Clear();

            }else if (credentialsValidManager)
            {
                if (authAtt >= 1)
                {
                    if (!CheckCaptcha())
                    {
                        MessageBox.Show("Неверная капча! Форма заблокирована на 10 секунд", "Ошибка",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                        BlockSystem();
                        return;
                    }
                }
                MessageBox.Show("Вход выполнен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                FormManagerNavigation f = new FormManagerNavigation();

                this.Hide();
                f.ShowDialog();
                this.Show();
                authAtt = 0;
                captchaTrue = false;
                textBoxCaptcha.Clear();
            }
            else if (credentialsValidHead)
            {
                if (authAtt >= 1)
                {
                    if (!CheckCaptcha())
                    {
                        MessageBox.Show("Неверная капча! Форма заблокирована на 10 секунд", "Ошибка",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                        BlockSystem();
                        return;
                    }
                }
                MessageBox.Show("Вход выполнен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                FormHeadNavigation f = new FormHeadNavigation();

                this.Hide();
                f.ShowDialog();
                this.Show();
                authAtt = 0;
                captchaTrue = false;
                textBoxCaptcha.Clear();
            }
            else
            {


                if (authAtt >= 1)
                {

                    if (!CheckCaptcha())
                    {
                        MessageBox.Show("Неверная капча! Форма заблокирована на 10 секунд", "Ошибка",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                        BlockSystem();
                        return;
                    }
                    else
                    {

                        MessageBox.Show("Неверный логин или пароль! Форма заблокирована на 10 секунд", "Ошибка",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                        BlockSystem();
                        return;
                    }
                }
                else
                {

                    authAtt++;
                    MessageBox.Show("Неверный логин или пароль! Введите капчу!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    captchaIMG.Visible = true;

                    label3.Visible = true;
                    textBoxCaptcha.Visible = true;

                    buttonCheckCaptcha2.Visible = true;
                    this.Width = 1000;
                    GenerateCaptcha();
                }
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
            this.Width = 1000;
            authAtt++;
            if (authAtt == 1)
            {
                MessageBox.Show("Логин или пароль введены неверно! Требуется ввод CAPTCHA.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ShowCaptcha();
            }
            else if (authAtt > 1)
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
                    cptAnswer = "$9874";
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
            Application.Exit();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
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
        }
    }
}
