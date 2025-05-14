namespace Mirage.Client.Forms
{
	partial class frmMapKey
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
            chkTake = new CheckBox();
            scrlItem = new HScrollBar();
            cmdOk = new Button();
            cmdCancel = new Button();
            Label2 = new Label();
            lblItem = new Label();
            Label1 = new Label();
            lblName = new Label();
            SuspendLayout();
            // 
            // chkTake
            // 
            chkTake.Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            chkTake.Location = new Point(8, 72);
            chkTake.Name = "chkTake";
            chkTake.Size = new Size(297, 25);
            chkTake.TabIndex = 7;
            chkTake.Text = "Take key away upon use";
            // 
            // scrlItem
            // 
            scrlItem.Location = new Point(56, 40);
            scrlItem.Maximum = 255;
            scrlItem.Name = "scrlItem";
            scrlItem.Size = new Size(217, 17);
            scrlItem.TabIndex = 2;
            scrlItem.Value = 1;
            scrlItem.Scroll += scrlItem_Scroll;
            // 
            // cmdOk
            // 
            cmdOk.Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            cmdOk.Location = new Point(8, 112);
            cmdOk.Name = "cmdOk";
            cmdOk.Size = new Size(145, 33);
            cmdOk.TabIndex = 1;
            cmdOk.Text = "Ok";
            cmdOk.Click += cmdOk_Click;
            // 
            // cmdCancel
            // 
            cmdCancel.Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            cmdCancel.Location = new Point(168, 112);
            cmdCancel.Name = "cmdCancel";
            cmdCancel.Size = new Size(145, 33);
            cmdCancel.TabIndex = 0;
            cmdCancel.Text = "Cancel";
            cmdCancel.Click += cmdCancel_Click;
            // 
            // Label2
            // 
            Label2.Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Label2.Location = new Point(8, 40);
            Label2.Name = "Label2";
            Label2.Size = new Size(41, 17);
            Label2.TabIndex = 6;
            Label2.Text = "Item";
            // 
            // lblItem
            // 
            lblItem.Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblItem.Location = new Point(272, 40);
            lblItem.Name = "lblItem";
            lblItem.Size = new Size(33, 17);
            lblItem.TabIndex = 5;
            lblItem.Text = "1";
            lblItem.TextAlign = ContentAlignment.TopRight;
            // 
            // Label1
            // 
            Label1.Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Label1.Location = new Point(8, 8);
            Label1.Name = "Label1";
            Label1.Size = new Size(41, 17);
            Label1.TabIndex = 4;
            Label1.Text = "Item";
            // 
            // lblName
            // 
            lblName.BackColor = Color.Transparent;
            lblName.Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblName.Location = new Point(56, 8);
            lblName.Name = "lblName";
            lblName.Size = new Size(249, 25);
            lblName.TabIndex = 3;
            // 
            // frmMapKey
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(321, 154);
            ControlBox = false;
            Controls.Add(chkTake);
            Controls.Add(scrlItem);
            Controls.Add(cmdOk);
            Controls.Add(cmdCancel);
            Controls.Add(Label2);
            Controls.Add(lblItem);
            Controls.Add(Label1);
            Controls.Add(lblName);
            Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmMapKey";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Map Key";
            Load += frmMapKey_Load;
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.CheckBox chkTake;
		private System.Windows.Forms.HScrollBar scrlItem;
		private System.Windows.Forms.Button cmdOk;
		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.Label Label2;
		private System.Windows.Forms.Label lblItem;
		private System.Windows.Forms.Label Label1;
		private System.Windows.Forms.Label lblName;
	}
}
