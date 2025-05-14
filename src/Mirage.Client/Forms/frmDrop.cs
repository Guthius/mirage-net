namespace Mirage.Client.Forms;

public partial class frmDrop : Form
{
    private int _amount;

    public frmDrop()
    {
        InitializeComponent();
    }

    private void frmDrop_Load(object sender, EventArgs e)
    {
    }

    private void cmdOk_Click(object sender, EventArgs e)
    {
    }

    private void cmdCancel_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void cmdPlus1_Click(object sender, EventArgs e)
    {
        AddAmount(1);
    }

    private void cmdMinus1_Click(object sender, EventArgs e)
    {
        AddAmount(-1);
    }

    private void cmdPlus10_Click(object sender, EventArgs e)
    {
        AddAmount(10);
    }

    private void cmdMinus10_Click(object sender, EventArgs e)
    {
        AddAmount(-10);
    }

    private void cmdPlus100_Click(object sender, EventArgs e)
    {
        AddAmount(100);
    }

    private void cmdMinus100_Click(object sender, EventArgs e)
    {
        AddAmount(-100);
    }

    private void cmdPlus1000_Click(object sender, EventArgs e)
    {
        AddAmount(1000);
    }

    private void cmdMinus1000_Click(object sender, EventArgs e)
    {
        AddAmount(-1000);
    }

    private void AddAmount(int change)
    {
        //lblAmount.Text = $"{_amount}/{maxAmount}";
    }

}