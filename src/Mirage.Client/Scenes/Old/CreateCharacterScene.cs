// using Mirage.Client.Layers.Menu;
// using Mirage.Client.Net;
// using Mirage.Client.UI;
// using Mirage.Engine.UI.Controls;
// using Mirage.Net.Protocol.FromClient;
//
// namespace Mirage.Client.Scenes;
//
// public sealed class CreateCharacterScene : Scene
// {
//     public CreateCharacterScene(ISceneManager sceneManager, Game game)
//     {
//         var window = new CreateCharacterWindow(game.Jobs);
//
//         UI.Add(new PictureBox {Image = "Content/Title.png"});
//         UI.Add(window);
//
//         window.Cancel += sceneManager.SwitchTo<MenuScene>;
//         window.CreateCharacter += CreateCharacter;
//         window.MoveToCenter();
//     }
//
//     private static void CreateCharacter(CreateCharacterEventArgs e)
//     {
//         Network.Send(new CreateCharacterRequest(e.Name, e.Gender, e.JobId));
//     }
// }