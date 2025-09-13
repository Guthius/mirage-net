using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Terestrium.Client.Core.Scenes;
using Terestrium.Client.UI.Controls;

namespace Terestrium.Client.Scenes.Editor;

public sealed class EditorScene : Scene, IEditorScene
{
    private const int TileSize = 32;
    private const float MinZoom = 0.25f;
    private const float MaxZoom = 4f;
    private const float ZoomStep = 1.1f;

    private enum EditMode
    {
        Paint,
        Block
    }

    private const int ToolbarHeight = 40;
    private const int SidebarWidth = 300;

    private readonly List<EditorLayer> _layers = [];
    private int _mapWidth = 30;
    private int _mapHeight = 30;
    private Texture? _tilesetTexture;
    private int _tilesetColumns;
    private bool _isPanningMap;
    private Vector2i _lastPanPoint;
    private Vector2f _mapOffset;
    private int _selectedTileIndex = -1;
    private int _brushCols = 1;
    private int _brushRows = 1;
    private int[] _brushIndices = [];
    private bool _mouseOverMap;
    private int _hoverTileX;
    private int _hoverTileY;
    private bool _selectingFromMap;
    private int _selectedTileStartX;
    private int _selectedTileStartY;
    private int _selectedTileEndX;
    private int _selectedTileEndY;
    private Panel? _topBar;
    private Panel? _rightPanel;
    private TilesetViewControl? _tilesetView;
    private ListBox? _layersList;
    private int _currentLayerIndex;
    private bool _paintingLeft;
    private bool _paintingRight;
    private bool _showGrid = true;
    private bool _highlightSelectedLayer;
    private string _mapName = "";
    private float _zoom = 1f;
    private EditMode _mode = EditMode.Paint;
    private bool[,] _blocked = new bool[1, 1];

    protected override void OnShow()
    {
        base.OnShow();

        _layers.Clear();
        _layers.Add(new EditorLayer(_mapWidth, _mapHeight, isSky: false));
        _layers[0].Fill(-1);

        _blocked = new bool[_mapWidth, _mapHeight];

        _tilesetTexture = TryLoadTilesetTexture(out _tilesetColumns);

        CreateTopToolbar();
        CreateRightPanel();
    }

    protected override void OnUpdate(float dt)
    {
        UpdateLayout();
    }

    private void OnMapPanDrag(Vector2i delta)
    {
        _mapOffset += new Vector2f(delta.X / _zoom, delta.Y / _zoom);
    }

    private void PaintAt(int mouseX, int mouseY, bool erase)
    {
        if (_mode != EditMode.Paint)
        {
            return;
        }

        var worldX = mouseX / _zoom - _mapOffset.X;
        var worldY = mouseY / _zoom - _mapOffset.Y;
        if (worldX < 0 || worldY < 0)
        {
            return;
        }

        var tileX = (int) (worldX / TileSize);
        var tileY = (int) (worldY / TileSize);
        if (tileX < 0 || tileX >= _mapWidth ||
            tileY < 0 || tileY >= _mapHeight)
        {
            return;
        }

        var layer = _layers[Math.Clamp(_currentLayerIndex, 0, _layers.Count - 1)];

        var cols = _brushCols;
        var rows = _brushRows;
        var indices = _brushIndices;

        var hasBrush = indices.Length == cols * rows && cols > 0 && rows > 0;
        if (!hasBrush)
        {
            if (_selectedTileIndex >= 0)
            {
                cols = 1;
                rows = 1;
                indices = [_selectedTileIndex];
                hasBrush = true;
            }
        }

        if (!hasBrush)
        {
            return;
        }

        for (var by = 0; by < rows; by++)
        {
            for (var bx = 0; bx < cols; bx++)
            {
                var mx = tileX + bx;
                var my = tileY + by;
                if (mx < 0 || mx >= _mapWidth || my < 0 || my >= _mapHeight)
                {
                    continue;
                }

                if (erase)
                {
                    layer.Set(mx, my, -1);
                }
                else
                {
                    var idx = indices[by * cols + bx];
                    if (idx >= 0)
                    {
                        layer.Set(mx, my, idx);
                    }
                }
            }
        }
    }

    private void BlockAt(int mouseX, int mouseY, bool block)
    {
        var worldX = mouseX / _zoom - _mapOffset.X;
        var worldY = mouseY / _zoom - _mapOffset.Y;
        if (worldX < 0 || worldY < 0)
        {
            return;
        }

        var tileX = (int) (worldX / TileSize);
        var tileY = (int) (worldY / TileSize);

        if (tileX < 0 || tileX >= _mapWidth ||
            tileY < 0 || tileY >= _mapHeight)
        {
            return;
        }

        _blocked[tileX, tileY] = block;
    }

