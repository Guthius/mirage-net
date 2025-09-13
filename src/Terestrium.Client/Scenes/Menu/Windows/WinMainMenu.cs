using Terestrium.Client.UI.Controls;

namespace Terestrium.Client.Scenes.Menu.Windows;

internal sealed class WinMainMenu
{
    private readonly Window _window;

    public WinMainMenu(Window window, IMenuScene menu)
    {
        _window = window;

        window.Get<Button>("LoginButton").Click += (_, _) => menu.ShowLogin();
        window.Get<Button>("RegisterButton").Click += (_, _) => menu.ShowCreateAccount();
        window.Get<Button>("DeleteButton").Click += (_, _) => menu.ShowDeleteAccount();
        window.Get<Button>("QuitButton").Click += (_, _) => menu.Quit();
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