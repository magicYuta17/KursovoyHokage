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
    public partial class FormAdminWorker : Form
    {
        public FormAdminWorker()
        {
            InitializeComponent();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            FormAdminNavigation formAdminNavigation = new FormAdminNavigation();
            formAdminNavigation.Show();
            this.Close();
        }
    }
}
