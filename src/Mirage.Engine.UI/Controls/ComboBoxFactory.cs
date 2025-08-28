using System.Xml;
using Mirage.Engine.UI.Skins;
using SFML.System;

namespace Mirage.Engine.UI.Controls;

internal sealed class ComboBoxFactory(ISkin skin) : ControlFactory<ComboBox>(skin)
{
    private readonly ISkin _skin = skin;

    public override ComboBox Create(XmlReader xmlReader, Window? parent)
    {
        var props = ReadCoreProperties(xmlReader, parent);

        return new ComboBox(ReadStyle(xmlReader, _skin.DefaultStyleName))
        {
            Position = new Vector2f(props.X, props.Y),
            Size = new Vector2i(props.Width, props.Height),
            Visible = props.Visible,
            Enabled = props.Enabled,
            Font = props.Font,
            FontSize = props.FontSize
        };
    }
}