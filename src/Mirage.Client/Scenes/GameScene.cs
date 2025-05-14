using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mirage.Client.Forms;
using Mirage.Client.Game;
using Mirage.Client.Modules;
using Mirage.Client.Net;
using Mirage.Game.Constants;
using Mirage.Game.Data;
using Mirage.Net.Protocol.FromClient;
using Color = Microsoft.Xna.Framework.Color;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Mirage.Client.Scenes;

public sealed class GameScene(GraphicsDevice graphicsDevice, IGameState gameState) : Scene
{
    private const int TileWidth = 32;
    private const int TileHeight = 32;

    private readonly Texture2D _sprites = Texture2D.FromFile(graphicsDevice, "Assets/Sprites.png");
    private readonly Texture2D _tiles = Texture2D.FromFile(graphicsDevice, "Assets/Tiles.png");
    private readonly Texture2D _items = Texture2D.FromFile(graphicsDevice, "Assets/Items.png");
    private int _mapAnim;
    private int _mapAnimTimer;

    protected override void OnShow()
    {
        gameState.ClearStatus();
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        if (IsKeyPressed(Keys.Enter) && !ImGui.GetIO().WantTextInput)
        {
            if (Environment.TickCount > modTypes.Player[modGameLogic.MyIndex].MapGetTimer + 250)
            {
                modTypes.Player[modGameLogic.MyIndex].MapGetTimer = Environment.TickCount;
                Network.Send<PickupItemRequest>();
            }
        }
        
        CheckMovement();
        CheckAttack();

        for (var playerId = 1; playerId <= Limits.MaxPlayers; playerId++)
        {
            if (modClientTCP.IsPlaying(playerId))
            {
                ProcessMovement(playerId);
            }
        }

        for (var slot = 1; slot <= Limits.MaxMapNpcs; slot++)
        {
            if (modTypes.Map.Npc[slot] > 0)
            {
                ProcessNpcMovement(slot);
            }
        }

        if (Environment.TickCount > _mapAnimTimer + 250)
        {
            _mapAnim = _mapAnim == 0 ? 1 : 0;
            _mapAnimTimer = Environment.TickCount;
        }
    }

    private (Vector2 Vector, int Direction) GetMovementVector()
    {
        if (IsKeyPressed(Keys.Up))
        {
            return (new Vector2(0, -1), modTypes.DIR_UP);
        }

        if (IsKeyPressed(Keys.Down))
        {
            return (new Vector2(0, 1), modTypes.DIR_DOWN);
        }

        if (IsKeyPressed(Keys.Left))
        {
            return (new Vector2(-1, 0), modTypes.DIR_LEFT);
        }

        if (IsKeyPressed(Keys.Right))
        {
            return (new Vector2(1, 0), modTypes.DIR_RIGHT);
        }

        return (Vector2.Zero, 0);
    }

    private static int GetAdjacentMapId(int direction)
    {
        return direction switch
        {
            modTypes.DIR_UP => modTypes.Map.Up,
            modTypes.DIR_DOWN => modTypes.Map.Down,
            modTypes.DIR_LEFT => modTypes.Map.Left,
            modTypes.DIR_RIGHT => modTypes.Map.Right,
            _ => 0
        };
    }

