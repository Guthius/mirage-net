using Mirage.Engine.UI.Styles;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Mirage.Engine.UI.Controls;

public sealed class Button : Control
{
    private readonly IStylePart? _stylePartNormal;
    private readonly IStylePart? _stylePartHot;
    private readonly IStylePart? _stylePartPressed;
    private readonly IStylePart? _stylePartDisabled;
    private readonly Text _text = new();
    private bool _mousePressed;
    private bool _mouseOver;

    public string Text { get; set; } = string.Empty;

    public event EventHandler? Click;

    public Button(Style style)
    {
        _stylePartNormal = style.GetPart("Button.Normal");
        _stylePartHot = style.GetPart("Button.Hot");
        _stylePartPressed = style.GetPart("Button.Pressed");
        _stylePartDisabled = style.GetPart("Button.Disabled");

        TabStop = true;
    }

    private IStylePart? GetActiveStylePart()
    {
        if (!Enabled || (Parent is not null && !Parent.Enabled))
        {
            return _stylePartDisabled;
        }

        if (_mousePressed)
        {
            return _stylePartPressed;
        }

        if (_mouseOver)
        {
            return _stylePartHot;
        }

        return _stylePartNormal;
    }

    public override void Draw(RenderTarget target, RenderStates states)
    {
        UpdateText();
        
        states.Transform.Translate(Position.X, Position.Y);

        GetActiveStylePart()?.Draw(target, states, Size);

        DrawText(target, states);
        DrawChildren(target, states);
    }

    private void DrawText(RenderTarget target, RenderStates states)
    {
        if (_mousePressed)
        {
            states.Transform.Translate(0, 1);
        }

        target.Draw(_text, states);
    }

    private void UpdateText()
    {
        _text.FillColor = Color.White;
        _text.CharacterSize = (uint) FontSize;
        _text.Font = Font;
        _text.DisplayedString = Text;

        var size = _text.GetLocalBounds();
        var x = (Size.X - size.Width) / 2;
        var y = (Size.Y - _text.CharacterSize) / 2 - 2;

        _text.Position = new Vector2f((int) x, (int) y);
    }

    protected override void OnMouseEnter()
    {
        base.OnMouseEnter();

        _mouseOver = true;
    }

    protected override void OnMouseLeave()
    {
        base.OnMouseLeave();

        _mouseOver = false;
    }

    protected override bool OnMousePressed(int x, int y, Mouse.Button button)
    {
        CaptureMouse();

        _mousePressed = true;
        return true;
    }

    protected override bool OnMouseReleased(int x, int y, Mouse.Button button)
    {
        ReleaseMouse();

        if (x >= 0 && x < Size.X && y >= 0 && y < Size.Y)
        {
            OnClick();
        }

        _mousePressed = false;
        return true;
    }

    private void OnClick()
    {
        Click?.Invoke(this, EventArgs.Empty);
    }
}