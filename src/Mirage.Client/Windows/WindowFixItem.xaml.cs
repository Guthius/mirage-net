using System.Windows;
using Mirage.Modules;

namespace Mirage.Windows;

public partial class WindowFixItem
{
    public WindowFixItem()
    {
        InitializeComponent();
    }

    private void OnAccept(object sender, RoutedEventArgs e)
    {
        modClientTCP.SendData("fixitem" + modTypes.SEP_CHAR + (cmbItem.SelectedIndex + 1) + modTypes.SEP_CHAR);
    }

    private void OnCancel(object sender, RoutedEventArgs e)
    {
        Close();
    }
}