    private static bool CanMove(int targetX, int targetY, int targetDirection)
    {
        ref var player = ref modTypes.Player[modGameLogic.MyIndex];

        if (player.Moving != 0)
        {
            return false;
        }

        if (player.CastedSpell == modTypes.YES)
        {
            if (Environment.TickCount > player.AttackTimer + 1000)
            {
                player.CastedSpell = modTypes.NO;
            }
            else
            {
                return false;
            }
        }

        var directionChanged = player.Dir != targetDirection;

        player.Dir = (byte) targetDirection;

        if (targetX < 0 || targetY < 0 || targetX > Limits.MaxMapWidth || targetY > Limits.MaxMapHeight)
        {
            if (directionChanged)
            {
                Network.Send(new SetDirectionRequest((Direction) player.Dir));
            }

            var targetMap = GetAdjacentMapId(targetDirection);
            if (targetMap == player.Map || targetMap <= 0)
            {
                return false;
            }

            Network.Send(new NewMapRequest((Direction) targetDirection));

            modGameLogic.GettingMap = true;

            return false;
        }

        /* Check whether the target tile is a wall or a closed door */
        switch (modTypes.Map.Tile[targetX, targetY].Type)
        {
            case modTypes.TILE_TYPE_BLOCKED:
            case modTypes.TILE_TYPE_KEY when modTypes.TempTile[targetX, targetY].DoorOpen == modTypes.NO:
            {
                if (directionChanged)
                {
                    Network.Send(new SetDirectionRequest((Direction) player.Dir));
                }

                return false;
            }
        }

        /* Check whether the target tile is occupied by another player */
        for (var playerId = 1; playerId <= Limits.MaxPlayers; playerId++)
        {
            if (!modClientTCP.IsPlaying(playerId) || modTypes.Player[playerId].Map != player.Map)
            {
                continue;
            }

            var playerX = modTypes.Player[playerId].X;
            var playerY = modTypes.Player[playerId].Y;

            if (playerX != targetX || playerY != targetY)
            {
                continue;
            }

            if (directionChanged)
            {
                Network.Send(new SetDirectionRequest((Direction) player.Dir));
            }

            return false;
        }

        /* Check whether the target tile is occupied by a NPC */
        for (var slot = 1; slot <= Limits.MaxMapNpcs; slot++)
        {
            if (modTypes.MapNpc[slot].Num <= 0)
            {
                continue;
            }

            if (modTypes.MapNpc[slot].X != targetX || modTypes.MapNpc[slot].Y != targetY)
            {
                continue;
            }

            if (directionChanged)
            {
                Network.Send(new SetDirectionRequest((Direction) player.Dir));
            }

            return false;
        }

        return true;
    }

    private void CheckMovement()
    {
        ref var player = ref modTypes.Player[modGameLogic.MyIndex];

        if (modGameLogic.GettingMap)
        {
            return;
        }

        var x = GetMovementVector();
        if (x.Vector == Vector2.Zero)
        {
            return;
        }

        var targetX = player.X + (int) x.Vector.X;
        var targetY = player.Y + (int) x.Vector.Y;

        if (!CanMove(targetX, targetY, x.Direction))
        {
            return;
        }

        var shiftDown = IsKeyPressed(Keys.LeftShift) || IsKeyPressed(Keys.RightShift);

        player.Moving = shiftDown ? modTypes.MOVING_RUNNING : modTypes.MOVING_WALKING;
        player.X = (byte) targetX;
        player.XOffset = TileWidth * (int) x.Vector.X;
        player.Y = (byte) targetY;
        player.YOffset = TileHeight * (int) x.Vector.Y;

        Network.Send(new MoveRequest((Direction) player.Dir, (MovementType) player.Moving));

        if (modTypes.Map.Tile[player.X, player.Y].Type == modTypes.TILE_TYPE_WARP)
        {
            modGameLogic.GettingMap = true;
        }
    }

    private void CheckAttack()
    {
        ref var player = ref modTypes.Player[modGameLogic.MyIndex];

        var controlDown = IsKeyPressed(Keys.LeftControl) || IsKeyPressed(Keys.RightControl);
        if (!controlDown || player.AttackTimer + 1000 >= Environment.TickCount || player.Attacking != 0)
        {
            return;
        }

        player.Attacking = 1;
        player.AttackTimer = Environment.TickCount;

        Network.Send<AttackRequest>();
    }

    private static void ProcessMovement(int playerId)
    {
        ref var player = ref modTypes.Player[playerId];

        var movementSpeed = player.Moving switch
        {
            modTypes.MOVING_WALKING => modGameLogic.WALK_SPEED,
            modTypes.MOVING_RUNNING => modGameLogic.RUN_SPEED,
            _ => 0
        };

        if (movementSpeed == 0)
        {
            return;
        }

        switch (player.Dir)
        {
            case modTypes.DIR_UP:
                player.YOffset -= movementSpeed;
                break;

            case modTypes.DIR_DOWN:
                player.YOffset += movementSpeed;
                break;

            case modTypes.DIR_LEFT:
                player.XOffset -= movementSpeed;
                break;

            case modTypes.DIR_RIGHT:
                player.XOffset += movementSpeed;
                break;
        }

        if (player is {XOffset: 0, YOffset: 0})
        {
            player.Moving = 0;
        }
    }

