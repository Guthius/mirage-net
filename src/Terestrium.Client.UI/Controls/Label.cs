using SFML.Graphics;
using SFML.System;

namespace Terestrium.Client.UI.Controls;

public sealed class Label : Control
{
    private static readonly Color DefaultColor = new(192, 224, 255);
    
    private Text[] _lines = [];
    private bool _updateText = true; // ensure first draw renders
    
    // Cache inputs that affect text layout to avoid unnecessary work
    private Font? _lastFont;
    private int _lastFontSize;
    private Vector2i _lastSize;

    private string _text = string.Empty;

    public string Text
    {
        get => _text;
        set
        {
            if (_text == value)
            {
                return;
            }

            _text = value;
            _updateText = true;
        }
    }

    public Color TextColor { get; set; } = DefaultColor;
    public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Left;

    public override void Draw(RenderTarget target, RenderStates states)
    {
        if (!_updateText)
        {
            if (!ReferenceEquals(_lastFont, Font) || _lastFontSize != FontSize || _lastSize != Size)
            {
                _updateText = true;
            }
        }

        if (_updateText)
        {
            UpdateText();
            _updateText = false;
        }

        states.Transform.Translate(Position.X, Position.Y);

        foreach (var line in _lines)
        {
            target.Draw(line, states);
        }
    }

    private void UpdateText()
    {
        var lines = _text.Split('\n');
        
        _lines = new Text[lines.Length];
        if (_lines.Length == 0)
        {
            _lastFont = Font;
            _lastFontSize = FontSize;
            _lastSize = Size;
            return;
        }
        
        var y = 0f;
        
        var height = Size.Y / _lines.Length;
        for (var i = 0; i < lines.Length; i++)
        {
            var text = _lines[i] = new Text();
            
            text.FillColor = TextColor;
            text.CharacterSize = (uint) FontSize;
            text.Font = Font;
            text.DisplayedString = lines[i];

            var size = text.GetLocalBounds();
            
            y += (height - size.Height) / 2 - size.Top - 1;

            text.Position = new Vector2f(GetTextX(size), (int) y);

            y += height;
        }

        _lastFont = Font;
        _lastFontSize = FontSize;
        _lastSize = Size;
    }

    private int GetTextX(FloatRect size)
    {
        return (int) (HorizontalAlignment switch
        {
            HorizontalAlignment.Left => 0f,
            HorizontalAlignment.Center => (Size.X - size.Width) / 2,
            HorizontalAlignment.Right => Size.X - size.Width,
            _ => 0f
        });
    }
}