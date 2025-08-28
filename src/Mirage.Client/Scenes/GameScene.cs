using Mirage.Client.Net;
using Mirage.Engine.UI.Chat;
using Mirage.Engine.UI.Controls;
using Mirage.Net.Protocol.FromClient;
using Mirage.Shared.Data;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Mirage.Client.Scenes;

public sealed class GameScene : Scene
{
    private int _itemPickupTimer;
    private readonly Game _game;

    private readonly ChatPanel _chatPanel = new()
    {
        Position = new Vector2f(2, 474),
        Size = new Vector2i(122, 600)
    };

    public GameScene(Game game)
    {
        _game = game;

        _chatInput = new TextBox(TempStyle.Style)
        {
            Position = new Vector2f(0, 600 - 28),
            Size = new Vector2i(800, 28)
        };

        _chatInput.Submit += () =>
        {
            _chatInput.Text = string.Empty;
            _chatInput.Visible = false;

            UI.ClearFocus();
        };

        _chatInput.Blur += () =>
        {
            _chatInput.Text = string.Empty;
            _chatInput.Visible = false;
        };

        //UI.Add(_chatPanel);
        UI.Add(_chatInput);
    }

    private readonly TextBox _chatInput;

    protected override void OnUpdate(float dt)
    {
        _game.Map.Update(dt);

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

        target.Draw(UI, states);
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

        text.Font = Game.Font;
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

        text.Font = Game.Font;
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

        text.Font = Game.Font;
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

        text.Font = Game.Font;
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
        if (key != Keyboard.Key.Enter || Environment.TickCount - _itemPickupTimer < 1000)
        {
            return;
        }

        _itemPickupTimer = Environment.TickCount;

        Network.Send<ItemPickupRequest>();
    }

    private void CheckForKeyboardInput()
    {
        if (UI.HasKeyboardFocus)
        {
            return;
        }

        if (Keyboard.IsKeyPressed(Keyboard.Key.T))
        {
            _chatInput.Visible = true;
            _chatInput.Focus();
        }

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

    public void AddChatMessage(string message, Color color)
    {
        _chatPanel.AddChatMessage(message, color);
    }
}