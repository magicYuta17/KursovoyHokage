namespace Kursivoy_Konkin.Manager
{
    partial class FormHeadDetailsClients
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormHeadDetailsClients));
            this.lblStatusValue = new System.Windows.Forms.Label();
            this.lblLtvValue = new System.Windows.Forms.Label();
            this.lblAgeValue = new System.Windows.Forms.Label();
            this.lblBdayValue = new System.Windows.Forms.Label();
            this.lblPhoneValue2 = new System.Windows.Forms.Label();
            this.lblFioValue = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblStatusValue
            // 
            this.lblStatusValue.AutoSize = true;
            this.lblStatusValue.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.lblStatusValue.Location = new System.Drawing.Point(21, 342);
            this.lblStatusValue.Name = "lblStatusValue";
            this.lblStatusValue.Size = new System.Drawing.Size(205, 47);
            this.lblStatusValue.TabIndex = 29;
            this.lblStatusValue.Text = "Сортировка";
            // 
            // lblLtvValue
            // 
            this.lblLtvValue.AutoSize = true;
            this.lblLtvValue.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.lblLtvValue.Location = new System.Drawing.Point(21, 400);
            this.lblLtvValue.Name = "lblLtvValue";
            this.lblLtvValue.Size = new System.Drawing.Size(205, 47);
            this.lblLtvValue.TabIndex = 28;
            this.lblLtvValue.Text = "Сортировка";
            // 
            // lblAgeValue
            // 
            this.lblAgeValue.AutoSize = true;
            this.lblAgeValue.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.lblAgeValue.Location = new System.Drawing.Point(21, 282);
            this.lblAgeValue.Name = "lblAgeValue";
            this.lblAgeValue.Size = new System.Drawing.Size(205, 47);
            this.lblAgeValue.TabIndex = 27;
            this.lblAgeValue.Text = "Сортировка";
            // 
            // lblBdayValue
            // 
            this.lblBdayValue.AutoSize = true;
            this.lblBdayValue.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.lblBdayValue.Location = new System.Drawing.Point(21, 215);
            this.lblBdayValue.Name = "lblBdayValue";
            this.lblBdayValue.Size = new System.Drawing.Size(205, 47);
            this.lblBdayValue.TabIndex = 26;
            this.lblBdayValue.Text = "Сортировка";
            // 
            // lblPhoneValue2
            // 
            this.lblPhoneValue2.AutoSize = true;
            this.lblPhoneValue2.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.lblPhoneValue2.Location = new System.Drawing.Point(21, 147);
            this.lblPhoneValue2.Name = "lblPhoneValue2";
            this.lblPhoneValue2.Size = new System.Drawing.Size(205, 47);
            this.lblPhoneValue2.TabIndex = 25;
            this.lblPhoneValue2.Text = "Сортировка";
            // 
            // lblFioValue
            // 
            this.lblFioValue.AutoSize = true;
            this.lblFioValue.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.lblFioValue.Location = new System.Drawing.Point(21, 67);
            this.lblFioValue.Name = "lblFioValue";
            this.lblFioValue.Size = new System.Drawing.Size(205, 47);
            this.lblFioValue.TabIndex = 24;
            this.lblFioValue.Text = "Сортировка";
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(204)))), ((int)(((byte)(153)))));
            this.btnClose.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.btnClose.Location = new System.Drawing.Point(21, 599);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(162, 76);
            this.btnClose.TabIndex = 23;
            this.btnClose.Text = "Назад";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // FormHeadDetailsClients
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(715, 688);
            this.Controls.Add(this.lblStatusValue);
            this.Controls.Add(this.lblLtvValue);
            this.Controls.Add(this.lblAgeValue);
            this.Controls.Add(this.lblBdayValue);
            this.Controls.Add(this.lblPhoneValue2);
            this.Controls.Add(this.lblFioValue);
            this.Controls.Add(this.btnClose);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormHeadDetailsClients";
            this.Text = "Детальный просмотр записи";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormHeadDetailsClients_FormClosing_1);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblStatusValue;
        private System.Windows.Forms.Label lblLtvValue;
        private System.Windows.Forms.Label lblAgeValue;
        private System.Windows.Forms.Label lblBdayValue;
        private System.Windows.Forms.Label lblPhoneValue2;
        private System.Windows.Forms.Label lblFioValue;
        private System.Windows.Forms.Button btnClose;
    }
}