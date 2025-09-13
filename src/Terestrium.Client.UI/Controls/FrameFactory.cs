using System.Xml;
using SFML.System;
using Terestrium.Client.UI.Skins;

namespace Terestrium.Client.UI.Controls;

internal sealed class FrameFactory(ISkin skin) : ControlFactory<Frame>(skin)
{
    public override Frame Create(XmlReader xmlReader, Control? parent)
    {
        var props = ReadCoreProperties(xmlReader, parent);

        return new Frame
        {
            Name = props.Name,
            Position = new Vector2i(props.X, props.Y),
            Size = new Vector2i(props.Width, props.Height),
            Visible = props.Visible,
            Enabled = props.Enabled,
            Font = props.Font,
            FontSize = props.FontSize
        };
    }
}