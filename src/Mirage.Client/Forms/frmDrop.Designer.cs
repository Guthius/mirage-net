namespace Mirage.Client.Forms
{
	partial class frmDrop
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
            cmdMinus1 = new Button();
            cmdMinus10 = new Button();
            cmdMinus100 = new Button();
            cmdMinus1000 = new Button();
            cmdPlus1000 = new Button();
            cmdPlus100 = new Button();
            cmdPlus10 = new Button();
            cmdPlus1 = new Button();
            cmdOk = new Button();
            cmdCancel = new Button();
            lblAmount = new Label();
            Label2 = new Label();
            Label1 = new Label();
            lblName = new Label();
            SuspendLayout();
            // 
            // cmdMinus1
            // 
            cmdMinus1.Location = new Point(8, 104);
            cmdMinus1.Name = "cmdMinus1";
            cmdMinus1.Size = new Size(81, 25);
            cmdMinus1.TabIndex = 13;
            cmdMinus1.Text = "-1";
            cmdMinus1.Click += cmdMinus1_Click;
            // 
            // cmdMinus10
            // 
            cmdMinus10.Location = new Point(96, 104);
            cmdMinus10.Name = "cmdMinus10";
            cmdMinus10.Size = new Size(81, 25);
            cmdMinus10.TabIndex = 12;
            cmdMinus10.Text = "-10";
            cmdMinus10.Click += cmdMinus10_Click;
            // 
            // cmdMinus100
            // 
            cmdMinus100.Location = new Point(192, 104);
            cmdMinus100.Name = "cmdMinus100";
            cmdMinus100.Size = new Size(81, 25);
            cmdMinus100.TabIndex = 11;
            cmdMinus100.Text = "-100";
            cmdMinus100.Click += cmdMinus100_Click;
            // 
            // cmdMinus1000
            // 
            cmdMinus1000.Location = new Point(280, 104);
            cmdMinus1000.Name = "cmdMinus1000";
            cmdMinus1000.Size = new Size(73, 25);
            cmdMinus1000.TabIndex = 10;
            cmdMinus1000.Text = "-1000";
            cmdMinus1000.Click += cmdMinus1000_Click;
            // 
            // cmdPlus1000
            // 
            cmdPlus1000.Location = new Point(280, 80);
            cmdPlus1000.Name = "cmdPlus1000";
            cmdPlus1000.Size = new Size(73, 25);
            cmdPlus1000.TabIndex = 9;
            cmdPlus1000.Text = "+1000";
            cmdPlus1000.Click += cmdPlus1000_Click;
            // 
            // cmdPlus100
            // 
            cmdPlus100.Location = new Point(192, 80);
            cmdPlus100.Name = "cmdPlus100";
            cmdPlus100.Size = new Size(81, 25);
            cmdPlus100.TabIndex = 8;
            cmdPlus100.Text = "+100";
            cmdPlus100.Click += cmdPlus100_Click;
            // 
            // cmdPlus10
            // 
            cmdPlus10.Location = new Point(96, 80);
            cmdPlus10.Name = "cmdPlus10";
            cmdPlus10.Size = new Size(81, 25);
            cmdPlus10.TabIndex = 7;
            cmdPlus10.Text = "+10";
            cmdPlus10.Click += cmdPlus10_Click;
            // 
            // cmdPlus1
            // 
            cmdPlus1.Location = new Point(8, 80);
            cmdPlus1.Name = "cmdPlus1";
            cmdPlus1.Size = new Size(81, 25);
            cmdPlus1.TabIndex = 6;
            cmdPlus1.Text = "+1";
            cmdPlus1.Click += cmdPlus1_Click;
            // 
            // cmdOk
            // 
            cmdOk.Location = new Point(8, 152);
            cmdOk.Name = "cmdOk";
            cmdOk.Size = new Size(169, 33);
            cmdOk.TabIndex = 1;
            cmdOk.Text = "Ok";
            cmdOk.Click += cmdOk_Click;
            // 
            // cmdCancel
            // 
            cmdCancel.Location = new Point(184, 152);
            cmdCancel.Name = "cmdCancel";
            cmdCancel.Size = new Size(169, 33);
            cmdCancel.TabIndex = 0;
            cmdCancel.Text = "Cancel";
            cmdCancel.Click += cmdCancel_Click;
            // 
            // lblAmount
            // 
            lblAmount.Location = new Point(88, 40);
            lblAmount.Name = "lblAmount";
            lblAmount.Size = new Size(257, 25);
            lblAmount.TabIndex = 5;
            lblAmount.Text = "1";
            // 
            // Label2
            // 
            Label2.Location = new Point(8, 40);
            Label2.Name = "Label2";
            Label2.Size = new Size(73, 25);
            Label2.TabIndex = 4;
            Label2.Text = "Ammount";
            // 
            // Label1
            // 
            Label1.Location = new Point(8, 8);
            Label1.Name = "Label1";
            Label1.Size = new Size(73, 25);
            Label1.TabIndex = 3;
            Label1.Text = "Item";
            // 
            // lblName
            // 
            lblName.Location = new Point(88, 8);
            lblName.Name = "lblName";
            lblName.Size = new Size(257, 25);
            lblName.TabIndex = 2;
            // 
            // frmDrop
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(361, 194);
            ControlBox = false;
            Controls.Add(cmdMinus1);
            Controls.Add(cmdMinus10);
            Controls.Add(cmdMinus100);
            Controls.Add(cmdMinus1000);
            Controls.Add(cmdPlus1000);
            Controls.Add(cmdPlus100);
            Controls.Add(cmdPlus10);
            Controls.Add(cmdPlus1);
            Controls.Add(cmdOk);
            Controls.Add(cmdCancel);
            Controls.Add(lblAmount);
            Controls.Add(Label2);
            Controls.Add(Label1);
            Controls.Add(lblName);
            Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmDrop";
            StartPosition = FormStartPosition.CenterScreen;
            Text = " ";
            Load += frmDrop_Load;
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button cmdMinus1;
		private System.Windows.Forms.Button cmdMinus10;
		private System.Windows.Forms.Button cmdMinus100;
		private System.Windows.Forms.Button cmdMinus1000;
		private System.Windows.Forms.Button cmdPlus1000;
		private System.Windows.Forms.Button cmdPlus100;
		private System.Windows.Forms.Button cmdPlus10;
		private System.Windows.Forms.Button cmdPlus1;
		private System.Windows.Forms.Button cmdOk;
		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.Label lblAmount;
		private System.Windows.Forms.Label Label2;
		private System.Windows.Forms.Label Label1;
		private System.Windows.Forms.Label lblName;
	}
}