    protected override void OnDraw(RenderTarget target, RenderStates states)
    {
        // Draw the map (below UI)
        DrawMap(target, states);

        base.OnDraw(target, states);
    }

    private void DrawMap(RenderTarget target, RenderStates states)
    {
        states = new RenderStates(states) {Transform = states.Transform};
        states.Transform.Translate(0, ToolbarHeight);
        states.Transform.Scale(_zoom, _zoom);
        states.Transform.Translate(_mapOffset.X, _mapOffset.Y);

        foreach (var layer in _layers)
        {
            if (!layer.IsSky)
            {
                DrawLayer(target, states, layer);
            }
        }

        foreach (var layer in _layers)
        {
            if (layer.IsSky)
            {
                DrawLayer(target, states, layer);
            }
        }

        if (_showGrid)
        {
            DrawGrid(target, states);
        }

        if (_mode == EditMode.Block)
        {
            DrawBlockedOverlay(target, states);
        }

        if (_mode == EditMode.Paint)
        {
            DrawBrushGhost(target, states);
        }

        if (_selectingFromMap)
        {
            DrawMapSelection(target, states);
        }
    }

    private void DrawMapSelection(RenderTarget target, RenderStates states)
    {
        var sx = Math.Clamp(Math.Min(_selectedTileStartX, _selectedTileEndX), 0, _mapWidth - 1);
        var sy = Math.Clamp(Math.Min(_selectedTileStartY, _selectedTileEndY), 0, _mapHeight - 1);
        var ex = Math.Clamp(Math.Max(_selectedTileStartX, _selectedTileEndX), 0, _mapWidth - 1);
        var ey = Math.Clamp(Math.Max(_selectedTileStartY, _selectedTileEndY), 0, _mapHeight - 1);

        var px = sx * TileSize + 1;
        var py = sy * TileSize + 1;
        var pw = (ex - sx + 1) * TileSize - 2;
        var ph = (ey - sy + 1) * TileSize - 2;

        if (pw <= 0 || ph <= 0) return;

        var rect = new RectangleShape(new Vector2f(pw, ph))
        {
            Position = new Vector2f(px, py),
            FillColor = new Color(255, 255, 0, 40),
            OutlineColor = new Color(255, 255, 0, 200),
            OutlineThickness = 2
        };

        target.Draw(rect, states);
    }

    private void DrawBrushGhost(RenderTarget target, RenderStates states)
    {
        if (!_mouseOverMap) return;
        if (_tilesetTexture is null)
        {
            return;
        }

        var cols = _brushCols;
        var rows = _brushRows;

        var indices = _brushIndices;
        if (indices.Length != cols * rows || cols <= 0 || rows <= 0)
        {
            if (_selectedTileIndex >= 0)
            {
                cols = 1;
                rows = 1;
                indices = [_selectedTileIndex];
            }
            else
            {
                return;
            }
        }

        var sprite = new Sprite(_tilesetTexture)
        {
            Color = new Color(255, 255, 255, 160)
        };

        for (var by = 0; by < rows; by++)
        {
            for (var bx = 0; bx < cols; bx++)
            {
                var mx = _hoverTileX + bx;
                var my = _hoverTileY + by;
                if (mx < 0 || mx >= _mapWidth || my < 0 || my >= _mapHeight)
                {
                    continue;
                }

                var index = indices[by * cols + bx];
                if (index < 0)
                {
                    continue;
                }

                var u = index % _tilesetColumns * TileSize;
                var v = index / _tilesetColumns * TileSize;

                sprite.TextureRect = new IntRect(u, v, TileSize, TileSize);
                sprite.Position = new Vector2f(mx * TileSize, my * TileSize);

                target.Draw(sprite, states);
            }
        }
    }

