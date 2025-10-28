using System.Windows;
using Mirage.Modules;
using MessageBox = System.Windows.MessageBox;

namespace Mirage.Windows;

public partial class WindowNewAccount
{
    public WindowNewAccount()
    {
        InitializeComponent();
    }

    private void OnAccept(object sender, RoutedEventArgs e)
    {
        if (txtName.Text.Trim().Length == 0 ||
            txtPassword.Text.Trim().Length == 0)
        {
            return;
        }

        var name = txtName.Text.Trim();

        // Prevent high ascii chars
        if (name.Any(c => c < 32 || c > 128))
        {
            MessageBox.Show(
                "You cannot use high ascii chars in your name, please reenter.",
                modTypes.GAME_NAME,
                MessageBoxButton.YesNo,
                MessageBoxImage.Error);

            txtName.Text = "";
            return;
        }
        
        modGameLogic.MenuState(modGameLogic.MENU_STATE_NEWACCOUNT);
    }

    private void OnCancel(object sender, RoutedEventArgs e)
    {
        My.Forms.frmMainMenu.Show();
        
        Hide();
    }
}