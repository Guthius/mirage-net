namespace Mirage.Client.Forms
{
	partial class frmMapProperties
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
            cmbShop = new ComboBox();
            scrlMusic = new HScrollBar();
            txtBootY = new TextBox();
            txtBootX = new TextBox();
            txtBootMap = new TextBox();
            cmbNpc_4 = new ComboBox();
            cmbNpc_3 = new ComboBox();
            cmbNpc_2 = new ComboBox();
            cmbNpc_1 = new ComboBox();
            cmbNpc_0 = new ComboBox();
            cmbMoral = new ComboBox();
            cmdCancel = new Button();
            cmdOk = new Button();
            txtLeft = new TextBox();
            txtRight = new TextBox();
            txtDown = new TextBox();
            txtUp = new TextBox();
            txtName = new TextBox();
            Label12 = new Label();
            Label11 = new Label();
            lblMusic = new Label();
            Label10 = new Label();
            Label9 = new Label();
            Label8 = new Label();
            Label7 = new Label();
            Label6 = new Label();
            Label5 = new Label();
            Label4 = new Label();
            Label3 = new Label();
            Label2 = new Label();
            Label1 = new Label();
            SuspendLayout();
            // 
            // cmbShop
            // 
            cmbShop.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbShop.Location = new Point(64, 152);
            cmbShop.Name = "cmbShop";
            cmbShop.Size = new Size(161, 26);
            cmbShop.TabIndex = 30;
            // 
            // scrlMusic
            // 
            scrlMusic.Location = new Point(64, 192);
            scrlMusic.Maximum = 255;
            scrlMusic.Name = "scrlMusic";
            scrlMusic.Size = new Size(161, 25);
            scrlMusic.TabIndex = 26;
            scrlMusic.Value = 1;
            scrlMusic.Scroll += scrlMusic_Scroll;
            // 
            // txtBootY
            // 
            txtBootY.Location = new Point(416, 232);
            txtBootY.Name = "txtBootY";
            txtBootY.Size = new Size(49, 26);
            txtBootY.TabIndex = 24;
            txtBootY.Text = "0";
            txtBootY.TextAlign = HorizontalAlignment.Right;
            // 
            // txtBootX
            // 
            txtBootX.Location = new Point(248, 232);
            txtBootX.Name = "txtBootX";
            txtBootX.Size = new Size(49, 26);
            txtBootX.TabIndex = 23;
            txtBootX.Text = "0";
            txtBootX.TextAlign = HorizontalAlignment.Right;
            // 
            // txtBootMap
            // 
            txtBootMap.Location = new Point(88, 232);
            txtBootMap.Name = "txtBootMap";
            txtBootMap.Size = new Size(49, 26);
            txtBootMap.TabIndex = 20;
            txtBootMap.Text = "0";
            txtBootMap.TextAlign = HorizontalAlignment.Right;
            // 
            // cmbNpc_4
            // 
            cmbNpc_4.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbNpc_4.Location = new Point(280, 160);
            cmbNpc_4.Name = "cmbNpc_4";
            cmbNpc_4.Size = new Size(273, 26);
            cmbNpc_4.TabIndex = 18;
            // 
            // cmbNpc_3
            // 
            cmbNpc_3.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbNpc_3.FormattingEnabled = true;
            cmbNpc_3.Location = new Point(280, 128);
            cmbNpc_3.Name = "cmbNpc_3";
            cmbNpc_3.Size = new Size(273, 26);
            cmbNpc_3.TabIndex = 17;
            // 
            // cmbNpc_2
            // 
            cmbNpc_2.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbNpc_2.Location = new Point(280, 96);
            cmbNpc_2.Name = "cmbNpc_2";
            cmbNpc_2.Size = new Size(273, 26);
            cmbNpc_2.TabIndex = 16;
            // 
            // cmbNpc_1
            // 
            cmbNpc_1.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbNpc_1.Location = new Point(280, 64);
            cmbNpc_1.Name = "cmbNpc_1";
            cmbNpc_1.Size = new Size(273, 26);
            cmbNpc_1.TabIndex = 15;
            // 
            // cmbNpc_0
            // 
            cmbNpc_0.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbNpc_0.Location = new Point(280, 32);
            cmbNpc_0.Name = "cmbNpc_0";
            cmbNpc_0.Size = new Size(273, 26);
            cmbNpc_0.TabIndex = 14;
            // 
            // cmbMoral
            // 
            cmbMoral.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMoral.FormattingEnabled = true;
            cmbMoral.Items.AddRange(new object[] { "None", "Safe Zone" });
            cmbMoral.Location = new Point(64, 120);
            cmbMoral.Name = "cmbMoral";
            cmbMoral.Size = new Size(161, 26);
            cmbMoral.TabIndex = 13;
            // 
            // cmdCancel
            // 
            cmdCancel.Location = new Point(288, 272);
            cmdCancel.Name = "cmdCancel";
            cmdCancel.Size = new Size(265, 33);
            cmdCancel.TabIndex = 6;
            cmdCancel.Text = "Cancel";
            cmdCancel.Click += cmdCancel_Click;
            // 
            // cmdOk
            // 
            cmdOk.Location = new Point(8, 272);
            cmdOk.Name = "cmdOk";
            cmdOk.Size = new Size(265, 33);
            cmdOk.TabIndex = 5;
            cmdOk.Text = "Ok";
            cmdOk.Click += cmdOk_Click;
            // 
            // txtLeft
            // 
            txtLeft.Location = new Point(176, 48);
            txtLeft.Name = "txtLeft";
            txtLeft.Size = new Size(49, 26);
            txtLeft.TabIndex = 3;
            txtLeft.Text = "0";
            txtLeft.TextAlign = HorizontalAlignment.Right;
            // 
            // txtRight
            // 
            txtRight.Location = new Point(176, 80);
            txtRight.Name = "txtRight";
            txtRight.Size = new Size(49, 26);
            txtRight.TabIndex = 4;
            txtRight.Text = "0";
            txtRight.TextAlign = HorizontalAlignment.Right;
            // 
            // txtDown
            // 
            txtDown.Location = new Point(64, 80);
            txtDown.Name = "txtDown";
            txtDown.Size = new Size(49, 26);
            txtDown.TabIndex = 2;
            txtDown.Text = "0";
            txtDown.TextAlign = HorizontalAlignment.Right;
            // 
            // txtUp
            // 
            txtUp.Location = new Point(64, 48);
            txtUp.Name = "txtUp";
            txtUp.Size = new Size(49, 26);
            txtUp.TabIndex = 1;
            txtUp.Text = "0";
            txtUp.TextAlign = HorizontalAlignment.Right;
            // 
            // txtName
            // 
            txtName.Location = new Point(64, 8);
            txtName.Name = "txtName";
            txtName.Size = new Size(201, 26);
            txtName.TabIndex = 0;
            // 
            // Label12
            // 
            Label12.Location = new Point(8, 152);
            Label12.Name = "Label12";
            Label12.Size = new Size(49, 25);
            Label12.TabIndex = 29;
            Label12.Text = "Shop";
            // 
            // Label11
            // 
            Label11.Location = new Point(280, 8);
            Label11.Name = "Label11";
            Label11.Size = new Size(273, 25);
            Label11.TabIndex = 28;
            Label11.Text = "NPC's";
            Label11.TextAlign = ContentAlignment.TopCenter;
            // 
            // lblMusic
            // 
            lblMusic.Location = new Point(232, 192);
            lblMusic.Name = "lblMusic";
            lblMusic.Size = new Size(33, 25);
            lblMusic.TabIndex = 27;
            lblMusic.Text = "0";
            lblMusic.TextAlign = ContentAlignment.TopRight;
            // 
            // Label10
            // 
            Label10.Location = new Point(8, 192);
            Label10.Name = "Label10";
            Label10.Size = new Size(49, 25);
            Label10.TabIndex = 25;
            Label10.Text = "Music";
            // 
            // Label9
            // 
            Label9.Location = new Point(336, 232);
            Label9.Name = "Label9";
            Label9.Size = new Size(73, 25);
            Label9.TabIndex = 22;
            Label9.Text = "Boot Y";
            // 
            // Label8
            // 
            Label8.Location = new Point(168, 232);
            Label8.Name = "Label8";
            Label8.Size = new Size(73, 25);
            Label8.TabIndex = 21;
            Label8.Text = "Boot X";
            // 
            // Label7
            // 
            Label7.Location = new Point(8, 232);
            Label7.Name = "Label7";
            Label7.Size = new Size(73, 25);
            Label7.TabIndex = 19;
            Label7.Text = "Boot Map";
            // 
            // Label6
            // 
            Label6.Location = new Point(8, 120);
            Label6.Name = "Label6";
            Label6.Size = new Size(49, 25);
            Label6.TabIndex = 12;
            Label6.Text = "Moral";
            // 
            // Label5
            // 
            Label5.Location = new Point(128, 80);
            Label5.Name = "Label5";
            Label5.Size = new Size(49, 25);
            Label5.TabIndex = 11;
            Label5.Text = "Right";
            // 
            // Label4
            // 
            Label4.Location = new Point(128, 48);
            Label4.Name = "Label4";
            Label4.Size = new Size(49, 25);
            Label4.TabIndex = 10;
            Label4.Text = "Left";
            // 
            // Label3
            // 
            Label3.Location = new Point(8, 80);
            Label3.Name = "Label3";
            Label3.Size = new Size(49, 25);
            Label3.TabIndex = 9;
            Label3.Text = "Down";
            // 
            // Label2
            // 
            Label2.Location = new Point(8, 48);
            Label2.Name = "Label2";
            Label2.Size = new Size(49, 25);
            Label2.TabIndex = 8;
            Label2.Text = "Up";
            // 
            // Label1
            // 
            Label1.Location = new Point(8, 8);
            Label1.Name = "Label1";
            Label1.Size = new Size(49, 25);
            Label1.TabIndex = 7;
            Label1.Text = "Name";
            // 
            // frmMapProperties
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(563, 317);
            ControlBox = false;
            Controls.Add(cmbShop);
            Controls.Add(scrlMusic);
            Controls.Add(txtBootY);
            Controls.Add(txtBootX);
            Controls.Add(txtBootMap);
            Controls.Add(cmbNpc_4);
            Controls.Add(cmbNpc_3);
            Controls.Add(cmbNpc_2);
            Controls.Add(cmbNpc_1);
            Controls.Add(cmbNpc_0);
            Controls.Add(cmbMoral);
            Controls.Add(cmdCancel);
            Controls.Add(cmdOk);
            Controls.Add(txtLeft);
            Controls.Add(txtRight);
            Controls.Add(txtDown);
            Controls.Add(txtUp);
            Controls.Add(txtName);
            Controls.Add(Label12);
            Controls.Add(Label11);
            Controls.Add(lblMusic);
            Controls.Add(Label10);
            Controls.Add(Label9);
            Controls.Add(Label8);
            Controls.Add(Label7);
            Controls.Add(Label6);
            Controls.Add(Label5);
            Controls.Add(Label4);
            Controls.Add(Label3);
            Controls.Add(Label2);
            Controls.Add(Label1);
            Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmMapProperties";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Map Properties";
            Load += frmMapProperties_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ComboBox cmbShop;
		private System.Windows.Forms.HScrollBar scrlMusic;
		private System.Windows.Forms.TextBox txtBootY;
		private System.Windows.Forms.TextBox txtBootX;
		private System.Windows.Forms.TextBox txtBootMap;
		private System.Windows.Forms.ComboBox cmbNpc_4;
		private System.Windows.Forms.ComboBox cmbNpc_3;
		private System.Windows.Forms.ComboBox cmbNpc_2;
		private System.Windows.Forms.ComboBox cmbNpc_1;
		private System.Windows.Forms.ComboBox cmbNpc_0;
		private System.Windows.Forms.ComboBox cmbMoral;
		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.Button cmdOk;
		private System.Windows.Forms.TextBox txtLeft;
		private System.Windows.Forms.TextBox txtRight;
		private System.Windows.Forms.TextBox txtDown;
		private System.Windows.Forms.TextBox txtUp;
		private System.Windows.Forms.TextBox txtName;
		private System.Windows.Forms.Label Label12;
		private System.Windows.Forms.Label Label11;
		private System.Windows.Forms.Label lblMusic;
		private System.Windows.Forms.Label Label10;
		private System.Windows.Forms.Label Label9;
		private System.Windows.Forms.Label Label8;
		private System.Windows.Forms.Label Label7;
		private System.Windows.Forms.Label Label6;
		private System.Windows.Forms.Label Label5;
		private System.Windows.Forms.Label Label4;
		private System.Windows.Forms.Label Label3;
		private System.Windows.Forms.Label Label2;
		private System.Windows.Forms.Label Label1;
	}
}
