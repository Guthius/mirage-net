using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mirage.Client.Inventory;
using Mirage.Client.Net;
using Mirage.Client.UI;
using Mirage.Net.Protocol.FromClient;
using Mirage.Shared.Data;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using ImGuiVec2 = System.Numerics.Vector2;
using ImGuiVec4 = System.Numerics.Vector4;

namespace Mirage.Client.Scenes;

public sealed class GameScene : Scene
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly Game _game;
    private int _itemPickupTimer;

    public GameScene(GraphicsDevice graphicsDevice, Game game)
    {
        _graphicsDevice = graphicsDevice;
        _game = game;

        Textures.Sprites = Texture2D.FromFile(graphicsDevice, "Content/Sprites.png");
        Textures.Items = Texture2D.FromFile(graphicsDevice, "Content/Items.png");
    }

    protected override void OnShow()
    {
        _game.ClearStatus();
    }

    protected override void OnHide()
    {
        _game.ClearChatHistory();
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        _game.Map.Update(gameTime);

        if (!ImGui.GetIO().WantTextInput)
        {
            CheckForKeyboardInput();
        }
    }

    private void CheckForKeyboardInput()
    {
        if (IsKeyJustPressed(Keys.Enter) && !ImGui.GetIO().WantTextInput && Environment.TickCount > _itemPickupTimer + 250)
        {
            _itemPickupTimer = Environment.TickCount;

            Network.Send<ItemPickupRequest>();
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

        if (localPlayer.Direction == direction)
        {
            return;
        }

        localPlayer.Direction = direction;

        Network.Send(new SetDirectionRequest(direction));
    }

    private (Direction, MovementType) CheckMovementKeys()
    {
        var movementType = IsKeyPressed(Keys.LeftShift) || IsKeyPressed(Keys.RightShift)
            ? MovementType.Running
            : MovementType.Walking;

        if (IsKeyPressed(Keys.Up)) return (Direction.Up, movementType);
        if (IsKeyPressed(Keys.Down)) return (Direction.Down, movementType);
        if (IsKeyPressed(Keys.Left)) return (Direction.Left, movementType);
        if (IsKeyPressed(Keys.Right)) return (Direction.Right, movementType);

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

        var controlDown = IsKeyPressed(Keys.LeftControl) || IsKeyPressed(Keys.RightControl);
        if (!controlDown)
        {
            return;
        }

        if (localPlayer.TryAttack())
        {
            Network.Send<AttackRequest>();
        }
    }

    public override void Draw(GameTime gameTime)
    {
        var spriteBatch = new SpriteBatch(_graphicsDevice);

        var localPlayer = _game.LocalPlayer;
        if (localPlayer is not null)
        {
            spriteBatch.Begin(transformMatrix:
                Matrix.CreateTranslation(-(localPlayer.X + 16), -(localPlayer.Y + 16), 0) *
                Matrix.CreateTranslation(
                    _graphicsDevice.Viewport.Width / 2f,
                    _graphicsDevice.Viewport.Height / 2f,
                    0));
        }
        else
        {
            spriteBatch.Begin();
        }


        _game.Map.Draw(spriteBatch);

        if (_game.GettingMap)
        {
            // TODO: modText.DrawText(50, 50, "Receiving Map...", modText.BrightCyan);
        }

        spriteBatch.End();
    }

    public override void DrawUI(GameTime gameTime)
    {
        var spriteBatch = new SpriteBatch(_graphicsDevice);

        spriteBatch.Begin();

        _game.Map.DrawUI(spriteBatch);

        spriteBatch.End();

        ShowMenu();

        InventoryWindow.Show(_game);
        CharacterWindow.Show(_game);

        ShowVitals();

        ChatWindow.Show(_game);
    }

    private void ShowVitals()
    {
        var localPlayer = _game.LocalPlayer;
        if (localPlayer is null)
        {
            return;
        }

        ImGui.Begin("Vitals", ImGuiWindowFlags.AlwaysAutoResize);

        ImGui.PushStyleColor(ImGuiCol.PlotHistogram, new ImGuiVec4(1.0f, 0.0f, 0.0f, 1.0f));
        ImGui.PushStyleColor(ImGuiCol.PlotHistogramHovered, new ImGuiVec4(1.0f, 0.0f, 0.0f, 1.0f));
        ImGui.ProgressBar((float) localPlayer.Health / localPlayer.MaxHealth, new ImGuiVec2(140, 16), "HP");
        ImGui.PopStyleColor(2);
        ImGui.SameLine();
        ImGui.Text($"{localPlayer.Health}/{localPlayer.MaxHealth}");

        ImGui.Spacing();
        ImGui.PushStyleColor(ImGuiCol.PlotHistogram, new ImGuiVec4(0.0f, 0.0f, 1.0f, 1.0f));
        ImGui.PushStyleColor(ImGuiCol.PlotHistogramHovered, new ImGuiVec4(0.0f, 0.0f, 1.0f, 1.0f));
        ImGui.ProgressBar((float) localPlayer.Mana / localPlayer.MaxMana, new ImGuiVec2(140, 16), "MP");
        ImGui.PopStyleColor(2);
        ImGui.SameLine();
        ImGui.Text($"{localPlayer.Mana}/{localPlayer.MaxMana}");

        ImGui.Spacing();
        ImGui.PushStyleColor(ImGuiCol.PlotHistogram, new ImGuiVec4(1.0f, 1.0f, 0.0f, 1.0f));
        ImGui.PushStyleColor(ImGuiCol.PlotHistogramHovered, new ImGuiVec4(1.0f, 1.0f, 0.0f, 1.0f));
        ImGui.PushStyleColor(ImGuiCol.Text, new ImGuiVec4(0.0f, 0.0f, 0.0f, 1.0f));
        ImGui.ProgressBar((float) localPlayer.Stamina / localPlayer.MaxStamina, new ImGuiVec2(140, 16), "SP");
        ImGui.PopStyleColor(3);
        ImGui.SameLine();
        ImGui.Text($"{localPlayer.Stamina}/{localPlayer.MaxStamina}");
    }

    private void ShowMenu()
    {
        var buttonSize = new ImGuiVec2(100, 26);

        ImGui.Begin("Menu", ImGuiWindowFlags.AlwaysAutoResize);
        if (ImGui.Button("Inventory", buttonSize))
        {
            InventoryWindow.Open();
        }

        if (ImGui.Button("Character", buttonSize))
        {
            CharacterWindow.Open();
        }

        if (ImGui.Button("Train", buttonSize))
        {
        }

        if (ImGui.Button("Quit", buttonSize))
        {
            _game.Exit();
        }

        ImGui.End();
    }
}