    private static void ProcessNpcMovement(int slot)
    {
        if (modTypes.MapNpc[slot].Moving != modTypes.MOVING_WALKING)
        {
            return;
        }

        switch (modTypes.MapNpc[slot].Dir)
        {
            case modTypes.DIR_UP:
                modTypes.MapNpc[slot].YOffset -= modGameLogic.WALK_SPEED;
                break;
            case modTypes.DIR_DOWN:
                modTypes.MapNpc[slot].YOffset += modGameLogic.WALK_SPEED;
                break;
            case modTypes.DIR_LEFT:
                modTypes.MapNpc[slot].XOffset -= modGameLogic.WALK_SPEED;
                break;
            case modTypes.DIR_RIGHT:
                modTypes.MapNpc[slot].XOffset += modGameLogic.WALK_SPEED;
                break;
        }

        if (modTypes.MapNpc[slot].XOffset == 0 && modTypes.MapNpc[slot].YOffset == 0)
        {
            modTypes.MapNpc[slot].Moving = 0;
        }
    }


    
    public override void Draw(GameTime gameTime)
    {
        var spriteBatch = new SpriteBatch(graphicsDevice);

        spriteBatch.Begin();

        for (var y = 0; y <= Limits.MaxMapHeight; y++)
        {
            for (var x = 0; x <= Limits.MaxMapWidth; x++)
            {
                DrawTile(spriteBatch, x, y);
            }
        }

        for (var slot = 1; slot <= Limits.MaxMapItems; slot++)
        {
            if (modTypes.MapItem[slot].Num > 0)
            {
                DrawItem(spriteBatch, slot);
            }
        }

        for (var slot = 1; slot <= Limits.MaxMapNpcs; slot++)
        {
            DrawNpc(spriteBatch, slot);
        }

        var mapId = modTypes.Player[modGameLogic.MyIndex].Map;
        for (var playerId = 1; playerId <= Limits.MaxPlayers; playerId++)
        {
            if (modClientTCP.IsPlaying(playerId) && modTypes.Player[playerId].Map == mapId)
            {
                DrawPlayer(spriteBatch, playerId);
            }
        }

        for (var y = 0; y <= Limits.MaxMapHeight; y++)
        {
            for (var x = 0; x <= Limits.MaxMapWidth; x++)
            {
                DrawFringeTile(spriteBatch, x, y);
            }
        }

        for (var playerId = 1; playerId <= Limits.MaxPlayers; playerId++)
        {
            if (modClientTCP.IsPlaying(playerId) && modTypes.GetPlayerMap(playerId) == mapId)
            {
                DrawPlayerName(playerId);
            }
        }
        
        DrawEditor(spriteBatch);
        
        // Draw map name
        var width = modText.MeasureText(modTypes.Map.Name);

        if (modTypes.Map.Moral == modTypes.MAP_MORAL_NONE)
        {
            var x = (modTypes.MAX_MAPX + 1) * modTypes.PIC_X / 2 - width / 2;
            // TODO: modText.DrawText(x, 8, modTypes.Map.Name, modText.BrightRed);
        }
        else
        {
            var x = (modTypes.MAX_MAPX + 1) * modTypes.PIC_X / 2 - width / 2;
            // TODO: modText.DrawText(x, 8, modTypes.Map.Name, modText.White);
        }


        if (modGameLogic.GettingMap)
        {
            // TODO: modText.DrawText(50, 50, "Receiving Map...", modText.BrightCyan);
        }

        
        
        spriteBatch.End();
    }

    private void DrawEditor(SpriteBatch spriteBatch)
    {
        // TODO:
        // if (modGameLogic.InEditor)
        // {
        //     for (var y = 0; y <= modTypes.MAX_MAPY; y++)
        //     {
        //         for (var x = 0; x <= modTypes.MAX_MAPX; x++)
        //         {
        //             var type = modTypes.Map.Tile[x, y].Type;
        //             switch (type)
        //             {
        //                 case modTypes.TILE_TYPE_BLOCKED:
        //                     modText.DrawText(x * modTypes.PIC_X + 8, y * modTypes.PIC_Y + 8, "B", modText.BrightRed);
        //                     break;
        //                 case modTypes.TILE_TYPE_WALKABLE:
        //                     modText.DrawText(x * modTypes.PIC_X + 8, y * modTypes.PIC_Y + 8, "P", modText.Yellow);
        //                     break;
        //                 case modTypes.TILE_TYPE_WARP:
        //                     modText.DrawText(x * modTypes.PIC_X + 8, y * modTypes.PIC_Y + 8, "W", modText.BrightBlue);
        //                     break;
        //                 case modTypes.TILE_TYPE_ITEM:
        //                     modText.DrawText(x * modTypes.PIC_X + 8, y * modTypes.PIC_Y + 8, "I", modText.White);
        //                     break;
        //                 case modTypes.TILE_TYPE_NPCAVOID:
        //                     modText.DrawText(x * modTypes.PIC_X + 8, y * modTypes.PIC_Y + 8, "N", modText.White);
        //                     break;
        //                 case modTypes.TILE_TYPE_KEY:
        //                     modText.DrawText(x * modTypes.PIC_X + 8, y * modTypes.PIC_Y + 8, "K", modText.White);
        //                     break;
        //                 case modTypes.TILE_TYPE_KEYOPEN:
        //                     modText.DrawText(x * modTypes.PIC_X + 8, y * modTypes.PIC_Y + 8, "O", modText.White);
        //                     break;
        //             }
        //         }
        //     }
        // }
    }
    
