using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Terestrium.Client.UI.Controls;

public sealed class ListBox : Control
{
    private readonly List<string> _items = [];
    private readonly Text _text = new();
    private int _selectedIndex = -1;

    public event Action<int>? SelectedIndexChanged;

    public IReadOnlyList<string> Items => _items;
    public int ItemHeight { get; set; } = 18;

    public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            if (_selectedIndex == value)
            {
                return;
            }

            _selectedIndex = value;

            SelectedIndexChanged?.Invoke(_selectedIndex);
        }
    }

    public void SetItems(IEnumerable<string> items, int selectedIndex = -1)
    {
        _items.Clear();
        _items.AddRange(items);

        if (selectedIndex >= -1 && selectedIndex < _items.Count)
        {
            _selectedIndex = selectedIndex;
        }
        else if (_items.Count == 0)
        {
            _selectedIndex = -1;
        }
        else if (_selectedIndex >= _items.Count)
        {
            _selectedIndex = _items.Count - 1;
        }
    }

    public void AddItem(string item)
    {
        _items.Add(item);
        if (_selectedIndex < 0) _selectedIndex = 0;
    }

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= _items.Count)
        {
            return;
        }

        _items.RemoveAt(index);

        if (_items.Count == 0)
        {
            _selectedIndex = -1;
        }
        else if (_selectedIndex >= _items.Count)
        {
            _selectedIndex = _items.Count - 1;
        }
    }

    public override void Draw(RenderTarget target, RenderStates states)
    {
        states.Transform.Translate(Position.X, Position.Y);

        // Background
        var background = new RectangleShape(new Vector2f(Size.X, Size.Y))
        {
            FillColor = new Color(20, 20, 20, 200)
        };
        target.Draw(background, states);

        // Border
        var border = new RectangleShape(new Vector2f(Size.X, Size.Y))
        {
            FillColor = Color.Transparent,
            OutlineColor = new Color(80, 80, 80),
            OutlineThickness = 1
        };
        target.Draw(border, states);

        // Text setup
        _text.Font = Font;
        _text.CharacterSize = (uint) FontSize;
        _text.FillColor = Color.White;

        var itemHeight = Math.Max(ItemHeight, FontSize + 6);
        var visibleCount = Size.Y / itemHeight;
        var count = Math.Min(visibleCount, _items.Count);

        for (var i = 0; i < count; i++)
        {
            var y = i * itemHeight;
            var selected = i == _selectedIndex;

            if (selected)
            {
                var selectedBackground = new RectangleShape(new Vector2f(Size.X - 2, itemHeight - 2))
                {
                    Position = new Vector2f(1, y + 1),
                    FillColor = new Color(60, 90, 150, 180)
                };

                target.Draw(selectedBackground, states);
            }

            _text.DisplayedString = _items[i];
            _text.Position = new Vector2f(6, y + (itemHeight - FontSize) / 2 - 2);

            target.Draw(_text, states);
        }
    }

    protected override bool OnMousePressed(int x, int y, Mouse.Button button)
    {
        if (button != Mouse.Button.Left)
        {
            return false;
        }

        var itemHeight = Math.Max(ItemHeight, FontSize + 6);

        var index = y / itemHeight;
        if (index >= 0 && index < _items.Count)
        {
            SelectedIndex = index;
        }

        return true;
    }
}