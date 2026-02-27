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
    public partial class FormHeadNavigation : Form
    {
        public FormHeadNavigation()
        {
            InitializeComponent();
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.ControlBox = false;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {

            FormHeadViewStatus f = new FormHeadViewStatus();
            this.Visible = false;
            f.ShowDialog();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FormHeadViewClients f = new FormHeadViewClients();
            this.Visible = false;
            f.ShowDialog();
            this.Close();
       
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FormViewObject f = new FormViewObject("FormHeadNavigation");
            this.Visible = false;
            f.ShowDialog();
            this.Close();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            FormHeadViewContract f = new FormHeadViewContract();
            this.Visible = false;
            f.ShowDialog();
            this.Close();
        }
    }
}
