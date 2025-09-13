using Mirage.Net.Protocol.FromClient;
using Mirage.Shared.Data;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Terestrium.Client.Core.Scenes;
using Terestrium.Client.Net;
using Terestrium.Client.UI.Chat;
using Terestrium.Client.UI.Controls;
using Window = Terestrium.Client.UI.Controls.Window;

namespace Terestrium.Client.Scenes.Game;

public sealed class GameScene : Scene, IGameScene
{
    private int _itemPickupTimer;
    private readonly Client.Game _game;

    // Chat UI fields
    private readonly ChatPanel _chatPanel = new();
    private TextBox _chatInput = null!;
    private Label _chatPrompt = null!;
    private CheckBox _cbLocal = null!;
    private CheckBox _cbPlayer = null!;
    private CheckBox _cbParty = null!;
    private CheckBox _cbGuild = null!;
    private CheckBox _cbGlobal = null!;
    private Window _chatWindow = null!;

    private readonly List<(string Message, string Channel, Color Color)> _chatMessages = new();

    private readonly Dictionary<string, bool> _channelFilters = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Local"] = true,
        ["Player"] = true,
        ["Party"] = true,
        ["Guild"] = true,
        ["Global"] = true,
    };

    private bool _chatWindowVisible;
    private int _lastEnterTick;
    private int _lastEscapeTick;

    public GameScene(Client.Game game)
    {
        _game = game;

        // Load chat window from XML layout
        _chatWindow = UI.CreateWindow("WinChat");

        // Map controls from the layout
        _chatInput = _chatWindow.Get<TextBox>("ChatInput");
        _chatPrompt = _chatWindow.Get<Label>("ChatPrompt");
        _cbLocal = _chatWindow.Get<CheckBox>("CbLocal");
        _cbPlayer = _chatWindow.Get<CheckBox>("CbPlayer");
        _cbParty = _chatWindow.Get<CheckBox>("CbParty");
        _cbGuild = _chatWindow.Get<CheckBox>("CbGuild");
        _cbGlobal = _chatWindow.Get<CheckBox>("CbGlobal");

        // Install chat panel into the host area
        var msgHost = _chatWindow.Get<Panel>("MsgHost");
        _chatPanel.Position = msgHost.Position;
        _chatPanel.Size = msgHost.Size;
        _chatWindow.Add(_chatPanel);

        // Configure filters row (checkboxes)
        _cbLocal.Tag = "Local";
        _cbPlayer.Tag = "Player";
        _cbParty.Tag = "Party";
        _cbGuild.Tag = "Guild";
        _cbGlobal.Tag = "Global";
        _cbLocal.CheckedChanged += OnChannelFilterChanged;
        _cbPlayer.CheckedChanged += OnChannelFilterChanged;
        _cbParty.CheckedChanged += OnChannelFilterChanged;
        _cbGuild.CheckedChanged += OnChannelFilterChanged;
        _cbGlobal.CheckedChanged += OnChannelFilterChanged;

        // Input events
        _chatInput.Submit += () =>
        {
            _chatInput.Text = string.Empty;
            ShowChatWindow(false);
            UI.ClearFocus();
        };
        _chatInput.Blur += () =>
        {
            _chatInput.Text = string.Empty;
            ShowChatWindow(false);
        };

        ShowChatWindow(false);

        AddChatMessage("Hello World", "Global", Color.White);
    }

    protected override void OnUpdate(float dt)
    {
        _game.Map.Update(dt);

        // Handle Escape to hide chat when input focused
        if (_chatInput.HasFocus && Keyboard.IsKeyPressed(Keyboard.Key.Escape))
        {
            var now = Environment.TickCount;
            if (now - _lastEscapeTick > 150)
            {
                _lastEscapeTick = now;
                _chatInput.Text = string.Empty;
                ShowChatWindow(false);
                UI.ClearFocus();
            }
        }

        CheckForKeyboardInput();
    }

    private void DrawMap(RenderTarget target)
    {
        if (_game.Map.WidthInPixels == 0 ||
            _game.Map.HeightInPixels == 0)
        {
            return;
        }

        var states = RenderStates.Default;

        var localPlayer = _game.LocalPlayer;
        if (localPlayer is not null)
        {
            var playerX = localPlayer.X + 16;
            var playerY = localPlayer.Y + 16;

            var cameraX = playerX - target.DefaultView.Size.X / 2;
            var cameraY = playerY - target.DefaultView.Size.Y / 2;

            var maxX = _game.Map.WidthInPixels - target.DefaultView.Size.X;
            var maxY = _game.Map.HeightInPixels - target.DefaultView.Size.Y;

            cameraX = Math.Clamp(cameraX, 0, maxX);
            cameraY = Math.Clamp(cameraY, 0, maxY);

            states.Transform.Translate(-cameraX, -cameraY);
        }

        target.Draw(_game.Map, states);
    }

    protected override void OnDraw(RenderTarget target, RenderStates states)
    {
        DrawMap(target);

        //  target.Draw(new Sprite(_frame));
        // target.Draw(new Sprite(_renderTexture.Texture));

        DrawStats(target);
        //DrawMapName(target);

        // UI is drawn by base Scene.Draw
    }

    private readonly Texture _tt = new Texture("Content/UI/CharacterInfo.png");

    private void DrawStats(RenderTarget target)
    {
        var sprite = new Sprite();

        sprite.Position = new Vector2f(10, 10);
        sprite.Texture = _tt;
        sprite.TextureRect = new IntRect(0, 0, 175, 72);

        target.Draw(sprite);

        var states = RenderStates.Default;

        states.Transform.Translate(10, 10);

        var localPlayer = _game.LocalPlayer;
        if (localPlayer is null)
        {
            return;
        }

        DrawName(target, states, localPlayer.Name);
        DrawLevel(target, states, localPlayer.Level);

        DrawBar(target, states, 5, 27, 0, localPlayer.Health, localPlayer.MaxHealth);
        DrawBar(target, states, 5, 41, 1, localPlayer.Mana, localPlayer.MaxMana);
        DrawBar(target, states, 5, 55, 2, localPlayer.Stamina, localPlayer.MaxStamina);
    }

    private static void DrawName(RenderTarget target, RenderStates states, string name)
    {
        var text = new Text();

        text.Font = Client.Game.Font;
        text.CharacterSize = 14;
        text.FillColor = Color.White;
        text.DisplayedString = name;

        var size = text.GetLocalBounds();

        text.Position = new Vector2f(5 + (int) ((145 - size.Width) / 2), 3);

        target.Draw(text, states);
    }

    private static void DrawLevel(RenderTarget target, RenderStates states, int level)
    {
        var text = new Text();

        text.Font = Client.Game.Font;
        text.CharacterSize = 14;
        text.FillColor = Color.White;
        text.DisplayedString = level.ToString();

        var size = text.GetLocalBounds();

        text.Position = new Vector2f(146 + (int) ((16 - size.Width) / 2), 3);

        target.Draw(text, states);
    }

    private void DrawBar(RenderTarget target, RenderStates states, int x, int y, int index, int value, int max)
    {
        var width = (int) ((float) value / max * 165);

        var sprite = new Sprite();
        sprite.Position = new Vector2f(x, y);
        sprite.Texture = _tt;
        sprite.TextureRect = new IntRect(0, 72 + index * 11, width, 11);

        target.Draw(sprite, states);

        var text = new Text();

        text.Font = Client.Game.Font;
        text.DisplayedString = $"{value}/{max}";
        text.CharacterSize = 13;
        text.FillColor = Color.White;
        text.OutlineColor = Color.Black;
        text.OutlineThickness = 1;
        text.Position = new Vector2f(x + 3, y - 3);

        target.Draw(text, states);
    }

    private void DrawMapName(RenderTarget target)
    {
        var info = _game.Map.Info;
        if (info is null || string.IsNullOrEmpty(info.Name))
        {
            return;
        }

        var text = new Text();

        text.Font = Client.Game.Font;
        text.CharacterSize = 16;
        text.FillColor = info.PvpEnabled ? Color.Red : Color.White;
        text.DisplayedString = info.Name;
        text.OutlineColor = Color.Black;
        text.OutlineThickness = 1;

        var size = text.GetLocalBounds();

        text.Position = new Vector2f((800 - size.Width) / 2, 452);

        target.Draw(text);
    }

    protected override void OnKeyPressed(Keyboard.Key key)
    {
        // Keep existing item pickup on Enter when not interacting with chat
        if (key == Keyboard.Key.Enter && !UI.HasKeyboardFocus)
        {
            if (Environment.TickCount - _itemPickupTimer < 1000)
            {
                return;
            }

            _itemPickupTimer = Environment.TickCount;
            Network.Send<ItemPickupRequest>();
        }
    }

    private void ShowChatWindow(bool visible)
    {
        // Chat window is always visible now; ignore parameter
        _chatWindowVisible = true;

        _chatPanel.Visible = true;
        _cbLocal.Visible = true;
        _cbPlayer.Visible = true;
        _cbParty.Visible = true;
        _cbGuild.Visible = true;
        _cbGlobal.Visible = true;
        _chatInput.Visible = true;

        _chatPrompt.Visible = false;
    }

    private void OnChannelFilterChanged(object? sender, EventArgs e)
    {
        if (sender is not CheckBox cb || cb.Tag is not string channel)
        {
            return;
        }

        _channelFilters[channel] = cb.Checked;
        RebuildChatPanel();
    }

    private void RebuildChatPanel()
    {
        _chatPanel.Clear();

        foreach (var m in _chatMessages)
        {
            if (!_channelFilters.TryGetValue(m.Channel, out var enabled) || !enabled)
            {
                continue;
            }

            _chatPanel.AddChatMessage(m.Message, m.Color);
        }

        _chatPanel.ScrollToBottom();
    }

    public void AddChatMessage(string message, string channel, Color color)
    {
        _chatMessages.Add((message, channel, color));

        if (_channelFilters.TryGetValue(channel, out var enabled) && enabled)
        {
            _chatPanel.AddChatMessage(message, color);
            _chatPanel.ScrollToBottom();
        }
    }

    private void CheckForKeyboardInput()
    {
        // If chat is not active and no other UI has focus (or only the prompt has focus), Enter opens chat
        var uiHasFocus = UI.HasKeyboardFocus;
        var promptHasFocus = ReferenceEquals(UI.ActiveControl, _chatPrompt);
        if ((!uiHasFocus || promptHasFocus) && Keyboard.IsKeyPressed(Keyboard.Key.Enter))
        {
            var now = Environment.TickCount;
            if (now - _lastEnterTick > 150)
            {
                _lastEnterTick = now;
                ShowChatWindow(true);
                _chatInput.Focus();
                return;
            }
        }

        if (UI.HasKeyboardFocus)
        {
            return;
        }

        // Game controls
        CheckAttack();
        CheckMovement();
    }

    private void CheckMovement()
    {
        if (_game.GettingMap)
        {
            return;
        }

        var localPlayer = _game.LocalPlayer;
        if (localPlayer is null || localPlayer.Busy)
        {
            return;
        }

        var (direction, movementType) = CheckMovementKeys();
        if (movementType == MovementType.None)
        {
            return;
        }

        if (localPlayer.TryMove(direction, movementType))
        {
            Network.Send(new MoveRequest(direction, movementType));
            if (_game.Map.GetTileType(localPlayer.TileX, localPlayer.TileY) == TileTypes.Warp)
            {
                _game.GettingMap = true;
            }

            return;
        }

        if (localPlayer.TryMoveMap(direction))
        {
            Network.Send(new MoveMapRequest(direction));

            _game.GettingMap = true;

            return;
        }

        if (localPlayer.Direction == direction)
        {
            return;
        }

        localPlayer.Direction = direction;

        Network.Send(new SetDirectionRequest(direction));
    }

    private static (Direction, MovementType) CheckMovementKeys()
    {
        var movementType =
            Keyboard.IsKeyPressed(Keyboard.Key.LShift) ||
            Keyboard.IsKeyPressed(Keyboard.Key.RShift)
                ? MovementType.Running
                : MovementType.Walking;

        if (Keyboard.IsKeyPressed(Keyboard.Key.Up)) return (Direction.Up, movementType);
        if (Keyboard.IsKeyPressed(Keyboard.Key.Down)) return (Direction.Down, movementType);
        if (Keyboard.IsKeyPressed(Keyboard.Key.Left)) return (Direction.Left, movementType);
        if (Keyboard.IsKeyPressed(Keyboard.Key.Right)) return (Direction.Right, movementType);

        return (Direction.Down, MovementType.None);
    }

    private void CheckAttack()
    {
        if (_game.GettingMap)
        {
            return;
        }

        var localPlayer = _game.LocalPlayer;
        if (localPlayer is null || localPlayer.Busy)
        {
            return;
        }

        var controlDown =
            Keyboard.IsKeyPressed(Keyboard.Key.LControl) ||
            Keyboard.IsKeyPressed(Keyboard.Key.RControl);

        if (!controlDown)
        {
            return;
        }

        if (localPlayer.TryAttack())
        {
            Network.Send<AttackRequest>();
        }
    }
}