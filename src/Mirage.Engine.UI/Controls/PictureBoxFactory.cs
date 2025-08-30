using System.Xml;
using Mirage.Engine.UI.Skins;
using SFML.System;

namespace Mirage.Engine.UI.Controls;

internal sealed class PictureBoxFactory(ISkin skin) : ControlFactory<PictureBox>(skin)
{
    public override PictureBox Create(XmlReader xmlReader, Control? parent)
    {
        var props = ReadCoreProperties(xmlReader, parent);

        return new PictureBox
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