    private void CreateTopToolbar()
    {
        var style = UI.Skin.GetStyle(UI.Skin.DefaultStyleName);

        _topBar = new Panel(style)
        {
            Position = new Vector2i(0, 0),
            Size = new Vector2i(UI.Size.X, ToolbarHeight)
        };

        UI.Add(_topBar);

        var propertiesButton = new Button(style)
        {
            Text = "Properties",
            Position = new Vector2i(8, 8),
            Size = new Vector2i(100, ToolbarHeight - 16),
            Font = UI.Skin.GetFont("Regular")
        };

        propertiesButton.Click += (_, _) => OpenPropertiesWindow();

        _topBar.Add(propertiesButton);

        var gridCheckBox = new CheckBox(style)
        {
            Text = "Grid",
            Position = new Vector2i(116, 8),
            Size = new Vector2i(100, ToolbarHeight - 16),
            Font = UI.Skin.GetFont("Regular"),
            Checked = _showGrid
        };
        gridCheckBox.CheckedChanged += (_, _) => _showGrid = gridCheckBox.Checked;

        _topBar.Add(gridCheckBox);

        var highlightCheckBox = new CheckBox(style)
        {
            Text = "Highlight",
            Position = new Vector2i(220, 8),
            Size = new Vector2i(110, ToolbarHeight - 16),
            Font = UI.Skin.GetFont("Regular"),
            Checked = _highlightSelectedLayer
        };

        highlightCheckBox.CheckedChanged += (_, _) => _highlightSelectedLayer = highlightCheckBox.Checked;

        _topBar.Add(highlightCheckBox);

        var paintRadioButton = new RadioButton(style)
        {
            Text = "Paint",
            Group = "mode",
            Position = new Vector2i(340, 8),
            Size = new Vector2i(80, ToolbarHeight - 16),
            Font = UI.Skin.GetFont("Regular"),
            Checked = _mode == EditMode.Paint
        };

        paintRadioButton.CheckedChanged += (_, _) =>
        {
            if (paintRadioButton.Checked)
            {
                _mode = EditMode.Paint;
            }
        };

        _topBar.Add(paintRadioButton);

        var blockRadioButton = new RadioButton(style)
        {
            Text = "Block",
            Group = "mode",
            Position = new Vector2i(420, 8),
            Size = new Vector2i(80, ToolbarHeight - 16),
            Font = UI.Skin.GetFont("Regular"),
            Checked = _mode == EditMode.Block
        };

        blockRadioButton.CheckedChanged += (_, _) =>
        {
            if (blockRadioButton.Checked)
            {
                _mode = EditMode.Block;
            }
        };

        _topBar.Add(blockRadioButton);
    }

    private void CreateRightPanel()
    {
        var style = UI.Skin.GetStyle(UI.Skin.DefaultStyleName);

        _rightPanel = new Panel(style)
        {
            Position = new Vector2i(UI.Size.X - SidebarWidth, ToolbarHeight),
            Size = new Vector2i(SidebarWidth, UI.Size.Y - ToolbarHeight)
        };

        UI.Add(_rightPanel);

        _rightPanel.Add(new Label
        {
            Text = "Layers",
            Position = new Vector2i(8, 6),
            Size = new Vector2i(SidebarWidth - 16, 20),
            Font = UI.Skin.GetFont("Regular")
        });

        _layersList = new ListBox
        {
            Position = new Vector2i(8, 28),
            Size = new Vector2i(SidebarWidth - 16, 150),
            Font = UI.Skin.GetFont("Regular")
        };

        _layersList.SelectedIndexChanged += idx =>
        {
            if (idx >= 0 && idx < _layers.Count)
            {
                _currentLayerIndex = idx;
            }
        };

        _rightPanel.Add(_layersList);

        RefreshLayersList();

        var addLayerButton = new Button(style)
        {
            Text = "Add",
            Position = new Vector2i(8, 184),
            Size = new Vector2i((SidebarWidth - 24) / 2, 24),
            Font = UI.Skin.GetFont("Regular")
        };

        addLayerButton.Click += (_, _) =>
        {
            _layers.Add(new EditorLayer(_mapWidth, _mapHeight, isSky: false));
            RefreshLayersList();
            _layersList!.SelectedIndex = _layers.Count - 1;
        };

        _rightPanel.Add(addLayerButton);

        var removeLayerButton = new Button(style)
        {
            Text = "Remove",
            Position = new Vector2i(12 + (SidebarWidth - 24) / 2, 184),
            Size = new Vector2i((SidebarWidth - 24) / 2, 24),
            Font = UI.Skin.GetFont("Regular")
        };

        removeLayerButton.Click += (_, _) =>
        {
            var index = _layersList!.SelectedIndex;
            if (index <= 0)
            {
                return;
            }

            _layers.RemoveAt(index);

            RefreshLayersList();

            _layersList.SelectedIndex = Math.Min(index, _layers.Count - 1);
        };

        _rightPanel.Add(removeLayerButton);

        _tilesetView = new TilesetViewControl
        {
            Position = new Vector2i(8, 216),
            Size = new Vector2i(SidebarWidth - 16, _rightPanel.Size.Y - 224)
        };

        if (_tilesetTexture is not null)
        {
            _tilesetView.SetTexture(_tilesetTexture);
        }

        _tilesetView.SelectedChanged += index =>
        {
            _selectedTileIndex = index;
            if (index < 0)
            {
                return;
            }

            _brushCols = 1;
            _brushRows = 1;
            _brushIndices = [index];
        };

        _tilesetView.SelectionChanged += (cols, rows, indices) =>
        {
            _brushCols = cols;
            _brushRows = rows;
            _brushIndices = indices;
            _selectedTileIndex = indices.Length > 0 ? indices[0] : -1;
        };

        _rightPanel.Add(_tilesetView);
    }

