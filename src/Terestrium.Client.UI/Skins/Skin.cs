using System.Xml;
using SFML.Graphics;
using Terestrium.Client.UI.Styles;

namespace Terestrium.Client.UI.Skins;

public sealed class Skin : ISkin
{
    private readonly Texture _emptyTexture = new(1, 1);
    private readonly Lock _textureLock = new();
    private readonly Dictionary<string, Texture> _textures = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Font> _fonts = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> _properties = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Style> _styles = new(StringComparer.OrdinalIgnoreCase);

    public string Path { get; }
    public string DefaultStyleName => GetPropertyString("DefaultStyle", "Blue");
    
    public Skin(string path)
    {
        Path = path;

        Load();
    }

    private void Load()
    {
        var path = System.IO.Path.Combine(Path, "Skin.xml");
        if (!File.Exists(path))
        {
            return;
        }

        using var fileStream = File.OpenRead(path);
        using var xmlReader = XmlReader.Create(fileStream, new XmlReaderSettings
        {
            IgnoreWhitespace = true,
            IgnoreComments = true
        });

        xmlReader.MoveToContent();
        if (xmlReader.NodeType != XmlNodeType.Element || xmlReader.Name != "Skin")
        {
            throw new XmlException("Skin file is missing root 'Skin' element.");
        }

        while (xmlReader.Read())
        {
            if (xmlReader.NodeType == XmlNodeType.Element)
            {
                switch (xmlReader.Name)
                {
                    case "Fonts":
                        ReadFonts(xmlReader);
                        break;

                    case "Properties":
                        ReadProperties(xmlReader);
                        break;

                    default:
                        if (!xmlReader.IsEmptyElement)
                        {
                            xmlReader.Skip();
                        }

                        break;
                }
            }
            else if (xmlReader.NodeType == XmlNodeType.EndElement)
            {
                break;
            }
        }
    }

    private void ReadFonts(XmlReader xmlReader)
    {
        while (xmlReader.Read())
        {
            if (xmlReader.NodeType == XmlNodeType.Element)
            {
                switch (xmlReader.Name)
                {
                    case "Font":
                        ReadFont(xmlReader);
                        break;
                }

                if (!xmlReader.IsEmptyElement)
                {
                    xmlReader.Skip();
                }
            }
            else if (xmlReader.NodeType == XmlNodeType.EndElement)
            {
                break;
            }
        }
    }

    private void ReadFont(XmlReader xmlReader)
    {
        var fontName = xmlReader.GetAttribute("Name");
        if (string.IsNullOrEmpty(fontName))
        {
            return;
        }

        var path = xmlReader.GetAttribute("Path");
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        path = System.IO.Path.Combine(Path, "Fonts", path);
        try
        {
            var font = new Font(path);

            var smoothAttr = xmlReader.GetAttribute("Smooth");
            if (!string.IsNullOrEmpty(smoothAttr) && bool.TryParse(smoothAttr, out var smooth))
            {
                font.SetSmooth(smooth);
            }

            _fonts[fontName] = font;
        }
        catch
        {
            // ignored
        }
    }

    private void ReadProperties(XmlReader xmlReader)
    {
        while (xmlReader.Read())
        {
            if (xmlReader.NodeType == XmlNodeType.Element)
            {
                switch (xmlReader.Name)
                {
                    case "Property":
                        ReadProperty(xmlReader);
                        break;
                }

                if (!xmlReader.IsEmptyElement)
                {
                    xmlReader.Skip();
                }
            }
            else if (xmlReader.NodeType == XmlNodeType.EndElement)
            {
                break;
            }
        }
    }

    private void ReadProperty(XmlReader xmlReader)
    {
        var key = xmlReader.GetAttribute("Key");
        if (string.IsNullOrEmpty(key))
        {
            return;
        }

        var value = xmlReader.GetAttribute("Value") ?? string.Empty;

        _properties[key] = value;
    }

    public Texture GetTexture(string textureName)
    {
        var path = System.IO.Path.Combine(Path, "Textures", textureName);

        if (!File.Exists(path))
        {
            throw new XmlException($"Unable to load style texture '{path}'.");
        }

        lock (_textureLock)
        {
            if (_textures.TryGetValue(path, out var texture))
            {
                return texture;
            }

            texture = LoadTexture(path);

            _textures.Add(path, texture);

            return texture;
        }
    }

    private Texture LoadTexture(string path)
    {
        try
        {
            return new Texture(path);
        }
        catch
        {
            return _emptyTexture;
        }
    }

