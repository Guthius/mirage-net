using Mirage.Client.Net;
using Mirage.Client.UI;
using Mirage.Engine.UI.Controls;
using Mirage.Net.Protocol.FromClient;

namespace Mirage.Client.Scenes;

public sealed class CharacterSelectScene : Scene
{
    private readonly ISceneManager _sceneManager;
    private readonly Game _game;

    public CharacterSelectScene(ISceneManager sceneManager, Game game)
    {
        _sceneManager = sceneManager;
        _game = game;

        var window = new CharacterSelectWindow(game.Characters, game.MaxCharacters);

        UI.Add(new PictureBox {Image = "Content/Title.png"});
        UI.Add(window);

        window.Cancel += sceneManager.SwitchTo<MainMenuScene>;
        window.SelectCharacter += SelectCharacter;
        window.DeleteCharacter += DeleteCharacter;
        window.CreateNewCharacter += sceneManager.SwitchTo<CreateCharacterScene>;
        window.MoveToCenter();
    }

    protected override void OnShow()
    {
        if (_game.Characters.Count == 0)
        {
            _sceneManager.SwitchTo<CreateCharacterScene>();
        }
    }

    private static void SelectCharacter(CharacterEventArgs e)
    {
        Network.Send(new SelectCharacterRequest(e.CharacterId));
    }

    private void DeleteCharacter(CharacterEventArgs e)
    {
        var window = new ConfirmWindow("Are you sure you want to delete this character?");

        UI.Add(window);

        window.Confirm += confirmed =>
        {
            UI.Remove(window);

            if (confirmed)
            {
                Network.Send(new DeleteCharacterRequest(e.CharacterId));
            }
        };
    }
}