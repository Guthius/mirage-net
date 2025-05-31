using Mirage.Engine.UI.Styles;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Mirage.Engine.UI.Controls;

public sealed class Button : Control
{
    private readonly StylePart? _stylePartNormal;
    private readonly StylePart? _stylePartHot;
    private readonly StylePart? _stylePartPressed;
    private readonly StylePart? _stylePartDisabled;
    private readonly Text _text = new();
    private bool _mousePressed;
    private bool _mouseOver;

    public string Text { get; set; } = string.Empty;

    public event Action? Click;

    public Button(Style style)
    {
        _stylePartNormal = style.GetPart("Button.Normal");
        _stylePartHot = style.GetPart("Button.Hot");
        _stylePartPressed = style.GetPart("Button.Pressed");
        _stylePartDisabled = style.GetPart("Button.Disabled");

        TabStop = true;
    }

    private StylePart? GetActiveStylePart()
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

        states.Transform *= Transform;

        var stylePart = GetActiveStylePart();
        if (stylePart is not null)
        {
            target.Draw(stylePart, states);
        }

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
        var x = (Width - size.Width) / 2;
        var y = (Height - _text.CharacterSize) / 2 - 2;

        _text.Position = new Vector2f((int) x, (int) y);
    }

    protected override void OnSizeChanged()
    {
        _stylePartNormal?.Size = new Vector2f(Width, Height);
        _stylePartHot?.Size = new Vector2f(Width, Height);
        _stylePartPressed?.Size = new Vector2f(Width, Height);
        _stylePartDisabled?.Size = new Vector2f(Width, Height);
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

    protected override void OnMousePressed(int x, int y, Mouse.Button button)
    {
        CaptureMouse();

        _mousePressed = true;
    }

    protected override void OnMouseReleased(int x, int y, Mouse.Button button)
    {
        ReleaseMouse();

        if (x >= 0 && x < Width && y >= 0 && y < Height)
        {
            OnClick();
        }

        _mousePressed = false;
    }

    private void OnClick()
    {
        Click?.Invoke();
    }
}