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
            f.Show();
            this.Close();
        }

        private void FormManagerNavigation_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {

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
            FormViewObject f = new FormViewObject();
            this.Visible = false;   
            f.ShowDialog();
            this.Close();
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
        }

        private void button5_Click(object sender, EventArgs e)
        {
            FormAutorization f = new FormAutorization();
            this.Visible = false;
            f.ShowDialog(); 
            this.Close();

        }
    }
}
