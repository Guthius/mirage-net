using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mirage.Client.Net;
using Mirage.Net.Protocol.FromClient;
using Mirage.Net.Protocol.FromClient.New;
using Mirage.Shared.Data;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using ImGuiVec2 = System.Numerics.Vector2;
using ImGuiVec4 = System.Numerics.Vector4;

namespace Mirage.Client.Scenes;

public sealed class GameScene : Scene
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly Game _gameState;
    private string _chatMessage = string.Empty;
    private int _itemPickupTimer;

    public GameScene(GraphicsDevice graphicsDevice, Game gameState)
    {
        _graphicsDevice = graphicsDevice;
        _gameState = gameState;

        Textures.Sprites = Texture2D.FromFile(graphicsDevice, "Content/Sprites.png");
        Textures.Items = Texture2D.FromFile(graphicsDevice, "Content/Items.png");
    }

    protected override void OnShow()
    {
        _gameState.ClearStatus();
    }

    protected override void OnHide()
    {
        _gameState.ChatHistory.Clear();
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        _gameState.Map.Update(gameTime);

        if (!ImGui.GetIO().WantTextInput)
        {
            CheckForKeyboardInput();
        }
    }

    private void CheckForKeyboardInput()
    {
        if (IsKeyJustPressed(Keys.Enter) && !ImGui.GetIO().WantTextInput)
        {
            if (Environment.TickCount > _itemPickupTimer + 250)
            {
                _itemPickupTimer = Environment.TickCount;

                Network.Send<PickupItemRequest>();
            }
        }

        CheckAttack();
        CheckMovement();
    }

    private void CheckMovement()
    {
        if (_gameState.GettingMap)
        {
            return;
        }

        var localPlayer = _gameState.LocalPlayer;
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
            if (_gameState.Map.GetTileType(localPlayer.TileX, localPlayer.TileY) == TileType.Warp)
            {
                _gameState.GettingMap = true;
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
        if (_gameState.GettingMap)
        {
            return;
        }

        var localPlayer = _gameState.LocalPlayer;
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

        var localPlayer = _gameState.LocalPlayer;
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


        _gameState.Map.Draw(spriteBatch);

        if (_gameState.GettingMap)
        {
            // TODO: modText.DrawText(50, 50, "Receiving Map...", modText.BrightCyan);
        }

        spriteBatch.End();
    }

    public override void DrawUI(GameTime gameTime)
    {
        var spriteBatch = new SpriteBatch(_graphicsDevice);

        spriteBatch.Begin();

        _gameState.Map.DrawUI(spriteBatch);

        spriteBatch.End();

        ShowMenu();
        //ShowInventory();
        ShowVitals();
        ShowChat();
    }

    private void ShowVitals()
    {
        var localPlayer = _gameState.LocalPlayer;
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
        ImGui.Begin("Menu", ImGuiWindowFlags.AlwaysAutoResize);
        if (ImGui.Button("Inventory"))
        {
            _openInventory = !_openInventory;
        }

        if (ImGui.Button("Spells"))
        {
        }

        if (ImGui.Button("Stats"))
        {
        }

        if (ImGui.Button("Train"))
        {
            // using var frmTraining = new frmTraining();
            //
            // frmTraining.ShowDialog();
        }

        if (ImGui.Button("Trade"))
        {
            Network.Send<ShopRequest>();
        }

        if (ImGui.Button("Quit"))
        {
            _gameState.Exit();
        }

        ImGui.End();
    }

    private bool _openInventory;
    private int _selectedInventorySlot;
    private string _dropItemName = string.Empty;
    private int _dropQuantity = 1;
    private int _maxDropQuantity;

    private void ShowInventory()
    {
        // if (!_openInventory)
        // {
        //     return;
        // }
        //
        // if (!ImGui.Begin("Backpack", ref _openInventory, ImGuiWindowFlags.AlwaysAutoResize))
        // {
        //     return;
        // }
        //
        // var inventorySlots = GetInventorySlots().ToArray();
        //
        // ImGui.ListBox("##Items", ref _selectedInventorySlot, inventorySlots, inventorySlots.Length);
        // ImGui.BeginDisabled(_gameState.Inventory[_selectedInventorySlot].ItemId <= 0);
        // if (ImGui.Button("Use Item"))
        // {
        //     Network.Send(new UseItemRequest(_selectedInventorySlot + 1));
        // }
        //
        // if (ImGui.Button("Drop Item"))
        // {
        //     var selectedItemId = _gameState.Inventory[_selectedInventorySlot].ItemId;
        //
        //     _dropQuantity = 1;
        //     _maxDropQuantity = Math.Max(1, _gameState.Inventory[_selectedInventorySlot].Quantity);
        //     _dropItemName = modTypes.Item[selectedItemId].Name;
        //
        //     ImGui.OpenPopup("Drop");
        // }
        //
        // ImGui.EndDisabled();
        //
        // if (ImGui.BeginPopupModal("Drop"))
        // {
        //     ImGui.Text($"Drop {_dropItemName}");
        //     ImGui.InputInt("Quantity", ref _dropQuantity, 1, 10);
        //     _dropQuantity = Math.Min(_dropQuantity, _maxDropQuantity);
        //     ImGui.SameLine();
        //     ImGui.Text($" of {_maxDropQuantity}");
        //     ImGui.Separator();
        //
        //     if (ImGui.Button("OK"))
        //     {
        //         Network.Send(new DropItemRequest(_selectedInventorySlot + 1, _dropQuantity));
        //         ImGui.CloseCurrentPopup();
        //     }
        //
        //     ImGui.SameLine();
        //     ImGui.SetItemDefaultFocus();
        //     if (ImGui.Button("Cancel"))
        //     {
        //         ImGui.CloseCurrentPopup();
        //     }
        //
        //     ImGui.EndPopup();
        // }
        //
        // ImGui.End();
        //
        // IEnumerable<string> GetInventorySlots()
        // {
        //     var index = 0;
        //
        //     foreach (var slot in _gameState.Inventory)
        //     {
        //         index++;
        //
        //         var itemId = slot.ItemId;
        //         if (itemId is <= 0 or > Limits.MaxItems)
        //         {
        //             yield return $"{index}: <Free>";
        //             continue;
        //         }
        //
        //         ref var itemInfo = ref modTypes.Item[itemId];
        //         if (itemInfo.Type == modTypes.ITEM_TYPE_CURRENCY)
        //         {
        //             yield return $"{index}: {itemInfo.Name} ({slot.Quantity})";
        //             continue;
        //         }
        //
        //         var equipped =
        //             modTypes.Player[modGameLogic.MyIndex].ArmorSlot == index ||
        //             modTypes.Player[modGameLogic.MyIndex].WeaponSlot == index ||
        //             modTypes.Player[modGameLogic.MyIndex].ShieldSlot == index ||
        //             modTypes.Player[modGameLogic.MyIndex].HelmetSlot == index;
        //         
        //         if (equipped)
        //         {
        //             yield return $"{index}: {itemInfo.Name} (worn)";
        //             continue;
        //         }
        //
        //         ImGui.Selectable($"{index}: {itemInfo.Name}");
        //     }
        // }
    }

    private void ShowChat()
    {
        var inputTextHeight = ImGui.GetTextLineHeight() + ImGui.GetStyle().FramePadding.Y * 2;
        var inputRowHeight = inputTextHeight + ImGui.GetStyle().FramePadding.Y * 2;

        ImGui.Begin("Chat");

        var contentArea = ImGui.GetContentRegionAvail();

        ImGui.BeginListBox("##ChatMessages", contentArea with {Y = contentArea.Y - inputRowHeight});

        foreach (var chat in _gameState.ChatHistory)
        {
            ImGui.PushStyleColor(ImGuiCol.Text, ColorCodeTranslator.GetImGuiColor(chat.ColorCode));
            ImGui.TextWrapped(chat.Message);
            ImGui.PopStyleColor();
        }

        if (_gameState.ChatHistoryUpdated)
        {
            ImGui.SetScrollHereY(1.0f);
            _gameState.ChatHistoryUpdated = false;
        }

        ImGui.EndListBox();
        if (ImGui.InputText("##Message", ref _chatMessage, 256, ImGuiInputTextFlags.EnterReturnsTrue))
        {
            ChatProcessor.Handle(_chatMessage);
            _chatMessage = string.Empty;
        }

        ImGui.SameLine();
        if (ImGui.Button("Send"))
        {
            ChatProcessor.Handle(_chatMessage);
            _chatMessage = string.Empty;
        }

        ImGui.End();
    }
}