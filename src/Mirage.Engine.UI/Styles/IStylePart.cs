using SFML.Graphics;
using SFML.System;

namespace Mirage.Engine.UI.Styles;

public interface IStylePart
{
    void Draw(RenderTarget target, RenderStates states, Vector2i size);
}