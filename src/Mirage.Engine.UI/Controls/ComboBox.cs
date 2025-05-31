using Mirage.Engine.UI.Controls.Utils;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Mirage.Engine.UI.Controls;

public sealed class ComboBox : Control
{
    private readonly Texture _texture = new("Content/Blue_TextBox.png");
    private readonly Texture _textureDisabled = new("Content/Blue_TextBox_Disabled.png");
    private readonly NineSlice _background = new(3, 3, 3, 3, new IntRect(0, 0, 12, 12));
    private readonly Font _font = new("Content/Fonts/Coolvetica Rg.otf");
    private readonly Texture _dropDownArrowTexture = new("Content/Blue_DropDown_Arrow.png");
    private readonly Sprite _dropDownArrowSprite;
    private readonly Text _text = new();
    private bool _update = true;
    private Panel? _popup;

    public int ItemHeight { get; set; } = 25;
    public List<object> Items { get; } = [];
    public object? SelectedItem { get; set; }

    public ComboBox()
    {
        _dropDownArrowSprite = new Sprite();
        _dropDownArrowSprite.Texture = _dropDownArrowTexture;
    }

    public override void Draw(RenderTarget target, RenderStates states)
    {
        states.Texture = _texture;
        states.Transform *= Transform;

        var arrowX = Width - _dropDownArrowTexture.Size.X - 4;
        var arrowY = (Height - _dropDownArrowTexture.Size.Y) / 2;

        _dropDownArrowSprite.Position = new Vector2f(arrowX, arrowY);

        target.Draw(_background, states);
        target.Draw(_dropDownArrowSprite, states);

        DrawSelectedItem(target, states);

        if (_popup is null)
        {
            return;
        }

        target.Draw(_popup, states);
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
        _text.CharacterSize = 14;
        _text.DisplayedString = SelectedItem?.ToString();
        _text.Font = _font;

        var y = (Height - _text.CharacterSize) / 2;

        _text.Position = new Vector2f(5, y);
    }

    protected override void OnSizeChanged()
    {
        _background.Size = new Vector2i(Width, Height);
    }

    protected override void OnMousePressed(int x, int y, Mouse.Button button)
    {
        if (button != Mouse.Button.Left)
        {
            return;
        }

        if (_popup is null)
        {
            OpenDropDown();

            return;
        }

        ClickDropDown(
            x - (int) _popup.Position.X,
            y - (int) _popup.Position.Y);
    }

    protected override void OnMouseMove(int x, int y)
    {
        _popup?.HandleMouseMoved(x, y);
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

        _popup = new Panel
        {
            Position = new Vector2f(0, Height),
            Width = Width,
            Height = Items.Count * ItemHeight + padding * 2
        };

        var y = padding;
        foreach (var item in Items)
        {
            _popup.Add(new ComboBoxItem(item)
            {
                Position = new Vector2f(padding, y),
                Text = item.ToString() ?? string.Empty,
                Width = _popup.Width - padding * 2,
                Height = ItemHeight,
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

            states.Transform *= Transform;

            var rectangle = new RectangleShape
            {
                Size = new Vector2f(Width, Height),
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

            var y = (Height - _text.CharacterSize) / 2 - 2;

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