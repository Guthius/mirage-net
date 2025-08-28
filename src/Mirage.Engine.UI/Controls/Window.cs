using Mirage.Engine.UI.Styles;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Mirage.Engine.UI.Controls;

public class Window(Style style) : Control
{
    private readonly IStylePart? _stylePartFrame = style.GetPart("Window.Frame");
    private readonly IStylePart? _stylePartFrameNoTitleBar = style.GetPart("Window.FrameNoTitleBar");
    private readonly Text _text = new();
    private bool _dragging;
    private Vector2f _dragPos;

    public string Text { get; set; } = string.Empty;
    public bool CanDrag { get; set; } = true;
    public bool ShowTitleBar { get; set; } = true;

    public override void Draw(RenderTarget target, RenderStates states)
    {
        UpdateText();

        states.Transform *= Transform;

        DrawFrame(target, states);
        DrawChildren(target, states);
    }

    private void DrawFrame(RenderTarget target, RenderStates states)
    {
        GetActiveStylePart()?.Draw(target, states, Size);

        target.Draw(_text, states);
    }

    private IStylePart? GetActiveStylePart()
    {
        return ShowTitleBar ? _stylePartFrame : _stylePartFrameNoTitleBar;
    }

    private void UpdateText()
    {
        _text.FillColor = Color.White;
        _text.CharacterSize = (uint) FontSize;
        _text.Font = Font;
        _text.DisplayedString = Text;
        _text.OutlineColor = new Color(18, 56, 132);
        _text.OutlineThickness = 1;

        var size = _text.GetLocalBounds();
        var x = (Size.X - size.Width) / 2;
        var y = (26 - _text.CharacterSize) / 2 - 2;

        _text.Position = new Vector2f((int) x, (int) y);
    }

    protected override void OnMouseMove(int x, int y)
    {
        if (!_dragging)
        {
            return;
        }

        Position += new Vector2f(x, y) - _dragPos;
    }

    protected override void OnMousePressed(int x, int y, Mouse.Button button)
    {
        if (button != Mouse.Button.Left)
        {
            return;
        }

        if (CanDrag && x >= 0 && y >= 0 && x < Size.X && y < 22)
        {
            BeginDrag(x, y);
        }
    }

    protected override void OnMouseReleased(int x, int y, Mouse.Button button)
    {
        if (button != Mouse.Button.Left)
        {
            return;
        }

        EndDrag();
    }

    private void BeginDrag(int x, int y)
    {
        _dragging = true;
        _dragPos = new Vector2f(x, y);

        CaptureMouse();
    }

    private void EndDrag()
    {
        _dragging = false;

        ReleaseMouse();
    }

    public void MoveToCenter()
    {
        if (Parent is null)
        {
            return;
        }

        var x = (Parent.Size.X - Size.X) / 2;
        var y = (Parent.Size.Y - Size.Y) / 2;

        Position = new Vector2f(x, y);
    }
}