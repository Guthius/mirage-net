namespace Mirage.Client.Forms
{
	partial class frmNpcEditor
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
            txtSpawnSecs = new TextBox();
            txtAttackSay = new TextBox();
            cmdCancel = new Button();
            cmdOk = new Button();
            scrlValue = new HScrollBar();
            scrlNum = new HScrollBar();
            txtChance = new TextBox();
            scrlMAGI = new HScrollBar();
            scrlSPEED = new HScrollBar();
            scrlDEF = new HScrollBar();
            scrlSTR = new HScrollBar();
            scrlRange = new HScrollBar();
            cmbBehavior = new ComboBox();
            txtName = new TextBox();
            scrlSprite = new HScrollBar();
            picSprite = new PictureBox();
            Label16 = new Label();
            Label14 = new Label();
            lblExpGiven = new Label();
            Label15 = new Label();
            lblStartHP = new Label();
            Label13 = new Label();
            lblValue = new Label();
            Label7 = new Label();
            lblItemName = new Label();
            Label11 = new Label();
            Label9 = new Label();
            lblNum = new Label();
            Label3 = new Label();
            Label12 = new Label();
            lblMAGI = new Label();
            Label10 = new Label();
            lblSPEED = new Label();
            Label8 = new Label();
            lblDEF = new Label();
            Label6 = new Label();
            lblSTR = new Label();
            Label4 = new Label();
            lblRange = new Label();
            Label2 = new Label();
            Label1 = new Label();
            Label5 = new Label();
            lblSprite = new Label();
            ((System.ComponentModel.ISupportInitialize)picSprite).BeginInit();
            SuspendLayout();
            // 
            // txtSpawnSecs
            // 
            txtSpawnSecs.Location = new Point(216, 352);
            txtSpawnSecs.Name = "txtSpawnSecs";
            txtSpawnSecs.Size = new Size(121, 26);
            txtSpawnSecs.TabIndex = 43;
            txtSpawnSecs.Text = "0";
            txtSpawnSecs.TextAlign = HorizontalAlignment.Right;
            // 
            // txtAttackSay
            // 
            txtAttackSay.Location = new Point(64, 40);
            txtAttackSay.Name = "txtAttackSay";
            txtAttackSay.Size = new Size(265, 26);
            txtAttackSay.TabIndex = 41;
            // 
            // cmdCancel
            // 
            cmdCancel.Location = new Point(176, 544);
            cmdCancel.Name = "cmdCancel";
            cmdCancel.Size = new Size(161, 41);
            cmdCancel.TabIndex = 34;
            cmdCancel.Text = "Cancel";
            cmdCancel.Click += cmdCancel_Click;
            // 
            // cmdOk
            // 
            cmdOk.Location = new Point(8, 544);
            cmdOk.Name = "cmdOk";
            cmdOk.Size = new Size(161, 41);
            cmdOk.TabIndex = 33;
            cmdOk.Text = "Ok";
            cmdOk.Click += cmdOk_Click;
            // 
            // scrlValue
            // 
            scrlValue.Location = new Point(72, 464);
            scrlValue.Maximum = 255;
            scrlValue.Name = "scrlValue";
            scrlValue.Size = new Size(225, 25);
            scrlValue.TabIndex = 31;
            scrlValue.Value = 1;
            scrlValue.Scroll += scrlValue_Scroll;
            // 
            // scrlNum
            // 
            scrlNum.Location = new Point(72, 432);
            scrlNum.Maximum = 255;
            scrlNum.Name = "scrlNum";
            scrlNum.Size = new Size(225, 25);
            scrlNum.TabIndex = 25;
            scrlNum.Value = 1;
            scrlNum.Scroll += scrlNum_Scroll;
            // 
            // txtChance
            // 
            txtChance.Location = new Point(216, 320);
            txtChance.Name = "txtChance";
            txtChance.Size = new Size(121, 26);
            txtChance.TabIndex = 24;
            txtChance.Text = "0";
            txtChance.TextAlign = HorizontalAlignment.Right;
            // 
            // scrlMAGI
            // 
            scrlMAGI.Location = new Point(72, 240);
            scrlMAGI.Maximum = 255;
            scrlMAGI.Name = "scrlMAGI";
            scrlMAGI.Size = new Size(193, 25);
            scrlMAGI.TabIndex = 20;
            scrlMAGI.Scroll += scrlMAGI_Scroll;
            // 
            // scrlSPEED
            // 
            scrlSPEED.Location = new Point(72, 208);
            scrlSPEED.Maximum = 255;
            scrlSPEED.Name = "scrlSPEED";
            scrlSPEED.Size = new Size(193, 25);
            scrlSPEED.TabIndex = 17;
            scrlSPEED.Scroll += scrlSPEED_Scroll;
            // 
            // scrlDEF
            // 
            scrlDEF.Location = new Point(72, 176);
            scrlDEF.Maximum = 255;
            scrlDEF.Name = "scrlDEF";
            scrlDEF.Size = new Size(193, 25);
            scrlDEF.TabIndex = 14;
            scrlDEF.Scroll += scrlDEF_Scroll;
            // 
            // scrlSTR
            // 
            scrlSTR.Location = new Point(72, 144);
            scrlSTR.Maximum = 255;
            scrlSTR.Name = "scrlSTR";
            scrlSTR.Size = new Size(193, 25);
            scrlSTR.TabIndex = 11;
            scrlSTR.Scroll += scrlSTR_Scroll;
            // 
            // scrlRange
            // 
            scrlRange.Location = new Point(72, 112);
            scrlRange.Maximum = 255;
            scrlRange.Name = "scrlRange";
            scrlRange.Size = new Size(193, 25);
            scrlRange.TabIndex = 8;
            scrlRange.Value = 1;
            scrlRange.Scroll += scrlRange_Scroll;
            // 
            // cmbBehavior
            // 
            cmbBehavior.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbBehavior.FormattingEnabled = true;
            cmbBehavior.Items.AddRange(new object[] { "Attack on sight", "Attack when attacked", "Friendly", "Shop keeper", "Guard" });
            cmbBehavior.Location = new Point(96, 280);
            cmbBehavior.Name = "cmbBehavior";
            cmbBehavior.Size = new Size(241, 26);
            cmbBehavior.TabIndex = 7;
            // 
            // txtName
            // 
            txtName.Location = new Point(64, 8);
            txtName.Name = "txtName";
            txtName.Size = new Size(265, 26);
            txtName.TabIndex = 2;
            // 
            // scrlSprite
            // 
            scrlSprite.Location = new Point(72, 80);
            scrlSprite.Maximum = 255;
            scrlSprite.Name = "scrlSprite";
            scrlSprite.Size = new Size(193, 25);
            scrlSprite.TabIndex = 1;
            scrlSprite.Scroll += scrlSprite_Scroll;
            // 
            // picSprite
            // 
            picSprite.BackColor = Color.FromArgb(0, 0, 0);
            picSprite.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            picSprite.Location = new Point(304, 80);
            picSprite.Name = "picSprite";
            picSprite.Size = new Size(32, 32);
            picSprite.TabIndex = 0;
            picSprite.TabStop = false;
            // 
            // Label16
            // 
            Label16.Location = new Point(16, 352);
            Label16.Name = "Label16";
            Label16.Size = new Size(193, 25);
            Label16.TabIndex = 42;
            Label16.Text = "Spawn Rate (in seconds)";
            // 
            // Label14
            // 
            Label14.Location = new Point(8, 40);
            Label14.Name = "Label14";
            Label14.Size = new Size(49, 25);
            Label14.TabIndex = 40;
            Label14.Text = "Say";
            // 
            // lblExpGiven
            // 
            lblExpGiven.Location = new Point(264, 504);
            lblExpGiven.Name = "lblExpGiven";
            lblExpGiven.Size = new Size(73, 25);
            lblExpGiven.TabIndex = 39;
            lblExpGiven.Text = "0";
            lblExpGiven.TextAlign = ContentAlignment.TopRight;
            // 
            // Label15
            // 
            Label15.Location = new Point(176, 504);
            Label15.Name = "Label15";
            Label15.Size = new Size(81, 25);
            Label15.TabIndex = 38;
            Label15.Text = "Exp Given";
            // 
            // lblStartHP
            // 
            lblStartHP.Location = new Point(88, 504);
            lblStartHP.Name = "lblStartHP";
            lblStartHP.Size = new Size(73, 25);
            lblStartHP.TabIndex = 37;
            lblStartHP.Text = "0";
            lblStartHP.TextAlign = ContentAlignment.TopRight;
            // 
            // Label13
            // 
            Label13.Location = new Point(16, 504);
            Label13.Name = "Label13";
            Label13.Size = new Size(65, 25);
            Label13.TabIndex = 36;
            Label13.Text = "Start HP";
            // 
            // lblValue
            // 
            lblValue.Location = new Point(304, 464);
            lblValue.Name = "lblValue";
            lblValue.Size = new Size(33, 25);
            lblValue.TabIndex = 32;
            lblValue.Text = "0";
            lblValue.TextAlign = ContentAlignment.TopRight;
            // 
            // Label7
            // 
            Label7.Location = new Point(16, 464);
            Label7.Name = "Label7";
            Label7.Size = new Size(49, 25);
            Label7.TabIndex = 30;
            Label7.Text = "Value";
            // 
            // lblItemName
            // 
            lblItemName.Location = new Point(64, 400);
            lblItemName.Name = "lblItemName";
            lblItemName.Size = new Size(273, 25);
            lblItemName.TabIndex = 29;
            // 
            // Label11
            // 
            Label11.Location = new Point(16, 400);
            Label11.Name = "Label11";
            Label11.Size = new Size(41, 25);
            Label11.TabIndex = 28;
            Label11.Text = "Item";
            // 
            // Label9
            // 
            Label9.Location = new Point(16, 432);
            Label9.Name = "Label9";
            Label9.Size = new Size(57, 25);
            Label9.TabIndex = 27;
            Label9.Text = "Num";
            // 
            // lblNum
            // 
            lblNum.Location = new Point(304, 432);
            lblNum.Name = "lblNum";
            lblNum.Size = new Size(33, 25);
            lblNum.TabIndex = 26;
            lblNum.Text = "0";
            lblNum.TextAlign = ContentAlignment.TopRight;
            // 
            // Label3
            // 
            Label3.Location = new Point(16, 320);
            Label3.Name = "Label3";
            Label3.Size = new Size(193, 25);
            Label3.TabIndex = 23;
            Label3.Text = "Drop Item Chance 1 out of";
            // 
            // Label12
            // 
            Label12.Location = new Point(16, 240);
            Label12.Name = "Label12";
            Label12.Size = new Size(57, 25);
            Label12.TabIndex = 22;
            Label12.Text = "MAGI";
            // 
            // lblMAGI
            // 
            lblMAGI.Location = new Point(264, 240);
            lblMAGI.Name = "lblMAGI";
            lblMAGI.Size = new Size(33, 25);
            lblMAGI.TabIndex = 21;
            lblMAGI.Text = "0";
            lblMAGI.TextAlign = ContentAlignment.TopRight;
            // 
            // Label10
            // 
            Label10.Location = new Point(16, 208);
            Label10.Name = "Label10";
            Label10.Size = new Size(57, 25);
            Label10.TabIndex = 19;
            Label10.Text = "SPD";
            // 
            // lblSPEED
            // 
            lblSPEED.Location = new Point(264, 208);
            lblSPEED.Name = "lblSPEED";
            lblSPEED.Size = new Size(33, 25);
            lblSPEED.TabIndex = 18;
            lblSPEED.Text = "0";
            lblSPEED.TextAlign = ContentAlignment.TopRight;
            // 
            // Label8
            // 
            Label8.Location = new Point(16, 176);
            Label8.Name = "Label8";
            Label8.Size = new Size(57, 25);
            Label8.TabIndex = 16;
            Label8.Text = "DEF";
            // 
            // lblDEF
            // 
            lblDEF.Location = new Point(264, 176);
            lblDEF.Name = "lblDEF";
            lblDEF.Size = new Size(33, 25);
            lblDEF.TabIndex = 15;
            lblDEF.Text = "0";
            lblDEF.TextAlign = ContentAlignment.TopRight;
            // 
            // Label6
            // 
            Label6.Location = new Point(16, 144);
            Label6.Name = "Label6";
            Label6.Size = new Size(57, 25);
            Label6.TabIndex = 13;
            Label6.Text = "STR";
            // 
            // lblSTR
            // 
            lblSTR.Location = new Point(264, 144);
            lblSTR.Name = "lblSTR";
            lblSTR.Size = new Size(33, 25);
            lblSTR.TabIndex = 12;
            lblSTR.Text = "0";
            lblSTR.TextAlign = ContentAlignment.TopRight;
            // 
            // Label4
            // 
            Label4.Location = new Point(16, 112);
            Label4.Name = "Label4";
            Label4.Size = new Size(57, 25);
            Label4.TabIndex = 10;
            Label4.Text = "Range";
            // 
            // lblRange
            // 
            lblRange.Location = new Point(264, 112);
            lblRange.Name = "lblRange";
            lblRange.Size = new Size(33, 25);
            lblRange.TabIndex = 9;
            lblRange.Text = "0";
            lblRange.TextAlign = ContentAlignment.TopRight;
            // 
            // Label2
            // 
            Label2.Location = new Point(16, 280);
            Label2.Name = "Label2";
            Label2.Size = new Size(73, 25);
            Label2.TabIndex = 6;
            Label2.Text = "Behavior";
            // 
            // Label1
            // 
            Label1.Location = new Point(8, 8);
            Label1.Name = "Label1";
            Label1.Size = new Size(49, 25);
            Label1.TabIndex = 5;
            Label1.Text = "Name";
            // 
            // Label5
            // 
            Label5.Location = new Point(16, 80);
            Label5.Name = "Label5";
            Label5.Size = new Size(57, 25);
            Label5.TabIndex = 4;
            Label5.Text = "Sprite";
            // 
            // lblSprite
            // 
            lblSprite.Location = new Point(264, 80);
            lblSprite.Name = "lblSprite";
            lblSprite.Size = new Size(33, 25);
            lblSprite.TabIndex = 3;
            lblSprite.Text = "0";
            lblSprite.TextAlign = ContentAlignment.TopRight;
            // 
            // frmNpcEditor
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(345, 594);
            ControlBox = false;
            Controls.Add(txtSpawnSecs);
            Controls.Add(txtAttackSay);
            Controls.Add(cmdCancel);
            Controls.Add(cmdOk);
            Controls.Add(scrlValue);
            Controls.Add(scrlNum);
            Controls.Add(txtChance);
            Controls.Add(scrlMAGI);
            Controls.Add(scrlSPEED);
            Controls.Add(scrlDEF);
            Controls.Add(scrlSTR);
            Controls.Add(scrlRange);
            Controls.Add(cmbBehavior);
            Controls.Add(txtName);
            Controls.Add(scrlSprite);
            Controls.Add(picSprite);
            Controls.Add(Label16);
            Controls.Add(Label14);
            Controls.Add(lblExpGiven);
            Controls.Add(Label15);
            Controls.Add(lblStartHP);
            Controls.Add(Label13);
            Controls.Add(lblValue);
            Controls.Add(Label7);
            Controls.Add(lblItemName);
            Controls.Add(Label11);
            Controls.Add(Label9);
            Controls.Add(lblNum);
            Controls.Add(Label3);
            Controls.Add(Label12);
            Controls.Add(lblMAGI);
            Controls.Add(Label10);
            Controls.Add(lblSPEED);
            Controls.Add(Label8);
            Controls.Add(lblDEF);
            Controls.Add(Label6);
            Controls.Add(lblSTR);
            Controls.Add(Label4);
            Controls.Add(lblRange);
            Controls.Add(Label2);
            Controls.Add(Label1);
            Controls.Add(Label5);
            Controls.Add(lblSprite);
            Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmNpcEditor";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Npc Editor";
            Load += frmNpcEditor_Load;
            ((System.ComponentModel.ISupportInitialize)picSprite).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox txtSpawnSecs;
        private System.Windows.Forms.TextBox txtAttackSay;
		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.Button cmdOk;
		private System.Windows.Forms.HScrollBar scrlValue;
		private System.Windows.Forms.HScrollBar scrlNum;
		private System.Windows.Forms.TextBox txtChance;
		private System.Windows.Forms.HScrollBar scrlMAGI;
		private System.Windows.Forms.HScrollBar scrlSPEED;
		private System.Windows.Forms.HScrollBar scrlDEF;
		private System.Windows.Forms.HScrollBar scrlSTR;
		private System.Windows.Forms.HScrollBar scrlRange;
		private System.Windows.Forms.ComboBox cmbBehavior;
		private System.Windows.Forms.TextBox txtName;
		private System.Windows.Forms.HScrollBar scrlSprite;
		private System.Windows.Forms.PictureBox picSprite;
		private System.Windows.Forms.Label Label16;
		private System.Windows.Forms.Label Label14;
		private System.Windows.Forms.Label lblExpGiven;
		private System.Windows.Forms.Label Label15;
		private System.Windows.Forms.Label lblStartHP;
		private System.Windows.Forms.Label Label13;
		private System.Windows.Forms.Label lblValue;
		private System.Windows.Forms.Label Label7;
		private System.Windows.Forms.Label lblItemName;
		private System.Windows.Forms.Label Label11;
		private System.Windows.Forms.Label Label9;
		private System.Windows.Forms.Label lblNum;
		private System.Windows.Forms.Label Label3;
		private System.Windows.Forms.Label Label12;
		private System.Windows.Forms.Label lblMAGI;
		private System.Windows.Forms.Label Label10;
		private System.Windows.Forms.Label lblSPEED;
		private System.Windows.Forms.Label Label8;
		private System.Windows.Forms.Label lblDEF;
		private System.Windows.Forms.Label Label6;
		private System.Windows.Forms.Label lblSTR;
		private System.Windows.Forms.Label Label4;
		private System.Windows.Forms.Label lblRange;
		private System.Windows.Forms.Label Label2;
		private System.Windows.Forms.Label Label1;
		private System.Windows.Forms.Label Label5;
		private System.Windows.Forms.Label lblSprite;
	}
}