    private void UpdateLayout()
    {
        if (_topBar is not null)
        {
            _topBar.Position = new Vector2i(0, 0);
            _topBar.Size = new Vector2i(UI.Size.X, ToolbarHeight);
        }

        if (_rightPanel is null)
        {
            return;
        }

        _rightPanel.Position = new Vector2i(UI.Size.X - SidebarWidth, ToolbarHeight);
        _rightPanel.Size = new Vector2i(SidebarWidth, UI.Size.Y - ToolbarHeight);

        if (_tilesetView is not null)
        {
            _tilesetView.Size = new Vector2i(SidebarWidth - 16, _rightPanel.Size.Y - 224);
        }
    }

    private void RefreshLayersList()
    {
        if (_layersList == null) return;
        var items = new List<string>();
        for (var i = 0; i < _layers.Count; i++)
        {
            items.Add(i == 0 ? $"Layer {i} (Ground)" : $"Layer {i}");
        }

        var selected = Math.Clamp(_currentLayerIndex, 0, Math.Max(0, _layers.Count - 1));
        _layersList.SetItems(items, selected);
    }

    private void OpenPropertiesWindow()
    {
        var style = UI.Skin.GetStyle(UI.Skin.DefaultStyleName);
        var window = UI.CreateWindow();

        window.Text = "Map Properties";
        window.Size = new Vector2i(320, 180);
        window.MoveToCenter();

        window.Add(new Label
        {
            Text = "Name:",
            Position = new Vector2i(10, 10),
            Size = new Vector2i(60, 20),
            Font = UI.Skin.GetFont("Regular")
        });

        var nameTextBox = new TextBox(style)
        {
            Position = new Vector2i(80, 8),
            Size = new Vector2i(220, 24),
            Text = _mapName,
            Font = UI.Skin.GetFont("Regular")
        };

        window.Add(nameTextBox);

        window.Add(new Label
        {
            Text = "Width:",
            Position = new Vector2i(10, 44),
            Size = new Vector2i(60, 20),
            Font = UI.Skin.GetFont("Regular")
        });

        var widthTextBox = new TextBox(style)
        {
            Position = new Vector2i(80, 42),
            Size = new Vector2i(60, 24),
            Text = _mapWidth.ToString(),
            Font = UI.Skin.GetFont("Regular")
        };

        window.Add(widthTextBox);

        window.Add(new Label
        {
            Text = "Height:",
            Position = new Vector2i(10, 74),
            Size = new Vector2i(60, 20),
            Font = UI.Skin.GetFont("Regular")
        });

        var heightTextBox = new TextBox(style)
        {
            Position = new Vector2i(80, 72),
            Size = new Vector2i(60, 24),
            Text = _mapHeight.ToString(),
            Font = UI.Skin.GetFont("Regular")
        };

        window.Add(heightTextBox);

        var okButton = new Button(style)
        {
            Text = "OK",
            Position = new Vector2i(80, 120),
            Size = new Vector2i(80, 26),
            Font = UI.Skin.GetFont("Regular")
        };

        okButton.Click += (_, _) =>
        {
            if (int.TryParse(widthTextBox.Text, out var nw) &&
                int.TryParse(heightTextBox.Text, out var nh))
            {
                nw = Math.Clamp(nw, 1, 200);
                nh = Math.Clamp(nh, 1, 200);

                ResizeMap(nw, nh);
            }

            _mapName = nameTextBox.Text;

            window.Visible = false;
        };

        window.Add(okButton);

        var cancelButton = new Button(style)
        {
            Text = "Cancel",
            Position = new Vector2i(180, 120),
            Size = new Vector2i(80, 26),
            Font = UI.Skin.GetFont("Regular")
        };

        cancelButton.Click += (_, _) => window.Visible = false;

        window.Add(cancelButton);
        window.MoveToFront();
    }

    private void ResizeMap(int newW, int newH)
    {
        if (newW == _mapWidth && newH == _mapHeight) return;
        for (var i = 0; i < _layers.Count; i++)
        {
            var old = _layers[i];
            var copy = new EditorLayer(newW, newH, old.IsSky);
            var cx = Math.Min(newW, _mapWidth);
            var cy = Math.Min(newH, _mapHeight);
            for (var y = 0; y < cy; y++)
            {
                for (var x = 0; x < cx; x++)
                {
                    copy.Set(x, y, old.Get(x, y));
                }
            }

            _layers[i] = copy;
        }

        // Resize blocked map
        var newBlocked = new bool[newW, newH];
        {
            var cx = Math.Min(newW, _mapWidth);
            var cy = Math.Min(newH, _mapHeight);
            for (var y = 0; y < cy; y++)
            {
                for (var x = 0; x < cx; x++)
                {
                    newBlocked[x, y] = _blocked[x, y];
                }
            }
        }
        _blocked = newBlocked;

        _mapWidth = newW;
        _mapHeight = newH;
        RefreshLayersList();
    }

