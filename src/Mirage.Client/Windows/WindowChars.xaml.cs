using System.Windows;
using Mirage.Modules;
using MessageBox = System.Windows.MessageBox;

namespace Mirage.Windows;

public partial class WindowChars
{
    public WindowChars()
    {
        InitializeComponent();
    }

    private void OnUse(object sender, RoutedEventArgs e)
    {
        modGameLogic.MenuState(modGameLogic.MENU_STATE_USECHAR);
    }

    private void OnNew(object sender, RoutedEventArgs e)
    {
        modGameLogic.MenuState(modGameLogic.MENU_STATE_NEWCHAR);
    }

    private void OnDelete(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show(
            "Are you sure you wish to delete this character?", 
            modTypes.GAME_NAME, 
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);
        
        if (result == MessageBoxResult.Yes)
        {
            modGameLogic.MenuState(modGameLogic.MENU_STATE_DELCHAR);
        }
    }

    private void OnCancel(object sender, RoutedEventArgs e)
    {
        modClientTCP.TcpDestroy();
        
        My.Forms.frmLogin.Show();
        
        Hide();
    }
}