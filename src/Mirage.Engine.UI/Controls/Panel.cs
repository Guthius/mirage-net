using Mirage.Engine.UI.Controls.Utils;
using SFML.Graphics;
using SFML.System;

namespace Mirage.Engine.UI.Controls;

public class Panel : Control
{
    private readonly Texture _texture = new("Content/Blue_TextBox.png");
    private readonly NineSlice _background = new(3, 3, 3, 3, new IntRect(0, 0, 12, 12));

    public override void Draw(RenderTarget target, RenderStates states)
    {
        states.Texture = _texture;
        states.Transform *= Transform;
        
        target.Draw(_background, states);

        DrawChildren(target, states);
    }

    protected override void OnSizeChanged()
    {
        _background.Size = new Vector2i(Width, Height);
    }
}