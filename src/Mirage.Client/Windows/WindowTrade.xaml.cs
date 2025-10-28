using System.Windows;
using Mirage.Modules;

namespace Mirage.Windows;

public partial class WindowTrade
{
    public WindowTrade()
    {
        InitializeComponent();
    }

    private void OnFix(object sender, RoutedEventArgs e)
    {
        var frmFixItem = new WindowFixItem();
		
        for (var i = 0; i <= modTypes.MAX_INV; i++)
        {
            if (modTypes.GetPlayerInvItemNum(modGameLogic.MyIndex, i) > 0)
            {
                frmFixItem.cmbItem.Items.Add(modTypes.Item[modTypes.GetPlayerInvItemNum(modGameLogic.MyIndex, i)].Name.Trim());
            }
            else
            {
                frmFixItem.cmbItem.Items.Add("Unused Slot");
            }
        }

        frmFixItem.cmbItem.SelectedIndex = 0;
        frmFixItem.ShowDialog();
    }

    private void OnDeal(object sender, RoutedEventArgs e)
    {
        if (lstTrade.Items.Count > 0)
        {
            modClientTCP.SendData(
                "traderequest" + 
                modTypes.SEP_CHAR + (lstTrade.SelectedIndex + 1) + 
                modTypes.SEP_CHAR);
        }
    }

    private void OnCancel(object sender, RoutedEventArgs e)
    {
        Close();
    }
}