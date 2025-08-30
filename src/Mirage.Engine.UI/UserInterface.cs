using Mirage.Engine.UI.Controls;
using Mirage.Engine.UI.Skins;

namespace Mirage.Engine.UI;

public sealed class UserInterface : Control
{
    private readonly WindowLoader _windowLoader;

    public Skin Skin { get; }

    public UserInterface(string skinName)
    {
        Skin = new Skin(Path.Combine("Content", "Skins", skinName));
        
        _windowLoader = new WindowLoader(Skin);
    }

    public Window CreateWindow(string layoutName)
    {
        return _windowLoader.Load(layoutName, this);
    }

    public void HideAllWindows()
    {
        foreach (var window in GetChildrenOfType<Window>())
        {
            window.Visible = false;
        }
    }
}