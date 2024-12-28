namespace Mirage
{
	partial class frmDeleteAccount
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDeleteAccount));
			picCancel = new System.Windows.Forms.PictureBox();
			picConnect = new System.Windows.Forms.PictureBox();
			txtPassword = new System.Windows.Forms.TextBox();
			txtName = new System.Windows.Forms.TextBox();
			picDeleteAccount = new System.Windows.Forms.PictureBox();
			Picture1 = new System.Windows.Forms.PictureBox();
			Label3 = new System.Windows.Forms.Label();
			Label2 = new System.Windows.Forms.Label();
			Label1 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.picCancel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.picConnect)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.picDeleteAccount)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.Picture1)).BeginInit();
			this.SuspendLayout();
			//
			// picCancel
			//
			this.picCancel.Name = "picCancel";
			this.picCancel.TabIndex = 8;
			this.picCancel.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.picCancel.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.picCancel.Image = ((System.Drawing.Image)(resources.GetObject("picCancel.Image")));
			this.picCancel.TabStop = false;
			this.picCancel.Location = new System.Drawing.Point(272, 264);
			this.picCancel.Size = new System.Drawing.Size(200, 34);
			//
			// picConnect
			//
			this.picConnect.Name = "picConnect";
			this.picConnect.TabIndex = 7;
			this.picConnect.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.picConnect.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.picConnect.Image = ((System.Drawing.Image)(resources.GetObject("picConnect.Image")));
			this.picConnect.TabStop = false;
			this.picConnect.Location = new System.Drawing.Point(272, 232);
			this.picConnect.Size = new System.Drawing.Size(200, 34);
			//
			// txtPassword
			//
			this.txtPassword.Name = "txtPassword";
			this.txtPassword.Location = new System.Drawing.Point(312, 200);
			this.txtPassword.MaxLength = 20;
			this.txtPassword.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtPassword.Size = new System.Drawing.Size(225, 26);
			this.txtPassword.TabIndex = 1;
			this.txtPassword.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.txtPassword.ForeColor = System.Drawing.Color.FromArgb(192, 192, 255);
			this.txtPassword.PasswordChar = '*';
			this.txtPassword.BackColor = System.Drawing.Color.FromArgb(0, 0, 64);
			//
			// txtName
			//
			this.txtName.Name = "txtName";
			this.txtName.ForeColor = System.Drawing.Color.FromArgb(192, 192, 255);
			this.txtName.MaxLength = 20;
			this.txtName.BackColor = System.Drawing.Color.FromArgb(0, 0, 64);
			this.txtName.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtName.Location = new System.Drawing.Point(312, 160);
			this.txtName.Size = new System.Drawing.Size(225, 26);
			this.txtName.TabIndex = 0;
			this.txtName.BorderStyle = System.Windows.Forms.BorderStyle.None;
			//
			// picDeleteAccount
			//
			this.picDeleteAccount.Name = "picDeleteAccount";
			this.picDeleteAccount.Location = new System.Drawing.Point(216, 8);
			this.picDeleteAccount.Size = new System.Drawing.Size(320, 55);
			this.picDeleteAccount.TabIndex = 3;
			this.picDeleteAccount.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.picDeleteAccount.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.picDeleteAccount.Image = ((System.Drawing.Image)(resources.GetObject("picDeleteAccount.Image")));
			this.picDeleteAccount.TabStop = false;
			//
			// Picture1
			//
			this.Picture1.Name = "Picture1";
			this.Picture1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Picture1.Image = ((System.Drawing.Image)(resources.GetObject("Picture1.Image")));
			this.Picture1.TabStop = false;
			this.Picture1.Location = new System.Drawing.Point(0, 0);
			this.Picture1.Size = new System.Drawing.Size(201, 309);
			this.Picture1.TabIndex = 2;
			this.Picture1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			//
			// Label3
			//
			this.Label3.Name = "Label3";
			this.Label3.Size = new System.Drawing.Size(81, 25);
			this.Label3.TabIndex = 6;
			this.Label3.ForeColor = System.Drawing.Color.FromArgb(0, 0, 255);
			this.Label3.Text = "Password";
			this.Label3.BackColor = System.Drawing.Color.Transparent;
			this.Label3.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Label3.Location = new System.Drawing.Point(224, 200);
			//
			// Label2
			//
			this.Label2.Name = "Label2";
			this.Label2.Text = "Enter a account name and password of the account you wish to delete.";
			this.Label2.BackColor = System.Drawing.Color.Transparent;
			this.Label2.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Label2.Location = new System.Drawing.Point(224, 72);
			this.Label2.Size = new System.Drawing.Size(305, 65);
			this.Label2.TabIndex = 5;
			this.Label2.ForeColor = System.Drawing.Color.FromArgb(0, 0, 255);
			//
			// Label1
			//
			this.Label1.Name = "Label1";
			this.Label1.Text = "Name";
			this.Label1.BackColor = System.Drawing.Color.Transparent;
			this.Label1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Label1.Location = new System.Drawing.Point(224, 160);
			this.Label1.Size = new System.Drawing.Size(57, 25);
			this.Label1.TabIndex = 4;
			this.Label1.ForeColor = System.Drawing.Color.FromArgb(0, 0, 255);
			//
			// frmDeleteAccount
			//
			this.Name = "frmDeleteAccount";
			this.BackColor = System.Drawing.Color.FromArgb(0, 0, 0);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(546, 304);
			this.Text = " ";
			this.MinimizeBox = false;
			this.MaximizeBox = false;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.ControlBox = false;
			this.Controls.Add(this.picCancel);
			this.Controls.Add(this.picConnect);
			this.Controls.Add(this.txtPassword);
			this.Controls.Add(this.txtName);
			this.Controls.Add(this.picDeleteAccount);
			this.Controls.Add(this.Picture1);
			this.Controls.Add(this.Label3);
			this.Controls.Add(this.Label2);
			this.Controls.Add(this.Label1);
			((System.ComponentModel.ISupportInitialize)(this.picCancel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.picConnect)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.picDeleteAccount)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.Picture1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		#endregion

		private System.Windows.Forms.PictureBox picCancel;
		private System.Windows.Forms.PictureBox picConnect;
		private System.Windows.Forms.TextBox txtPassword;
		private System.Windows.Forms.TextBox txtName;
		private System.Windows.Forms.PictureBox picDeleteAccount;
		private System.Windows.Forms.PictureBox Picture1;
		private System.Windows.Forms.Label Label3;
		private System.Windows.Forms.Label Label2;
		private System.Windows.Forms.Label Label1;
	}
}
