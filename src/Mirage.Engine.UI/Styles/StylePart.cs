using SFML.Graphics;
using SFML.System;

namespace Mirage.Engine.UI.Styles;

public sealed class StylePart(string name, Texture texture, IntRect textureRect, IntRect? ninePatchRect) : Drawable
{
    private const int VertexCount = 24;

    private static readonly int[] VertexOrder = [0, 4, 1, 5, 2, 6, 3, 7, 7, 11, 6, 10, 5, 9, 4, 8, 8, 12, 9, 13, 10, 14, 11, 15];

    private readonly VertexArray _vertices = new(PrimitiveType.TriangleStrip, VertexCount);
    private bool _mustUpdate = true;
    
    public override string ToString()
    {
        return name;
    }
    
    public Vector2f Size
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            field = value;

            _mustUpdate = true;
        }
    }

    public void Draw(RenderTarget target, RenderStates states)
    {
        if (_mustUpdate)
        {
            Update();

            _mustUpdate = false;
        }

        states.Texture = texture;

        target.Draw(_vertices, states);
    }

    private void Update()
    {
        var (left, top, width, height) = ninePatchRect ?? new IntRect(0, 0, textureRect.Width, textureRect.Height);
        
        var right = textureRect.Width - left - width;
        var bottom = textureRect.Height - top - height;

        var vertices = new Vector2f[32];

        var positionXs = new[] {0, left, Size.X - right, Size.X};
        var positionYs = new[] {0, top, Size.Y - bottom, Size.Y};

        var textureXs = new[]
        {
            textureRect.Left,
            textureRect.Left + left,
            textureRect.Left + textureRect.Width - right,
            textureRect.Left + textureRect.Width
        };

        var textureYs = new[]
        {
            textureRect.Top,
            textureRect.Top + top,
            textureRect.Top + textureRect.Height - bottom,
            textureRect.Top + textureRect.Height
        };

        for (var x = 0; x < 4; x++)
        {
            for (var y = 0; y < 4; y++)
            {
                vertices[y * 4 + x] = new Vector2f(positionXs[x], positionYs[y]);
                vertices[16 + y * 4 + x] = new Vector2f(textureXs[x], textureYs[y]);
            }
        }

        for (var i = 0; i < VertexCount; i++)
        {
            _vertices[(uint) i] = _vertices[(uint) i] with
            {
                Position = vertices[VertexOrder[i]],
                TexCoords = vertices[16 + VertexOrder[i]]
            };
        }
    }
}