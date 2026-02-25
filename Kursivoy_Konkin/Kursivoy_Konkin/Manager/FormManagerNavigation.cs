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
    public partial class FormManagerNavigation : Form
    {
        public FormManagerNavigation()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FormViewClients f = new FormViewClients();
            this.Visible = false;
            f.ShowDialog(); 
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FormManagerViewContract f = new FormManagerViewContract();
            this.Visible = false;
            f.ShowDialog();
            this.Close();

        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            FormViewObject f = new FormViewObject("FormManagerNavigation");
            this.Visible = false;
            f.ShowDialog();
            this.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close(); // только кнопка назад закрывает навигацию
        }
    }
}
