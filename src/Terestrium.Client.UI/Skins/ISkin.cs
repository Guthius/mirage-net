using SFML.Graphics;
using Terestrium.Client.UI.Styles;

namespace Terestrium.Client.UI.Skins;

public interface ISkin
{
    string DefaultStyleName { get; }
    Style GetStyle(string styleName);
    Font? GetFont(string fontName);
    string GetPropertyString(string key, string defaultValue);
    int GetPropertyInt32(string key, int defaultValue);
}