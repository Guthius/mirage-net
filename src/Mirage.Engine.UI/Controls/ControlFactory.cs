using System.Numerics;
using System.Xml;
using Mirage.Engine.UI.Skins;
using Mirage.Engine.UI.Styles;
using SFML.Graphics;

namespace Mirage.Engine.UI.Controls;

internal abstract class ControlFactory<TControl>(ISkin skin) : IControlFactory<TControl> where TControl : Control
{
    protected const string DefaultFontName = "Coolvetica";
    protected const int DefaultFontSize = 14;

    protected sealed record ControlProperties(
        string Name,
        string Text,
        float X,
        float Y,
        int Width,
        int Height,
        bool Enabled,
        bool Visible,
        Font? Font,
        int FontSize);

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

    protected Style ReadStyle(XmlReader xmlReader, string defaultStyleName)
    {
        var styleName = ReadString(xmlReader, "Style", defaultStyleName);

        return skin.GetStyle(styleName);
    }

    protected ControlProperties ReadCoreProperties(XmlReader xmlReader, Window? parent)
    {
        var position = ReadVector(xmlReader, "Position", Vector2.Zero);
        var size = ReadVector(xmlReader, "Size", Vector2.Zero);

        var x = position.X;
        var y = position.Y;

        if (parent is not null)
        {
            if (x < 0)
            {
                x = parent.Size.X + x;
            }

            if (y < 0)
            {
                y = parent.Size.Y + y;
            }
        }

        var defaultFontName = skin.GetPropertyString("DefaultFont", DefaultFontName);
        var fontName = ReadString(xmlReader, "Font", defaultFontName);

        return new ControlProperties(
            Name: ReadString(xmlReader, "Name"),
            Text: ReadString(xmlReader, "Text"),
            X: x, Y: y,
            Width: (int) size.X,
            Height: (int) size.Y,
            Enabled: ReadBoolean(xmlReader, "Enabled", true),
            Visible: ReadBoolean(xmlReader, "Visible", true),
            Font: skin.GetFont(fontName),
            FontSize: ReadInt32(xmlReader, "FontSize", skin.GetPropertyInt32("DefaultFontSize", DefaultFontSize)));
    }
}