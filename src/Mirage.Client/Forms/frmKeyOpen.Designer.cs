namespace Mirage.Client.Forms
{
	partial class frmKeyOpen
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
            scrlY = new HScrollBar();
            scrlX = new HScrollBar();
            lblY = new Label();
            lblX = new Label();
            Label2 = new Label();
            Label1 = new Label();
            SuspendLayout();
            // 
            // cmdCancel
            // 
            cmdCancel.Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            cmdCancel.Location = new Point(168, 88);
            cmdCancel.Name = "cmdCancel";
            cmdCancel.Size = new Size(145, 33);
            cmdCancel.TabIndex = 5;
            cmdCancel.Text = "Cancel";
            cmdCancel.Click += cmdCancel_Click;
            // 
            // cmdOk
            // 
            cmdOk.Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            cmdOk.Location = new Point(8, 88);
            cmdOk.Name = "cmdOk";
            cmdOk.Size = new Size(145, 33);
            cmdOk.TabIndex = 4;
            cmdOk.Text = "Ok";
            cmdOk.Click += cmdOk_Click;
            // 
            // scrlY
            // 
            scrlY.Location = new Point(32, 48);
            scrlY.Maximum = 11;
            scrlY.Name = "scrlY";
            scrlY.Size = new Size(249, 25);
            scrlY.TabIndex = 3;
            scrlY.Scroll += scrlY_Scroll;
            // 
            // scrlX
            // 
            scrlX.Location = new Point(32, 16);
            scrlX.Maximum = 15;
            scrlX.Name = "scrlX";
            scrlX.Size = new Size(249, 25);
            scrlX.TabIndex = 2;
            scrlX.Scroll += scrlX_Scroll;
            // 
            // lblY
            // 
            lblY.Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblY.Location = new Point(288, 48);
            lblY.Name = "lblY";
            lblY.Size = new Size(25, 25);
            lblY.TabIndex = 7;
            lblY.Text = "0";
            lblY.TextAlign = ContentAlignment.TopRight;
            // 
            // lblX
            // 
            lblX.Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblX.Location = new Point(288, 16);
            lblX.Name = "lblX";
            lblX.Size = new Size(25, 25);
            lblX.TabIndex = 6;
            lblX.Text = "0";
            lblX.TextAlign = ContentAlignment.TopRight;
            // 
            // Label2
            // 
            Label2.Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Label2.Location = new Point(8, 48);
            Label2.Name = "Label2";
            Label2.Size = new Size(17, 25);
            Label2.TabIndex = 1;
            Label2.Text = "Y";
            // 
            // Label1
            // 
            Label1.Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Label1.Location = new Point(8, 16);
            Label1.Name = "Label1";
            Label1.Size = new Size(17, 25);
            Label1.TabIndex = 0;
            Label1.Text = "X";
            // 
            // frmKeyOpen
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(321, 130);
            ControlBox = false;
            Controls.Add(cmdCancel);
            Controls.Add(cmdOk);
            Controls.Add(scrlY);
            Controls.Add(scrlX);
            Controls.Add(lblY);
            Controls.Add(lblX);
            Controls.Add(Label2);
            Controls.Add(Label1);
            Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmKeyOpen";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Key Open";
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.Button cmdOk;
		private System.Windows.Forms.HScrollBar scrlY;
		private System.Windows.Forms.HScrollBar scrlX;
		private System.Windows.Forms.Label lblY;
		private System.Windows.Forms.Label lblX;
		private System.Windows.Forms.Label Label2;
		private System.Windows.Forms.Label Label1;
	}
}
