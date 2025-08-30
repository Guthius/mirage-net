using System.Globalization;
using System.Xml;
using Mirage.Engine.UI.Skins;
using SFML.Graphics;
using SFML.System;

namespace Mirage.Engine.UI.Controls;

internal sealed class WindowFactory(ISkin skin) : ControlFactory<Window>(skin)
{
    private readonly ISkin _skin = skin;

    public override Window Create(XmlReader xmlReader, Control? parent)
    {
        var props = ReadCoreProperties(xmlReader, parent);

        var window = new Window(ReadStyle(xmlReader, _skin.DefaultStyleName))
        {
            Name = props.Name,
            Position = new Vector2i(props.X, props.Y),
            Size = new Vector2i(props.Width, props.Height),
            Visible = props.Visible,
            CanDrag = ReadBoolean(xmlReader, "CanDrag", true),
            Font = props.Font,
            FontSize = props.FontSize,
            Text = props.Text,
            BackColor = ReadColor(xmlReader, "BackColor"),
            ShowFrame = ReadBoolean(xmlReader, "ShowFrame", true),
            ShowTitleBar = ReadBoolean(xmlReader, "ShowTitlebar", true)
        };

        var startPosition = xmlReader.GetAttribute("StartPosition");
        if (string.IsNullOrEmpty(startPosition) || parent is null ||
            !startPosition.Equals("Center", StringComparison.OrdinalIgnoreCase) &&
            !startPosition.Equals("CenterScreen", StringComparison.OrdinalIgnoreCase))
        {
            return window;
        }

        var x = (parent.Size.X - window.Size.X) / 2;
        var y = (parent.Size.Y - window.Size.Y) / 2;

        window.Position = new Vector2i(x, y);

        return window;
    }

    private static Color? ReadColor(XmlReader xmlReader, string attributeName, Color? defaultValue = null)
    {
        var value = xmlReader.GetAttribute(attributeName);
        if (string.IsNullOrEmpty(value))
        {
            return defaultValue;
        }

        value = value.TrimStart('#');
        switch (value.Length)
        {
            case 6:
            {
                if (byte.TryParse(value.AsSpan(0, 2), NumberStyles.HexNumber, null, out var r) &&
                    byte.TryParse(value.AsSpan(2, 2), NumberStyles.HexNumber, null, out var g) &&
                    byte.TryParse(value.AsSpan(4, 2), NumberStyles.HexNumber, null, out var b))
                {
                    return new Color(r, g, b);
                }

                break;
            }

            case 8:
            {
                if (byte.TryParse(value.AsSpan(0, 2), NumberStyles.HexNumber, null, out var r) &&
                    byte.TryParse(value.AsSpan(2, 2), NumberStyles.HexNumber, null, out var g) &&
                    byte.TryParse(value.AsSpan(4, 2), NumberStyles.HexNumber, null, out var b) &&
                    byte.TryParse(value.AsSpan(6, 2), NumberStyles.HexNumber, null, out var a))
                {
                    return new Color(r, g, b, a);
                }

                break;
            }
        }

        return defaultValue;
    }
}