using System.Xml;
using Mirage.Engine.UI.Skins;
using SFML.System;

namespace Mirage.Engine.UI.Controls;

internal sealed class TextBoxFactory(ISkin skin) : ControlFactory<TextBox>(skin)
{
    private readonly ISkin _skin = skin;

    public override TextBox Create(XmlReader xmlReader, Control? parent)
    {
        var props = ReadCoreProperties(xmlReader, parent);

        return new TextBox(ReadStyle(xmlReader, _skin.DefaultStyleName))
        {
            Name = props.Name,
            Position = new Vector2i(props.X, props.Y),
            Size = new Vector2i(props.Width, props.Height),
            Visible = props.Visible,
            Text = props.Text,
            Enabled = props.Enabled,
            Font = props.Font,
            FontSize = props.FontSize,
            IsPassword = ReadBoolean(xmlReader, "IsPassword", false)
        };
    }
}