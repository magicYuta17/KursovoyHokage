namespace Kursivoy_Konkin
{
    partial class FormSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSettings));
            this.numericTimeout = new System.Windows.Forms.NumericUpDown();
            this.lblCurrentTimeout = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numericTimeout)).BeginInit();
            this.SuspendLayout();
            // 
            // numericTimeout
            // 
            this.numericTimeout.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.numericTimeout.Location = new System.Drawing.Point(815, 17);
            this.numericTimeout.Name = "numericTimeout";
            this.numericTimeout.Size = new System.Drawing.Size(146, 55);
            this.numericTimeout.TabIndex = 0;
            // 
            // lblCurrentTimeout
            // 
            this.lblCurrentTimeout.AutoSize = true;
            this.lblCurrentTimeout.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.lblCurrentTimeout.Location = new System.Drawing.Point(616, 19);
            this.lblCurrentTimeout.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCurrentTimeout.Name = "lblCurrentTimeout";
            this.lblCurrentTimeout.Size = new System.Drawing.Size(122, 47);
            this.lblCurrentTimeout.TabIndex = 1;
            this.lblCurrentTimeout.Text = "Логин";
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.lblDescription.Location = new System.Drawing.Point(13, 19);
            this.lblDescription.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(541, 47);
            this.lblDescription.TabIndex = 2;
            this.lblDescription.Text = "Таймаут бездействия (минуты):";
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(204)))), ((int)(((byte)(153)))));
            this.btnSave.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.btnSave.Location = new System.Drawing.Point(21, 591);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(332, 70);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "Сохранить";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click_1);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(204)))), ((int)(((byte)(153)))));
            this.button1.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.button1.Location = new System.Drawing.Point(375, 591);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(332, 70);
            this.button1.TabIndex = 6;
            this.button1.Text = "Назад";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // FormSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1136, 685);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.lblCurrentTimeout);
            this.Controls.Add(this.numericTimeout);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormSettings";
            this.Text = "Настройки";
            this.Load += new System.EventHandler(this.FormSettings_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericTimeout)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numericTimeout;
        private System.Windows.Forms.Label lblCurrentTimeout;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button button1;
    }
}