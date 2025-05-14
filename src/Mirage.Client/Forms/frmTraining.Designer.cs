namespace Mirage.Client.Forms
{
	partial class frmTraining
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTraining));
            cmbStat = new ComboBox();
            picCancel = new PictureBox();
            picTrain = new PictureBox();
            picTraining = new PictureBox();
            Picture1 = new PictureBox();
            Label2 = new Label();
            ((System.ComponentModel.ISupportInitialize)picCancel).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picTrain).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picTraining).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Picture1).BeginInit();
            SuspendLayout();
            // 
            // cmbStat
            // 
            cmbStat.BackColor = Color.FromArgb(0, 0, 0);
            cmbStat.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStat.Font = new Font("Arial", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            cmbStat.FormattingEnabled = true;
            cmbStat.Items.AddRange(new object[] { "Strength", "Defense", "Magic", "Speed" });
            cmbStat.Location = new Point(224, 144);
            cmbStat.Name = "cmbStat";
            cmbStat.Size = new Size(305, 27);
            cmbStat.TabIndex = 5;
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
            // picTrain
            // 
            picTrain.Image = (Image)resources.GetObject("picTrain.Image");
            picTrain.Location = new Point(272, 232);
            picTrain.Name = "picTrain";
            picTrain.Size = new Size(200, 34);
            picTrain.SizeMode = PictureBoxSizeMode.AutoSize;
            picTrain.TabIndex = 2;
            picTrain.TabStop = false;
            picTrain.Click += picTrain_Click;
            // 
            // picTraining
            // 
            picTraining.Image = (Image)resources.GetObject("picTraining.Image");
            picTraining.Location = new Point(216, 8);
            picTraining.Name = "picTraining";
            picTraining.Size = new Size(320, 55);
            picTraining.SizeMode = PictureBoxSizeMode.AutoSize;
            picTraining.TabIndex = 1;
            picTraining.TabStop = false;
            // 
            // Picture1
            // 
            Picture1.Image = (Image)resources.GetObject("Picture1.Image");
            Picture1.Location = new Point(0, 0);
            Picture1.Name = "Picture1";
            Picture1.Size = new Size(201, 309);
            Picture1.SizeMode = PictureBoxSizeMode.AutoSize;
            Picture1.TabIndex = 0;
            Picture1.TabStop = false;
            // 
            // Label2
            // 
            Label2.BackColor = Color.Transparent;
            Label2.Font = new Font("Arial", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Label2.ForeColor = Color.FromArgb(0, 0, 255);
            Label2.Location = new Point(224, 72);
            Label2.Name = "Label2";
            Label2.Size = new Size(305, 65);
            Label2.TabIndex = 4;
            Label2.Text = "What stat would you like to train?";
            // 
            // frmTraining
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.FromArgb(0, 0, 0);
            ClientSize = new Size(546, 304);
            ControlBox = false;
            Controls.Add(cmbStat);
            Controls.Add(picCancel);
            Controls.Add(picTrain);
            Controls.Add(picTraining);
            Controls.Add(Picture1);
            Controls.Add(Label2);
            Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmTraining";
            StartPosition = FormStartPosition.CenterScreen;
            Text = " ";
            Load += frmTraining_Load;
            ((System.ComponentModel.ISupportInitialize)picCancel).EndInit();
            ((System.ComponentModel.ISupportInitialize)picTrain).EndInit();
            ((System.ComponentModel.ISupportInitialize)picTraining).EndInit();
            ((System.ComponentModel.ISupportInitialize)Picture1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ComboBox cmbStat;
		private System.Windows.Forms.PictureBox picCancel;
		private System.Windows.Forms.PictureBox picTrain;
		private System.Windows.Forms.PictureBox picTraining;
		private System.Windows.Forms.PictureBox Picture1;
		private System.Windows.Forms.Label Label2;
	}
}
