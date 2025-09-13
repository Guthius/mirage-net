using SFML.Graphics;

namespace Terestrium.Client.UI.Styles;

public sealed class StylePartBuilder(Texture texture, IntRect textureRect, IntRect? ninePatchRect)
{
    public IStylePart Build()
    {
        if (ninePatchRect is null)
        {
            return new StylePartQuad(texture, textureRect);
        }
        
        return new StylePartNineSliced(texture,  textureRect, ninePatchRect.Value);
    }
}