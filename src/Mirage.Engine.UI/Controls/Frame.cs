using SFML.Graphics;
using SFML.System;

namespace Mirage.Engine.UI.Controls;

public class Frame : Control
{
    public Color BackColor { get; set; } = Color.White;
    public int BorderSize { get; set; } = 1;
    public Color BorderColor { get; set; } = Color.Black;

    public override void Draw(RenderTarget target, RenderStates states)
    {
        states.Transform *= Transform;
        states.Transform.Translate(BorderSize, BorderSize);

        var rectangle = new RectangleShape();

        var borderSize = BorderSize * 2;

        rectangle.Size = new Vector2f(Size.X - borderSize, Size.Y - borderSize);
        rectangle.FillColor = BackColor;
        rectangle.OutlineColor = BorderColor;
        rectangle.OutlineThickness = BorderSize;

        target.Draw(rectangle, states);

        DrawChildren(target, states);
    }
}