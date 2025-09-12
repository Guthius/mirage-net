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

    // Creates a simple, empty window using the default style without requiring a layout file.
    public Window CreateWindow()
    {
        var style = Skin.GetStyle(Skin.DefaultStyleName);
        var window = new Window(style)
        {
            ShowFrame = true,
            ShowTitleBar = true,
            Visible = true,
            Font = Skin.GetFont("Regular")
        };
        Add(window);
        return window;
    }

    public void HideAllWindows()
    {
        foreach (var window in GetChildrenOfType<Window>())
        {
            window.Visible = false;
        }
    }
}