using Mirage.Modules;
using System.Windows;

namespace Mirage.Windows;

public partial class WIndowTraining
{
    public WIndowTraining()
    {
        InitializeComponent();
    }

    private void OnAccept(object sender, RoutedEventArgs e)
    {
        modClientTCP.SendData("usestatpoint" + modTypes.SEP_CHAR + cmbStat.SelectedIndex + modTypes.SEP_CHAR);
    }

    private void OnCancel(object sender, RoutedEventArgs e)
    {
        Close();
    }
}