using Mirage.Engine.UI.Styles;
using SFML.Graphics;

namespace Mirage.Engine.UI.Skins;

public interface ISkin
{
    string DefaultStyleName { get; }
    Style GetStyle(string styleName);
    Font? GetFont(string fontName);
}