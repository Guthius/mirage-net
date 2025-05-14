namespace Mirage.Client.Forms
{
	partial class frmMapItem
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
            cmdCancel = new Button();
            cmdOk = new Button();
            scrlValue = new HScrollBar();
            scrlItem = new HScrollBar();
            lblName = new Label();
            Label1 = new Label();
            lblValue = new Label();
            lblItem = new Label();
            Label3 = new Label();
            Label2 = new Label();
            SuspendLayout();
            // 
            // cmdCancel
            // 
            cmdCancel.Location = new Point(176, 96);
            cmdCancel.Name = "cmdCancel";
            cmdCancel.Size = new Size(153, 33);
            cmdCancel.TabIndex = 3;
            cmdCancel.Text = "Cancel";
            cmdCancel.Click += cmdCancel_Click;
            // 
            // cmdOk
            // 
            cmdOk.Location = new Point(8, 96);
            cmdOk.Name = "cmdOk";
            cmdOk.Size = new Size(153, 33);
            cmdOk.TabIndex = 2;
            cmdOk.Text = "Ok";
            cmdOk.Click += cmdOk_Click;
            // 
            // scrlValue
            // 
            scrlValue.Location = new Point(56, 64);
            scrlValue.Name = "scrlValue";
            scrlValue.Size = new Size(217, 17);
            scrlValue.TabIndex = 1;
            scrlValue.Value = 1;
            scrlValue.Scroll += scrlValue_Scroll;
            // 
            // scrlItem
            // 
            scrlItem.Location = new Point(56, 40);
            scrlItem.Maximum = 255;
            scrlItem.Name = "scrlItem";
            scrlItem.Size = new Size(217, 17);
            scrlItem.TabIndex = 0;
            scrlItem.Value = 1;
            scrlItem.Scroll += scrlItem_Scroll;
            // 
            // lblName
            // 
            lblName.BackColor = Color.Transparent;
            lblName.Location = new Point(56, 8);
            lblName.Name = "lblName";
            lblName.Size = new Size(249, 25);
            lblName.TabIndex = 9;
            // 
            // Label1
            // 
            Label1.Location = new Point(8, 8);
            Label1.Name = "Label1";
            Label1.Size = new Size(41, 17);
            Label1.TabIndex = 8;
            Label1.Text = "Item";
            // 
            // lblValue
            // 
            lblValue.Location = new Point(272, 64);
            lblValue.Name = "lblValue";
            lblValue.Size = new Size(57, 17);
            lblValue.TabIndex = 7;
            lblValue.Text = "0";
            lblValue.TextAlign = ContentAlignment.TopRight;
            // 
            // lblItem
            // 
            lblItem.Location = new Point(272, 40);
            lblItem.Name = "lblItem";
            lblItem.Size = new Size(57, 17);
            lblItem.TabIndex = 6;
            lblItem.Text = "0";
            lblItem.TextAlign = ContentAlignment.TopRight;
            // 
            // Label3
            // 
            Label3.Location = new Point(8, 64);
            Label3.Name = "Label3";
            Label3.Size = new Size(41, 17);
            Label3.TabIndex = 5;
            Label3.Text = "Value";
            // 
            // Label2
            // 
            Label2.Location = new Point(8, 40);
            Label2.Name = "Label2";
            Label2.Size = new Size(41, 17);
            Label2.TabIndex = 4;
            Label2.Text = "Item";
            // 
            // frmMapItem
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(338, 139);
            ControlBox = false;
            Controls.Add(cmdCancel);
            Controls.Add(cmdOk);
            Controls.Add(scrlValue);
            Controls.Add(scrlItem);
            Controls.Add(lblName);
            Controls.Add(Label1);
            Controls.Add(lblValue);
            Controls.Add(lblItem);
            Controls.Add(Label3);
            Controls.Add(Label2);
            Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmMapItem";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Map Item";
            Load += frmMapItem_Load;
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.Button cmdOk;
		private System.Windows.Forms.HScrollBar scrlValue;
		private System.Windows.Forms.HScrollBar scrlItem;
		private System.Windows.Forms.Label lblName;
		private System.Windows.Forms.Label Label1;
		private System.Windows.Forms.Label lblValue;
		private System.Windows.Forms.Label lblItem;
		private System.Windows.Forms.Label Label3;
		private System.Windows.Forms.Label Label2;
	}
}
