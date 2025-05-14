using Mirage.Client.Modules;

namespace Mirage.Client.Forms;

public partial class frmMapItem : Form
{
    public frmMapItem()
    {
        InitializeComponent();
    }

    private void frmMapItem_Load(object sender, EventArgs e)
    {
        lblName.Text = modTypes.Item[scrlItem.Value].Name.Trim();
    }

    private void cmdOk_Click(object sender, EventArgs e)
    {
        modGameLogic.ItemEditorNum = scrlItem.Value;
        modGameLogic.ItemEditorValue = scrlValue.Value;
        Close();
    }

    private void cmdCancel_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void scrlItem_Scroll(object sender, ScrollEventArgs e)
    {
        lblItem.Text = scrlItem.Value.ToString();
        lblName.Text = modTypes.Item[scrlItem.Value].Name.Trim();
    }

    private void scrlValue_Scroll(object sender, ScrollEventArgs e)
    {
        lblValue.Text = scrlValue.Value.ToString();
    }
}