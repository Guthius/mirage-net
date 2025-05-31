using Mirage.Engine.UI.Controls.Utils;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Mirage.Engine.UI.Controls;

public class HScroll : Control
{
    private const int MinimumThumbSize = 20;

    private readonly Texture _textureNormal = new("Content/Blue_HScroll_Normal.png");
    private readonly Texture _textureHot = new("Content/Blue_HScroll_Hot.png");
    private readonly Texture _textureDisabled = new("Content/Blue_HScroll_Disabled.png");
    private readonly NineSlice _bar = new(4, 2, 4, 2, new IntRect(18, 0, 6, 14));
    private readonly NineSlice _thumb = new(5, 2, 5, 2, new IntRect(0, 0, 18, 14));
    private FloatRect _thumbRect;
    private bool _dragging;
    private bool _hot;

    public event Action? ValueChanged;

    public int MinValue { get; set; } = 0;
    public int MaxValue { get; set; } = 10;
    public int Value { get; set; }

    public override void Draw(RenderTarget target, RenderStates states)
    {
        UpdateBar();
        
        states.Texture = GetTexture();
        states.Transform *= Transform;
        
        target.Draw(_bar, states);

        DrawThumb(target, states);
    }

    private void DrawThumb(RenderTarget target, RenderStates states)
    {
        states.Transform.Translate(
            _thumbRect.Position.X,
            _thumbRect.Position.Y);

        target.Draw(_thumb, states);
    }

    private Texture GetTexture()
    {
        if (!Enabled)
        {
            return _textureDisabled;
        }

        if (_hot || _dragging)
        {
            return _textureHot;
        }

        return _textureNormal;
    }

    private void UpdateBar()
    {
        var range = MaxValue - MinValue;
        var thumbWidth = Math.Max(Width / (range + 1), MinimumThumbSize);
        var availableWidth = Width - thumbWidth;
        var normalizedValue = (float) (Value - MinValue) / range;

        _thumbRect = new FloatRect(normalizedValue * availableWidth, 0, thumbWidth, Height);
        _thumb.Size = new Vector2i(
            (int) _thumbRect.Size.X,
            (int) _thumbRect.Size.Y);

        _bar.Size = new Vector2i(Width, Height);
    }

    protected override void OnMouseEnter()
    {
        base.OnMouseEnter();

        _hot = true;
    }

    protected override void OnMouseLeave()
    {
        base.OnMouseLeave();

        _hot = false;
    }

    protected override void OnMousePressed(int x, int y, Mouse.Button button)
    {
        if (button != Mouse.Button.Left || !Enabled)
        {
            return;
        }

        if (x < _thumbRect.Position.X ||
            y < _thumbRect.Position.Y ||
            x > _thumbRect.Position.X + _thumbRect.Size.X ||
            y > _thumbRect.Position.Y + _thumbRect.Size.Y)
        {
            return;
        }

        _dragging = true;

        CaptureMouse();
    }

    protected override void OnMouseReleased(int x, int y, Mouse.Button button)
    {
        if (button != Mouse.Button.Left)
        {
            return;
        }

        _dragging = false;

        ReleaseMouse();
    }

    protected override void OnMouseMove(int x, int y)
    {
        if (!_dragging)
        {
            return;
        }

        var thumbWidth = _thumbRect.Size.X;
        var availableWidth = Width - thumbWidth;
        var clampedX = Math.Clamp(x - thumbWidth / 2, 0, availableWidth);
        var percentage = clampedX / availableWidth;
        var range = MaxValue - MinValue;

        var newValue = MinValue + (int) Math.Round(percentage * range);
        if (newValue == Value)
        {
            return;
        }

        Value = newValue;

        OnValueChanged();
    }

    protected virtual void OnValueChanged()
    {
        ValueChanged?.Invoke();
    }
}