    public Style GetStyle(string styleName)
    {
        if (_styles.TryGetValue(styleName, out var style))
        {
            return style;
        }

        var path = System.IO.Path.Combine(Path, "Styles", styleName + ".xml");
        if (!File.Exists(path))
        {
            throw new XmlException(
                $"Unable to load style '{styleName}'. " +
                $"File '{path}' not found.");
        }

        using var fileStream = File.OpenRead(path);
        using var xmlReader = XmlReader.Create(fileStream, new XmlReaderSettings
        {
            IgnoreWhitespace = true,
            IgnoreComments = true
        });

        xmlReader.MoveToContent();
        if (xmlReader.NodeType != XmlNodeType.Element || xmlReader.Name != "Style")
        {
            throw new XmlException("Style file is missing root 'Style' element.");
        }

        var texturePath = xmlReader.GetAttribute("Texture");
        if (string.IsNullOrEmpty(texturePath))
        {
            throw new XmlException("Style definition is missing 'Texture' attribute.");
        }

        var texture = GetTexture(texturePath);

        var parts = new Dictionary<string, StylePartBuilder>(StringComparer.OrdinalIgnoreCase);
        var sprites = new Dictionary<string, Sprite>(StringComparer.OrdinalIgnoreCase);
        var properties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        while (xmlReader.Read())
        {
            if (xmlReader.NodeType == XmlNodeType.Element)
            {
                switch (xmlReader.Name)
                {
                    case "Part":
                        ReadStylePartBuilder(xmlReader, texture, parts);
                        break;

                    case "Sprite":
                        ReadSprite(xmlReader, texture, sprites);
                        break;

                    case "Property":
                        ReadProperty(xmlReader, properties);
                        break;
                }

                if (!xmlReader.IsEmptyElement)
                {
                    xmlReader.Skip();
                }
            }
            else if (xmlReader.NodeType == XmlNodeType.EndElement)
            {
                break;
            }
        }

        _styles[styleName] = style = new Style(parts, sprites, properties);

        return style;
    }

    private static void ReadStylePartBuilder(XmlReader xmlReader, Texture texture, Dictionary<string, StylePartBuilder> dictionary)
    {
        var name = xmlReader.GetAttribute("Name");
        if (string.IsNullOrEmpty(name))
        {
            throw new XmlException("Style part is missing 'Name' attribute.");
        }

        var textureRect = ReadIntRect(xmlReader, "TextureRect") ?? new IntRect(0, 0, (int) texture.Size.X, (int) texture.Size.Y);
        var ninePatchRect = ReadIntRect(xmlReader, "NinePatchRect");

        dictionary[name] = new StylePartBuilder(texture, textureRect, ninePatchRect);
    }

    private static void ReadSprite(XmlReader xmlReader, Texture texture, Dictionary<string, Sprite> dictionary)
    {
        var name = xmlReader.GetAttribute("Name");
        if (string.IsNullOrEmpty(name))
        {
            throw new XmlException("Style sprite is missing 'Name' attribute.");
        }

        var textureRect = ReadIntRect(xmlReader, "TextureRect") ?? new IntRect(0, 0, (int) texture.Size.X, (int) texture.Size.Y);

        dictionary[name] = new Sprite(texture, textureRect);
    }

    private static void ReadProperty(XmlReader xmlReader, Dictionary<string, string> dictionary)
    {
        var key = xmlReader.GetAttribute("Name");
        if (string.IsNullOrEmpty(key))
        {
            throw new XmlException("Property is missing 'Name' attribute.");
        }

        var value = xmlReader.GetAttribute("Value");
        if (string.IsNullOrEmpty(value))
        {
            throw new XmlException($"Property '{key}' is missing 'Value' attribute.");
        }

        dictionary[key] = value;
    }

    private static IntRect? ReadIntRect(XmlReader xmlReader, string attributeName)
    {
        var value = xmlReader.GetAttribute(attributeName);
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }

        var tokens = value.Split(',');
        if (tokens.Length != 4)
        {
            return null;
        }

        if (!int.TryParse(tokens[0], out var x) ||
            !int.TryParse(tokens[1], out var y) ||
            !int.TryParse(tokens[2], out var width) ||
            !int.TryParse(tokens[3], out var height))
        {
            return null;
        }

        return new IntRect(x, y, width, height);
    }

    public Font? GetFont(string fontName)
    {
        return _fonts.GetValueOrDefault(fontName);
    }

    public string GetPropertyString(string key, string defaultValue)
    {
        return _properties.GetValueOrDefault(key, defaultValue);
    }

    public int GetPropertyInt32(string key, int defaultValue)
    {
        return int.TryParse(GetPropertyString(key, string.Empty), out var value) ? value : defaultValue;
    }
}