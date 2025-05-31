using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Mirage.Engine.UI.Controls;

public sealed class RadioButton : Control
{
    private const int CheckBoxSize = 15;

    private static readonly Color Defaultcolor = new(192, 224, 255);

    private readonly Texture _texture = new("Content/Blue_RadioButton.png");
    private readonly Sprite _sprite = new();
    private readonly Font _font = new("Content/Fonts/Coolvetica Rg.otf");
    private readonly Text _text = new();
    private bool _down;
    private bool _hot;

    public string Group { get; set; } = "default";
    public string Text { get; set; } = string.Empty;
    public bool Checked { get; set; }

    public event Action? CheckedChanged;

    public RadioButton()
    {
        TabStop = true;
    }

    public override void Draw(RenderTarget target, RenderStates states)
    {
        UpdateText();
        UpdateSprite();

        states.Transform *= Transform;

        target.Draw(_sprite, states);
        target.Draw(_text, states);
    }

    private void UpdateText()
    {
        _text.FillColor = _hot || _down ? Color.White : Defaultcolor;
        _text.CharacterSize = 14;
        _text.Font = _font;
        _text.DisplayedString = Text;

        var size = _text.GetLocalBounds();
        var y = (26 - size.Height) / 2 - size.Top;

        _text.Position = new Vector2f(18, (int) y);
    }

    private void UpdateSprite()
    {
        var y = (Height - CheckBoxSize) / 2;

        _sprite.Texture = _texture;
        _sprite.TextureRect = new IntRect(0, 
            Checked ? CheckBoxSize : 0, 
            CheckBoxSize, CheckBoxSize);
        _sprite.Position = new Vector2f(0, y);
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
        CaptureMouse();

        _down = true;
    }

    protected override void OnMouseReleased(int x, int y, Mouse.Button button)
    {
        ReleaseMouse();

        if (x >= 0 && x < Width && y >= 0 && y < Height)
        {
            SetChecked(true);
        }

        _down = false;
    }

    private void SetChecked(bool value)
    {
        if (Checked == value)
        {
            return;
        }
        
        Checked = value;
        CheckedChanged?.Invoke();

        if (!Checked || Parent is null)
        {
            return;
        }

        var otherRadioButtonsInGroup = Parent
            .Children<RadioButton>()
            .Where(radioButton =>
                radioButton != this &&
                radioButton.Group == Group);

        foreach (var radioButton in otherRadioButtonsInGroup)
        {
            radioButton.SetChecked(false);
        }
    }
}