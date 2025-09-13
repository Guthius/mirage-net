using Terestrium.Client.Scenes;
using Terestrium.Client.UI.Controls;

namespace Terestrium.Client.Windows;

internal sealed class WinLogin : WinBase
{
    public WinLogin(Window window, ISceneMainMenu menu) : base(window)
    {
        var textBoxAccountName = Get<TextBox>("AccountNameTextBox");
        var textBoxPassword = Get<TextBox>("PasswordTextBox");

        Get<Button>("LoginButton").Click += (_, _) => menu.Login(textBoxAccountName.Text, textBoxPassword.Text);
        Get<Button>("CancelButton").Click += (_, _) => menu.ShowMainMenu();
    }

    protected override void OnShown()
    {
        MoveToCenter();
    }
}