using SFML.Graphics;
using Terestrium.Client.Core.Scenes;

namespace Terestrium.Client.Scenes.Game;

public interface IGameScene : IScene
{
    void AddChatMessage(string message, string channel, Color color);
}