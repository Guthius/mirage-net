using SFML.Graphics;
using Terestrium.Client.UI.Styles;

namespace Terestrium.Client.UI.Controls;

public class Panel(Style style) : Control
{
    private readonly IStylePart? _stylePartBackground = style.GetPart("Panel.Normal");

    public override void Draw(RenderTarget target, RenderStates states)
    {
        states.Transform.Translate(Position.X, Position.Y);

        _stylePartBackground?.Draw(target, states, Size);

        DrawChildren(target, states);
    }
}