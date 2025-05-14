namespace Mirage.Client.Forms
{
	partial class frmSpellEditor
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
            scrlLevelReq = new HScrollBar();
            cmbClassReq = new ComboBox();
            scrlItemValue = new HScrollBar();
            scrlItemNum = new HScrollBar();
            lblItemValue = new Label();
            Label5 = new Label();
            lblItemNum = new Label();
            Label2 = new Label();
            fraGiveItem = new GroupBox();
            cmdOk = new Button();
            cmdCancel = new Button();
            cmbType = new ComboBox();
            scrlVitalMod = new HScrollBar();
            Label4 = new Label();
            lblVitalMod = new Label();
            fraVitals = new GroupBox();
            txtName = new TextBox();
            lblLevelReq = new Label();
            Label3 = new Label();
            Label1 = new Label();
            fraGiveItem.SuspendLayout();
            fraVitals.SuspendLayout();
            SuspendLayout();
            // 
            // scrlLevelReq
            // 
            scrlLevelReq.Location = new Point(64, 88);
            scrlLevelReq.Maximum = 255;
            scrlLevelReq.Name = "scrlLevelReq";
            scrlLevelReq.Size = new Size(233, 25);
            scrlLevelReq.TabIndex = 18;
            scrlLevelReq.Value = 1;
            scrlLevelReq.Scroll += scrlLevelReq_Scroll;
            // 
            // cmbClassReq
            // 
            cmbClassReq.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbClassReq.FormattingEnabled = true;
            cmbClassReq.Location = new Point(8, 48);
            cmbClassReq.Name = "cmbClassReq";
            cmbClassReq.Size = new Size(321, 26);
            cmbClassReq.TabIndex = 16;
            // 
            // scrlItemValue
            // 
            scrlItemValue.Location = new Point(88, 56);
            scrlItemValue.Name = "scrlItemValue";
            scrlItemValue.Size = new Size(193, 25);
            scrlItemValue.TabIndex = 14;
            scrlItemValue.Scroll += scrlItemValue_Scroll;
            // 
            // scrlItemNum
            // 
            scrlItemNum.Location = new Point(88, 24);
            scrlItemNum.Maximum = 255;
            scrlItemNum.Name = "scrlItemNum";
            scrlItemNum.Size = new Size(193, 25);
            scrlItemNum.TabIndex = 10;
            scrlItemNum.Value = 1;
            scrlItemNum.Scroll += scrlItemNum_Scroll;
            // 
            // lblItemValue
            // 
            lblItemValue.Location = new Point(280, 56);
            lblItemValue.Name = "lblItemValue";
            lblItemValue.Size = new Size(33, 25);
            lblItemValue.TabIndex = 15;
            lblItemValue.Text = "0";
            lblItemValue.TextAlign = ContentAlignment.TopRight;
            // 
            // Label5
            // 
            Label5.Location = new Point(8, 56);
            Label5.Name = "Label5";
            Label5.Size = new Size(73, 25);
            Label5.TabIndex = 13;
            Label5.Text = "Value";
            // 
            // lblItemNum
            // 
            lblItemNum.Location = new Point(280, 24);
            lblItemNum.Name = "lblItemNum";
            lblItemNum.Size = new Size(33, 25);
            lblItemNum.TabIndex = 12;
            lblItemNum.Text = "1";
            lblItemNum.TextAlign = ContentAlignment.TopRight;
            // 
            // Label2
            // 
            Label2.Location = new Point(8, 24);
            Label2.Name = "Label2";
            Label2.Size = new Size(73, 25);
            Label2.TabIndex = 11;
            Label2.Text = "Item";
            // 
            // fraGiveItem
            // 
            fraGiveItem.Controls.Add(scrlItemValue);
            fraGiveItem.Controls.Add(scrlItemNum);
            fraGiveItem.Controls.Add(lblItemValue);
            fraGiveItem.Controls.Add(Label5);
            fraGiveItem.Controls.Add(lblItemNum);
            fraGiveItem.Controls.Add(Label2);
            fraGiveItem.Location = new Point(8, 176);
            fraGiveItem.Name = "fraGiveItem";
            fraGiveItem.Size = new Size(321, 97);
            fraGiveItem.TabIndex = 9;
            fraGiveItem.TabStop = false;
            fraGiveItem.Text = "Give Item";
            fraGiveItem.Visible = false;
            // 
            // cmdOk
            // 
            cmdOk.Location = new Point(8, 288);
            cmdOk.Name = "cmdOk";
            cmdOk.Size = new Size(153, 33);
            cmdOk.TabIndex = 8;
            cmdOk.Text = "Ok";
            cmdOk.Click += cmdOk_Click;
            // 
            // cmdCancel
            // 
            cmdCancel.Location = new Point(176, 288);
            cmdCancel.Name = "cmdCancel";
            cmdCancel.Size = new Size(153, 33);
            cmdCancel.TabIndex = 7;
            cmdCancel.Text = "Cancel";
            cmdCancel.Click += cmdCancel_Click;
            // 
            // cmbType
            // 
            cmbType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbType.FormattingEnabled = true;
            cmbType.Items.AddRange(new object[] { "Add HP", "Add MP", "Add SP", "Sub HP", "Sub MP", "Sub SP", "Give Item" });
            cmbType.Location = new Point(8, 136);
            cmbType.Name = "cmbType";
            cmbType.Size = new Size(321, 26);
            cmbType.TabIndex = 6;
            cmbType.SelectedIndexChanged += cmbType_SelectedIndexChanged;
            // 
            // scrlVitalMod
            // 
            scrlVitalMod.Location = new Point(88, 24);
            scrlVitalMod.Maximum = 255;
            scrlVitalMod.Name = "scrlVitalMod";
            scrlVitalMod.Size = new Size(193, 25);
            scrlVitalMod.TabIndex = 3;
            scrlVitalMod.Value = 1;
            scrlVitalMod.Scroll += scrlVitalMod_Scroll;
            // 
            // Label4
            // 
            Label4.Location = new Point(8, 24);
            Label4.Name = "Label4";
            Label4.Size = new Size(73, 25);
            Label4.TabIndex = 5;
            Label4.Text = "Vital Mod";
            // 
            // lblVitalMod
            // 
            lblVitalMod.Location = new Point(280, 24);
            lblVitalMod.Name = "lblVitalMod";
            lblVitalMod.Size = new Size(33, 25);
            lblVitalMod.TabIndex = 4;
            lblVitalMod.Text = "1";
            lblVitalMod.TextAlign = ContentAlignment.TopRight;
            // 
            // fraVitals
            // 
            fraVitals.Controls.Add(scrlVitalMod);
            fraVitals.Controls.Add(Label4);
            fraVitals.Controls.Add(lblVitalMod);
            fraVitals.Location = new Point(8, 176);
            fraVitals.Name = "fraVitals";
            fraVitals.Size = new Size(321, 97);
            fraVitals.TabIndex = 2;
            fraVitals.TabStop = false;
            fraVitals.Text = "Vitals Data";
            fraVitals.Visible = false;
            // 
            // txtName
            // 
            txtName.Location = new Point(64, 8);
            txtName.Name = "txtName";
            txtName.Size = new Size(265, 26);
            txtName.TabIndex = 0;
            // 
            // lblLevelReq
            // 
            lblLevelReq.Location = new Point(296, 88);
            lblLevelReq.Name = "lblLevelReq";
            lblLevelReq.Size = new Size(33, 25);
            lblLevelReq.TabIndex = 19;
            lblLevelReq.Text = "1";
            lblLevelReq.TextAlign = ContentAlignment.TopRight;
            // 
            // Label3
            // 
            Label3.Location = new Point(8, 88);
            Label3.Name = "Label3";
            Label3.Size = new Size(49, 25);
            Label3.TabIndex = 17;
            Label3.Text = "Level";
            // 
            // Label1
            // 
            Label1.Location = new Point(8, 8);
            Label1.Name = "Label1";
            Label1.Size = new Size(49, 25);
            Label1.TabIndex = 1;
            Label1.Text = "Name";
            // 
            // frmSpellEditor
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(337, 329);
            ControlBox = false;
            Controls.Add(scrlLevelReq);
            Controls.Add(cmbClassReq);
            Controls.Add(fraGiveItem);
            Controls.Add(cmdOk);
            Controls.Add(cmdCancel);
            Controls.Add(cmbType);
            Controls.Add(fraVitals);
            Controls.Add(txtName);
            Controls.Add(lblLevelReq);
            Controls.Add(Label3);
            Controls.Add(Label1);
            Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmSpellEditor";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Spell Editor";
            Load += frmSpellEditor_Load;
            fraGiveItem.ResumeLayout(false);
            fraVitals.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.HScrollBar scrlLevelReq;
		private System.Windows.Forms.ComboBox cmbClassReq;
		private System.Windows.Forms.HScrollBar scrlItemValue;
		private System.Windows.Forms.HScrollBar scrlItemNum;
		private System.Windows.Forms.Label lblItemValue;
		private System.Windows.Forms.Label Label5;
		private System.Windows.Forms.Label lblItemNum;
		private System.Windows.Forms.Label Label2;
		private System.Windows.Forms.GroupBox fraGiveItem;
		private System.Windows.Forms.Button cmdOk;
		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.ComboBox cmbType;
		private System.Windows.Forms.HScrollBar scrlVitalMod;
		private System.Windows.Forms.Label Label4;
		private System.Windows.Forms.Label lblVitalMod;
		private System.Windows.Forms.GroupBox fraVitals;
		private System.Windows.Forms.TextBox txtName;
		private System.Windows.Forms.Label lblLevelReq;
		private System.Windows.Forms.Label Label3;
		private System.Windows.Forms.Label Label1;
	}
}
