namespace Mirage.Client.Forms
{
	partial class frmFixItem
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmFixItem));
            cmbItem = new ComboBox();
            picCancel = new PictureBox();
            chkFix = new PictureBox();
            picNewAccount = new PictureBox();
            Picture1 = new PictureBox();
            Label1 = new Label();
            Label2 = new Label();
            ((System.ComponentModel.ISupportInitialize)picCancel).BeginInit();
            ((System.ComponentModel.ISupportInitialize)chkFix).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picNewAccount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Picture1).BeginInit();
            SuspendLayout();
            // 
            // cmbItem
            // 
            cmbItem.BackColor = Color.FromArgb(0, 0, 0);
            cmbItem.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbItem.Font = new Font("Arial", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            cmbItem.Location = new Point(280, 120);
            cmbItem.Name = "cmbItem";
            cmbItem.Size = new Size(249, 27);
            cmbItem.TabIndex = 4;
            // 
            // picCancel
            // 
            picCancel.Image = (Image)resources.GetObject("picCancel.Image");
            picCancel.Location = new Point(272, 264);
            picCancel.Name = "picCancel";
            picCancel.Size = new Size(200, 34);
            picCancel.SizeMode = PictureBoxSizeMode.AutoSize;
            picCancel.TabIndex = 3;
            picCancel.TabStop = false;
            picCancel.Click += picCancel_Click;
            // 
            // chkFix
            // 
            chkFix.Image = (Image)resources.GetObject("chkFix.Image");
            chkFix.Location = new Point(272, 232);
            chkFix.Name = "chkFix";
            chkFix.Size = new Size(200, 34);
            chkFix.SizeMode = PictureBoxSizeMode.AutoSize;
            chkFix.TabIndex = 2;
            chkFix.TabStop = false;
            chkFix.Click += chkFix_Click;
            // 
            // picNewAccount
            // 
            picNewAccount.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            picNewAccount.Image = (Image)resources.GetObject("picNewAccount.Image");
            picNewAccount.Location = new Point(216, 8);
            picNewAccount.Name = "picNewAccount";
            picNewAccount.Size = new Size(320, 55);
            picNewAccount.SizeMode = PictureBoxSizeMode.AutoSize;
            picNewAccount.TabIndex = 1;
            picNewAccount.TabStop = false;
            // 
            // Picture1
            // 
            Picture1.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Picture1.Image = (Image)resources.GetObject("Picture1.Image");
            Picture1.Location = new Point(0, 0);
            Picture1.Name = "Picture1";
            Picture1.Size = new Size(201, 309);
            Picture1.SizeMode = PictureBoxSizeMode.AutoSize;
            Picture1.TabIndex = 0;
            Picture1.TabStop = false;
            // 
            // Label1
            // 
            Label1.BackColor = Color.Transparent;
            Label1.Font = new Font("Arial", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Label1.ForeColor = Color.FromArgb(0, 0, 255);
            Label1.Location = new Point(224, 80);
            Label1.Name = "Label1";
            Label1.Size = new Size(305, 25);
            Label1.TabIndex = 6;
            Label1.Text = "Please select the item you wish to fix.";
            // 
            // Label2
            // 
            Label2.BackColor = Color.Transparent;
            Label2.Font = new Font("Arial", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Label2.ForeColor = Color.FromArgb(0, 0, 255);
            Label2.Location = new Point(224, 120);
            Label2.Name = "Label2";
            Label2.Size = new Size(57, 25);
            Label2.TabIndex = 5;
            Label2.Text = "Item";
            // 
            // frmFixItem
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.FromArgb(0, 0, 0);
            ClientSize = new Size(546, 304);
            ControlBox = false;
            Controls.Add(cmbItem);
            Controls.Add(picCancel);
            Controls.Add(chkFix);
            Controls.Add(picNewAccount);
            Controls.Add(Picture1);
            Controls.Add(Label1);
            Controls.Add(Label2);
            Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmFixItem";
            StartPosition = FormStartPosition.CenterScreen;
            Text = " ";
            ((System.ComponentModel.ISupportInitialize)picCancel).EndInit();
            ((System.ComponentModel.ISupportInitialize)chkFix).EndInit();
            ((System.ComponentModel.ISupportInitialize)picNewAccount).EndInit();
            ((System.ComponentModel.ISupportInitialize)Picture1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        public System.Windows.Forms.ComboBox cmbItem;
		private System.Windows.Forms.PictureBox picCancel;
		private System.Windows.Forms.PictureBox chkFix;
		private System.Windows.Forms.PictureBox picNewAccount;
		private System.Windows.Forms.PictureBox Picture1;
		private System.Windows.Forms.Label Label1;
		private System.Windows.Forms.Label Label2;
	}
}
