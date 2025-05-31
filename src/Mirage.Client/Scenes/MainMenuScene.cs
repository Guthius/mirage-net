using Mirage.Client.UI;
using Mirage.Engine.UI.Controls;

namespace Mirage.Client.Scenes;

public sealed class MainMenuScene : Scene
{
    public MainMenuScene(ISceneManager sceneManager)
    {
        var window = new MainMenuWindow();

        UI.Add(new PictureBox {Image = "Content/Title.png"});
        UI.Add(window);
        
        window.GoToLogin += sceneManager.SwitchTo<LoginScene>;
        window.GoToNewAccount += sceneManager.SwitchTo<CreateAccountScene>;
        window.GoToDeleteAccount += sceneManager.SwitchTo<DeleteAccountScene>;
        window.Quit += () => Environment.Exit(0);
        window.MoveToCenter();
    }
}