    private void DrawTile(SpriteBatch spriteBatch, int x, int y, int tileId)
    {
        var ty = tileId / 7;
        var tx = tileId % 7;

        spriteBatch.Draw(_tiles,
            new Vector2(x * TileWidth, y * TileHeight),
            new Rectangle(tx * TileWidth, ty * TileHeight, TileWidth, TileHeight),
            Color.White);
    }

    private void DrawTile(SpriteBatch spriteBatch, int x, int y)
    {
        DrawTile(spriteBatch, x, y, modTypes.Map.Tile[x, y].Ground);

        var anim1 = modTypes.Map.Tile[x, y].Mask;
        var anim2 = modTypes.Map.Tile[x, y].Anim;

        if (anim2 > 0)
        {
            if (_mapAnim == 0)
            {
                if (anim1 > 0 && modTypes.TempTile[x, y].DoorOpen == modTypes.NO)
                {
                    DrawTile(spriteBatch, x, y, anim1);
                }
            }
            else
            {
                DrawTile(spriteBatch, x, y, anim2);
            }
        }
        else
        {
            if (anim1 > 0 && modTypes.TempTile[x, y].DoorOpen == modTypes.NO)
            {
                DrawTile(spriteBatch, x, y, anim1);
            }
        }
    }

    private void DrawFringeTile(SpriteBatch spriteBatch, int x, int y)
    {
        var fringe = modTypes.Map.Tile[x, y].Fringe;

        if (fringe > 0)
        {
            DrawTile(spriteBatch, x, y, fringe);
        }
    }

    private void DrawItem(SpriteBatch spriteBatch, int slot)
    {
        ref var mapItem = ref modTypes.MapItem[slot];
        ref var item = ref modTypes.Item[mapItem.Num];

        var y = item.Pic * TileHeight;

        spriteBatch.Draw(_items,
            new Vector2(mapItem.X * TileWidth, mapItem.Y * TileHeight),
            new Rectangle(0, y, TileWidth, TileHeight),
            Color.White);
    }

    private void DrawNpc(SpriteBatch spriteBatch, int mapNpcNum)
    {
        if (modTypes.MapNpc[mapNpcNum].Num <= 0)
        {
            return;
        }

        ref var npc = ref modTypes.Npc[modTypes.MapNpc[mapNpcNum].Num];

        var anim = 0;
        if (modTypes.MapNpc[mapNpcNum].Attacking == 0)
        {
            switch (modTypes.MapNpc[mapNpcNum].Dir)
            {
                case modTypes.DIR_UP:
                    if (modTypes.MapNpc[mapNpcNum].YOffset < TileHeight / 2) anim = 1;
                    break;
                case modTypes.DIR_DOWN:
                    if (modTypes.MapNpc[mapNpcNum].YOffset < TileHeight / 2 * -1) anim = 1;
                    break;
                case modTypes.DIR_LEFT:
                    if (modTypes.MapNpc[mapNpcNum].XOffset < TileWidth / 2) anim = 1;
                    break;
                case modTypes.DIR_RIGHT:
                    if (modTypes.MapNpc[mapNpcNum].XOffset < TileWidth / 2 * -1) anim = 1;
                    break;
            }
        }
        else
        {
            if (modTypes.MapNpc[mapNpcNum].AttackTimer + 500 > Environment.TickCount)
            {
                anim = 2;
            }
        }

        if (modTypes.MapNpc[mapNpcNum].AttackTimer + 1000 < Environment.TickCount)
        {
            modTypes.MapNpc[mapNpcNum].Attacking = 0;
            modTypes.MapNpc[mapNpcNum].AttackTimer = 0;
        }

        var x = modTypes.MapNpc[mapNpcNum].X * TileWidth + modTypes.MapNpc[mapNpcNum].XOffset;
        var y = modTypes.MapNpc[mapNpcNum].Y * TileHeight + modTypes.MapNpc[mapNpcNum].YOffset - 4;
        if (y < 0)
        {
            y = 0;
        }

        spriteBatch.Draw(_sprites,
            new Vector2(x, y),
            new Rectangle((modTypes.MapNpc[mapNpcNum].Dir * 3 + anim) * TileWidth, npc.Sprite * TileHeight, TileWidth, TileHeight),
            Color.White);
    }

