using System.Windows;
using Mirage.Modules;

namespace Mirage.Windows;

public partial class WindowDeleteAccount
{
    public WindowDeleteAccount()
    {
        InitializeComponent();
    }

    private void OnAccept(object sender, RoutedEventArgs e)
    {
        if (txtName.Text.Trim().Length > 0 && txtPassword.Text.Trim().Length > 0)
        {
            modGameLogic.MenuState(modGameLogic.MENU_STATE_DELACCOUNT);
        }
    }

    private void OnCancel(object sender, RoutedEventArgs e)
    {
        My.Forms.frmMainMenu.Show();
        
        Hide();
    }
}