namespace Mirage.Engine.UI.Styles;

public sealed class Style(Dictionary<string, StylePart> parts, Dictionary<string, string> properties)
{
    public static readonly Style Empty = new([], []);

    public StylePart? GetPart(string partName)
    {
        return parts.GetValueOrDefault(partName);
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