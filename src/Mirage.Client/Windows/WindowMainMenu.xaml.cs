using System.Windows;
using Mirage.Modules;
using MessageBox = System.Windows.MessageBox;

namespace Mirage.Windows;

public partial class WindowMainMenu
{
    public WindowMainMenu()
    {
        InitializeComponent();
    }

    private void OnCreateAccount(object sender, RoutedEventArgs e)
    {
        My.Forms.frmNewAccount.Show();

        Hide();
    }

    private void OnDeleteAccount(object sender, RoutedEventArgs e)
    {
        var confirm = MessageBox.Show(
            "You are on the path for a character deletion, are you sure you want to go through with this?",
            modTypes.GAME_NAME,
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (confirm == MessageBoxResult.No)
        {
            return;
        }

        My.Forms.frmDeleteAccount.Show();

        Hide();
    }

    private void OnLogin(object sender, RoutedEventArgs e)
    {
        My.Forms.frmLogin.Show();

        Hide();
    }

    private void OnQuit(object sender, RoutedEventArgs e)
    {
        modGameLogic.GameDestroy();
    }
}