using SFML.Graphics;

namespace Terestrium.Client.UI.Styles;

public sealed class Style(Dictionary<string, StylePartBuilder> parts, Dictionary<string, Sprite> sprites, Dictionary<string, string> properties)
{
    public static readonly Style Empty = new([], [], []);

    public IStylePart? GetPart(string partName)
    {
        return parts.GetValueOrDefault(partName)?.Build();
    }

    public Sprite? GetSprite(string spriteName)
    {
        return sprites.GetValueOrDefault(spriteName);
    }

    public string GetString(string propertyName, string defaultValue = "")
    {
        return properties.GetValueOrDefault(propertyName, defaultValue);
    }

    public int GetInt32(string propertyName, int defaultValue = 0)
    {
        return int.TryParse(GetString(propertyName), out var value) ? value : defaultValue;
    }
}