using System.Xml;
using SFML.System;
using Terestrium.Client.UI.Skins;

namespace Terestrium.Client.UI.Controls;

internal sealed class HScrollFactory(ISkin skin) : ControlFactory<HScroll>(skin)
{
    private readonly ISkin _skin = skin;

    public override HScroll Create(XmlReader xmlReader, Control? parent)
    {
        var props = ReadCoreProperties(xmlReader, parent);

        return new HScroll(ReadStyle(xmlReader, _skin.DefaultStyleName))
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