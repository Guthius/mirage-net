using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Terestrium.Client.UI.Styles;

namespace Terestrium.Client.UI.Controls;

public class Window(Style style) : Control
{
    // TODO: The height of the title bar should be configured as part of the style.

    private readonly IStylePart? _stylePartFrame = style.GetPart("Window.Frame");
    private readonly IStylePart? _stylePartFrameNoTitleBar = style.GetPart("Window.FrameNoTitleBar");
    private readonly Text _text = new();
    private bool _dragging;
    private Vector2i _dragPos;

    public string Text { get; set; } = string.Empty;
    public bool CanDrag { get; set; } = true;
    public Color? BackColor { get; set; }
    public bool ShowFrame { get; set; } = true;
    public bool ShowTitleBar { get; set; } = true;

    public override void Draw(RenderTarget target, RenderStates states)
    {
        UpdateText();

        states.Transform.Translate(Position.X, Position.Y);

        DrawBackColor(target, states);
        DrawFrame(target, states);
        DrawChildren(target, states);
    }

    private void DrawBackColor(RenderTarget target, RenderStates states)
    {
        if (BackColor is null)
        {
            return;
        }

        var rectangleShape = new RectangleShape();

        rectangleShape.FillColor = BackColor.Value;
        rectangleShape.Size = new Vector2f(Size.X, Size.Y);

        target.Draw(rectangleShape, states);
    }

    private void DrawFrame(RenderTarget target, RenderStates states)
    {
        if (!ShowFrame)
        {
            return;
        }

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


    protected override bool OnMouseMove(int x, int y)
    {
        if (!_dragging)
        {
            return false;
        }

        Position += new Vector2i(x, y) - _dragPos;
        return true;
    }

    protected override bool OnMousePressed(int x, int y, Mouse.Button button)
    {
        if (button != Mouse.Button.Left)
        {
            return false;
        }

        if (!CanDrag || x < 0 || y < 0 || x >= Size.X || y >= 22)
        {
            return false;
        }

        BeginDrag(x, y);
        return true;
    }

    protected override bool OnMouseReleased(int x, int y, Mouse.Button button)
    {
        if (button != Mouse.Button.Left)
        {
            return false;
        }

        EndDrag();
        return true;
    }

    private void BeginDrag(int x, int y)
    {
        _dragging = true;
        _dragPos = new Vector2i(x, y);

        CaptureMouse();
    }

    private void EndDrag()
    {
        _dragging = false;

        ReleaseMouse();
    }

    public void MoveToFront()
    {
        Parent?.MoveToFront(this);
    }

    public void MoveToCenter()
    {
        if (Parent is null)
        {
            return;
        }

        var x = (Parent.Size.X - Size.X) / 2;
        var y = (Parent.Size.Y - Size.Y) / 2;

        Position = new Vector2i(x, y);
    }
}