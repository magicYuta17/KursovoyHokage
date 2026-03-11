namespace YourNamespace
{
    partial class FormManagerAddContract
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormManagerAddContract));
            this.btnCancel = new System.Windows.Forms.Button();
            this.dgvClients = new System.Windows.Forms.DataGridView();
            this.lblClient = new System.Windows.Forms.Label();
            this.txtSearchClient = new System.Windows.Forms.TextBox();
            this.txtSearchWorker = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dgvWorkers = new System.Windows.Forms.DataGridView();
            this.dgvObjects = new System.Windows.Forms.DataGridView();
            this.txtSearchObject = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lblContractInfo = new System.Windows.Forms.Label();
            this.lblSelectedClient = new System.Windows.Forms.Label();
            this.lblClientValue = new System.Windows.Forms.Label();
            this.lblWorkerValue = new System.Windows.Forms.Label();
            this.lblSelectedWorker = new System.Windows.Forms.Label();
            this.lblObjectValue = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.dtpDateSigning = new System.Windows.Forms.DateTimePicker();
            this.dtpEndDate = new System.Windows.Forms.DateTimePicker();
            this.label6 = new System.Windows.Forms.Label();
            this.btnAddContract = new System.Windows.Forms.Button();
            this.txtContractName = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvClients)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvWorkers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvObjects)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(204)))), ((int)(((byte)(153)))));
            this.btnCancel.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.btnCancel.Location = new System.Drawing.Point(1106, 559);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(184, 52);
            this.btnCancel.TabIndex = 31;
            this.btnCancel.Text = "Назад";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // dgvClients
            // 
            this.dgvClients.AllowUserToAddRows = false;
            this.dgvClients.AllowUserToDeleteRows = false;
            this.dgvClients.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvClients.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvClients.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvClients.Location = new System.Drawing.Point(20, 90);
            this.dgvClients.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.dgvClients.Name = "dgvClients";
            this.dgvClients.ReadOnly = true;
            this.dgvClients.RowHeadersVisible = false;
            this.dgvClients.RowHeadersWidth = 51;
            this.dgvClients.RowTemplate.Height = 24;
            this.dgvClients.RowTemplate.ReadOnly = true;
            this.dgvClients.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvClients.Size = new System.Drawing.Size(727, 156);
            this.dgvClients.TabIndex = 30;
            // 
            // lblClient
            // 
            this.lblClient.AutoSize = true;
            this.lblClient.Font = new System.Drawing.Font("Comic Sans MS", 15.25F);
            this.lblClient.Location = new System.Drawing.Point(15, 7);
            this.lblClient.Name = "lblClient";
            this.lblClient.Size = new System.Drawing.Size(131, 29);
            this.lblClient.TabIndex = 32;
            this.lblClient.Text = "👤 КЛИЕНТ";
            // 
            // txtSearchClient
            // 
            this.txtSearchClient.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.txtSearchClient.Location = new System.Drawing.Point(20, 40);
            this.txtSearchClient.Name = "txtSearchClient";
            this.txtSearchClient.Size = new System.Drawing.Size(728, 45);
            this.txtSearchClient.TabIndex = 33;
            // 
            // txtSearchWorker
            // 
            this.txtSearchWorker.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.txtSearchWorker.Location = new System.Drawing.Point(20, 288);
            this.txtSearchWorker.Name = "txtSearchWorker";
            this.txtSearchWorker.Size = new System.Drawing.Size(728, 45);
            this.txtSearchWorker.TabIndex = 36;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Comic Sans MS", 15.25F);
            this.label1.Location = new System.Drawing.Point(15, 255);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(172, 29);
            this.label1.TabIndex = 35;
            this.label1.Text = "👷 СОТРУДНИК";
            // 
            // dgvWorkers
            // 
            this.dgvWorkers.AllowUserToAddRows = false;
            this.dgvWorkers.AllowUserToDeleteRows = false;
            this.dgvWorkers.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvWorkers.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvWorkers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvWorkers.Location = new System.Drawing.Point(20, 338);
            this.dgvWorkers.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.dgvWorkers.Name = "dgvWorkers";
            this.dgvWorkers.ReadOnly = true;
            this.dgvWorkers.RowHeadersVisible = false;
            this.dgvWorkers.RowHeadersWidth = 51;
            this.dgvWorkers.RowTemplate.Height = 24;
            this.dgvWorkers.RowTemplate.ReadOnly = true;
            this.dgvWorkers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvWorkers.Size = new System.Drawing.Size(727, 152);
            this.dgvWorkers.TabIndex = 37;
            // 
            // dgvObjects
            // 
            this.dgvObjects.AllowUserToAddRows = false;
            this.dgvObjects.AllowUserToDeleteRows = false;
            this.dgvObjects.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvObjects.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvObjects.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvObjects.Location = new System.Drawing.Point(20, 578);
            this.dgvObjects.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.dgvObjects.Name = "dgvObjects";
            this.dgvObjects.ReadOnly = true;
            this.dgvObjects.RowHeadersVisible = false;
            this.dgvObjects.RowHeadersWidth = 51;
            this.dgvObjects.RowTemplate.Height = 24;
            this.dgvObjects.RowTemplate.ReadOnly = true;
            this.dgvObjects.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvObjects.Size = new System.Drawing.Size(727, 217);
            this.dgvObjects.TabIndex = 40;
            // 
            // txtSearchObject
            // 
            this.txtSearchObject.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.txtSearchObject.Location = new System.Drawing.Point(20, 527);
            this.txtSearchObject.Name = "txtSearchObject";
            this.txtSearchObject.Size = new System.Drawing.Size(728, 45);
            this.txtSearchObject.TabIndex = 39;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Comic Sans MS", 15.25F);
            this.label2.Location = new System.Drawing.Point(15, 495);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(134, 29);
            this.label2.TabIndex = 38;
            this.label2.Text = "🏗️ ОБЪЕКТ";
            // 
            // lblContractInfo
            // 
            this.lblContractInfo.AutoSize = true;
            this.lblContractInfo.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.lblContractInfo.Location = new System.Drawing.Point(765, 7);
            this.lblContractInfo.Name = "lblContractInfo";
            this.lblContractInfo.Size = new System.Drawing.Size(321, 38);
            this.lblContractInfo.TabIndex = 41;
            this.lblContractInfo.Text = "ДАННЫЕ КОНТРАКТА";
            // 
            // lblSelectedClient
            // 
            this.lblSelectedClient.AutoSize = true;
            this.lblSelectedClient.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.lblSelectedClient.Location = new System.Drawing.Point(765, 57);
            this.lblSelectedClient.Name = "lblSelectedClient";
            this.lblSelectedClient.Size = new System.Drawing.Size(119, 38);
            this.lblSelectedClient.TabIndex = 42;
            this.lblSelectedClient.Text = "Клиент:";
            // 
            // lblClientValue
            // 
            this.lblClientValue.AutoSize = true;
            this.lblClientValue.Font = new System.Drawing.Font("Comic Sans MS", 15.25F);
            this.lblClientValue.Location = new System.Drawing.Point(884, 65);
            this.lblClientValue.Name = "lblClientValue";
            this.lblClientValue.Size = new System.Drawing.Size(154, 29);
            this.lblClientValue.TabIndex = 43;
            this.lblClientValue.Text = "Не выбран ❌";
            // 
            // lblWorkerValue
            // 
            this.lblWorkerValue.AutoSize = true;
            this.lblWorkerValue.Font = new System.Drawing.Font("Comic Sans MS", 15.25F);
            this.lblWorkerValue.Location = new System.Drawing.Point(921, 128);
            this.lblWorkerValue.Name = "lblWorkerValue";
            this.lblWorkerValue.Size = new System.Drawing.Size(154, 29);
            this.lblWorkerValue.TabIndex = 45;
            this.lblWorkerValue.Text = "Не выбран ❌";
            // 
            // lblSelectedWorker
            // 
            this.lblSelectedWorker.AutoSize = true;
            this.lblSelectedWorker.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.lblSelectedWorker.Location = new System.Drawing.Point(765, 119);
            this.lblSelectedWorker.Name = "lblSelectedWorker";
            this.lblSelectedWorker.Size = new System.Drawing.Size(159, 38);
            this.lblSelectedWorker.TabIndex = 44;
            this.lblSelectedWorker.Text = "Сотрудник:";
            // 
            // lblObjectValue
            // 
            this.lblObjectValue.AutoSize = true;
            this.lblObjectValue.Font = new System.Drawing.Font("Comic Sans MS", 15.25F);
            this.lblObjectValue.Location = new System.Drawing.Point(884, 188);
            this.lblObjectValue.Name = "lblObjectValue";
            this.lblObjectValue.Size = new System.Drawing.Size(154, 29);
            this.lblObjectValue.TabIndex = 47;
            this.lblObjectValue.Text = "Не выбран ❌";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.label4.Location = new System.Drawing.Point(765, 179);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(119, 38);
            this.label4.TabIndex = 46;
            this.label4.Text = "Объект:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.label3.Location = new System.Drawing.Point(765, 251);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(280, 38);
            this.label3.TabIndex = 48;
            this.label3.Text = "Название контракта:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.label5.Location = new System.Drawing.Point(765, 346);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(252, 38);
            this.label5.TabIndex = 50;
            this.label5.Text = "Дата подписания:";
            // 
            // dtpDateSigning
            // 
            this.dtpDateSigning.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.dtpDateSigning.Location = new System.Drawing.Point(771, 387);
            this.dtpDateSigning.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.dtpDateSigning.Name = "dtpDateSigning";
            this.dtpDateSigning.Size = new System.Drawing.Size(268, 45);
            this.dtpDateSigning.TabIndex = 51;
            // 
            // dtpEndDate
            // 
            this.dtpEndDate.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.dtpEndDate.Location = new System.Drawing.Point(771, 476);
            this.dtpEndDate.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.dtpEndDate.Name = "dtpEndDate";
            this.dtpEndDate.Size = new System.Drawing.Size(268, 45);
            this.dtpEndDate.TabIndex = 53;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.label6.Location = new System.Drawing.Point(765, 436);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(229, 38);
            this.label6.TabIndex = 52;
            this.label6.Text = "Дата окончания:";
            // 
            // btnAddContract
            // 
            this.btnAddContract.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(204)))), ((int)(((byte)(153)))));
            this.btnAddContract.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.btnAddContract.Location = new System.Drawing.Point(771, 559);
            this.btnAddContract.Name = "btnAddContract";
            this.btnAddContract.Size = new System.Drawing.Size(318, 52);
            this.btnAddContract.TabIndex = 54;
            this.btnAddContract.Text = "Добавить контракт";
            this.btnAddContract.UseVisualStyleBackColor = false;
            // 
            // txtContractName
            // 
            this.txtContractName.Font = new System.Drawing.Font("Comic Sans MS", 20.25F);
            this.txtContractName.Location = new System.Drawing.Point(771, 298);
            this.txtContractName.Name = "txtContractName";
            this.txtContractName.Size = new System.Drawing.Size(520, 45);
            this.txtContractName.TabIndex = 56;
            // 
            // FormManagerAddContract
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1443, 857);
            this.Controls.Add(this.txtContractName);
            this.Controls.Add(this.btnAddContract);
            this.Controls.Add(this.dtpEndDate);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.dtpDateSigning);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblObjectValue);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblWorkerValue);
            this.Controls.Add(this.lblSelectedWorker);
            this.Controls.Add(this.lblClientValue);
            this.Controls.Add(this.lblSelectedClient);
            this.Controls.Add(this.lblContractInfo);
            this.Controls.Add(this.dgvObjects);
            this.Controls.Add(this.txtSearchObject);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dgvWorkers);
            this.Controls.Add(this.txtSearchWorker);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtSearchClient);
            this.Controls.Add(this.lblClient);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.dgvClients);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "FormManagerAddContract";
            this.Text = "Добавление контракта - Менеджер";
            ((System.ComponentModel.ISupportInitialize)(this.dgvClients)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvWorkers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvObjects)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.DataGridView dgvClients;
        private System.Windows.Forms.Label lblClient;
        private System.Windows.Forms.TextBox txtSearchClient;
        private System.Windows.Forms.TextBox txtSearchWorker;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dgvWorkers;
        private System.Windows.Forms.DataGridView dgvObjects;
        private System.Windows.Forms.TextBox txtSearchObject;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblContractInfo;
        private System.Windows.Forms.Label lblSelectedClient;
        private System.Windows.Forms.Label lblClientValue;
        private System.Windows.Forms.Label lblWorkerValue;
        private System.Windows.Forms.Label lblSelectedWorker;
        private System.Windows.Forms.Label lblObjectValue;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DateTimePicker dtpDateSigning;
        private System.Windows.Forms.DateTimePicker dtpEndDate;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnAddContract;
        private System.Windows.Forms.TextBox txtContractName;
    }
}