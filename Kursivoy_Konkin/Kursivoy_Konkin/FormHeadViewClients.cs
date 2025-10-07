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
    public partial class FormHeadViewClients : Form
    {
        public FormHeadViewClients()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FormHeadNavigation formHeadNavigation = new FormHeadNavigation();
            formHeadNavigation.Show();
            this.Close();
        }
    }
}
