namespace Mirage.Client.Forms
{
	partial class frmShopEditor
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
            chkFixesItems = new CheckBox();
            cmdUpdate = new Button();
            txtItemGetValue = new TextBox();
            cmbItemGet = new ComboBox();
            cmdOk = new Button();
            cmdCancel = new Button();
            txtItemGiveValue = new TextBox();
            cmbItemGive = new ComboBox();
            lstTradeItem = new ListBox();
            txtLeaveSay = new TextBox();
            txtName = new TextBox();
            txtJoinSay = new TextBox();
            Label6 = new Label();
            Label5 = new Label();
            Label4 = new Label();
            Label3 = new Label();
            Label2 = new Label();
            Label1 = new Label();
            Label14 = new Label();
            SuspendLayout();
            // 
            // chkFixesItems
            // 
            chkFixesItems.Location = new Point(8, 112);
            chkFixesItems.Name = "chkFixesItems";
            chkFixesItems.Size = new Size(353, 25);
            chkFixesItems.TabIndex = 18;
            chkFixesItems.Text = "Fixes Items";
            // 
            // cmdUpdate
            // 
            cmdUpdate.Location = new Point(200, 256);
            cmdUpdate.Name = "cmdUpdate";
            cmdUpdate.Size = new Size(161, 25);
            cmdUpdate.TabIndex = 17;
            cmdUpdate.Text = "Update ";
            cmdUpdate.Click += cmdUpdate_Click;
            // 
            // txtItemGetValue
            // 
            txtItemGetValue.Location = new Point(96, 256);
            txtItemGetValue.Name = "txtItemGetValue";
            txtItemGetValue.Size = new Size(89, 26);
            txtItemGetValue.TabIndex = 6;
            txtItemGetValue.Text = "1";
            txtItemGetValue.TextAlign = HorizontalAlignment.Right;
            // 
            // cmbItemGet
            // 
            cmbItemGet.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbItemGet.Location = new Point(96, 224);
            cmbItemGet.Name = "cmbItemGet";
            cmbItemGet.Size = new Size(265, 26);
            cmbItemGet.TabIndex = 5;
            // 
            // cmdOk
            // 
            cmdOk.Location = new Point(8, 424);
            cmdOk.Name = "cmdOk";
            cmdOk.Size = new Size(169, 41);
            cmdOk.TabIndex = 8;
            cmdOk.Text = "Ok";
            cmdOk.Click += cmdOk_Click;
            // 
            // cmdCancel
            // 
            cmdCancel.Location = new Point(192, 424);
            cmdCancel.Name = "cmdCancel";
            cmdCancel.Size = new Size(169, 41);
            cmdCancel.TabIndex = 9;
            cmdCancel.Text = "Cancel";
            cmdCancel.Click += cmdCancel_Click;
            // 
            // txtItemGiveValue
            // 
            txtItemGiveValue.Location = new Point(96, 184);
            txtItemGiveValue.Name = "txtItemGiveValue";
            txtItemGiveValue.Size = new Size(89, 26);
            txtItemGiveValue.TabIndex = 4;
            txtItemGiveValue.Text = "1";
            txtItemGiveValue.TextAlign = HorizontalAlignment.Right;
            // 
            // cmbItemGive
            // 
            cmbItemGive.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbItemGive.Location = new Point(96, 152);
            cmbItemGive.Name = "cmbItemGive";
            cmbItemGive.Size = new Size(265, 26);
            cmbItemGive.TabIndex = 3;
            // 
            // lstTradeItem
            // 
            lstTradeItem.Font = new Font("Arial", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lstTradeItem.Items.AddRange(new object[] { "1.", "2.", "3.", "4.", "5.", "6.", "7.", "8." });
            lstTradeItem.Location = new Point(8, 296);
            lstTradeItem.Name = "lstTradeItem";
            lstTradeItem.Size = new Size(353, 116);
            lstTradeItem.TabIndex = 7;
            // 
            // txtLeaveSay
            // 
            txtLeaveSay.Location = new Point(96, 72);
            txtLeaveSay.Name = "txtLeaveSay";
            txtLeaveSay.Size = new Size(265, 26);
            txtLeaveSay.TabIndex = 2;
            // 
            // txtName
            // 
            txtName.Location = new Point(96, 8);
            txtName.Name = "txtName";
            txtName.Size = new Size(265, 26);
            txtName.TabIndex = 0;
            // 
            // txtJoinSay
            // 
            txtJoinSay.Location = new Point(96, 40);
            txtJoinSay.Name = "txtJoinSay";
            txtJoinSay.Size = new Size(265, 26);
            txtJoinSay.TabIndex = 1;
            // 
            // Label6
            // 
            Label6.Location = new Point(8, 256);
            Label6.Name = "Label6";
            Label6.Size = new Size(81, 25);
            Label6.TabIndex = 16;
            Label6.Text = "Value";
            // 
            // Label5
            // 
            Label5.Location = new Point(8, 224);
            Label5.Name = "Label5";
            Label5.Size = new Size(81, 25);
            Label5.TabIndex = 15;
            Label5.Text = "Item Get";
            // 
            // Label4
            // 
            Label4.Location = new Point(8, 184);
            Label4.Name = "Label4";
            Label4.Size = new Size(81, 25);
            Label4.TabIndex = 14;
            Label4.Text = "Value";
            // 
            // Label3
            // 
            Label3.Location = new Point(8, 152);
            Label3.Name = "Label3";
            Label3.Size = new Size(81, 25);
            Label3.TabIndex = 13;
            Label3.Text = "Item Give";
            // 
            // Label2
            // 
            Label2.Location = new Point(8, 72);
            Label2.Name = "Label2";
            Label2.Size = new Size(81, 25);
            Label2.TabIndex = 12;
            Label2.Text = "Leave Say";
            // 
            // Label1
            // 
            Label1.Location = new Point(8, 8);
            Label1.Name = "Label1";
            Label1.Size = new Size(81, 25);
            Label1.TabIndex = 11;
            Label1.Text = "Name";
            // 
            // Label14
            // 
            Label14.Location = new Point(8, 40);
            Label14.Name = "Label14";
            Label14.Size = new Size(81, 25);
            Label14.TabIndex = 10;
            Label14.Text = "Join Say";
            // 
            // frmShopEditor
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(369, 476);
            ControlBox = false;
            Controls.Add(chkFixesItems);
            Controls.Add(cmdUpdate);
            Controls.Add(txtItemGetValue);
            Controls.Add(cmbItemGet);
            Controls.Add(cmdOk);
            Controls.Add(cmdCancel);
            Controls.Add(txtItemGiveValue);
            Controls.Add(cmbItemGive);
            Controls.Add(lstTradeItem);
            Controls.Add(txtLeaveSay);
            Controls.Add(txtName);
            Controls.Add(txtJoinSay);
            Controls.Add(Label6);
            Controls.Add(Label5);
            Controls.Add(Label4);
            Controls.Add(Label3);
            Controls.Add(Label2);
            Controls.Add(Label1);
            Controls.Add(Label14);
            Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmShopEditor";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Shop Editor";
            Load += frmShopEditor_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.CheckBox chkFixesItems;
		private System.Windows.Forms.Button cmdUpdate;
		private System.Windows.Forms.TextBox txtItemGetValue;
		private System.Windows.Forms.ComboBox cmbItemGet;
		private System.Windows.Forms.Button cmdOk;
		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.TextBox txtItemGiveValue;
		private System.Windows.Forms.ComboBox cmbItemGive;
		private System.Windows.Forms.ListBox lstTradeItem;
		private System.Windows.Forms.TextBox txtLeaveSay;
		private System.Windows.Forms.TextBox txtName;
		private System.Windows.Forms.TextBox txtJoinSay;
		private System.Windows.Forms.Label Label6;
		private System.Windows.Forms.Label Label5;
		private System.Windows.Forms.Label Label4;
		private System.Windows.Forms.Label Label3;
		private System.Windows.Forms.Label Label2;
		private System.Windows.Forms.Label Label1;
		private System.Windows.Forms.Label Label14;
	}
}
