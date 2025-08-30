using Mirage.Engine.UI.Controls;

namespace Mirage.Client.Scenes.Menu.Windows;

internal sealed class WinDeleteAccount
{
    private readonly Window _window;

    public WinDeleteAccount(Window window, IMenuScene menu)
    {
        _window = window;

        var textBoxAccountName = window.Get<TextBox>("AccountNameTextBox");
        var textBoxPassword = window.Get<TextBox>("PasswordTextBox");

        window.Get<Button>("DeleteButton").Click += (_, _) => menu.DeleteAccount(textBoxAccountName.Text, textBoxPassword.Text);
        window.Get<Button>("CancelButton").Click += (_, _) => menu.ShowMainMenu();
    }

    public void Show()
    {
        _window.Visible = true;
        _window.MoveToCenter();
    }

    public void Hide()
    {
        _window.Visible = false;
    }
}