    private void DrawPlayer(SpriteBatch spriteBatch, int index)
    {
        var anim = 0;
        if (modTypes.Player[index].Attacking == 0)
        {
            switch (modTypes.Player[index].Dir)
            {
                case modTypes.DIR_UP:
                    if (modTypes.Player[index].YOffset < TileHeight / 2) anim = 1;
                    break;
                case modTypes.DIR_DOWN:
                    if (modTypes.Player[index].YOffset < TileHeight / 2 * -1) anim = 1;
                    break;
                case modTypes.DIR_LEFT:
                    if (modTypes.Player[index].XOffset < TileWidth / 2) anim = 1;
                    break;
                case modTypes.DIR_RIGHT:
                    if (modTypes.Player[index].XOffset < TileWidth / 2 * -1) anim = 1;
                    break;
            }
        }
        else
        {
            if (modTypes.Player[index].AttackTimer + 500 > Environment.TickCount)
            {
                anim = 2;
            }
        }

        if (modTypes.Player[index].AttackTimer + 1000 < Environment.TickCount)
        {
            modTypes.Player[index].Attacking = 0;
            modTypes.Player[index].AttackTimer = 0;
        }

        var x = modTypes.Player[index].X * TileWidth + modTypes.Player[index].XOffset;
        var y = modTypes.Player[index].Y * TileHeight + modTypes.Player[index].YOffset - 4;
        if (y < 0)
        {
            y = 0;
        }

        spriteBatch.Draw(_sprites,
            new Vector2(x, y),
            new Rectangle((modTypes.Player[index].Dir * 3 + anim) * TileWidth, modTypes.Player[index].Sprite * TileHeight, TileWidth, TileHeight),
            Color.White);
    }

    private void DrawPlayerName(int playerId)
    {
        int color;
        if (modTypes.Player[playerId].PK == modTypes.NO)
        {
            color = modTypes.Player[playerId].Access switch
            {
                0 => modText.White,
                1 => modText.DarkGrey,
                2 => modText.Cyan,
                3 => modText.Blue,
                4 => modText.Pink,
                _ => modText.White
            };
        }
        else
        {
            color = modText.BrightRed;
        }

        // Draw name
        var width = modText.MeasureText(modTypes.Player[playerId].Name);
        var x = modTypes.Player[playerId].X * modTypes.PIC_X + modTypes.Player[playerId].XOffset + modTypes.PIC_X / 2 - width / 2;
        var y = modTypes.Player[playerId].Y * modTypes.PIC_Y + modTypes.Player[playerId].YOffset - 24;

        // TODO: modText.DrawText(x, y, modTypes.Player[playerId].Name, color);
    }

    public override void DrawUI(GameTime gameTime)
    {
        GameEditor.ShowMapEditor();
        
        ShowMenu();
        ShowInventory();
        ShowChat();
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
            Network.Send<SpellsRequest>();
        }

        if (ImGui.Button("Stats"))
        {
            Network.Send<GetStatsRequest>();
        }

        if (ImGui.Button("Train"))
        {
            using var frmTraining = new frmTraining();

            frmTraining.ShowDialog();
        }

        if (ImGui.Button("Trade"))
        {
            Network.Send<ShopRequest>();
        }

