using System.Xml;
using Mirage.Engine.UI.Skins;

namespace Mirage.Engine.UI.Controls;

public sealed class WindowLoader
{
    public static WindowLoader Instance { get; } = new("Crystalshire");

    private readonly string _basePath;
    private readonly WindowFactory _windowFactory;
    private readonly Dictionary<string, ControlFactory<Control>> _controlFactories;
    
    public WindowLoader(string skinName)
    {
        _basePath = Path.Combine("Content", "Skins", skinName);

        var skin = new Skin(_basePath);

        _windowFactory = new WindowFactory(skin);
        _controlFactories = new Dictionary<string, ControlFactory<Control>>(StringComparer.OrdinalIgnoreCase)
        {
            // {"Button", new ButtonFactory(skin)},
            // {"CheckBox", new CheckBoxFactory(skin)},
            // {"HScroll", new HScrollLoader(skin)},
            // {"Label", new LabelFactory(skin)},
            // {"PictureBox", new PictureBoxFactory(skin)},
            // {"TextBox", new TextBoxFactory(skin)},
            // {"VScroll", new VScrollLoader(skin)}
        };
    }

    public Window Load(string windowName)
    {
        var path = Path.Combine(_basePath, "Layouts", windowName + ".xml");
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

        return ReadWindow(xmlReader, windowName);
    }

    private Window ReadWindow(XmlReader xmlReader, string windowName)
    {
        var window = _windowFactory.Create(xmlReader, null);

        // window.Name = windowName;
        //
        // WindowManager.Add(window);

        while (xmlReader.Read())
        {
            if (xmlReader.NodeType == XmlNodeType.Element)
            {
                ReadControl(xmlReader, window);
            }
            else if (xmlReader.NodeType == XmlNodeType.EndElement)
            {
                break;
            }
        }

        return window;
    }

    private void ReadControl(XmlReader xmlReader, Window window)
    {
        if (_controlFactories.TryGetValue(xmlReader.Name, out var factory))
        {
            var control = factory.Create(xmlReader, window);

            // window.Controls.Add(control);
        }

        if (!xmlReader.IsEmptyElement)
        {
            xmlReader.Skip();
        }
    }
}