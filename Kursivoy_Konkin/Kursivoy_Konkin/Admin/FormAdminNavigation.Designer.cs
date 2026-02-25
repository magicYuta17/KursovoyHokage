
namespace Kursivoy_Konkin
{
    partial class FormAdminNavigation
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAdminNavigation));
            this.button5 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.buttonViewObject = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button5
            // 
            this.button5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(204)))), ((int)(((byte)(153)))));
            this.button5.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.button5.Location = new System.Drawing.Point(55, 716);
            this.button5.Margin = new System.Windows.Forms.Padding(4);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(838, 85);
            this.button5.TabIndex = 25;
            this.button5.Text = "Назад";
            this.button5.UseVisualStyleBackColor = false;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(204)))), ((int)(((byte)(153)))));
            this.button1.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.button1.Location = new System.Drawing.Point(55, 55);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(838, 124);
            this.button1.TabIndex = 21;
            this.button1.Text = "Просмотр, добавление, удаление и редактирование сотрудников";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonViewObject
            // 
            this.buttonViewObject.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(204)))), ((int)(((byte)(153)))));
            this.buttonViewObject.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.buttonViewObject.Location = new System.Drawing.Point(55, 187);
            this.buttonViewObject.Margin = new System.Windows.Forms.Padding(4);
            this.buttonViewObject.Name = "buttonViewObject";
            this.buttonViewObject.Size = new System.Drawing.Size(838, 124);
            this.buttonViewObject.TabIndex = 26;
            this.buttonViewObject.Text = "Просмотр, добавление, удаление и редактирование объектов";
            this.buttonViewObject.UseVisualStyleBackColor = false;
            this.buttonViewObject.Click += new System.EventHandler(this.buttonViewObject_Click);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(204)))), ((int)(((byte)(153)))));
            this.button3.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.button3.Location = new System.Drawing.Point(55, 319);
            this.button3.Margin = new System.Windows.Forms.Padding(4);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(838, 124);
            this.button3.TabIndex = 27;
            this.button3.Text = "Просмотр, добавление, удаление и редактирование ролей";
            this.button3.UseVisualStyleBackColor = false;
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(204)))), ((int)(((byte)(153)))));
            this.button4.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.button4.Location = new System.Drawing.Point(55, 451);
            this.button4.Margin = new System.Windows.Forms.Padding(4);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(838, 176);
            this.button4.TabIndex = 28;
            this.button4.Text = "Восстановление БД,\r\nимпорт и экспорт данных,\r\nрезервное копирование БД\r\n\r\n\r\n\r\n";
            this.button4.UseVisualStyleBackColor = false;
            // 
            // FormAdminNavigation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(922, 833);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.buttonViewObject);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FormAdminNavigation";
            this.Text = "Навигация - Админ";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormAdmin_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button buttonViewObject;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
    }
}