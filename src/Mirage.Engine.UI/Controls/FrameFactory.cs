using System.Xml;
using Mirage.Engine.UI.Skins;
using SFML.System;

namespace Mirage.Engine.UI.Controls;

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