using SFML.Graphics;
using Terestrium.Client.Core.Scenes;

namespace Terestrium.Client.Scenes;

public interface ISceneGame : IScene
{
    void AddChatMessage(string message, string channel, Color color);
}