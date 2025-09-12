using Mirage.Engine.UI.Styles;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Mirage.Engine.UI.Controls;

public sealed class ComboBox(Style style) : Control
{
    private readonly IStylePart? _stylePartNormal = style.GetPart("ComboBox.Normal");
    private readonly IStylePart? _stylePartDisabled = style.GetPart("ComboBox.Disabled");
    private readonly Sprite? _spriteArrow = style.GetSprite("ComboBox.Arrow");
    private readonly Text _text = new();
    private bool _update = true;
    private Panel? _popup;

    public int ItemHeight { get; set; } = 25;
    public List<object> Items { get; } = [];
    public object? SelectedItem { get; set; }

    private IStylePart? GetActiveStylePart()
    {
        return Enabled ? _stylePartNormal : _stylePartDisabled;
    }

    public override void Draw(RenderTarget target, RenderStates states)
    {
        states.Transform.Translate(Position.X, Position.Y);

        GetActiveStylePart()?.Draw(target, states, Size);

        DrawArrow(target, states);
        DrawSelectedItem(target, states);

        if (_popup is null)
        {
            return;
        }

        target.Draw(_popup, states);
    }

    private void DrawArrow(RenderTarget target, RenderStates states)
    {
        if (_spriteArrow is null)
        {
            return;
        }

        var arrowX = Size.X - _spriteArrow.TextureRect.Width - 4;
        var arrowY = (Size.Y - _spriteArrow.TextureRect.Height) / 2;

        _spriteArrow.Position = new Vector2f(arrowX, arrowY);

        target.Draw(_spriteArrow, states);
    }

    private void DrawSelectedItem(RenderTarget target, RenderStates states)
    {
        if (SelectedItem is null)
        {
            return;
        }

        if (_update)
        {
            UpdateText();

            _update = false;
        }

        target.Draw(_text, states);
    }

    private void UpdateText()
    {
        _text.CharacterSize = (uint) FontSize;
        _text.DisplayedString = SelectedItem?.ToString();
        _text.Font = Font;

        var y = (Size.Y - _text.CharacterSize) / 2;

        _text.Position = new Vector2f(5, y);
    }

    protected override bool OnMousePressed(int x, int y, Mouse.Button button)
    {
        if (button != Mouse.Button.Left)
        {
            return false;
        }

        if (_popup is null)
        {
            OpenDropDown();

            return true;
        }

        ClickDropDown(x - _popup.Position.X, y - _popup.Position.Y);
        return true;
    }

    protected override bool OnMouseMove(int x, int y)
    {
        if (_popup is null)
        {
            return false;
        }

        return _popup.HandleMouseMoved(x, y);
    }

    private void ClickDropDown(int x, int y)
    {
        if (_popup?.GetChildAt(x, y) is ComboBoxItem item)
        {
            SelectItem(item.Value);
        }

        _popup = null;

        ReleaseMouse();
    }

    private void OpenDropDown()
    {
        const int padding = 3;

        _popup = new Panel(style)
        {
            Position = new Vector2i(0, Size.Y),
            Size = new Vector2i(Size.X, Items.Count * ItemHeight + padding * 2)
        };

        var y = padding;
        foreach (var item in Items)
        {
            _popup.Add(new ComboBoxItem(item)
            {
                Position = new Vector2i(padding, y),
                Text = item.ToString() ?? string.Empty,
                Size = new Vector2i(_popup.Size.X - padding * 2, ItemHeight)
            });

            y += ItemHeight;
        }

        CaptureMouse();
    }

    private void SelectItem(object item)
    {
        SelectedItem = item;

        _update = true;
    }

    public sealed class ComboBoxItem(object value) : Control
    {
        private readonly Font _font = new("Content/Fonts/Coolvetica Rg.otf");
        private readonly Text _text = new();
        private bool _update = true;
        private bool _hot;

        public string Text { get; set; } = string.Empty;
        public object Value => value;

        public override void Draw(RenderTarget target, RenderStates states)
        {
            if (_update)
            {
                UpdateText();

                _update = false;
            }

            states.Transform.Translate(Position.X, Position.Y);

            var rectangle = new RectangleShape
            {
                Size = new Vector2f(Size.X, Size.Y),
                FillColor = GetBackColor()
            };

            target.Draw(rectangle, states);

            DrawText(target, states);
            DrawChildren(target, states);
        }

        private Color GetBackColor()
        {
            if (_hot)
            {
                return new Color(255, 255, 255, 40);
            }

            return Color.Transparent;
        }

        private void DrawText(RenderTarget target, RenderStates states)
        {
            target.Draw(_text, states);
        }

        private void UpdateText()
        {
            _text.FillColor = Color.White;
            _text.CharacterSize = 14;
            _text.Font = _font;
            _text.DisplayedString = Text;

            var y = (Size.Y - _text.CharacterSize) / 2 - 2;

            _text.Position = new Vector2f(5, (int) y);
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
    }
}