using System.Xml;
using Mirage.Engine.UI.Skins;
using SFML.System;

namespace Mirage.Engine.UI.Controls;

internal sealed class WindowFactory(ISkin skin) : ControlFactory<Window>(skin)
{
    private readonly ISkin _skin = skin;

    public override Window Create(XmlReader xmlReader, Window? parent)
    {
        var control = ReadControlProperties(xmlReader, parent);
   

        var window = new Window(ReadStyle(xmlReader, _skin.DefaultStyleName))
        {
            Position = new Vector2f(control.X, control.Y),
            Width = control.Width,
            Height = control.Height,
            Visible = control.Visible,
            CanDrag = ReadBoolean(xmlReader, "CanDrag", true),
            // CanFocus = ReadBoolean(xmlReader, "CanFocus", true),
            // Font = ReadFont(xmlReader),
            Text = control.Text,
            // ShowTitleBar = ReadBoolean(xmlReader, "ShowTitlebar", true),
            // Clickthrough = ReadBoolean(xmlReader, "Clickthrough", false),
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