        if (ImGui.Button("Quit"))
        {
            gameState.Exit();
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
        if (!_openInventory)
        {
            return;
        }

        if (!ImGui.Begin("Backpack", ref _openInventory, ImGuiWindowFlags.AlwaysAutoResize))
        {
            return;
        }

        var inventorySlots = GetInventorySlots().ToArray();

        ImGui.ListBox("##Items", ref _selectedInventorySlot, inventorySlots, inventorySlots.Length);
        ImGui.BeginDisabled(gameState.Inventory[_selectedInventorySlot].ItemId <= 0);
        if (ImGui.Button("Use Item"))
        {
            Network.Send(new UseItemRequest(_selectedInventorySlot + 1));
        }

        if (ImGui.Button("Drop Item"))
        {
            var selectedItemId = gameState.Inventory[_selectedInventorySlot].ItemId;

            _dropQuantity = 1;
            _maxDropQuantity = Math.Max(1, gameState.Inventory[_selectedInventorySlot].Quantity);
            _dropItemName = modTypes.Item[selectedItemId].Name;

            ImGui.OpenPopup("Drop");
        }

        ImGui.EndDisabled();

        if (ImGui.BeginPopupModal("Drop"))
        {
            ImGui.Text($"Drop {_dropItemName}");
            ImGui.InputInt("Quantity", ref _dropQuantity, 1, 10);
            _dropQuantity = Math.Min(_dropQuantity, _maxDropQuantity);
            ImGui.SameLine();
            ImGui.Text($" of {_maxDropQuantity}");
            ImGui.Separator();

            if (ImGui.Button("OK"))
            {
                Network.Send(new DropItemRequest(_selectedInventorySlot + 1, _dropQuantity));
                ImGui.CloseCurrentPopup();
            }

            ImGui.SameLine();
            ImGui.SetItemDefaultFocus();
            if (ImGui.Button("Cancel"))
            {
                ImGui.CloseCurrentPopup();
            }

            ImGui.EndPopup();
        }

        ImGui.End();

        IEnumerable<string> GetInventorySlots()
        {
            var index = 0;

            foreach (var slot in gameState.Inventory)
            {
                index++;

                var itemId = slot.ItemId;
                if (itemId is <= 0 or > Limits.MaxItems)
                {
                    yield return $"{index}: <Free>";
                    continue;
                }

                ref var itemInfo = ref modTypes.Item[itemId];
                if (itemInfo.Type == modTypes.ITEM_TYPE_CURRENCY)
                {
                    yield return $"{index}: {itemInfo.Name} ({slot.Quantity})";
                    continue;
                }

                var equipped =
                    modTypes.Player[modGameLogic.MyIndex].ArmorSlot == index ||
                    modTypes.Player[modGameLogic.MyIndex].WeaponSlot == index ||
                    modTypes.Player[modGameLogic.MyIndex].ShieldSlot == index ||
                    modTypes.Player[modGameLogic.MyIndex].HelmetSlot == index;

                if (equipped)
                {
                    yield return $"{index}: {itemInfo.Name} (worn)";
                    continue;
                }

                ImGui.Selectable($"{index}: {itemInfo.Name}");
            }
        }
    }

    private void ShowChat()
    {
        var inputTextHeight = ImGui.GetTextLineHeight() + ImGui.GetStyle().FramePadding.Y * 2;
        var inputRowHeight = inputTextHeight + ImGui.GetStyle().FramePadding.Y * 2;

        ImGui.Begin("Chat");

        var contentArea = ImGui.GetContentRegionAvail();

        ImGui.BeginListBox("##ChatMessages", contentArea with {Y = contentArea.Y - inputRowHeight});

        foreach (var chat in gameState.ChatHistory)
        {
            ImGui.PushStyleColor(ImGuiCol.Text, modText.GetColor(chat.Color));
            ImGui.TextWrapped(chat.Message);
            ImGui.PopStyleColor();
        }

        if (gameState.ChatHistoryUpdated)
        {
            ImGui.SetScrollHereY(1.0f);
            gameState.ChatHistoryUpdated = false;
        }

        ImGui.EndListBox();
        if (ImGui.InputText("##Message", ref _chatMessage, 256, ImGuiInputTextFlags.EnterReturnsTrue))
        {
            GameChat.Handle(_chatMessage);
            _chatMessage = string.Empty;
        }

        ImGui.SameLine();
        if (ImGui.Button("Send"))
        {
            GameChat.Handle(_chatMessage);
            _chatMessage = string.Empty;
        }

        ImGui.End();
    }

    private string _chatMessage = string.Empty;
}