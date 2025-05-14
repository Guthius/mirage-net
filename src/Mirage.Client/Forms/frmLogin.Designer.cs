namespace Mirage.Client.Forms
{
	partial class frmLogin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLogin));
            Picture1 = new PictureBox();
            picNewAccount = new PictureBox();
            txtName = new TextBox();
            txtPassword = new TextBox();
            picConnect = new PictureBox();
            picCancel = new PictureBox();
            Label1 = new Label();
            Label2 = new Label();
            Label3 = new Label();
            ((System.ComponentModel.ISupportInitialize)Picture1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picNewAccount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picConnect).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picCancel).BeginInit();
            SuspendLayout();
            // 
            // Picture1
            // 
            Picture1.Image = (Image)resources.GetObject("Picture1.Image");
            Picture1.Location = new Point(0, 0);
            Picture1.Name = "Picture1";
            Picture1.Size = new Size(201, 309);
            Picture1.SizeMode = PictureBoxSizeMode.AutoSize;
            Picture1.TabIndex = 5;
            Picture1.TabStop = false;
            // 
            // picNewAccount
            // 
            picNewAccount.Image = (Image)resources.GetObject("picNewAccount.Image");
            picNewAccount.Location = new Point(216, 8);
            picNewAccount.Name = "picNewAccount";
            picNewAccount.Size = new Size(320, 55);
            picNewAccount.SizeMode = PictureBoxSizeMode.AutoSize;
            picNewAccount.TabIndex = 4;
            picNewAccount.TabStop = false;
            // 
            // txtName
            // 
            txtName.BackColor = Color.FromArgb(0, 0, 64);
            txtName.BorderStyle = BorderStyle.None;
            txtName.Font = new Font("Arial", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtName.ForeColor = Color.FromArgb(192, 192, 255);
            txtName.Location = new Point(312, 160);
            txtName.MaxLength = 20;
            txtName.Name = "txtName";
            txtName.Size = new Size(225, 19);
            txtName.TabIndex = 0;
            // 
            // txtPassword
            // 
            txtPassword.BackColor = Color.FromArgb(0, 0, 64);
            txtPassword.BorderStyle = BorderStyle.None;
            txtPassword.Font = new Font("Arial", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtPassword.ForeColor = Color.FromArgb(192, 192, 255);
            txtPassword.Location = new Point(312, 200);
            txtPassword.MaxLength = 20;
            txtPassword.Name = "txtPassword";
            txtPassword.PasswordChar = '*';
            txtPassword.Size = new Size(225, 19);
            txtPassword.TabIndex = 1;
            // 
            // picConnect
            // 
            picConnect.Image = (Image)resources.GetObject("picConnect.Image");
            picConnect.Location = new Point(272, 232);
            picConnect.Name = "picConnect";
            picConnect.Size = new Size(200, 34);
            picConnect.SizeMode = PictureBoxSizeMode.AutoSize;
            picConnect.TabIndex = 3;
            picConnect.TabStop = false;
            picConnect.Click += picConnect_Click;
            // 
            // picCancel
            // 
            picCancel.Image = (Image)resources.GetObject("picCancel.Image");
            picCancel.Location = new Point(272, 264);
            picCancel.Name = "picCancel";
            picCancel.Size = new Size(200, 34);
            picCancel.SizeMode = PictureBoxSizeMode.AutoSize;
            picCancel.TabIndex = 2;
            picCancel.TabStop = false;
            picCancel.Click += picCancel_Click;
            // 
            // Label1
            // 
            Label1.AutoSize = true;
            Label1.BackColor = Color.Transparent;
            Label1.Font = new Font("Arial", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Label1.ForeColor = Color.FromArgb(0, 0, 255);
            Label1.Location = new Point(224, 160);
            Label1.Name = "Label1";
            Label1.Size = new Size(53, 19);
            Label1.TabIndex = 8;
            Label1.Text = "Name";
            // 
            // Label2
            // 
            Label2.BackColor = Color.Transparent;
            Label2.Font = new Font("Arial", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Label2.ForeColor = Color.FromArgb(0, 0, 255);
            Label2.Location = new Point(224, 72);
            Label2.Name = "Label2";
            Label2.Size = new Size(305, 57);
            Label2.TabIndex = 7;
            Label2.Text = "Enter a account name and password.  ";
            // 
            // Label3
            // 
            Label3.AutoSize = true;
            Label3.BackColor = Color.Transparent;
            Label3.Font = new Font("Arial", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Label3.ForeColor = Color.FromArgb(0, 0, 255);
            Label3.Location = new Point(224, 200);
            Label3.Name = "Label3";
            Label3.Size = new Size(86, 19);
            Label3.TabIndex = 6;
            Label3.Text = "Password";
            // 
            // frmLogin
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.FromArgb(0, 0, 0);
            ClientSize = new Size(546, 304);
            ControlBox = false;
            Controls.Add(Picture1);
            Controls.Add(picNewAccount);
            Controls.Add(txtName);
            Controls.Add(txtPassword);
            Controls.Add(picConnect);
            Controls.Add(picCancel);
            Controls.Add(Label1);
            Controls.Add(Label2);
            Controls.Add(Label3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmLogin";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Mirage Online (Login)";
            ((System.ComponentModel.ISupportInitialize)Picture1).EndInit();
            ((System.ComponentModel.ISupportInitialize)picNewAccount).EndInit();
            ((System.ComponentModel.ISupportInitialize)picConnect).EndInit();
            ((System.ComponentModel.ISupportInitialize)picCancel).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.PictureBox Picture1;
		private System.Windows.Forms.PictureBox picNewAccount;
		public System.Windows.Forms.TextBox txtName;
		public System.Windows.Forms.TextBox txtPassword;
		private System.Windows.Forms.PictureBox picConnect;
		private System.Windows.Forms.PictureBox picCancel;
		private System.Windows.Forms.Label Label1;
		private System.Windows.Forms.Label Label2;
		private System.Windows.Forms.Label Label3;
	}
}
