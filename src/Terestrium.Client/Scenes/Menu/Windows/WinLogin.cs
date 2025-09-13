using Terestrium.Client.UI.Controls;

namespace Terestrium.Client.Scenes.Menu.Windows;

internal sealed class WinLogin
{
    private readonly Window _window;

    public WinLogin(Window window, IMenuScene menu)
    {
        _window = window;

        var textBoxAccountName = window.Get<TextBox>("AccountNameTextBox");
        var textBoxPassword = window.Get<TextBox>("PasswordTextBox");

        window.Get<Button>("LoginButton").Click += (_, _) => menu.Login(textBoxAccountName.Text, textBoxPassword.Text);
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