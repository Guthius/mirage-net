namespace Mirage.Client.Forms
{
	partial class frmIndex
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
            lstIndex = new ListBox();
            SuspendLayout();
            // 
            // cmdCancel
            // 
            cmdCancel.Location = new Point(184, 256);
            cmdCancel.Name = "cmdCancel";
            cmdCancel.Size = new Size(161, 33);
            cmdCancel.TabIndex = 2;
            cmdCancel.Text = "Cancel";
            cmdCancel.Click += cmdCancel_Click;
            // 
            // cmdOk
            // 
            cmdOk.Location = new Point(8, 256);
            cmdOk.Name = "cmdOk";
            cmdOk.Size = new Size(161, 33);
            cmdOk.TabIndex = 1;
            cmdOk.Text = "Ok";
            cmdOk.Click += cmdOk_Click;
            // 
            // lstIndex
            // 
            lstIndex.Location = new Point(8, 8);
            lstIndex.Name = "lstIndex";
            lstIndex.Size = new Size(337, 238);
            lstIndex.TabIndex = 0;
            // 
            // frmIndex
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(353, 298);
            ControlBox = false;
            Controls.Add(cmdCancel);
            Controls.Add(cmdOk);
            Controls.Add(lstIndex);
            Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmIndex";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Index";
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.Button cmdOk;
		public System.Windows.Forms.ListBox lstIndex;
	}
}
