using System.Xml;
using Terestrium.Client.UI.Skins;

namespace Terestrium.Client.UI.Controls;

public sealed class WindowLoader(Skin skin)
{
    private readonly WindowFactory _windowFactory = new(skin);
    private readonly Dictionary<string, IControlFactory<Control>> _controlFactories = new(StringComparer.OrdinalIgnoreCase)
    {
        {"Button", new ButtonFactory(skin)},
        {"CheckBox", new CheckBoxFactory(skin)},
        {"ComboBox", new ComboBoxFactory(skin)},
        {"Frame", new FrameFactory(skin)},
        {"HScroll", new HScrollFactory(skin)},
        {"Label", new LabelFactory(skin)},
        {"Panel", new PanelFactory(skin)},
        {"PictureBox", new PictureBoxFactory(skin)},
        {"RadioButton", new RadioButtonFactory(skin)},
        {"TextBox", new TextBoxFactory(skin)},
        {"VScroll", new VScrollFactory(skin)},
        {"Window", new WindowFactory(skin)}
    };

    public Window Load(string windowName, Control? parent = null)
    {
        var path = Path.Combine(skin.Path, "Windows", windowName + ".xml");
        if (!File.Exists(path))
        {
            throw new InvalidOperationException(
                $"Unable to load window layout '{windowName}'. " +
                $"Layout file '{path}' does not exist.");
        }

        using var stream = File.OpenRead(path);

        using var xmlReader = XmlReader.Create(stream, new XmlReaderSettings
        {
            IgnoreWhitespace = true,
            IgnoreComments = true
        });

        xmlReader.MoveToContent();

        if (xmlReader.NodeType != XmlNodeType.Element || xmlReader.Name != "Window")
        {
            throw new XmlException("Window layout file is missing root 'Window' element.");
        }

        return ReadWindow(xmlReader, parent);
    }

    private Window ReadWindow(XmlReader xmlReader, Control? parent)
    {
        var window = _windowFactory.Create(xmlReader, parent);

        parent?.Add(window);

        ReadControls(xmlReader, window);

        return window;
    }

    private void ReadControls(XmlReader xmlReader, Control parent)
    {
        if (xmlReader.IsEmptyElement)
        {
            xmlReader.Skip();

            return;
        }

        while (xmlReader.Read())
        {
            if (xmlReader.NodeType == XmlNodeType.Element)
            {
                ReadControl(xmlReader, parent);
            }
            else if (xmlReader.NodeType == XmlNodeType.EndElement)
            {
                break;
            }
        }
    }

    private void ReadControl(XmlReader xmlReader, Control parent)
    {
        if (!_controlFactories.TryGetValue(xmlReader.Name, out var factory))
        {
            if (!xmlReader.IsEmptyElement)
            {
                xmlReader.Skip();
            }

            return;
        }

        var control = factory.Create(xmlReader, parent);

        parent.Add(control);

        if (xmlReader.IsEmptyElement)
        {
            return;
        }

        ReadControls(xmlReader, control);
    }
}