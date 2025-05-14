using Mirage.Client.Modules;

namespace Mirage.Client.Forms;

public partial class frmKeyOpen : Form
{
    public frmKeyOpen()
    {
        InitializeComponent();
    }

    private void cmdOk_Click(object sender, EventArgs e)
    {
        modGameLogic.KeyOpenEditorX = scrlX.Value;
        modGameLogic.KeyOpenEditorY = scrlY.Value;
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
}