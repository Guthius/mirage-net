using Mirage.Engine.UI.Controls;

namespace Mirage.Client.Scenes.Lobby;

public interface IGuiService
{
    Control? Load(string layoutName);
}