    private void DrawLayer(RenderTarget target, RenderStates states, EditorLayer layer)
    {
        byte alpha = 255;
        if (_highlightSelectedLayer)
        {
            // Dim non-selected layers
            var isCurrent = _layers[Math.Clamp(_currentLayerIndex, 0, _layers.Count - 1)] == layer;
            if (!isCurrent)
            {
                alpha = 96;
            }
        }

        if (_tilesetTexture == null)
        {
            // Draw colored squares as fallback
            var rect = new RectangleShape(new Vector2f(TileSize - 1, TileSize - 1));
            rect.FillColor = new Color(70, 70, 70, alpha);

            for (var y = 0; y < _mapHeight; y++)
            {
                for (var x = 0; x < _mapWidth; x++)
                {
                    if (layer.Get(x, y) < 0) continue;

                    rect.Position = new Vector2f(x * TileSize, y * TileSize);
                    target.Draw(rect, states);
                }
            }

            return;
        }

        // Batch draw via per-tile sprites (simple, adequate for editor)
        var sprite = new Sprite(_tilesetTexture);

        for (var y = 0; y < _mapHeight; y++)
        {
            for (var x = 0; x < _mapWidth; x++)
            {
                var idx = layer.Get(x, y);
                if (idx < 0) continue;

                var u = idx % _tilesetColumns * TileSize;
                var v = idx / _tilesetColumns * TileSize;

                sprite.TextureRect = new IntRect(u, v, TileSize, TileSize);
                sprite.Position = new Vector2f(x * TileSize, y * TileSize);
                sprite.Color = new Color(255, 255, 255, alpha);

                target.Draw(sprite, states);
            }
        }
    }

    private void DrawGrid(RenderTarget target, RenderStates states)
    {
        var lines = new VertexArray(PrimitiveType.Lines);
        
        var color = new Color(255, 255, 255, 40);
        var w = _mapWidth * TileSize;
        var h = _mapHeight * TileSize;

        for (var x = 0; x <= _mapWidth; x++)
        {
            var px = x * TileSize;
            
            lines.Append(new Vertex(new Vector2f(px, 0), color));
            lines.Append(new Vertex(new Vector2f(px, h), color));
        }

        for (var y = 0; y <= _mapHeight; y++)
        {
            var py = y * TileSize;
            
            lines.Append(new Vertex(new Vector2f(0, py), color));
            lines.Append(new Vertex(new Vector2f(w, py), color));
        }

        target.Draw(lines, states);
    }

    private void DrawBlockedOverlay(RenderTarget target, RenderStates states)
    {
        var rect = new RectangleShape(new Vector2f(TileSize - 1, TileSize - 1))
        {
            FillColor = new Color(255, 0, 0, 100)
        };

        for (var y = 0; y < _mapHeight; y++)
        {
            for (var x = 0; x < _mapWidth; x++)
            {
                if (!_blocked[x, y])
                {
                    continue;
                }

                rect.Position = new Vector2f(x * TileSize, y * TileSize);
                target.Draw(rect, states);
            }
        }
    }

    private static Texture TryLoadTilesetTexture(out int columns)
    {
        var texture = new Texture("Content/Tilesets/Terrain.png");

        columns = (int) texture.Size.X / TileSize;

        return texture;
    }

    private sealed class EditorLayer(int width, int height, bool isSky)
    {
        private readonly int[] _tiles = new int[width * height];
        public bool IsSky { get; } = isSky;

        public int Get(int x, int y) => _tiles[y * width + x];
        public void Set(int x, int y, int idx) => _tiles[y * width + x] = idx;

        public void Fill(int value)
        {
            for (var i = 0; i < _tiles.Length; i++) _tiles[i] = value;
        }
    }

    private bool IsInMapArea(int x, int y)
    {
        return y >= ToolbarHeight && x >= 0 && x < UI.Size.X - SidebarWidth && y < UI.Size.Y;
    }

    protected override void OnMouseButtonPressed(int x, int y, Mouse.Button button)
    {
        if (!IsInMapArea(x, y))
        {
            return;
        }

        var local = new Vector2i(x, y - ToolbarHeight);

        switch (button)
        {
            case Mouse.Button.Middle:
                _isPanningMap = true;
                _lastPanPoint = local;
                break;

            case Mouse.Button.Left:
                if (Keyboard.IsKeyPressed(Keyboard.Key.LControl) || Keyboard.IsKeyPressed(Keyboard.Key.RControl))
                {
                    var worldX = local.X / _zoom - _mapOffset.X;
                    var worldY = local.Y / _zoom - _mapOffset.Y;

                    if (worldX >= 0 && worldY >= 0)
                    {
                        var tileX = (int) (worldX / TileSize);
                        var tileY = (int) (worldY / TileSize);

                        if (tileX >= 0 && tileX < _mapWidth && tileY >= 0 && tileY < _mapHeight)
                        {
                            _selectingFromMap = true;
                            _selectedTileStartX = _selectedTileEndX = tileX;
                            _selectedTileStartY = _selectedTileEndY = tileY;
                        }
                    }
                }
                else
                {
                    _paintingLeft = true;
                    if (_mode == EditMode.Block)
                    {
                        BlockAt(local.X, local.Y, true);
                    }
                    else
                    {
                        PaintAt(local.X, local.Y, false);
                    }
                }

                break;

            case Mouse.Button.Right:
                _paintingRight = true;
                if (_mode == EditMode.Block)
                {
                    BlockAt(local.X, local.Y, false);
                }
                else
                {
                    PaintAt(local.X, local.Y, true);
                }

                break;
        }
    }

