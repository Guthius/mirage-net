using Mirage.Engine.UI.Styles;
using SFML.Graphics;

namespace Mirage.Engine.UI.Controls;

public class Panel(Style style) : Control
{
    private readonly IStylePart? _stylePartBackground = style.GetPart("Panel.Normal");

    public override void Draw(RenderTarget target, RenderStates states)
    {
        states.Transform *= Transform;

        _stylePartBackground?.Draw(target, states, Size);

        DrawChildren(target, states);
    }
}