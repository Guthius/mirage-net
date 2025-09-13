using Terestrium.Client.Scenes;
using Terestrium.Client.UI.Controls;

namespace Terestrium.Client.Windows;

internal sealed class WinDeleteAccount : WinBase
{
    public WinDeleteAccount(Window window, ISceneMainMenu menu) : base(window)
    {
        var textBoxAccountName = Get<TextBox>("AccountNameTextBox");
        var textBoxPassword = Get<TextBox>("PasswordTextBox");

        Get<Button>("DeleteButton").Click += (_, _) => menu.DeleteAccount(textBoxAccountName.Text, textBoxPassword.Text);
        Get<Button>("CancelButton").Click += (_, _) => menu.ShowMainMenu();
    }

    protected override void OnShown()
    {
        MoveToCenter();
    }
}