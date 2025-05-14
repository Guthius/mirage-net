using Mirage.Client.Net;
using Mirage.Game.Data;
using Mirage.Net.Protocol.FromClient;

namespace Mirage.Client.Forms;

public partial class frmTraining : Form
{
    public frmTraining()
    {
        InitializeComponent();
    }

    private void frmTraining_Load(object sender, EventArgs e)
    {
        cmbStat.SelectedIndex = 0;
    }

    private void picTrain_Click(object sender, EventArgs e)
    {
        Network.Send(new UseStatPointRequest((StatType) cmbStat.SelectedIndex));
    }

    private void picCancel_Click(object sender, EventArgs e)
    {
        Close();
    }
}