using Mirage.Engine.UI.Styles;
using SFML.Graphics;
using SFML.Window;

namespace Mirage.Engine.UI.Controls;

public class VScroll(Style style) : Control
{
    private const int MinimumThumbSize = 20;

    private readonly IStylePart? _stylePartBarNormal = style.GetPart("VScroll.BarNormal");
    private readonly IStylePart? _stylePartBarHot = style.GetPart("VScroll.BarHot");
    private readonly IStylePart? _stylePartBarDisabled = style.GetPart("VScroll.BarDisabled");
    private readonly IStylePart? _stylePartTrack = style.GetPart("VScroll.Track");
    private readonly IStylePart? _stylePartTrackDisabled = style.GetPart("VScroll.TrackDisabled");
    private IntRect _thumbRect;
    private bool _dragging;
    private bool _hot;

    public event Action? ValueChanged;

    public int MinValue { get; set; } = 0;
    public int MaxValue { get; set; } = 10;
    public int Value { get; set; }

    public override void Draw(RenderTarget target, RenderStates states)
    {
        UpdateBar();

        states.Transform *= Transform;

        GetActiveTrackStylePart()?.Draw(target, states, Size);

        states.Transform.Translate(_thumbRect.Position.X, _thumbRect.Position.Y);

        GetActiveThumbStylePart()?.Draw(target, states, _thumbRect.Size);
    }

    private IStylePart? GetActiveTrackStylePart()
    {
        return Enabled ? _stylePartTrack : _stylePartTrackDisabled;
    }

    private IStylePart? GetActiveThumbStylePart()
    {
        if (!Enabled)
        {
            return _stylePartBarDisabled;
        }

        if (_hot || _dragging)
        {
            return _stylePartBarHot;
        }

        return _stylePartBarNormal;
    }

    private void UpdateBar()
    {
        var range = MaxValue - MinValue;
        var thumbHeight = Math.Max(Size.Y / (range + 1), MinimumThumbSize);
        var availableHeight = Size.Y - thumbHeight;
        var normalizedValue = (float) (Value - MinValue) / range;

        _thumbRect = new IntRect(0, (int) (normalizedValue * availableHeight), Size.X, thumbHeight);
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
        var availableHeight = Size.Y - thumbHeight;
        var clampedY = Math.Clamp(y - thumbHeight / 2, 0, availableHeight);
        var percentage = clampedY / availableHeight;
        var range = MaxValue - MinValue;

        var newValue = MinValue + percentage * range;
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