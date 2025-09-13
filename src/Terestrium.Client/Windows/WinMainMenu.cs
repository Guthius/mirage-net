using Terestrium.Client.Scenes;
using Terestrium.Client.UI.Controls;

namespace Terestrium.Client.Windows;

internal sealed class WinMainMenu : WinBase
{
    public WinMainMenu(Window window, ISceneMainMenu menu) : base(window)
    {
        Get<Button>("LoginButton").Click += (_, _) => menu.ShowLogin();
        Get<Button>("RegisterButton").Click += (_, _) => menu.ShowCreateAccount();
        Get<Button>("DeleteButton").Click += (_, _) => menu.ShowDeleteAccount();
        Get<Button>("QuitButton").Click += (_, _) => menu.Quit();
    }

    protected override void OnShown()
    {
        MoveToCenter();
    }
}