    protected override void OnMouseButtonReleased(int x, int y, Mouse.Button button)
    {
        switch (button)
        {
            case Mouse.Button.Middle:
                _isPanningMap = false;
                break;

            case Mouse.Button.Left:
                if (_selectingFromMap)
                {
                    var sx = Math.Clamp(Math.Min(_selectedTileStartX, _selectedTileEndX), 0, _mapWidth - 1);
                    var sy = Math.Clamp(Math.Min(_selectedTileStartY, _selectedTileEndY), 0, _mapHeight - 1);
                    var ex = Math.Clamp(Math.Max(_selectedTileStartX, _selectedTileEndX), 0, _mapWidth - 1);
                    var ey = Math.Clamp(Math.Max(_selectedTileStartY, _selectedTileEndY), 0, _mapHeight - 1);

                    var cols = ex - sx + 1;
                    var rows = ey - sy + 1;

                    if (cols > 0 && rows > 0)
                    {
                        var layer = _layers[Math.Clamp(_currentLayerIndex, 0, _layers.Count - 1)];
                        var indices = new int[cols * rows];

                        var k = 0;
                        for (var ty = sy; ty <= ey; ty++)
                        {
                            for (var tx = sx; tx <= ex; tx++)
                            {
                                indices[k++] = layer.Get(tx, ty);
                            }
                        }

                        _brushCols = cols;
                        _brushRows = rows;
                        _brushIndices = indices;
                        _selectedTileIndex = indices.Length > 0 ? indices[0] : -1;
                    }

                    _selectingFromMap = false;
                }
                else
                {
                    _paintingLeft = false;
                }

                break;

            case Mouse.Button.Right:
                _paintingRight = false;
                break;
        }
    }

    protected override void OnMouseMoved(int x, int y)
    {
        var local = new Vector2i(x, y - ToolbarHeight);

        if (_isPanningMap)
        {
            var delta = local - _lastPanPoint;

            _lastPanPoint = local;

            OnMapPanDrag(delta);
        }

        if (IsInMapArea(x, y))
        {
            _mouseOverMap = true;

            var worldX = local.X / _zoom - _mapOffset.X;
            var worldY = local.Y / _zoom - _mapOffset.Y;
            if (worldX >= 0 && worldY >= 0)
            {
                _hoverTileX = (int) (worldX / TileSize);
                _hoverTileY = (int) (worldY / TileSize);
            }

            if (_selectingFromMap)
            {
                var txSel = _hoverTileX;
                var tySel = _hoverTileY;

                if (txSel < 0 || txSel >= _mapWidth ||
                    tySel < 0 || tySel >= _mapHeight)
                {
                    return;
                }

                _selectedTileEndX = txSel;
                _selectedTileEndY = tySel;
            }
            else if (_paintingLeft)
            {
                if (_mode == EditMode.Block)
                {
                    BlockAt(local.X, local.Y, true);
                }
                else
                {
                    PaintAt(local.X, local.Y, false);
                }
            }
            else if (_paintingRight)
            {
                if (_mode == EditMode.Block)
                {
                    BlockAt(local.X, local.Y, false);
                }
                else
                {
                    PaintAt(local.X, local.Y, true);
                }
            }
        }
        else
        {
            _mouseOverMap = false;
        }
    }

    protected override void OnMouseWheelScrolled(int x, int y, float delta)
    {
        if (!IsInMapArea(x, y))
        {
            return;
        }

        var local = new Vector2f(x, y - ToolbarHeight);

        // World coordinate under the cursor before zoom change
        var worldBefore = local / _zoom - _mapOffset;

        // Adjust zoom
        var factor = delta > 0 ? ZoomStep : 1f / ZoomStep;

        var zoom = Math.Clamp(_zoom * factor, MinZoom, MaxZoom);
        if (Math.Abs(zoom - _zoom) < 0.0001f)
        {
            return;
        }

        _zoom = zoom;

        _mapOffset = local / _zoom - worldBefore;
    }

