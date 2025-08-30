using Mirage.Engine.UI.Styles;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Mirage.Engine.UI.Controls;

public sealed class CheckBox : Control
{
    private static readonly Color Defaultcolor = new(192, 224, 255);

    private readonly Sprite? _spriteNormal;
    private readonly Sprite? _spriteNormalChecked;
    private readonly Sprite? _spriteHot;
    private readonly Sprite? _spriteHotChecked;
    private readonly Text _text = new();
    private bool _mousePressed;
    private bool _mouseOver;

    public string Text { get; set; } = string.Empty;
    public bool Checked { get; set; }

    public event EventHandler? CheckedChanged;

    public CheckBox(Style style)
    {
        _spriteNormal = style.GetSprite("CheckBox.Normal");
        _spriteNormalChecked = style.GetSprite("CheckBox.NormalChecked");
        _spriteHot = style.GetSprite("CheckBox.Hot");
        _spriteHotChecked = style.GetSprite("CheckBox.HotChecked");

        TabStop = true;
    }

    public override void Draw(RenderTarget target, RenderStates states)
    {
        UpdateText();

        states.Transform.Translate(Position.X, Position.Y);
        
        DrawSprite(target, states);

        target.Draw(_text, states);
    }

    private void DrawSprite(RenderTarget target, RenderStates states)
    {
        var sprite = GetActiveSprite();
        if (sprite is null)
        {
            return;
        }

        var y = (Size.Y - sprite.TextureRect.Height) / 2;

        sprite.Position = new Vector2f(0, y);

        target.Draw(sprite, states);
    }

    private Sprite? GetActiveSprite()
    {
        if (_mouseOver || _mousePressed)
        {
            return Checked ? _spriteHotChecked : _spriteHot;
        }

        return Checked ? _spriteNormalChecked : _spriteNormal;
    }

    private void UpdateText()
    {
        _text.FillColor = _mouseOver || _mousePressed ? Color.White : Defaultcolor;
        _text.CharacterSize = (uint) FontSize;
        _text.Font = Font;
        _text.DisplayedString = Text;

        var size = _text.GetLocalBounds();
        var y = (26 - size.Height) / 2 - size.Top;

        _text.Position = new Vector2f(18, (int) y);
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

        if (x >= 0 && x < Size.X && y >= 0 && y < Size.Y)
        {
            Checked = !Checked;
            CheckedChanged?.Invoke(this, EventArgs.Empty);
        }

        _mousePressed = false;
    }
}