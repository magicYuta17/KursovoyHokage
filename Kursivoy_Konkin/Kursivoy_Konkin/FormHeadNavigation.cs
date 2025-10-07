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
    }
}
