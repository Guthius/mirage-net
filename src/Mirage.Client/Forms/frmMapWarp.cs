using Mirage.Client.Modules;

namespace Mirage.Client.Forms;

public partial class frmMapWarp : Form
{
    public frmMapWarp()
    {
        InitializeComponent();
    }

    private void cmdOk_Click(object sender, EventArgs e)
    {
        modGameLogic.EditorWarpMap = int.Parse(txtMap.Text);
        modGameLogic.EditorWarpX = scrlX.Value;
        modGameLogic.EditorWarpY = scrlY.Value;
        Close();
    }

    private void cmdCancel_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void scrlX_Scroll(object sender, ScrollEventArgs e)
    {
        lblX.Text = scrlX.Value.ToString();
    }

    private void scrlY_Scroll(object sender, ScrollEventArgs e)
    {
        lblY.Text = scrlY.Value.ToString();
    }

    private void txtMap_TextChanged(object sender, EventArgs e)
    {
        if (!int.TryParse(txtMap.Text, out var map) || map < 0 || map > modTypes.MAX_MAPS)
        {
            txtMap.Text = "1";
        }
    }
}