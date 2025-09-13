using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Terestrium.Client.UI.Styles;

namespace Terestrium.Client.UI.Controls;

public sealed class TextBox : Control
{
    private const double BlinkInterval = 0.5;
    private const int CaretWidth = 2;

    private readonly IStylePart? _stylePartNormal;
    private readonly IStylePart? _stylePartDisabled;
    private readonly Text _text = new();
    private readonly Clock _clock = new();
    private bool _showCaret;
    private int _caretIndex;
    private int? _selectionAnchor;
    private bool _isSelecting;

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

    private IStylePart? GetActiveStylePart()
    {
        if (!Enabled || (Parent is not null && !Parent.Enabled))
        {
            return _stylePartDisabled;
        }

        return _stylePartNormal;
    }

    private string Displayed => IsPassword ? new string(PasswordChar, Text.Length) : Text;

    private bool HasSelection => _selectionAnchor.HasValue && _selectionAnchor.Value != _caretIndex;
    private int SelectionStart => HasSelection ? Math.Min(_caretIndex, _selectionAnchor!.Value) : _caretIndex;
    private int SelectionEnd => HasSelection ? Math.Max(_caretIndex, _selectionAnchor!.Value) : _caretIndex;

    public override void Draw(RenderTarget target, RenderStates states)
    {
        UpdateText();

        states.Transform.Translate(Position.X, Position.Y);

        GetActiveStylePart()?.Draw(target, states, Size);

        // Draw selection highlight behind the text
        DrawSelection(target, states);

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

    private int GetCaretXForIndex(int index)
    {
        index = Math.Clamp(index, 0, Text.Length);

        // Left padding matches _text.Position.X
        var baseX = (int) _text.Position.X;
        if (index == 0)
        {
            return baseX;
        }

        var width = MeasureSubstringWidth(index);
        return baseX + (int) Math.Round(width);
    }

    private float MeasureSubstringWidth(int length)
    {
        length = Math.Clamp(length, 0, Text.Length);
        var s = Displayed.Substring(0, length);
        // Construct a temp Text with same font/size to measure
        var t = new Text(s, Font, (uint) FontSize)
        {
            CharacterSize = (uint) FontSize
        };
        var bounds = t.GetLocalBounds();
        return bounds.Width;
    }

    private int GetIndexFromX(int x)
    {
        // x is in local coords relative to control; compare against text origin
        var localX = x - (int) _text.Position.X;
        if (localX <= 0)
        {
            return 0;
        }

        // Binary search over indices for efficiency
        int lo = 0, hi = Text.Length;
        while (lo < hi)
        {
            var mid = (lo + hi) / 2;
            var w = MeasureSubstringWidth(mid);
            if (w < localX)
            {
                lo = mid + 1;
            }
            else
            {
                hi = mid;
            }
        }

        // Place caret either before or after char depending on proximity
        var beforeWidth = MeasureSubstringWidth(lo - 1);
        var atWidth = MeasureSubstringWidth(lo);
        var distBefore = Math.Abs(localX - beforeWidth);
        var distAfter = Math.Abs(atWidth - localX);
        if (lo > 0 && distBefore < distAfter)
        {
            return lo - 1;
        }

        return Math.Clamp(lo, 0, Text.Length);
    }

    private void DrawSelection(RenderTarget target, RenderStates states)
    {
        if (!HasSelection)
        {
            return;
        }

        var start = SelectionStart;
        var end = SelectionEnd;
        var x1 = GetCaretXForIndex(start);
        var x2 = GetCaretXForIndex(end);
        var w = Math.Max(1, x2 - x1);

        var caretHeight = (int) Math.Min(Size.Y - 4, Math.Max(2, Math.Round(FontSize * 0.9)));
        var y = (Size.Y - caretHeight) / 2;

        var rect = new RectangleShape(new Vector2f(w, caretHeight))
        {
            Position = new Vector2f(x1, y),
            FillColor = new Color(80, 120, 220, 140)
        };
        target.Draw(rect, states);
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

        var x = GetCaretXForIndex(_caretIndex);
        var caretHeight = (int) Math.Min(Size.Y - 4, Math.Max(2, Math.Round(FontSize * 0.9)));
        var y = (Size.Y - caretHeight) / 2;

        var caret = new RectangleShape(new Vector2f(CaretWidth, caretHeight))
        {
            Position = new Vector2f(x, y),
            FillColor = Color.White
        };

        target.Draw(caret, states);
    }

    private void UpdateText()
    {
        _text.FillColor = Color.White;
        _text.CharacterSize = (uint) FontSize;
        _text.Font = Font;
        _text.DisplayedString = Displayed;

        // Vertically center within control height with slight offset
        var y = (Size.Y - _text.CharacterSize) / 2 - 2;

        _text.Position = new Vector2f(4, (int) y);
    }

    protected override bool OnTextEntered(string character)
    {
        foreach (var ch in character)
        {
            if (char.IsControl(ch))
            {
                continue;
            }

            InsertText(ch.ToString());
        }

        _clock.Restart();
        _showCaret = true;
        return true;
    }

    private void InsertText(string? text)
    {
        DeleteSelectionIfAny();

        text ??= string.Empty;
        if (text.Length == 0)
        {
            return;
        }

        _caretIndex = Math.Clamp(_caretIndex, 0, Text.Length);

        Text = Text.Insert(_caretIndex, text);

        _caretIndex += text.Length;

        ClearSelection();
    }

    private void DeleteSelectionIfAny()
    {
        if (!HasSelection)
        {
            return;
        }

        var start = Math.Clamp(SelectionStart, 0, Text.Length);
        var end = Math.Clamp(SelectionEnd, 0, Text.Length);

        var len = end - start;
        if (len <= 0)
        {
            ClearSelection();
            return;
        }

        Text = Text.Remove(start, len);

        _caretIndex = start;

        ClearSelection();
    }

    private void ClearSelection()
    {
        _selectionAnchor = null;
    }

    protected override bool OnKeyPressed(bool control, bool alt, bool shift, Keyboard.Key key)
    {
        switch (key)
        {
            case Keyboard.Key.Enter:
                Submit?.Invoke();
                return true;

            case Keyboard.Key.Backspace:
                if (HasSelection)
                {
                    DeleteSelectionIfAny();
                }
                else if (_caretIndex > 0)
                {
                    Text = Text.Remove(_caretIndex - 1, 1);
                    
                    _caretIndex--;
                    
                    ClearSelection();
                }

                ResetBlink();
                return true;

            case Keyboard.Key.Delete:
                if (HasSelection)
                {
                    DeleteSelectionIfAny();
                }
                else if (_caretIndex < Text.Length)
                {
                    Text = Text.Remove(_caretIndex, 1);
                    
                    ClearSelection();
                }

                ResetBlink();
                return true;

            case Keyboard.Key.Left:
                MoveCaret(-1, shift);
                return true;

            case Keyboard.Key.Right:
                MoveCaret(1, shift);
                return true;

            case Keyboard.Key.Home:
                SetCaret(0, shift);
                return true;

            case Keyboard.Key.End:
                SetCaret(Text.Length, shift);
                return true;
        }

        if (control)
        {
            switch (key)
            {
                case Keyboard.Key.A: // Select all
                    _selectionAnchor = 0;
                    _caretIndex = Text.Length;
                    ResetBlink();
                    return true;

                case Keyboard.Key.C:
                    CopySelectionToClipboard();
                    return true;

                case Keyboard.Key.X:
                    CopySelectionToClipboard();
                    DeleteSelectionIfAny();
                    ResetBlink();
                    return true;

                case Keyboard.Key.V:
                    PasteFromClipboard();
                    ResetBlink();
                    return true;
            }
        }

        return base.OnKeyPressed(control, alt, shift, key);
    }

    private void ResetBlink()
    {
        _clock.Restart();
        _showCaret = true;
    }

    private void MoveCaret(int delta, bool keepSelection)
    {
        var newIndex = Math.Clamp(_caretIndex + delta, 0, Text.Length);

        SetCaret(newIndex, keepSelection);
    }

    private void SetCaret(int index, bool keepSelection)
    {
        index = Math.Clamp(index, 0, Text.Length);
        if (keepSelection)
        {
            _selectionAnchor ??= _caretIndex;
        }
        else
        {
            ClearSelection();
        }

        _caretIndex = index;

        ResetBlink();
    }

    private void CopySelectionToClipboard()
    {
        if (!HasSelection)
        {
            return;
        }

        var start = SelectionStart;
        var len = SelectionEnd - start;
        var text = Text.Substring(start, len);

        Clipboard.Contents = text;
    }

    private void PasteFromClipboard()
    {
        var s = Clipboard.Contents ?? string.Empty;
        if (s.Length == 0)
        {
            return;
        }

        InsertText(s);
    }

    protected override bool OnMousePressed(int x, int y, Mouse.Button button)
    {
        if (button != Mouse.Button.Left)
        {
            return false;
        }

        Focus();

        var index = GetIndexFromX(x);

        _caretIndex = index;
        _selectionAnchor = index;
        _isSelecting = true;

        CaptureMouse();
        ResetBlink();
        return true;
    }

    protected override bool OnMouseReleased(int x, int y, Mouse.Button button)
    {
        if (button != Mouse.Button.Left)
        {
            return false;
        }

        _isSelecting = false;

        ReleaseMouse();
        ResetBlink();
        return true;
    }

    protected override bool OnMouseMove(int x, int y)
    {
        if (!_isSelecting)
        {
            return false;
        }

        var index = GetIndexFromX(x);
        if (_caretIndex == index)
        {
            return true;
        }

        _caretIndex = index;
        ResetBlink();
        return true;
    }

    protected override void OnFocus()
    {
        _caretIndex = Math.Clamp(_caretIndex, 0, Text.Length);
        if (_caretIndex == 0 && Text.Length > 0)
        {
            _caretIndex = Text.Length;
        }

        ClearSelection();
        ResetBlink();
    }

    protected override void OnBlur()
    {
        base.OnBlur();

        _showCaret = false;
        _isSelecting = false;

        ClearSelection();
    }
}