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
    public partial class FormAdmin : Form
    {
        public FormAdmin()
        {
            InitializeComponent();
        }

        private void FormAdmin_FormClosing(object sender, FormClosingEventArgs e)
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
