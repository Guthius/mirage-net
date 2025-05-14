namespace Mirage.Client.Forms;

public partial class frmLogin : Form
{
	public frmLogin()
	{
		InitializeComponent();
	}
    
	private void picCancel_Click(object sender, EventArgs e)
	{
		Hide();
	}

	private void picConnect_Click(object sender, EventArgs e)
	{
		if (txtName.Text.Trim().Length > 0 && txtPassword.Text.Trim().Length > 0)
		{
		}
	}
}