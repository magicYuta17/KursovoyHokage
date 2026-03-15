using System;
using System.Drawing;
using System.Windows.Forms;

namespace Kursivoy_Konkin
{
    public partial class FormSettings : Form
    {
        public FormSettings()
        {
            InitializeComponent();
            
        }

        

        private void button1_Click(object sender, EventArgs e)
        {
            FormAutorization f = new FormAutorization();
            this.Visible = false;
            f.ShowDialog();
            this.Close();
        }

        private void btnSave_Click_1(object sender, EventArgs e)
        {
            int minutes = (int)numericTimeout.Value;

            if (minutes < 1 || minutes > 60)
            {
                MessageBox.Show("Введите значение от 1 до 60 минут.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ActivityMonitor.SetTimeoutMinutes(minutes);

            MessageBox.Show($"Таймаут бездействия установлен: {minutes} мин.", "Готово",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            lblCurrentTimeout.Text = $"{minutes} мин.";
        }

        private void FormSettings_Load(object sender, EventArgs e)
        {
            var comicSans = new Font("Comic Sans MS", 14f, FontStyle.Regular, GraphicsUnit.Point);
            var comicSansBold = new Font("Comic Sans MS", 14f, FontStyle.Bold, GraphicsUnit.Point);

            numericTimeout.Minimum = 1;   
            numericTimeout.Maximum = 60;

            numericTimeout.Value = ActivityMonitor.GetTimeoutMinutes();


            lblCurrentTimeout.Text = $"{ActivityMonitor.GetTimeoutMinutes()} мин.";

           
        }
    
    }
}