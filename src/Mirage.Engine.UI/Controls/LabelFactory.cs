using System.Xml;
using Mirage.Engine.UI.Skins;
using SFML.System;

namespace Mirage.Engine.UI.Controls;

internal sealed class LabelFactory(ISkin skin) : ControlFactory<Label>(skin)
{
    public override Label Create(XmlReader xmlReader, Control? parent)
    {
        var props = ReadCoreProperties(xmlReader, parent);

        return new Label
        {
            Name = props.Name,
            Position = new Vector2i(props.X, props.Y),
            Size = new Vector2i(props.Width, props.Height),
            Visible = props.Visible,
            Text = ParseText(props.Text),
            Enabled = props.Enabled,
            Font = props.Font,
            FontSize = props.FontSize,
            HorizontalAlignment = ReadEnum(xmlReader, "TextAlignment", HorizontalAlignment.Left)
        };
    }

    private static string ParseText(string text)
    {
        return text.Replace("{br}", "\n");
    }
}