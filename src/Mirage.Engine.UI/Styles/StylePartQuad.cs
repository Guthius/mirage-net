using SFML.Graphics;
using SFML.System;

namespace Mirage.Engine.UI.Styles;

public sealed class StylePartQuad(Texture texture, IntRect textureRect) : IStylePart
{
    private readonly RectangleShape _sprite = new()
    {
        Texture = texture,
        TextureRect = textureRect
    };

    public void Draw(RenderTarget target, RenderStates states, Vector2i size)
    {
        _sprite.Size = new Vector2f(size.X, size.Y);

        target.Draw(_sprite, states);
    }
}