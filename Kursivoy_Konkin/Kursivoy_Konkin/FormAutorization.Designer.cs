
namespace Kursivoy_Konkin
{
    partial class FormAutorization
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAutorization));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.captchaIMG = new System.Windows.Forms.PictureBox();
            this.buttonCheckCaptcha2 = new System.Windows.Forms.Button();
            this.textBoxCaptcha = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.captchaIMG)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.label1.Location = new System.Drawing.Point(33, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 38);
            this.label1.TabIndex = 0;
            this.label1.Text = "Логин";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.label2.Location = new System.Drawing.Point(33, 150);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(116, 38);
            this.label2.TabIndex = 1;
            this.label2.Text = "Пароль";
            // 
            // textBox2
            // 
            this.textBox2.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.textBox2.Location = new System.Drawing.Point(40, 87);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(249, 45);
            this.textBox2.TabIndex = 2;
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.textBox1.Location = new System.Drawing.Point(40, 202);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(249, 45);
            this.textBox1.TabIndex = 3;
            this.textBox1.UseSystemPasswordChar = true;
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(204)))), ((int)(((byte)(153)))));
            this.button2.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.button2.Location = new System.Drawing.Point(40, 290);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(249, 57);
            this.button2.TabIndex = 4;
            this.button2.Text = "Войти";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(204)))), ((int)(((byte)(153)))));
            this.button1.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.button1.Location = new System.Drawing.Point(40, 366);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(249, 57);
            this.button1.TabIndex = 5;
            this.button1.Text = "Выход";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // captchaIMG
            // 
            this.captchaIMG.Location = new System.Drawing.Point(317, 87);
            this.captchaIMG.Name = "captchaIMG";
            this.captchaIMG.Size = new System.Drawing.Size(374, 160);
            this.captchaIMG.TabIndex = 6;
            this.captchaIMG.TabStop = false;
            // 
            // buttonCheckCaptcha2
            // 
            this.buttonCheckCaptcha2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(204)))), ((int)(((byte)(153)))));
            this.buttonCheckCaptcha2.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.buttonCheckCaptcha2.Location = new System.Drawing.Point(317, 366);
            this.buttonCheckCaptcha2.Name = "buttonCheckCaptcha2";
            this.buttonCheckCaptcha2.Size = new System.Drawing.Size(249, 57);
            this.buttonCheckCaptcha2.TabIndex = 12;
            this.buttonCheckCaptcha2.Text = "Обновить";
            this.buttonCheckCaptcha2.UseVisualStyleBackColor = false;
            this.buttonCheckCaptcha2.Click += new System.EventHandler(this.buttonCheckCaptcha2_Click);
            // 
            // textBoxCaptcha
            // 
            this.textBoxCaptcha.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.textBoxCaptcha.Location = new System.Drawing.Point(317, 302);
            this.textBoxCaptcha.Name = "textBoxCaptcha";
            this.textBoxCaptcha.Size = new System.Drawing.Size(249, 45);
            this.textBoxCaptcha.TabIndex = 11;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.label3.Location = new System.Drawing.Point(310, 261);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(217, 38);
            this.label3.TabIndex = 10;
            this.label3.Text = "Введите каптчу";
            // 
            // FormAutorization
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(721, 466);
            this.Controls.Add(this.buttonCheckCaptcha2);
            this.Controls.Add(this.textBoxCaptcha);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.captchaIMG);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormAutorization";
            this.Text = "Авторизация";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormAutorization_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.captchaIMG)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox captchaIMG;
        private System.Windows.Forms.Button buttonCheckCaptcha2;
        private System.Windows.Forms.TextBox textBoxCaptcha;
        private System.Windows.Forms.Label label3;
    }
}

