using Mirage.Client.Modules;

namespace Mirage.Client.Forms;

public partial class frmMapKey : Form
{
    public frmMapKey()
    {
        InitializeComponent();
    }

    private void frmMapKey_Load(object sender, EventArgs e)
    {
        lblName.Text = modTypes.Item[scrlItem.Value].Name.Trim();
    }

    private void cmdOk_Click(object sender, EventArgs e)
    {
        modGameLogic.KeyEditorNum = scrlItem.Value;
        modGameLogic.KeyEditorTake = chkTake.Checked ? 1 : 0;
        Close();
    }

    private void scrlItem_Scroll(object sender, ScrollEventArgs e)
    {
        lblItem.Text = scrlItem.Value.ToString();
        lblName.Text = modTypes.Item[scrlItem.Value].Name.Trim();
    }

    private void cmdCancel_Click(object sender, EventArgs e)
    {
        Close();
    }
}