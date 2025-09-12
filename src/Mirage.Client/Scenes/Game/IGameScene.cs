using SFML.Graphics;

namespace Mirage.Client.Scenes.Game;

public interface IGameScene : IScene
{
    void AddChatMessage(string message, string channel, Color color);
}