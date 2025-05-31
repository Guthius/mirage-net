using Mirage.Engine.UI.Styles;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Mirage.Engine.UI.Controls;

public sealed class TextBox : Control
{
    private const double BlinkInterval = 0.5;
    private const int CaretWidth = 2;
    private const int CaretHeight = 14;

    private readonly StylePart? _stylePartNormal;
    private readonly StylePart? _stylePartDisabled;
    private readonly Text _text = new();
    private readonly Clock _clock = new();
    private bool _showCaret;

    public event Action? Submit;

    public string Text { get; set; } = string.Empty;
    public char PasswordChar { get; set; } = '•';
    public bool IsPassword { get; set; }

    public TextBox(Style style)
    {
        _stylePartNormal = style.GetPart("TextBox.Normal");
        _stylePartDisabled = style.GetPart("TextBox.Disabled");
        _clock.Restart();

        TabStop = true;
    }

    private StylePart? GetActiveStylePart()
    {
        if (!Enabled || (Parent is not null && !Parent.Enabled))
        {
            return _stylePartDisabled;
        }

        return _stylePartNormal;
    }

    public override void Draw(RenderTarget target, RenderStates states)
    {
        UpdateText();

        states.Transform *= Transform;

        var part = GetActiveStylePart();
        if (part is not null)
        {
            target.Draw(part, states);
        }

        target.Draw(_text, states);

        DrawCaret(target, states);
    }

    private void Blink()
    {
        if (_clock.ElapsedTime.AsSeconds() < BlinkInterval)
        {
            return;
        }

        _clock.Restart();

        _showCaret = !_showCaret;
    }

    private void DrawCaret(RenderTarget target, RenderStates states)
    {
        if (!Enabled || (Parent is not null && !Parent.Enabled))
        {
            return;
        }

        if (!HasFocus)
        {
            return;
        }

        Blink();
        if (!_showCaret)
        {
            return;
        }

        var size = _text.GetLocalBounds();
        var x = _text.Position.X + size.Width + 2;
        var y = (Height - CaretHeight) / 2;

        var caret = new RectangleShape(new Vector2f(CaretWidth, CaretHeight));

        caret.Position = new Vector2f(x, y);
        caret.FillColor = Color.White;

        target.Draw(caret, states);
    }

    private void UpdateText()
    {
        _text.FillColor = Color.White;
        _text.CharacterSize = (uint) FontSize;
        _text.Font = Font;
        _text.DisplayedString = IsPassword ? new string(PasswordChar, Text.Length) : Text;

        var y = (26 - _text.CharacterSize) / 2 - 3;

        _text.Position = new Vector2f(4, (int) y);
    }

    protected override void OnSizeChanged()
    {
        _stylePartNormal?.Size = new Vector2f(Width, Height);
        _stylePartDisabled?.Size = new Vector2f(Width, Height);
    }

    protected override void OnTextEntered(string character)
    {
        foreach (var ch in character)
        {
            if (char.IsControl(ch))
            {
                continue;
            }

            Text += ch;
        }

        _clock.Restart();
        _showCaret = true;
    }

    protected override void OnKeyPressed(bool control, bool alt, bool shift, Keyboard.Key key)
    {
        switch (key)
        {
            case Keyboard.Key.Enter:
                Submit?.Invoke();
                break;

            case Keyboard.Key.Backspace when Text.Length > 0:
                Text = Text.Remove(Text.Length - 1);
                break;

            default:
                base.OnKeyPressed(control, alt, shift, key);
                break;
        }
    }
}