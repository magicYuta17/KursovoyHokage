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
        }

        private void button5_Click(object sender, EventArgs e)
        {
            FormAutorization formAutorization = new FormAutorization();
            formAutorization.Show();
            this.Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            FormHeadViewStatus f = new FormHeadViewStatus();
            this.Hide();
            f.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FormHeadViewClients f = new FormHeadViewClients();  
            this.Hide();
            f.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FormHeadContract contract = new FormHeadContract();
            this.Hide();
            contract.ShowDialog();

        }
    }
}
