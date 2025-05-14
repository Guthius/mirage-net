namespace Mirage.Client.Forms
{
	partial class frmTrade
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTrade));
            picFixItems = new PictureBox();
            lstTrade = new ListBox();
            Picture1 = new PictureBox();
            picNewAccount = new PictureBox();
            picDeal = new PictureBox();
            picCancel = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)picFixItems).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Picture1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picNewAccount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picDeal).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picCancel).BeginInit();
            SuspendLayout();
            // 
            // picFixItems
            // 
            picFixItems.Image = (Image)resources.GetObject("picFixItems.Image");
            picFixItems.Location = new Point(272, 200);
            picFixItems.Name = "picFixItems";
            picFixItems.Size = new Size(200, 34);
            picFixItems.SizeMode = PictureBoxSizeMode.AutoSize;
            picFixItems.TabIndex = 5;
            picFixItems.TabStop = false;
            picFixItems.Visible = false;
            picFixItems.Click += picFixItems_Click;
            // 
            // lstTrade
            // 
            lstTrade.BackColor = Color.FromArgb(0, 0, 0);
            lstTrade.BorderStyle = BorderStyle.FixedSingle;
            lstTrade.Font = new Font("Arial", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lstTrade.Location = new Point(224, 64);
            lstTrade.Name = "lstTrade";
            lstTrade.Size = new Size(305, 128);
            lstTrade.TabIndex = 4;
            // 
            // Picture1
            // 
            Picture1.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Picture1.Image = (Image)resources.GetObject("Picture1.Image");
            Picture1.Location = new Point(0, 0);
            Picture1.Name = "Picture1";
            Picture1.Size = new Size(201, 309);
            Picture1.SizeMode = PictureBoxSizeMode.AutoSize;
            Picture1.TabIndex = 3;
            Picture1.TabStop = false;
            // 
            // picNewAccount
            // 
            picNewAccount.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            picNewAccount.Image = (Image)resources.GetObject("picNewAccount.Image");
            picNewAccount.Location = new Point(216, 8);
            picNewAccount.Name = "picNewAccount";
            picNewAccount.Size = new Size(320, 55);
            picNewAccount.SizeMode = PictureBoxSizeMode.AutoSize;
            picNewAccount.TabIndex = 2;
            picNewAccount.TabStop = false;
            // 
            // picDeal
            // 
            picDeal.Image = (Image)resources.GetObject("picDeal.Image");
            picDeal.Location = new Point(272, 232);
            picDeal.Name = "picDeal";
            picDeal.Size = new Size(200, 34);
            picDeal.SizeMode = PictureBoxSizeMode.AutoSize;
            picDeal.TabIndex = 1;
            picDeal.TabStop = false;
            picDeal.Click += picDeal_Click;
            // 
            // picCancel
            // 
            picCancel.Image = (Image)resources.GetObject("picCancel.Image");
            picCancel.Location = new Point(272, 264);
            picCancel.Name = "picCancel";
            picCancel.Size = new Size(200, 34);
            picCancel.SizeMode = PictureBoxSizeMode.AutoSize;
            picCancel.TabIndex = 0;
            picCancel.TabStop = false;
            picCancel.Click += picCancel_Click;
            // 
            // frmTrade
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.FromArgb(0, 0, 0);
            ClientSize = new Size(546, 304);
            ControlBox = false;
            Controls.Add(picFixItems);
            Controls.Add(lstTrade);
            Controls.Add(Picture1);
            Controls.Add(picNewAccount);
            Controls.Add(picDeal);
            Controls.Add(picCancel);
            Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmTrade";
            StartPosition = FormStartPosition.CenterScreen;
            Text = " ";
            ((System.ComponentModel.ISupportInitialize)picFixItems).EndInit();
            ((System.ComponentModel.ISupportInitialize)Picture1).EndInit();
            ((System.ComponentModel.ISupportInitialize)picNewAccount).EndInit();
            ((System.ComponentModel.ISupportInitialize)picDeal).EndInit();
            ((System.ComponentModel.ISupportInitialize)picCancel).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        public System.Windows.Forms.PictureBox picFixItems;
        public System.Windows.Forms.ListBox lstTrade;
		private System.Windows.Forms.PictureBox Picture1;
		private System.Windows.Forms.PictureBox picNewAccount;
		private System.Windows.Forms.PictureBox picDeal;
		private System.Windows.Forms.PictureBox picCancel;
	}
}
