using System.Xml;
using Mirage.Engine.UI.Skins;
using SFML.System;

namespace Mirage.Engine.UI.Controls;

internal sealed class LabelFactory(ISkin skin) : ControlFactory<Label>(skin)
{
    private readonly ISkin _skin = skin;

    public override Label Create(XmlReader xmlReader, Window? parent)
    {
        var props = ReadCoreProperties(xmlReader, parent);

        return new Label(ReadStyle(xmlReader, _skin.DefaultStyleName))
        {
            Position = new Vector2f(props.X, props.Y),
            Size = new Vector2i(props.Width, props.Height),
            Visible = props.Visible,
            Text = props.Text,
            Enabled = props.Enabled,
            Font = props.Font,
            FontSize = props.FontSize,
            HorizontalAlignment = ReadEnum(xmlReader, "TextAlignment", HorizontalAlignment.Left)
        };
    }
}