    private sealed class TilesetViewControl : Control
    {
        private Texture? _texture;
        private bool _panning;
        private Vector2i _last;
        private Vector2f _offset;
        private int _columns;
        private int _selectedIndex = -1;
        private bool _selecting;
        private Vector2i _selectedTileStart;
        private Vector2i _selectedTileEnd;
        private int _selectedColumns = 1;
        private int _selectedRows = 1;

        public event Action<int>? SelectedChanged;
        public event Action<int, int, int[]>? SelectionChanged;

        public void SetTexture(Texture texture)
        {
            _texture = texture;
            _columns = (int) texture.Size.X / TileSize;
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            states.Transform.Translate(Position.X, Position.Y);

            if (_texture is null)
            {
                return;
            }

            var texW = (int) _texture.Size.X;
            var texH = (int) _texture.Size.Y;
            var offsetX = (int) MathF.Floor(_offset.X);
            var offsetY = (int) MathF.Floor(_offset.Y);

            var visLeft = Math.Max(0, offsetX);
            var visTop = Math.Max(0, offsetY);
            var visRight = Math.Min(Size.X, offsetX + texW);
            var visBottom = Math.Min(Size.Y, offsetY + texH);

            if (visRight <= visLeft || visBottom <= visTop)
            {
                return;
            }

            var texLeft = visLeft - offsetX;
            var texTop = visTop - offsetY;
            var texWidth = visRight - visLeft;
            var texHeight = visBottom - visTop;

            var sprite = new Sprite(_texture)
            {
                TextureRect = new IntRect(texLeft, texTop, texWidth, texHeight),
                Position = new Vector2f(visLeft, visTop)
            };

            target.Draw(sprite, states);

            // Draw grid overlay only within visible area
            var cols = _columns;
            var rows = (int) _texture.Size.Y / TileSize;
            var grid = new VertexArray(PrimitiveType.Lines);
            var color = new Color(255, 255, 255, 40);

            var startCol = Math.Max(0, (int) MathF.Floor((visLeft - offsetX) / (float) TileSize));
            var endCol = Math.Min(cols, (int) MathF.Ceiling((visRight - offsetX) / (float) TileSize));
            var startRow = Math.Max(0, (int) MathF.Floor((visTop - offsetY) / (float) TileSize));
            var endRow = Math.Min(rows, (int) MathF.Ceiling((visBottom - offsetY) / (float) TileSize));

            for (var x = startCol; x <= endCol; x++)
            {
                var px = offsetX + x * TileSize;
                if (px < visLeft || px > visRight)
                {
                    continue;
                }

                grid.Append(new Vertex(new Vector2f(px, visTop), color));
                grid.Append(new Vertex(new Vector2f(px, visBottom), color));
            }

            for (var y = startRow; y <= endRow; y++)
            {
                var py = offsetY + y * TileSize;
                if (py < visTop || py > visBottom)
                {
                    continue;
                }

                grid.Append(new Vertex(new Vector2f(visLeft, py), color));
                grid.Append(new Vertex(new Vector2f(visRight, py), color));
            }

            target.Draw(grid, states);

            var drawCols = _selectedColumns;
            var drawRows = _selectedRows;
            var drawIndex = _selectedIndex;

            if (_selecting)
            {
                var sxTile = Math.Min(_selectedTileStart.X, _selectedTileEnd.X);
                var syTile = Math.Min(_selectedTileStart.Y, _selectedTileEnd.Y);
                var exTile = Math.Max(_selectedTileStart.X, _selectedTileEnd.X);
                var eyTile = Math.Max(_selectedTileStart.Y, _selectedTileEnd.Y);

                drawCols = exTile - sxTile + 1;
                drawRows = eyTile - syTile + 1;
                drawIndex = syTile * cols + sxTile;
            }

            if (drawCols > 1 || drawRows > 1)
            {
                var sx = drawIndex % cols;
                var sy = drawIndex / cols;
                var rx = offsetX + sx * TileSize + 1;
                var ry = offsetY + sy * TileSize + 1;
                var rw = drawCols * TileSize - 2;
                var rh = drawRows * TileSize - 2;

                var selLeft = Math.Max(visLeft, rx);
                var selTop = Math.Max(visTop, ry);
                var selRight = Math.Min(visRight, rx + rw);
                var selBottom = Math.Min(visBottom, ry + rh);

                if (selRight > selLeft && selBottom > selTop)
                {
                    var rect = new RectangleShape(new Vector2f(selRight - selLeft, selBottom - selTop))
                    {
                        Position = new Vector2f(selLeft, selTop),
                        FillColor = new Color(0, 0, 0, 0),
                        OutlineColor = new Color(255, 255, 0, 180),
                        OutlineThickness = 2
                    };

                    target.Draw(rect, states);
                }
            }
            else if (drawIndex >= 0)
            {
                var sx = drawIndex % cols;
                var sy = drawIndex / cols;
                var rx = offsetX + sx * TileSize + 1;
                var ry = offsetY + sy * TileSize + 1;

                const int rw = TileSize - 2;
                const int rh = TileSize - 2;

                var selLeft = Math.Max(visLeft, rx);
                var selTop = Math.Max(visTop, ry);
                var selRight = Math.Min(visRight, rx + rw);
                var selBottom = Math.Min(visBottom, ry + rh);

                if (selRight > selLeft && selBottom > selTop)
                {
                    var rect = new RectangleShape(new Vector2f(selRight - selLeft, selBottom - selTop))
                    {
                        Position = new Vector2f(selLeft, selTop),
                        FillColor = new Color(0, 0, 0, 0),
                        OutlineColor = new Color(255, 255, 0, 180),
                        OutlineThickness = 2
                    };

                    target.Draw(rect, states);
                }
            }
        }

