namespace Mirage.Client.Forms
{
	partial class frmItemEditor
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
            scrlSpell = new HScrollBar();
            lblSpellName = new Label();
            Label6 = new Label();
            Label7 = new Label();
            lblSpell = new Label();
            fraSpell = new GroupBox();
            picPic = new PictureBox();
            scrlPic = new HScrollBar();
            cmdCancel = new Button();
            cmdOk = new Button();
            scrlVitalMod = new HScrollBar();
            lblVitalMod = new Label();
            Label4 = new Label();
            fraVitals = new GroupBox();
            scrlStrength = new HScrollBar();
            scrlDurability = new HScrollBar();
            lblStrength = new Label();
            lblDurability = new Label();
            Label3 = new Label();
            Label2 = new Label();
            fraEquipment = new GroupBox();
            txtName = new TextBox();
            cmbType = new ComboBox();
            lblPic = new Label();
            Label5 = new Label();
            Label1 = new Label();
            fraSpell.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picPic).BeginInit();
            fraVitals.SuspendLayout();
            fraEquipment.SuspendLayout();
            SuspendLayout();
            // 
            // scrlSpell
            // 
            scrlSpell.Location = new Point(88, 56);
            scrlSpell.Maximum = 255;
            scrlSpell.Name = "scrlSpell";
            scrlSpell.Size = new Size(193, 25);
            scrlSpell.TabIndex = 22;
            scrlSpell.Value = 1;
            scrlSpell.Scroll += scrlSpell_Scroll;
            // 
            // lblSpellName
            // 
            lblSpellName.Location = new Point(88, 24);
            lblSpellName.Name = "lblSpellName";
            lblSpellName.Size = new Size(225, 25);
            lblSpellName.TabIndex = 26;
            // 
            // Label6
            // 
            Label6.Location = new Point(8, 24);
            Label6.Name = "Label6";
            Label6.Size = new Size(73, 25);
            Label6.TabIndex = 25;
            Label6.Text = "Name";
            // 
            // Label7
            // 
            Label7.Location = new Point(8, 56);
            Label7.Name = "Label7";
            Label7.Size = new Size(73, 25);
            Label7.TabIndex = 24;
            Label7.Text = "Num";
            // 
            // lblSpell
            // 
            lblSpell.Location = new Point(280, 56);
            lblSpell.Name = "lblSpell";
            lblSpell.Size = new Size(33, 25);
            lblSpell.TabIndex = 23;
            lblSpell.Text = "1";
            lblSpell.TextAlign = ContentAlignment.TopRight;
            // 
            // fraSpell
            // 
            fraSpell.Controls.Add(scrlSpell);
            fraSpell.Controls.Add(lblSpellName);
            fraSpell.Controls.Add(Label6);
            fraSpell.Controls.Add(Label7);
            fraSpell.Controls.Add(lblSpell);
            fraSpell.Location = new Point(8, 128);
            fraSpell.Name = "fraSpell";
            fraSpell.Size = new Size(321, 97);
            fraSpell.TabIndex = 21;
            fraSpell.TabStop = false;
            fraSpell.Text = "Spell Data";
            fraSpell.Visible = false;
            // 
            // picPic
            // 
            picPic.BackColor = Color.FromArgb(0, 0, 0);
            picPic.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            picPic.Location = new Point(296, 40);
            picPic.Name = "picPic";
            picPic.Size = new Size(32, 32);
            picPic.TabIndex = 19;
            picPic.TabStop = false;
            // 
            // scrlPic
            // 
            scrlPic.Location = new Point(64, 40);
            scrlPic.Maximum = 255;
            scrlPic.Name = "scrlPic";
            scrlPic.Size = new Size(193, 25);
            scrlPic.TabIndex = 17;
            scrlPic.Scroll += scrlPic_Scroll;
            // 
            // cmdCancel
            // 
            cmdCancel.Location = new Point(176, 240);
            cmdCancel.Name = "cmdCancel";
            cmdCancel.Size = new Size(153, 33);
            cmdCancel.TabIndex = 15;
            cmdCancel.Text = "Cancel";
            cmdCancel.Click += cmdCancel_Click;
            // 
            // cmdOk
            // 
            cmdOk.Location = new Point(8, 240);
            cmdOk.Name = "cmdOk";
            cmdOk.Size = new Size(153, 33);
            cmdOk.TabIndex = 14;
            cmdOk.Text = "Ok";
            cmdOk.Click += cmdOk_Click;
            // 
            // scrlVitalMod
            // 
            scrlVitalMod.Location = new Point(88, 24);
            scrlVitalMod.Maximum = 255;
            scrlVitalMod.Name = "scrlVitalMod";
            scrlVitalMod.Size = new Size(193, 25);
            scrlVitalMod.TabIndex = 12;
            scrlVitalMod.Value = 1;
            scrlVitalMod.Scroll += scrlVitalMod_Scroll;
            // 
            // lblVitalMod
            // 
            lblVitalMod.Location = new Point(280, 24);
            lblVitalMod.Name = "lblVitalMod";
            lblVitalMod.Size = new Size(33, 25);
            lblVitalMod.TabIndex = 13;
            lblVitalMod.Text = "1";
            lblVitalMod.TextAlign = ContentAlignment.TopRight;
            // 
            // Label4
            // 
            Label4.Location = new Point(8, 24);
            Label4.Name = "Label4";
            Label4.Size = new Size(73, 25);
            Label4.TabIndex = 11;
            Label4.Text = "Vital Mod";
            // 
            // fraVitals
            // 
            fraVitals.Controls.Add(scrlVitalMod);
            fraVitals.Controls.Add(lblVitalMod);
            fraVitals.Controls.Add(Label4);
            fraVitals.Location = new Point(8, 128);
            fraVitals.Name = "fraVitals";
            fraVitals.Size = new Size(321, 97);
            fraVitals.TabIndex = 10;
            fraVitals.TabStop = false;
            fraVitals.Text = "Vitals Data";
            fraVitals.Visible = false;
            // 
            // scrlStrength
            // 
            scrlStrength.Location = new Point(88, 64);
            scrlStrength.Maximum = 255;
            scrlStrength.Name = "scrlStrength";
            scrlStrength.Size = new Size(193, 25);
            scrlStrength.TabIndex = 8;
            scrlStrength.Value = 1;
            scrlStrength.Scroll += scrlStrength_Scroll;
            // 
            // scrlDurability
            // 
            scrlDurability.Location = new Point(88, 32);
            scrlDurability.Maximum = 255;
            scrlDurability.Name = "scrlDurability";
            scrlDurability.Size = new Size(193, 25);
            scrlDurability.TabIndex = 6;
            scrlDurability.Value = 1;
            scrlDurability.Scroll += scrlDurability_Scroll;
            // 
            // lblStrength
            // 
            lblStrength.Location = new Point(280, 64);
            lblStrength.Name = "lblStrength";
            lblStrength.Size = new Size(33, 25);
            lblStrength.TabIndex = 9;
            lblStrength.Text = "1";
            lblStrength.TextAlign = ContentAlignment.TopRight;
            // 
            // lblDurability
            // 
            lblDurability.Location = new Point(280, 32);
            lblDurability.Name = "lblDurability";
            lblDurability.Size = new Size(33, 25);
            lblDurability.TabIndex = 7;
            lblDurability.Text = "1";
            lblDurability.TextAlign = ContentAlignment.TopRight;
            // 
            // Label3
            // 
            Label3.Location = new Point(8, 64);
            Label3.Name = "Label3";
            Label3.Size = new Size(65, 25);
            Label3.TabIndex = 5;
            Label3.Text = "Strength";
            // 
            // Label2
            // 
            Label2.Location = new Point(8, 32);
            Label2.Name = "Label2";
            Label2.Size = new Size(73, 25);
            Label2.TabIndex = 4;
            Label2.Text = "Durability";
            // 
            // fraEquipment
            // 
            fraEquipment.Controls.Add(scrlStrength);
            fraEquipment.Controls.Add(scrlDurability);
            fraEquipment.Controls.Add(lblStrength);
            fraEquipment.Controls.Add(lblDurability);
            fraEquipment.Controls.Add(Label3);
            fraEquipment.Controls.Add(Label2);
            fraEquipment.Location = new Point(8, 128);
            fraEquipment.Name = "fraEquipment";
            fraEquipment.Size = new Size(321, 97);
            fraEquipment.TabIndex = 3;
            fraEquipment.TabStop = false;
            fraEquipment.Text = "Equipment Data";
            fraEquipment.Visible = false;
            // 
            // txtName
            // 
            txtName.Location = new Point(64, 8);
            txtName.Name = "txtName";
            txtName.Size = new Size(265, 26);
            txtName.TabIndex = 1;
            // 
            // cmbType
            // 
            cmbType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbType.FormattingEnabled = true;
            cmbType.Items.AddRange(new object[] { "None", "Weapon", "Armor", "Helmet", "Shield", "Potion Add HP", "Potion Add MP", "Potion Add SP", "Potion Sub HP", "Potion Sub MP", "Potion Sub SP", "Key", "Currency", "Spell" });
            cmbType.Location = new Point(8, 88);
            cmbType.Name = "cmbType";
            cmbType.Size = new Size(321, 26);
            cmbType.TabIndex = 0;
            cmbType.SelectedIndexChanged += cmbType_SelectedIndexChanged;
            // 
            // lblPic
            // 
            lblPic.Location = new Point(256, 40);
            lblPic.Name = "lblPic";
            lblPic.Size = new Size(33, 25);
            lblPic.TabIndex = 18;
            lblPic.Text = "0";
            lblPic.TextAlign = ContentAlignment.TopRight;
            // 
            // Label5
            // 
            Label5.Location = new Point(8, 40);
            Label5.Name = "Label5";
            Label5.Size = new Size(57, 25);
            Label5.TabIndex = 16;
            Label5.Text = "Pic";
            // 
            // Label1
            // 
            Label1.Location = new Point(8, 8);
            Label1.Name = "Label1";
            Label1.Size = new Size(49, 25);
            Label1.TabIndex = 2;
            Label1.Text = "Name";
            // 
            // frmItemEditor
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(337, 284);
            ControlBox = false;
            Controls.Add(fraSpell);
            Controls.Add(picPic);
            Controls.Add(scrlPic);
            Controls.Add(cmdCancel);
            Controls.Add(cmdOk);
            Controls.Add(fraVitals);
            Controls.Add(fraEquipment);
            Controls.Add(txtName);
            Controls.Add(cmbType);
            Controls.Add(lblPic);
            Controls.Add(Label5);
            Controls.Add(Label1);
            Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmItemEditor";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Item Editor";
            Load += frmItemEditor_Load;
            fraSpell.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picPic).EndInit();
            fraVitals.ResumeLayout(false);
            fraEquipment.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.HScrollBar scrlSpell;
		private System.Windows.Forms.Label lblSpellName;
		private System.Windows.Forms.Label Label6;
		private System.Windows.Forms.Label Label7;
		private System.Windows.Forms.Label lblSpell;
		private System.Windows.Forms.GroupBox fraSpell;
		private System.Windows.Forms.PictureBox picPic;
		private System.Windows.Forms.HScrollBar scrlPic;
		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.Button cmdOk;
		private System.Windows.Forms.HScrollBar scrlVitalMod;
		private System.Windows.Forms.Label lblVitalMod;
		private System.Windows.Forms.Label Label4;
		private System.Windows.Forms.GroupBox fraVitals;
		private System.Windows.Forms.HScrollBar scrlStrength;
		private System.Windows.Forms.HScrollBar scrlDurability;
		private System.Windows.Forms.Label lblStrength;
		private System.Windows.Forms.Label lblDurability;
		private System.Windows.Forms.Label Label3;
		private System.Windows.Forms.Label Label2;
		private System.Windows.Forms.GroupBox fraEquipment;
		private System.Windows.Forms.TextBox txtName;
		private System.Windows.Forms.ComboBox cmbType;
		private System.Windows.Forms.Label lblPic;
		private System.Windows.Forms.Label Label5;
		private System.Windows.Forms.Label Label1;
	}
}
