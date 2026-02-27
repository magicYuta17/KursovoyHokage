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
    public partial class FormAdminNavigation : Form
    {
        public FormAdminNavigation()
        {
            InitializeComponent();
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.ControlBox = false;
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

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FormAdminWorker formAdminWorkers = new FormAdminWorker();
            this.Visible = false;
            formAdminWorkers.ShowDialog();
            this.Close();
        }

        private void buttonViewObject_Click(object sender, EventArgs e)
        {
            FormAdminObject formAdminObject = new FormAdminObject();
            formAdminObject.ShowDialog();
            this.Close();
        }

        private void FormAdminNavigation_Load(object sender, EventArgs e)
        {
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.ControlBox = false;
        }
    }
}