        protected override bool OnMousePressed(int x, int y, Mouse.Button button)
        {
            switch (button)
            {
                case Mouse.Button.Middle:
                    _panning = true;
                    _last = new Vector2i(x, y);

                    CaptureMouse();
                    return true;

                case Mouse.Button.Left when _texture is null:
                    return false;

                case Mouse.Button.Left:
                {
                    var local = new Vector2f(x, y) - _offset;
                    if (local.X < 0 || local.Y < 0)
                    {
                        return false;
                    }

                    var col = (int) (local.X / TileSize);
                    var row = (int) (local.Y / TileSize);
                    var cols = _columns;
                    var rows = (int) _texture.Size.Y / TileSize;

                    if (col >= 0 && col < cols && row >= 0 && row < rows)
                    {
                        _selecting = true;
                        _selectedTileStart = new Vector2i(col, row);
                        _selectedTileEnd = _selectedTileStart;
                        CaptureMouse();
                        return true;
                    }

                    return false;
                }

                default:
                {
                    return false;
                }
            }
        }

        protected override bool OnMouseReleased(int x, int y, Mouse.Button button)
        {
            switch (button)
            {
                case Mouse.Button.Middle:
                    _panning = false;
                    ReleaseMouse();
                    return true;

                case Mouse.Button.Left when _texture is null:
                    _selecting = false;
                    ReleaseMouse();
                    return false;

                case Mouse.Button.Left:
                {
                    var local = new Vector2f(x, y) - _offset;
                    var col = (int) (local.X / TileSize);
                    var row = (int) (local.Y / TileSize);
                    var cols = _columns;
                    var rows = (int) _texture.Size.Y / TileSize;

                    col = Math.Clamp(col, 0, Math.Max(0, cols - 1));
                    row = Math.Clamp(row, 0, Math.Max(0, rows - 1));

                    if (_selecting)
                    {
                        _selectedTileEnd = new Vector2i(col, row);

                        var sxTile = Math.Min(_selectedTileStart.X, _selectedTileEnd.X);
                        var syTile = Math.Min(_selectedTileStart.Y, _selectedTileEnd.Y);
                        var exTile = Math.Max(_selectedTileStart.X, _selectedTileEnd.X);
                        var eyTile = Math.Max(_selectedTileStart.Y, _selectedTileEnd.Y);

                        _selectedColumns = exTile - sxTile + 1;
                        _selectedRows = eyTile - syTile + 1;

                        _selectedIndex = syTile * cols + sxTile;

                        var indices = new int[_selectedColumns * _selectedRows];

                        var idx = 0;
                        for (var ty = syTile; ty <= eyTile; ty++)
                        {
                            for (var tx = sxTile; tx <= exTile; tx++)
                            {
                                indices[idx++] = ty * cols + tx;
                            }
                        }

                        SelectedChanged?.Invoke(_selectedIndex);
                        SelectionChanged?.Invoke(_selectedColumns, _selectedRows, indices);
                    }

                    _selecting = false;
                    ReleaseMouse();
                    return true;
                }
                default:
                    return false;
            }
        }

        protected override bool OnMouseMove(int x, int y)
        {
            if (_panning)
            {
                var cur = new Vector2i(x, y);
                var delta = cur - _last;
                _last = cur;
                _offset += new Vector2f(delta.X, delta.Y);
                return true;
            }

            if (!_selecting || _texture is null)
            {
                return false;
            }

            var local = new Vector2f(x, y) - _offset;
            var col = (int) (local.X / TileSize);
            var row = (int) (local.Y / TileSize);
            var cols = _columns;
            var rows = (int) _texture.Size.Y / TileSize;

            col = Math.Clamp(col, 0, Math.Max(0, cols - 1));
            row = Math.Clamp(row, 0, Math.Max(0, rows - 1));

            _selectedTileEnd = new Vector2i(col, row);

            return true;
        }
    }
}