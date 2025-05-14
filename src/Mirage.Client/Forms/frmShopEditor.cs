using Mirage.Client.Modules;
using Mirage.Game.Constants;

namespace Mirage.Client.Forms;

public partial class frmShopEditor : Form
{
    public frmShopEditor()
    {
        InitializeComponent();
    }

    private void frmShopEditor_Load(object sender, EventArgs e)
    {
        txtName.Text = modTypes.Shop[modGameLogic.EditorIndex].Name;
        txtJoinSay.Text = modTypes.Shop[modGameLogic.EditorIndex].JoinSay;
        txtLeaveSay.Text = modTypes.Shop[modGameLogic.EditorIndex].LeaveSay;

        chkFixesItems.Checked = modTypes.Shop[modGameLogic.EditorIndex].FixesItems != 0;

        cmbItemGive.Items.Clear();
        cmbItemGive.Items.Add("None");
        cmbItemGet.Items.Clear();
        cmbItemGet.Items.Add("None");

        for (var i = 1; i <= Limits.MaxItems; i++)
        {
            cmbItemGive.Items.Add($"{i}: {modTypes.Item[i].Name.Trim()}");
            cmbItemGet.Items.Add($"{i}: {modTypes.Item[i].Name.Trim()}");
        }

        cmbItemGive.SelectedIndex = 0;
        cmbItemGet.SelectedIndex = 0;

        UpdateShopTrade();
    }

    private void cmdOk_Click(object sender, EventArgs e)
    {
        modTypes.Shop[modGameLogic.EditorIndex].Name = txtName.Text.Trim();
        modTypes.Shop[modGameLogic.EditorIndex].JoinSay = txtJoinSay.Text.Trim();
        modTypes.Shop[modGameLogic.EditorIndex].LeaveSay = txtLeaveSay.Text.Trim();
        modTypes.Shop[modGameLogic.EditorIndex].FixesItems = chkFixesItems.Checked ? 1 : 0;

        modClientTCP.SendSaveShop(modGameLogic.EditorIndex);
        modGameLogic.InShopEditor = false;
        Close();
    }

    private void cmdCancel_Click(object sender, EventArgs e)
    {
        modGameLogic.InShopEditor = false;
        Close();
    }

    private void cmdUpdate_Click(object sender, EventArgs e)
    {
        var index = lstTradeItem.SelectedIndex + 1;

        modTypes.Shop[modGameLogic.EditorIndex].TradeItem[index].GiveItem = cmbItemGive.SelectedIndex;
        modTypes.Shop[modGameLogic.EditorIndex].TradeItem[index].GiveValue = int.Parse(txtItemGiveValue.Text);
        modTypes.Shop[modGameLogic.EditorIndex].TradeItem[index].GetItem = cmbItemGet.SelectedIndex;
        modTypes.Shop[modGameLogic.EditorIndex].TradeItem[index].GetValue = int.Parse(txtItemGetValue.Text);

        UpdateShopTrade();
    }

    private void UpdateShopTrade()
    {
        lstTradeItem.BeginUpdate();
        lstTradeItem.Items.Clear();

        for (var i = 1; i <= Limits.MaxShopTrades; i++)
        {
            var tradeItem = modTypes.Shop[modGameLogic.EditorIndex].TradeItem[i];


            if (tradeItem is { GetItem: > 0, GiveItem: > 0 })
            {
                lstTradeItem.Items.Add($"{i}: {tradeItem.GiveValue} {modTypes.Item[tradeItem.GiveItem].Name} for {tradeItem.GetValue} {modTypes.Item[tradeItem.GetItem].Name}");
            }
            else
            {
                lstTradeItem.Items.Add("Empty Trade Slot");
            }
        }

        lstTradeItem.SelectedIndex = 0;
        lstTradeItem.EndUpdate();
    }
}