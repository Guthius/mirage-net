using Mirage.Client.Net;
using Mirage.Net.Protocol.FromClient;

namespace Mirage.Client.Forms;

public partial class frmFixItem : Form
{
    public frmFixItem()
    {
        InitializeComponent();
    }

    private void chkFix_Click(object sender, EventArgs e)
    {
        Network.Send(new FixItemRequest(cmbItem.SelectedIndex + 1));
    }

    private void picCancel_Click(object sender, EventArgs e)
    {
        Close();
    }
}