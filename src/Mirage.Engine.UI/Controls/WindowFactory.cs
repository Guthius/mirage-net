using System.Xml;
using Mirage.Engine.UI.Skins;
using SFML.System;

namespace Mirage.Engine.UI.Controls;

internal sealed class WindowFactory(ISkin skin) : ControlFactory<Window>(skin)
{
    private readonly ISkin _skin = skin;

    public override Window Create(XmlReader xmlReader, Window? parent)
    {
        var props = ReadCoreProperties(xmlReader, parent);

        var window = new Window(ReadStyle(xmlReader, _skin.DefaultStyleName))
        {
            Position = new Vector2f(props.X, props.Y),
            Size = new Vector2i(props.Width, props.Height),
            Visible = props.Visible,
            CanDrag = ReadBoolean(xmlReader, "CanDrag", true),
            Font = props.Font,
            FontSize = props.FontSize,
            Text = props.Text,
            ShowTitleBar = ReadBoolean(xmlReader, "ShowTitlebar", true)
        };

        var startPosition = xmlReader.GetAttribute("StartPosition");
        if (string.IsNullOrEmpty(startPosition))
        {
            return window;
        }

        if (startPosition.Equals("Center", StringComparison.OrdinalIgnoreCase) ||
            startPosition.Equals("CenterScreen", StringComparison.OrdinalIgnoreCase))
        {
            window.MoveToCenter();
        }

        return window;
    }
}