namespace Mirage.Client.Forms
{
	partial class frmMapWarp
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
            scrlX = new HScrollBar();
            scrlY = new HScrollBar();
            cmdOk = new Button();
            cmdCancel = new Button();
            txtMap = new TextBox();
            Label1 = new Label();
            Label2 = new Label();
            Label3 = new Label();
            lblX = new Label();
            lblY = new Label();
            SuspendLayout();
            // 
            // scrlX
            // 
            scrlX.Location = new Point(48, 40);
            scrlX.Maximum = 15;
            scrlX.Name = "scrlX";
            scrlX.Size = new Size(217, 17);
            scrlX.TabIndex = 4;
            scrlX.Scroll += scrlX_Scroll;
            // 
            // scrlY
            // 
            scrlY.Location = new Point(48, 64);
            scrlY.Maximum = 11;
            scrlY.Name = "scrlY";
            scrlY.Size = new Size(217, 17);
            scrlY.TabIndex = 3;
            scrlY.Scroll += scrlY_Scroll;
            // 
            // cmdOk
            // 
            cmdOk.Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            cmdOk.Location = new Point(8, 96);
            cmdOk.Name = "cmdOk";
            cmdOk.Size = new Size(137, 33);
            cmdOk.TabIndex = 2;
            cmdOk.Text = "Ok";
            cmdOk.Click += cmdOk_Click;
            // 
            // cmdCancel
            // 
            cmdCancel.Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            cmdCancel.Location = new Point(168, 96);
            cmdCancel.Name = "cmdCancel";
            cmdCancel.Size = new Size(137, 33);
            cmdCancel.TabIndex = 1;
            cmdCancel.Text = "Cancel";
            cmdCancel.Click += cmdCancel_Click;
            // 
            // txtMap
            // 
            txtMap.Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtMap.Location = new Point(48, 8);
            txtMap.Name = "txtMap";
            txtMap.Size = new Size(257, 26);
            txtMap.TabIndex = 0;
            txtMap.Text = "1";
            txtMap.TextAlign = HorizontalAlignment.Right;
            txtMap.TextChanged += txtMap_TextChanged;
            // 
            // Label1
            // 
            Label1.Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Label1.Location = new Point(8, 8);
            Label1.Name = "Label1";
            Label1.Size = new Size(33, 17);
            Label1.TabIndex = 9;
            Label1.Text = "Map";
            // 
            // Label2
            // 
            Label2.Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Label2.Location = new Point(8, 40);
            Label2.Name = "Label2";
            Label2.Size = new Size(33, 17);
            Label2.TabIndex = 8;
            Label2.Text = "X";
            // 
            // Label3
            // 
            Label3.Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Label3.Location = new Point(8, 64);
            Label3.Name = "Label3";
            Label3.Size = new Size(33, 17);
            Label3.TabIndex = 7;
            Label3.Text = "Y";
            // 
            // lblX
            // 
            lblX.Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblX.Location = new Point(272, 40);
            lblX.Name = "lblX";
            lblX.Size = new Size(33, 17);
            lblX.TabIndex = 6;
            lblX.Text = "0";
            lblX.TextAlign = ContentAlignment.TopRight;
            // 
            // lblY
            // 
            lblY.Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblY.Location = new Point(272, 64);
            lblY.Name = "lblY";
            lblY.Size = new Size(33, 17);
            lblY.TabIndex = 5;
            lblY.Text = "0";
            lblY.TextAlign = ContentAlignment.TopRight;
            // 
            // frmMapWarp
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(314, 138);
            ControlBox = false;
            Controls.Add(scrlX);
            Controls.Add(scrlY);
            Controls.Add(cmdOk);
            Controls.Add(cmdCancel);
            Controls.Add(txtMap);
            Controls.Add(Label1);
            Controls.Add(Label2);
            Controls.Add(Label3);
            Controls.Add(lblX);
            Controls.Add(lblY);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmMapWarp";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Map Warp";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.HScrollBar scrlX;
		private System.Windows.Forms.HScrollBar scrlY;
		private System.Windows.Forms.Button cmdOk;
		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.TextBox txtMap;
		private System.Windows.Forms.Label Label1;
		private System.Windows.Forms.Label Label2;
		private System.Windows.Forms.Label Label3;
		private System.Windows.Forms.Label lblX;
		private System.Windows.Forms.Label lblY;
	}
}
