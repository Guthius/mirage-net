using Mirage.Engine.UI.Controls.Utils;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Mirage.Engine.UI.Controls;

public class VScroll : Control
{
    private const int MinimumThumbSize = 20;

    private readonly Texture _textureNormal = new("Content/Blue_VScroll_Normal.png");
    private readonly Texture _textureHot = new("Content/Blue_VScroll_Hot.png");
    private readonly Texture _textureDisabled = new("Content/Blue_VScroll_Disabled.png");
    private readonly NineSlice _bar = new(2, 4, 2, 4, new IntRect(0, 18, 14, 6));
    private readonly NineSlice _thumb = new(2, 5, 2, 5, new IntRect(0, 0, 14, 18));
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
        var thumbHeight = Math.Max(Height / (range + 1), MinimumThumbSize);
        var availableHeight = Height - thumbHeight;
        var normalizedValue = (float)(Value - MinValue) / range;

        _thumbRect = new FloatRect(0, normalizedValue * availableHeight, Width, thumbHeight);
        _thumb.Size = new Vector2i(
            (int)_thumbRect.Size.X,
            (int)_thumbRect.Size.Y);

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

        var thumbHeight = _thumbRect.Size.Y;
        var availableHeight = Height - thumbHeight;
        var clampedY = Math.Clamp(y - thumbHeight / 2, 0, availableHeight);
        var percentage = clampedY / availableHeight;
        var range = MaxValue - MinValue;

        var newValue = MinValue + (int)Math.Round(percentage * range);
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