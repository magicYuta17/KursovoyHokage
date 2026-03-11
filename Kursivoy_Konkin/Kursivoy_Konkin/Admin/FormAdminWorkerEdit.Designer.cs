namespace Kursivoy_Konkin
{
    partial class FormAdminWorkerEdit
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAdminWorkerEdit));
            this.btnCancel = new System.Windows.Forms.Button();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.dgvClients = new System.Windows.Forms.DataGridView();
            this.tbClientSearch = new System.Windows.Forms.TextBox();
            this.lblClient = new System.Windows.Forms.Label();
            this.cbRole = new System.Windows.Forms.ComboBox();
            this.lblRole = new System.Windows.Forms.Label();
            this.tbAge = new System.Windows.Forms.TextBox();
            this.lblAge = new System.Windows.Forms.Label();
            this.mtbPhone = new System.Windows.Forms.MaskedTextBox();
            this.lblPhone = new System.Windows.Forms.Label();
            this.tbFIO = new System.Windows.Forms.TextBox();
            this.lblFIO = new System.Windows.Forms.Label();
            this.buttonDeletePhoto = new System.Windows.Forms.Button();
            this.buttonAddPhoto = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvClients)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(204)))), ((int)(((byte)(153)))));
            this.btnCancel.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.btnCancel.Location = new System.Drawing.Point(16, 504);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(290, 57);
            this.btnCancel.TabIndex = 66;
            this.btnCancel.Text = "Назад";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click_1);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(864, 480);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(143, 104);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 65;
            this.pictureBox2.TabStop = false;
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(204)))), ((int)(((byte)(153)))));
            this.btnSave.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.btnSave.Location = new System.Drawing.Point(16, 389);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(290, 108);
            this.btnSave.TabIndex = 64;
            this.btnSave.Text = "Сохранить";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // dgvClients
            // 
            this.dgvClients.AllowUserToAddRows = false;
            this.dgvClients.AllowUserToDeleteRows = false;
            this.dgvClients.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvClients.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvClients.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvClients.BackgroundColor = System.Drawing.Color.AntiqueWhite;
            this.dgvClients.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvClients.EnableHeadersVisualStyles = false;
            this.dgvClients.Location = new System.Drawing.Point(512, 110);
            this.dgvClients.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.dgvClients.Name = "dgvClients";
            this.dgvClients.ReadOnly = true;
            this.dgvClients.RowHeadersVisible = false;
            this.dgvClients.RowHeadersWidth = 51;
            this.dgvClients.RowTemplate.Height = 24;
            this.dgvClients.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvClients.Size = new System.Drawing.Size(496, 182);
            this.dgvClients.TabIndex = 63;
            // 
            // tbClientSearch
            // 
            this.tbClientSearch.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.tbClientSearch.Location = new System.Drawing.Point(512, 48);
            this.tbClientSearch.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tbClientSearch.Name = "tbClientSearch";
            this.tbClientSearch.Size = new System.Drawing.Size(497, 45);
            this.tbClientSearch.TabIndex = 62;
            // 
            // lblClient
            // 
            this.lblClient.AutoSize = true;
            this.lblClient.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.lblClient.Location = new System.Drawing.Point(506, 7);
            this.lblClient.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblClient.Name = "lblClient";
            this.lblClient.Size = new System.Drawing.Size(214, 38);
            this.lblClient.TabIndex = 61;
            this.lblClient.Text = "Поиск клиента";
            // 
            // cbRole
            // 
            this.cbRole.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRole.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.cbRole.FormattingEnabled = true;
            this.cbRole.Items.AddRange(new object[] {
            "Да",
            "Нет"});
            this.cbRole.Location = new System.Drawing.Point(16, 325);
            this.cbRole.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cbRole.Name = "cbRole";
            this.cbRole.Size = new System.Drawing.Size(290, 46);
            this.cbRole.TabIndex = 60;
            // 
            // lblRole
            // 
            this.lblRole.AutoSize = true;
            this.lblRole.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.lblRole.Location = new System.Drawing.Point(10, 284);
            this.lblRole.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblRole.Name = "lblRole";
            this.lblRole.Size = new System.Drawing.Size(77, 38);
            this.lblRole.TabIndex = 59;
            this.lblRole.Text = "Роль";
            // 
            // tbAge
            // 
            this.tbAge.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.tbAge.Location = new System.Drawing.Point(16, 237);
            this.tbAge.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tbAge.Name = "tbAge";
            this.tbAge.Size = new System.Drawing.Size(290, 45);
            this.tbAge.TabIndex = 58;
            // 
            // lblAge
            // 
            this.lblAge.AutoSize = true;
            this.lblAge.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.lblAge.Location = new System.Drawing.Point(10, 197);
            this.lblAge.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblAge.Name = "lblAge";
            this.lblAge.Size = new System.Drawing.Size(115, 38);
            this.lblAge.TabIndex = 57;
            this.lblAge.Text = "Вазраст";
            // 
            // mtbPhone
            // 
            this.mtbPhone.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.mtbPhone.Location = new System.Drawing.Point(16, 139);
            this.mtbPhone.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.mtbPhone.Mask = "+7 (000) 000-00-00";
            this.mtbPhone.Name = "mtbPhone";
            this.mtbPhone.Size = new System.Drawing.Size(290, 45);
            this.mtbPhone.TabIndex = 56;
            // 
            // lblPhone
            // 
            this.lblPhone.AutoSize = true;
            this.lblPhone.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.lblPhone.Location = new System.Drawing.Point(10, 98);
            this.lblPhone.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPhone.Name = "lblPhone";
            this.lblPhone.Size = new System.Drawing.Size(135, 38);
            this.lblPhone.TabIndex = 55;
            this.lblPhone.Text = "Телефон";
            // 
            // tbFIO
            // 
            this.tbFIO.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.tbFIO.Location = new System.Drawing.Point(16, 48);
            this.tbFIO.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tbFIO.Name = "tbFIO";
            this.tbFIO.Size = new System.Drawing.Size(492, 45);
            this.tbFIO.TabIndex = 54;
            // 
            // lblFIO
            // 
            this.lblFIO.AutoSize = true;
            this.lblFIO.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.lblFIO.Location = new System.Drawing.Point(10, 7);
            this.lblFIO.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblFIO.Name = "lblFIO";
            this.lblFIO.Size = new System.Drawing.Size(230, 38);
            this.lblFIO.TabIndex = 53;
            this.lblFIO.Text = "ФИО сотрудника";
            // 
            // buttonDeletePhoto
            // 
            this.buttonDeletePhoto.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(204)))), ((int)(((byte)(153)))));
            this.buttonDeletePhoto.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.buttonDeletePhoto.Location = new System.Drawing.Point(752, 389);
            this.buttonDeletePhoto.Name = "buttonDeletePhoto";
            this.buttonDeletePhoto.Size = new System.Drawing.Size(255, 85);
            this.buttonDeletePhoto.TabIndex = 69;
            this.buttonDeletePhoto.Text = "Удалить фото";
            this.buttonDeletePhoto.UseVisualStyleBackColor = false;
            this.buttonDeletePhoto.Click += new System.EventHandler(this.buttonDeletePhoto_Click);
            // 
            // buttonAddPhoto
            // 
            this.buttonAddPhoto.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(204)))), ((int)(((byte)(153)))));
            this.buttonAddPhoto.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.buttonAddPhoto.Location = new System.Drawing.Point(752, 298);
            this.buttonAddPhoto.Name = "buttonAddPhoto";
            this.buttonAddPhoto.Size = new System.Drawing.Size(255, 85);
            this.buttonAddPhoto.TabIndex = 68;
            this.buttonAddPhoto.Text = "Добавить фото";
            this.buttonAddPhoto.UseVisualStyleBackColor = false;
            this.buttonAddPhoto.Click += new System.EventHandler(this.buttonAddPhoto_Click_1);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Kursivoy_Konkin.Properties.Resources.picture;
            this.pictureBox1.InitialImage = global::Kursivoy_Konkin.Properties.Resources.picture;
            this.pictureBox1.Location = new System.Drawing.Point(512, 297);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(226, 177);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 67;
            this.pictureBox1.TabStop = false;
            // 
            // FormAdminWorkerEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1018, 594);
            this.Controls.Add(this.buttonDeletePhoto);
            this.Controls.Add(this.buttonAddPhoto);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.dgvClients);
            this.Controls.Add(this.tbClientSearch);
            this.Controls.Add(this.lblClient);
            this.Controls.Add(this.cbRole);
            this.Controls.Add(this.lblRole);
            this.Controls.Add(this.tbAge);
            this.Controls.Add(this.lblAge);
            this.Controls.Add(this.mtbPhone);
            this.Controls.Add(this.lblPhone);
            this.Controls.Add(this.tbFIO);
            this.Controls.Add(this.lblFIO);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "FormAdminWorkerEdit";
            this.Text = "Редактирование сотрудника - Админ";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvClients)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.DataGridView dgvClients;
        private System.Windows.Forms.TextBox tbClientSearch;
        private System.Windows.Forms.Label lblClient;
        private System.Windows.Forms.ComboBox cbRole;
        private System.Windows.Forms.Label lblRole;
        private System.Windows.Forms.TextBox tbAge;
        private System.Windows.Forms.Label lblAge;
        private System.Windows.Forms.MaskedTextBox mtbPhone;
        private System.Windows.Forms.Label lblPhone;
        private System.Windows.Forms.TextBox tbFIO;
        private System.Windows.Forms.Label lblFIO;
        private System.Windows.Forms.Button buttonDeletePhoto;
        private System.Windows.Forms.Button buttonAddPhoto;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}