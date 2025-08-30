using System.Xml;
using Mirage.Engine.UI.Skins;
using SFML.System;

namespace Mirage.Engine.UI.Controls;

internal sealed class RadioButtonFactory(ISkin skin) : ControlFactory<RadioButton>(skin)
{
    private readonly ISkin _skin = skin;

    public override RadioButton Create(XmlReader xmlReader, Control? parent)
    {
        var props = ReadCoreProperties(xmlReader, parent);

        return new RadioButton(ReadStyle(xmlReader, _skin.DefaultStyleName))
        {
            Name = props.Name,
            Position = new Vector2i(props.X, props.Y),
            Size = new Vector2i(props.Width, props.Height),
            Visible = props.Visible,
            Text = props.Text,
            Enabled = props.Enabled,
            Font = props.Font,
            FontSize = props.FontSize,
            Group = ReadString(xmlReader, "Group", "default"),
            Checked = ReadBoolean(xmlReader, "IsChecked", false)
        };
    }
}