using System.Numerics;
using System.Xml;
using Mirage.Engine.UI.Skins;
using Mirage.Engine.UI.Styles;

namespace Mirage.Engine.UI.Controls;

internal abstract class ControlFactory<TControl>(ISkin skin) where TControl : Control
{
    protected const string DefaultFontName = "Coolvetica";
    protected const int DefaultFontSize = 14;

    protected sealed record ControlProperties(string Name, string Text, int X, int Y, int Width, int Height, bool Enabled, bool Visible);

    protected sealed record FontProperties(string FontName, int Size);

    public abstract TControl Create(XmlReader xmlReader, Window? parent);

    protected static string ReadString(XmlReader xmlReader, string attributeName, string defaultValue = "")
    {
        return xmlReader.GetAttribute(attributeName) ?? defaultValue;
    }

    protected static int ReadInt32(XmlReader xmlReader, string attributeName, int defaultValue)
    {
        var value = xmlReader.GetAttribute(attributeName);
        if (string.IsNullOrEmpty(value))
        {
            return defaultValue;
        }

        if (int.TryParse(value, out var result))
        {
            return result;
        }

        return defaultValue;
    }

    protected static bool ReadBoolean(XmlReader xmlReader, string attributeName, bool defaultValue)
    {
        var value = xmlReader.GetAttribute(attributeName);
        if (string.IsNullOrEmpty(value))
        {
            return defaultValue;
        }

        if (bool.TryParse(value, out var result))
        {
            return result;
        }

        return defaultValue;
    }

    protected static TEnum ReadEnum<TEnum>(XmlReader xmlReader, string attributeName, TEnum defaultValue) where TEnum : struct
    {
        var value = xmlReader.GetAttribute(attributeName);
        if (string.IsNullOrEmpty(value))
        {
            return defaultValue;
        }

        if (Enum.TryParse<TEnum>(value, true, out var result))
        {
            return result;
        }

        return defaultValue;
    }

    protected static Vector2 ReadVector(XmlReader xmlReader, string attributeName, Vector2 defaultValue)
    {
        var value = xmlReader.GetAttribute(attributeName);
        if (string.IsNullOrEmpty(value))
        {
            return defaultValue;
        }

        var tokens = value.Split(',');
        if (tokens.Length != 2 || !int.TryParse(tokens[0], out var x) || !int.TryParse(tokens[1], out var y))
        {
            return defaultValue;
        }

        return new Vector2(x, y);
    }

    protected static FontProperties ReadFont(XmlReader xmlReader, string defaultFontName = DefaultFontName, int defaultFontSize = DefaultFontSize)
    {
        var fontName = ReadString(xmlReader, "Font", defaultFontName);
        var fontSize = ReadInt32(xmlReader, "FontSize", defaultFontSize);

        return new FontProperties(fontName, fontSize);
    }

    protected Style ReadStyle(XmlReader xmlReader, string defaultStyleName)
    {
        var styleName = ReadString(xmlReader, "Style", defaultStyleName);

        return skin.GetStyle(styleName);
    }

    protected Style ReadStyle(XmlReader xmlReader, Style defaultStyle)
    {
        var styleName = ReadString(xmlReader, "Style");
        if (string.IsNullOrEmpty(styleName))
        {
            return defaultStyle;
        }

        return skin.GetStyle(styleName);
    }

    protected static ControlProperties ReadControlProperties(XmlReader xmlReader, Window? parent)
    {
        var position = ReadVector(xmlReader, "Position", Vector2.Zero);
        var size = ReadVector(xmlReader, "Size", Vector2.Zero);

        var x = (int) position.X;
        var y = (int) position.Y;

        if (parent is not null)
        {
            if (x < 0)
            {
                x = parent.Width + x;
            }

            if (y < 0)
            {
                y = parent.Height + y;
            }
        }

        return new ControlProperties(
            Name: ReadString(xmlReader, "Name"),
            Text: ReadString(xmlReader, "Text"),
            X: x, Y: y,
            Width: (int) size.X,
            Height: (int) size.Y,
            Enabled: ReadBoolean(xmlReader, "Enabled", true),
            Visible: ReadBoolean(xmlReader, "Visible", true));
    }
}