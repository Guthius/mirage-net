namespace Mirage.Client.Forms
{
	partial class frmMirage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMirage));
            optLayers = new RadioButton();
            optAttribs = new RadioButton();
            cmdCancel = new Button();
            cmdSend = new Button();
            cmdProperties = new Button();
            scrlPicture = new VScrollBar();
            picBackSelect = new PictureBox();
            picBack = new PictureBox();
            picSelect = new PictureBox();
            cmdFill = new Button();
            optGround = new RadioButton();
            optMask = new RadioButton();
            optAnim = new RadioButton();
            optFringe = new RadioButton();
            cmdClear = new Button();
            fraLayers = new GroupBox();
            optPass = new RadioButton();
            optKeyOpen = new RadioButton();
            optBlocked = new RadioButton();
            optWarp = new RadioButton();
            cmdClear2 = new Button();
            optItem = new RadioButton();
            optNpcAvoid = new RadioButton();
            optKey = new RadioButton();
            fraAttribs = new GroupBox();
            picMapEditor = new PictureBox();
            txtChat = new RichTextBox();
            picScreen = new PictureBox();
            lstSpells = new ListBox();
            Label5 = new Label();
            lblCast = new Label();
            lblSpellsCancel = new Label();
            picPlayerSpells = new PictureBox();
            lblName = new Label();
            lblHP = new Label();
            lblMP = new Label();
            lblSP = new Label();
            picGUI = new PictureBox();
            Socket = new Compat.Winsock();
            ((System.ComponentModel.ISupportInitialize)picBackSelect).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picBack).BeginInit();
            picBack.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picSelect).BeginInit();
            fraLayers.SuspendLayout();
            fraAttribs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picMapEditor).BeginInit();
            picMapEditor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picScreen).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picPlayerSpells).BeginInit();
            picPlayerSpells.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picGUI).BeginInit();
            picGUI.SuspendLayout();
            SuspendLayout();
            // 
            // optLayers
            // 
            optLayers.Checked = true;
            optLayers.Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            optLayers.ForeColor = Color.White;
            optLayers.Location = new Point(120, 248);
            optLayers.Name = "optLayers";
            optLayers.Size = new Size(105, 17);
            optLayers.TabIndex = 22;
            optLayers.TabStop = true;
            optLayers.Text = "Layers";
            optLayers.Click += optLayers_Click;
            // 
            // optAttribs
            // 
            optAttribs.Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            optAttribs.ForeColor = Color.White;
            optAttribs.Location = new Point(120, 264);
            optAttribs.Name = "optAttribs";
            optAttribs.Size = new Size(105, 17);
            optAttribs.TabIndex = 21;
            optAttribs.Text = "Attributes";
            optAttribs.Click += optAttribs_Click;
            // 
            // cmdCancel
            // 
            cmdCancel.Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            cmdCancel.ForeColor = Color.White;
            cmdCancel.Location = new Point(136, 399);
            cmdCancel.Name = "cmdCancel";
            cmdCancel.Size = new Size(113, 18);
            cmdCancel.TabIndex = 20;
            cmdCancel.Text = "Cancel";
            cmdCancel.Click += cmdCancel_Click;
            // 
            // cmdSend
            // 
            cmdSend.Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            cmdSend.ForeColor = Color.White;
            cmdSend.Location = new Point(8, 399);
            cmdSend.Name = "cmdSend";
            cmdSend.Size = new Size(113, 18);
            cmdSend.TabIndex = 19;
            cmdSend.Text = "Send";
            cmdSend.Click += cmdSend_Click;
            // 
            // cmdProperties
            // 
            cmdProperties.Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            cmdProperties.ForeColor = Color.White;
            cmdProperties.Location = new Point(160, 304);
            cmdProperties.Name = "cmdProperties";
            cmdProperties.Size = new Size(89, 33);
            cmdProperties.TabIndex = 18;
            cmdProperties.Text = "Properties";
            cmdProperties.Click += cmdProperties_Click;
            // 
            // scrlPicture
            // 
            scrlPicture.Location = new Point(232, 8);
            scrlPicture.Maximum = 255;
            scrlPicture.Name = "scrlPicture";
            scrlPicture.Size = new Size(17, 225);
            scrlPicture.TabIndex = 17;
            scrlPicture.Scroll += scrlPicture_Scroll;
            // 
            // picBackSelect
            // 
            picBackSelect.BackColor = Color.FromArgb(0, 0, 0);
            picBackSelect.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            picBackSelect.Location = new Point(0, 0);
            picBackSelect.Name = "picBackSelect";
            picBackSelect.Size = new Size(64, 64);
            picBackSelect.TabIndex = 16;
            picBackSelect.TabStop = false;
            picBackSelect.MouseDown += picBackSelect_MouseDown;
            picBackSelect.MouseMove += picBackSelect_MouseMove;
            // 
            // picBack
            // 
            picBack.BackColor = Color.FromArgb(0, 0, 0);
            picBack.Controls.Add(picBackSelect);
            picBack.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            picBack.Location = new Point(8, 8);
            picBack.Name = "picBack";
            picBack.Size = new Size(224, 224);
            picBack.TabIndex = 15;
            picBack.TabStop = false;
            // 
            // picSelect
            // 
            picSelect.BackColor = Color.FromArgb(0, 0, 0);
            picSelect.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            picSelect.Location = new Point(120, 304);
            picSelect.Name = "picSelect";
            picSelect.Size = new Size(32, 32);
            picSelect.TabIndex = 14;
            picSelect.TabStop = false;
            // 
            // cmdFill
            // 
            cmdFill.Location = new Point(8, 104);
            cmdFill.Name = "cmdFill";
            cmdFill.Size = new Size(81, 17);
            cmdFill.TabIndex = 49;
            cmdFill.Text = "Fill";
            cmdFill.Visible = false;
            // 
            // optGround
            // 
            optGround.Checked = true;
            optGround.Font = new Font("Arial", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            optGround.Location = new Point(8, 24);
            optGround.Name = "optGround";
            optGround.Size = new Size(81, 17);
            optGround.TabIndex = 35;
            optGround.TabStop = true;
            optGround.Text = "Ground";
            // 
            // optMask
            // 
            optMask.Font = new Font("Arial", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            optMask.Location = new Point(8, 40);
            optMask.Name = "optMask";
            optMask.Size = new Size(81, 17);
            optMask.TabIndex = 34;
            optMask.Text = "Mask";
            // 
            // optAnim
            // 
            optAnim.Font = new Font("Arial", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            optAnim.Location = new Point(8, 56);
            optAnim.Name = "optAnim";
            optAnim.Size = new Size(81, 17);
            optAnim.TabIndex = 33;
            optAnim.Text = "Animation";
            // 
            // optFringe
            // 
            optFringe.Font = new Font("Arial", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            optFringe.Location = new Point(8, 72);
            optFringe.Name = "optFringe";
            optFringe.Size = new Size(81, 17);
            optFringe.TabIndex = 32;
            optFringe.Text = "Fringe";
            // 
            // cmdClear
            // 
            cmdClear.Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            cmdClear.Location = new Point(8, 128);
            cmdClear.Name = "cmdClear";
            cmdClear.Size = new Size(81, 18);
            cmdClear.TabIndex = 31;
            cmdClear.Text = "Clear";
            cmdClear.Click += cmdClear_Click;
            // 
            // fraLayers
            // 
            fraLayers.Controls.Add(cmdFill);
            fraLayers.Controls.Add(optGround);
            fraLayers.Controls.Add(optMask);
            fraLayers.Controls.Add(optAnim);
            fraLayers.Controls.Add(optFringe);
            fraLayers.Controls.Add(cmdClear);
            fraLayers.Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            fraLayers.ForeColor = Color.White;
            fraLayers.Location = new Point(8, 240);
            fraLayers.Name = "fraLayers";
            fraLayers.Size = new Size(97, 153);
            fraLayers.TabIndex = 30;
            fraLayers.TabStop = false;
            fraLayers.Text = "Layers";
            // 
            // optPass
            // 
            optPass.Font = new Font("Arial", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            optPass.Location = new Point(8, 112);
            optPass.Name = "optPass";
            optPass.Size = new Size(81, 17);
            optPass.TabIndex = 48;
            optPass.Text = "Passable";
            // 
            // optKeyOpen
            // 
            optKeyOpen.Font = new Font("Arial", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            optKeyOpen.Location = new Point(8, 96);
            optKeyOpen.Name = "optKeyOpen";
            optKeyOpen.Size = new Size(81, 16);
            optKeyOpen.TabIndex = 47;
            optKeyOpen.Text = "Key Open";
            optKeyOpen.Click += optKeyOpen_Click;
            // 
            // optBlocked
            // 
            optBlocked.Font = new Font("Arial", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            optBlocked.Location = new Point(8, 16);
            optBlocked.Name = "optBlocked";
            optBlocked.Size = new Size(81, 17);
            optBlocked.TabIndex = 29;
            optBlocked.Text = "Blocked";
            // 
            // optWarp
            // 
            optWarp.Font = new Font("Arial", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            optWarp.Location = new Point(8, 32);
            optWarp.Name = "optWarp";
            optWarp.Size = new Size(81, 17);
            optWarp.TabIndex = 28;
            optWarp.Text = "Warp";
            optWarp.Click += optWarp_Click;
            // 
            // cmdClear2
            // 
            cmdClear2.Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            cmdClear2.Location = new Point(8, 128);
            cmdClear2.Name = "cmdClear2";
            cmdClear2.Size = new Size(81, 18);
            cmdClear2.TabIndex = 27;
            cmdClear2.Text = "Clear";
            cmdClear2.Click += cmdClear2_Click;
            // 
            // optItem
            // 
            optItem.Font = new Font("Arial", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            optItem.Location = new Point(8, 48);
            optItem.Name = "optItem";
            optItem.Size = new Size(81, 18);
            optItem.TabIndex = 26;
            optItem.Text = "Item";
            optItem.Click += optItem_Click;
            // 
            // optNpcAvoid
            // 
            optNpcAvoid.Font = new Font("Arial", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            optNpcAvoid.Location = new Point(8, 64);
            optNpcAvoid.Name = "optNpcAvoid";
            optNpcAvoid.Size = new Size(81, 18);
            optNpcAvoid.TabIndex = 25;
            optNpcAvoid.Text = "Npc Avoid";
            // 
            // optKey
            // 
            optKey.Font = new Font("Arial", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            optKey.Location = new Point(8, 80);
            optKey.Name = "optKey";
            optKey.Size = new Size(81, 18);
            optKey.TabIndex = 24;
            optKey.Text = "Key";
            optKey.Click += optKey_Click;
            // 
            // fraAttribs
            // 
            fraAttribs.Controls.Add(optPass);
            fraAttribs.Controls.Add(optKeyOpen);
            fraAttribs.Controls.Add(optBlocked);
            fraAttribs.Controls.Add(optWarp);
            fraAttribs.Controls.Add(cmdClear2);
            fraAttribs.Controls.Add(optItem);
            fraAttribs.Controls.Add(optNpcAvoid);
            fraAttribs.Controls.Add(optKey);
            fraAttribs.Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            fraAttribs.ForeColor = Color.White;
            fraAttribs.Location = new Point(8, 240);
            fraAttribs.Name = "fraAttribs";
            fraAttribs.Size = new Size(97, 153);
            fraAttribs.TabIndex = 23;
            fraAttribs.TabStop = false;
            fraAttribs.Text = "Attributes";
            fraAttribs.Visible = false;
            // 
            // picMapEditor
            // 
            picMapEditor.Controls.Add(optLayers);
            picMapEditor.Controls.Add(optAttribs);
            picMapEditor.Controls.Add(cmdCancel);
            picMapEditor.Controls.Add(cmdSend);
            picMapEditor.Controls.Add(cmdProperties);
            picMapEditor.Controls.Add(scrlPicture);
            picMapEditor.Controls.Add(picBack);
            picMapEditor.Controls.Add(picSelect);
            picMapEditor.Controls.Add(fraLayers);
            picMapEditor.Controls.Add(fraAttribs);
            picMapEditor.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            picMapEditor.Location = new Point(73, 12);
            picMapEditor.Name = "picMapEditor";
            picMapEditor.Size = new Size(257, 427);
            picMapEditor.TabIndex = 13;
            picMapEditor.TabStop = false;
            picMapEditor.Visible = false;
            // 
            // txtChat
            // 
            txtChat.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtChat.BackColor = Color.FromArgb(0, 0, 0);
            txtChat.Location = new Point(0, 384);
            txtChat.Name = "txtChat";
            txtChat.ScrollBars = RichTextBoxScrollBars.Vertical;
            txtChat.Size = new Size(903, 119);
            txtChat.TabIndex = 1;
            txtChat.Text = "";
            txtChat.Enter += txtChat_Enter;
            // 
            // picScreen
            // 
            picScreen.BackColor = Color.FromArgb(0, 0, 0);
            picScreen.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            picScreen.Location = new Point(0, 0);
            picScreen.Name = "picScreen";
            picScreen.Size = new Size(512, 384);
            picScreen.TabIndex = 0;
            picScreen.TabStop = false;
            picScreen.MouseDown += picScreen_MouseDown;
            picScreen.MouseMove += picScreen_MouseMove;
            picScreen.PreviewKeyDown += picScreen_PreviewKeyDown;
            // 
            // lstSpells
            // 
            lstSpells.BackColor = Color.FromArgb(0, 0, 0);
            lstSpells.BorderStyle = BorderStyle.FixedSingle;
            lstSpells.Font = new Font("Arial", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lstSpells.Location = new Point(0, 16);
            lstSpells.Name = "lstSpells";
            lstSpells.Size = new Size(257, 128);
            lstSpells.TabIndex = 43;
            // 
            // Label5
            // 
            Label5.BackColor = Color.Transparent;
            Label5.Font = new Font("Arial", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Label5.ForeColor = Color.FromArgb(255, 0, 255);
            Label5.Location = new Point(0, 0);
            Label5.Name = "Label5";
            Label5.Size = new Size(129, 17);
            Label5.TabIndex = 46;
            Label5.Text = "Spells";
            Label5.TextAlign = ContentAlignment.TopCenter;
            // 
            // lblCast
            // 
            lblCast.BackColor = Color.Transparent;
            lblCast.Font = new Font("Arial", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblCast.ForeColor = Color.FromArgb(255, 0, 255);
            lblCast.Location = new Point(0, 168);
            lblCast.Name = "lblCast";
            lblCast.Size = new Size(129, 17);
            lblCast.TabIndex = 45;
            lblCast.Text = "Cast";
            lblCast.TextAlign = ContentAlignment.TopCenter;
            lblCast.Click += lblCast_Click;
            // 
            // lblSpellsCancel
            // 
            lblSpellsCancel.BackColor = Color.Transparent;
            lblSpellsCancel.Font = new Font("Arial", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblSpellsCancel.ForeColor = Color.FromArgb(255, 0, 255);
            lblSpellsCancel.Location = new Point(0, 192);
            lblSpellsCancel.Name = "lblSpellsCancel";
            lblSpellsCancel.Size = new Size(129, 17);
            lblSpellsCancel.TabIndex = 44;
            lblSpellsCancel.Text = "Cancel";
            lblSpellsCancel.TextAlign = ContentAlignment.TopCenter;
            lblSpellsCancel.Click += lblSpellsCancel_Click;
            // 
            // picPlayerSpells
            // 
            picPlayerSpells.BackColor = Color.FromArgb(0, 0, 64);
            picPlayerSpells.Controls.Add(lstSpells);
            picPlayerSpells.Controls.Add(Label5);
            picPlayerSpells.Controls.Add(lblCast);
            picPlayerSpells.Controls.Add(lblSpellsCancel);
            picPlayerSpells.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            picPlayerSpells.Location = new Point(630, 133);
            picPlayerSpells.Name = "picPlayerSpells";
            picPlayerSpells.Size = new Size(257, 219);
            picPlayerSpells.TabIndex = 42;
            picPlayerSpells.TabStop = false;
            picPlayerSpells.Visible = false;
            // 
            // lblName
            // 
            lblName.BackColor = Color.Transparent;
            lblName.Font = new Font("Arial", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblName.ForeColor = Color.FromArgb(255, 0, 255);
            lblName.Location = new Point(0, 8);
            lblName.Name = "lblName";
            lblName.Size = new Size(129, 17);
            lblName.TabIndex = 12;
            lblName.Text = "Mirage Online";
            lblName.TextAlign = ContentAlignment.TopCenter;
            // 
            // lblHP
            // 
            lblHP.BackColor = Color.Transparent;
            lblHP.Font = new Font("Arial", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblHP.ForeColor = Color.FromArgb(255, 0, 0);
            lblHP.Location = new Point(24, 120);
            lblHP.Name = "lblHP";
            lblHP.Size = new Size(105, 17);
            lblHP.TabIndex = 11;
            lblHP.Text = "0%";
            lblHP.TextAlign = ContentAlignment.TopCenter;
            // 
            // lblMP
            // 
            lblMP.BackColor = Color.Transparent;
            lblMP.Font = new Font("Arial", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblMP.ForeColor = Color.FromArgb(0, 0, 255);
            lblMP.Location = new Point(24, 144);
            lblMP.Name = "lblMP";
            lblMP.Size = new Size(105, 17);
            lblMP.TabIndex = 10;
            lblMP.Text = "0%";
            lblMP.TextAlign = ContentAlignment.TopCenter;
            // 
            // lblSP
            // 
            lblSP.BackColor = Color.Transparent;
            lblSP.Font = new Font("Arial", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblSP.ForeColor = Color.FromArgb(255, 0, 255);
            lblSP.Location = new Point(24, 168);
            lblSP.Name = "lblSP";
            lblSP.Size = new Size(105, 17);
            lblSP.TabIndex = 9;
            lblSP.Text = "0%";
            lblSP.TextAlign = ContentAlignment.TopCenter;
            // 
            // picGUI
            // 
            picGUI.Controls.Add(lblName);
            picGUI.Controls.Add(lblHP);
            picGUI.Controls.Add(lblMP);
            picGUI.Controls.Add(lblSP);
            picGUI.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            picGUI.Image = (Image)resources.GetObject("picGUI.Image");
            picGUI.Location = new Point(512, 0);
            picGUI.Name = "picGUI";
            picGUI.Size = new Size(128, 384);
            picGUI.SizeMode = PictureBoxSizeMode.AutoSize;
            picGUI.TabIndex = 2;
            picGUI.TabStop = false;
            // 
            // Socket
            // 
            Socket.RemoteHost = "";
            Socket.RemotePort = 0;
            // 
            // frmMirage
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.FromArgb(0, 0, 0);
            ClientSize = new Size(903, 505);
            Controls.Add(picMapEditor);
            Controls.Add(picScreen);
            Controls.Add(txtChat);
            Controls.Add(picPlayerSpells);
            Controls.Add(picGUI);
            Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            KeyPreview = true;
            Name = "frmMirage";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Mirage Online";
            FormClosed += frmMirage_FormClosed;
            KeyPress += frmMirage_KeyPress;
            KeyUp += frmMirage_KeyUp;
            ((System.ComponentModel.ISupportInitialize)picBackSelect).EndInit();
            ((System.ComponentModel.ISupportInitialize)picBack).EndInit();
            picBack.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picSelect).EndInit();
            fraLayers.ResumeLayout(false);
            fraAttribs.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picMapEditor).EndInit();
            picMapEditor.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picScreen).EndInit();
            ((System.ComponentModel.ISupportInitialize)picPlayerSpells).EndInit();
            picPlayerSpells.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picGUI).EndInit();
            picGUI.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        public System.Windows.Forms.RadioButton optLayers;
		private System.Windows.Forms.RadioButton optAttribs;
		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.Button cmdSend;
		private System.Windows.Forms.Button cmdProperties;
		public System.Windows.Forms.VScrollBar scrlPicture;
		public System.Windows.Forms.PictureBox picBackSelect;
		private System.Windows.Forms.PictureBox picBack;
		public System.Windows.Forms.PictureBox picSelect;
		private System.Windows.Forms.Button cmdFill;
		public System.Windows.Forms.RadioButton optGround;
		public System.Windows.Forms.RadioButton optMask;
		public System.Windows.Forms.RadioButton optAnim;
		public System.Windows.Forms.RadioButton optFringe;
		private System.Windows.Forms.Button cmdClear;
		private System.Windows.Forms.GroupBox fraLayers;
		public System.Windows.Forms.RadioButton optPass;
		public System.Windows.Forms.RadioButton optKeyOpen;
		public System.Windows.Forms.RadioButton optBlocked;
		public System.Windows.Forms.RadioButton optWarp;
		private System.Windows.Forms.Button cmdClear2;
		public System.Windows.Forms.RadioButton optItem;
		public System.Windows.Forms.RadioButton optNpcAvoid;
		public System.Windows.Forms.RadioButton optKey;
		private System.Windows.Forms.GroupBox fraAttribs;
		public System.Windows.Forms.PictureBox picMapEditor;
		public System.Windows.Forms.RichTextBox txtChat;
		public System.Windows.Forms.PictureBox picScreen;
		public System.Windows.Forms.ListBox lstSpells;
		private System.Windows.Forms.Label Label5;
		private System.Windows.Forms.Label lblCast;
		private System.Windows.Forms.Label lblSpellsCancel;
		public System.Windows.Forms.PictureBox picPlayerSpells;
		private System.Windows.Forms.Label lblName;
		public System.Windows.Forms.Label lblHP;
		public System.Windows.Forms.Label lblMP;
		public System.Windows.Forms.Label lblSP;
		public System.Windows.Forms.PictureBox picGUI;
        public Compat.Winsock Socket;
    }
}
