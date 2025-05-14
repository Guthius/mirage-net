using CommunityToolkit.Mvvm.DependencyInjection;
using Mirage.Client.Forms;
using Mirage.Client.Game;
using Mirage.Client.Modules;
using Mirage.Client.Repositories;
using Mirage.Client.Scenes;
using Mirage.Client.Services;
using Mirage.Game.Constants;
using Mirage.Net.Protocol.FromClient;
using Mirage.Net.Protocol.FromClient.New;
using Mirage.Net.Protocol.FromServer;
using Mirage.Net.Protocol.FromServer.New;

namespace Mirage.Client.Net;

public static class NetworkHandlers
{
    private static readonly IGameState GameState = Ioc.Default.GetRequiredService<IGameState>();
    private static readonly ISceneManager SceneManager = Ioc.Default.GetRequiredService<ISceneManager>();

    public static void HandleAuth(AuthResponse response)
    {
        GameState.ClearStatus();

        switch (response.Result)
        {
            case AuthResult.Ok:
                break;

            case AuthResult.InvalidAccountNameOrPassword:
                GameState.ShowAlert("Incorrect account name or password.");
                break;

            case AuthResult.InvalidProtocolVersion:
                GameState.ShowAlert("Your client is out of date. Please update your client and try again.");
                break;

            case AuthResult.AlreadyLoggedIn:
                GameState.ShowAlert("Account is already logged in.");
                break;

            default:
                GameState.ShowAlert("Unknown error.");
                break;
        }
    }

    public static void HandleJobList(JobList jobs)
    {
        GameState.Jobs = jobs.Jobs;
    }

    public static void HandleCharacterList(CharacterList characterList)
    {
        GameState.MaxCharacters = characterList.MaxCharacters;
        GameState.Characters = characterList.Characters;

        SceneManager.SwitchTo<CharacterSelectScene>();
    }

    public static void HandleCreateCharacter(CreateCharacterResponse response)
    {
        GameState.ClearStatus();

        switch (response.Result)
        {
            case CreateCharacterResult.Ok:
                break;

            case CreateCharacterResult.CharacterNameInvalid:
                GameState.ShowAlert("Invalid name, only letters, numbers, spaces, and _ allowed in names.");
                break;

            case CreateCharacterResult.CharacterNameTooShort:
                GameState.ShowAlert("Character name must be at least three characters in length.");
                break;

            case CreateCharacterResult.CharacterNameInUse:
                GameState.ShowAlert("Sorry, but that name is in use!");
                break;

            case CreateCharacterResult.InvalidJob:
                GameState.ShowAlert("Invalid character job.");
                break;

            default:
                GameState.ShowAlert("Unknown error.");
                break;
        }

        SceneManager.SwitchTo<CharacterSelectScene>();
    }

    public static void HandleSelectCharacter(SelectCharacterResponse response)
    {
        GameState.ClearStatus();

        switch (response.Result)
        {
            case SelectCharacterResult.Ok:
                GameState.SetStatus("Entering game...");
                modGameLogic.MyIndex = response.PlayerId;
                SceneManager.SwitchTo<LoadingScene>();
                return;

            case SelectCharacterResult.InvalidCharacter:
                GameState.ShowAlert("Invalid character.");
                return;

            default:
                GameState.ShowAlert("Unknown error.");
                return;
        }
    }

    public static void HandleLoadMap(LoadMapCommand command)
    {
        var map = MapManager.LoadMap(command.MapName, command.Revision);
        if (map is null)
        {
            Network.Send(new DownloadMapRequest(command.MapName));
        }
    }

    public static void HandleCreatePlayer(CreatePlayerCommand command)
    {
        
    }
    
    //---

    public static void HandleAlertMessage(AlertMessage alertMessage)
    {
        GameState.ShowAlert(alertMessage.Message);
        GameState.ClearStatus();
    }

    public static void HandleNewCharClasses(NewCharClasses newCharClasses)
    {
        GameState.Jobs = newCharClasses.Classes.ToList();

        SceneManager.SwitchTo<CreateCharacterScene>();
    }

    public static void HandleInGame(InGame inGame)
    {
        // modGameLogic.GameInit();
        // modGameLogic.GameLoop();

        SceneManager.SwitchTo<GameScene>();
    }

    public static void HandleInventory(PlayerInventory playerInventory)
    {
        GameState.Inventory = playerInventory.Slots;
    }

    public static void HandlePlayerInventoryUpdate(PlayerInventoryUpdate playerInventoryUpdate)
    {
        var slot = playerInventoryUpdate.InventorySlot;

        GameState.Inventory[slot - 1].ItemId = playerInventoryUpdate.ItemId;
        GameState.Inventory[slot - 1].Quantity = playerInventoryUpdate.Quantity;
        GameState.Inventory[slot - 1].Durability = playerInventoryUpdate.Durability;
    }

    public static void HandlePlayerEquipment(PlayerEquipment playerEquipment)
    {
        modTypes.Player[modGameLogic.MyIndex].ArmorSlot = playerEquipment.ArmorSlot;
        modTypes.Player[modGameLogic.MyIndex].WeaponSlot = playerEquipment.WeaponSlot;
        modTypes.Player[modGameLogic.MyIndex].HelmetSlot = playerEquipment.HelmetSlot;
        modTypes.Player[modGameLogic.MyIndex].ShieldSlot = playerEquipment.ShieldSlot;
    }

    public static void HandlePlayerHP(PlayerHp playerHp)
    {
        modTypes.Player[modGameLogic.MyIndex].MaxHP = playerHp.MaxHealth;
        modTypes.SetPlayerHP(modGameLogic.MyIndex, playerHp.Health);

        // if (modTypes.GetPlayerMaxHP(modGameLogic.MyIndex) > 0)
        // {
        //     var percent = (float) modTypes.GetPlayerHP(modGameLogic.MyIndex) / modTypes.GetPlayerMaxHP(modGameLogic.MyIndex) * 100;
        //     My.Forms.frmMirage.lblHP.Text = $"{(int) percent}%";
        // }
    }

    public static void HandlePlayerMP(PlayerMp playerMp)
    {
        modTypes.Player[modGameLogic.MyIndex].MaxMP = playerMp.MaxMana;
        modTypes.SetPlayerMP(modGameLogic.MyIndex, playerMp.Mana);
    }

    public static void HandlePlayerSP(PlayerSp playerSp)
    {
        modTypes.Player[modGameLogic.MyIndex].MaxSP = playerSp.MaxStamina;
        modTypes.SetPlayerSP(modGameLogic.MyIndex, playerSp.Stamina);
    }

    public static void HandlePlayerStats(PlayerStats playerStats)
    {
        modTypes.Player[modGameLogic.MyIndex].STR = (byte) playerStats.Strength;
        modTypes.Player[modGameLogic.MyIndex].DEF = (byte) playerStats.Defense;
        modTypes.Player[modGameLogic.MyIndex].SPEED = (byte) playerStats.Speed;
        modTypes.Player[modGameLogic.MyIndex].MAGI = (byte) playerStats.Magi;
    }

    public static void HandlePlayerData(PlayerData playerData)
    {
        var playerId = playerData.PlayerId;

        modTypes.Player[playerId].Name = playerData.Name;
        modTypes.Player[playerId].Sprite = playerData.Sprite;
        modTypes.Player[playerId].Map = playerData.MapId;
        modTypes.Player[playerId].X = (byte) playerData.X;
        modTypes.Player[playerId].Y = (byte) playerData.Y;
        modTypes.Player[playerId].Dir = (byte) playerData.Dir;
        modTypes.Player[playerId].Access = (byte) playerData.Access;
        modTypes.Player[playerId].PK = (byte) (playerData.PlayerKiller ? 1 : 0);
        modTypes.Player[playerId].Moving = 0;
        modTypes.Player[playerId].XOffset = 0;
        modTypes.Player[playerId].YOffset = 0;
    }

    public static void HandlePlayerMove(PlayerMove playerMove)
    {
        var playerId = playerMove.PlayerId;

        modTypes.Player[playerId].X = (byte) playerMove.X;
        modTypes.Player[playerId].Y = (byte) playerMove.Y;
        modTypes.Player[playerId].Dir = (byte) playerMove.Direction;
        modTypes.Player[playerId].XOffset = 0;
        modTypes.Player[playerId].YOffset = 0;
        modTypes.Player[playerId].Moving = (byte) playerMove.MovementType;

        switch (modTypes.GetPlayerDir(playerId))
        {
            case modTypes.DIR_UP:
                modTypes.Player[playerId].YOffset = modTypes.PIC_Y;
                break;
            case modTypes.DIR_DOWN:
                modTypes.Player[playerId].YOffset = modTypes.PIC_Y * -1;
                break;
            case modTypes.DIR_LEFT:
                modTypes.Player[playerId].XOffset = modTypes.PIC_X;
                break;
            case modTypes.DIR_RIGHT:
                modTypes.Player[playerId].XOffset = modTypes.PIC_X * -1;
                break;
        }
    }

    public static void HandlePlayerDir(PlayerDir playerDir)
    {
        var playerId = playerDir.PlayerId;

        modTypes.Player[playerId].Dir = (byte) playerDir.Direction;
        modTypes.Player[playerId].XOffset = 0;
        modTypes.Player[playerId].YOffset = 0;
        modTypes.Player[playerId].Moving = 0;
    }

    public static void HandleNpcMove(NpcMove npcMove)
    {
        var slot = npcMove.Slot;

        modTypes.MapNpc[slot].X = npcMove.X;
        modTypes.MapNpc[slot].Y = npcMove.Y;
        modTypes.MapNpc[slot].Dir = (int) npcMove.Direction;
        modTypes.MapNpc[slot].XOffset = 0;
        modTypes.MapNpc[slot].YOffset = 0;
        modTypes.MapNpc[slot].Moving = (int) npcMove.MovementType;

        switch (modTypes.MapNpc[slot].Dir)
        {
            case modTypes.DIR_UP:
                modTypes.MapNpc[slot].YOffset = modTypes.PIC_Y;
                break;
            case modTypes.DIR_DOWN:
                modTypes.MapNpc[slot].YOffset = modTypes.PIC_Y * -1;
                break;
            case modTypes.DIR_LEFT:
                modTypes.MapNpc[slot].XOffset = modTypes.PIC_X;
                break;
            case modTypes.DIR_RIGHT:
                modTypes.MapNpc[slot].XOffset = modTypes.PIC_X * -1;
                break;
        }
    }

    public static void HandleNpcDir(NpcDir npcDir)
    {
        var slot = npcDir.Slot;

        modTypes.MapNpc[slot].Dir = (byte) npcDir.Direction;
        modTypes.MapNpc[slot].XOffset = 0;
        modTypes.MapNpc[slot].YOffset = 0;
        modTypes.MapNpc[slot].Moving = 0;
    }

    public static void HandlePlayerPosition(PlayerPosition playerPosition)
    {
        modTypes.Player[modGameLogic.MyIndex].X = (byte) playerPosition.X;
        modTypes.Player[modGameLogic.MyIndex].Y = (byte) playerPosition.Y;
        modTypes.Player[modGameLogic.MyIndex].XOffset = 0;
        modTypes.Player[modGameLogic.MyIndex].YOffset = 0;
        modTypes.Player[modGameLogic.MyIndex].Moving = 0;
    }

    public static void HandleAttack(Attack attack)
    {
        var playerId = attack.PlayerId;

        modTypes.Player[playerId].Attacking = 1;
        modTypes.Player[playerId].AttackTimer = Environment.TickCount;
    }

    public static void HandleNpcAttack(NpcAttack npcAttack)
    {
        var slot = npcAttack.Slot;

        modTypes.MapNpc[slot].Attacking = 1;
        modTypes.MapNpc[slot].AttackTimer = Environment.TickCount;
    }

    public static void HandleCheckForMap(CheckForMap checkFormap)
    {
        for (var playerId = 1; playerId <= Limits.MaxPlayers; playerId++)
        {
            if (playerId != modGameLogic.MyIndex)
            {
                modTypes.Player[playerId].Map = 0;
            }
        }

        modTypes.ClearTempTile();

        var mapId = checkFormap.MapId;
        var revision = checkFormap.Revision;

        if (MapRepository.MapExist(mapId))
        {
            if (MapRepository.GetMapRevision(mapId) == revision)
            {
                MapRepository.LoadMap(mapId);

                Network.Send(new NeedMapRequest(false));
                return;
            }
        }

        Network.Send(new NeedMapRequest(true));
    }

    public static void HandleMapData(MapData mapData)
    {
        modGameLogic.SaveMap.Name = mapData.Map.Name;
        modGameLogic.SaveMap.Revision = mapData.Map.Revision;
        modGameLogic.SaveMap.Moral = (int) mapData.Map.Moral;
        modGameLogic.SaveMap.Up = mapData.Map.Up;
        modGameLogic.SaveMap.Down = mapData.Map.Down;
        modGameLogic.SaveMap.Left = mapData.Map.Left;
        modGameLogic.SaveMap.Right = mapData.Map.Right;
        modGameLogic.SaveMap.Music = mapData.Map.Music;
        modGameLogic.SaveMap.BootMap = mapData.Map.BootMapId;
        modGameLogic.SaveMap.BootX = mapData.Map.BootX;
        modGameLogic.SaveMap.BootY = mapData.Map.BootY;
        modGameLogic.SaveMap.Shop = mapData.Map.ShopId;

        for (var y = 0; y <= modTypes.MAX_MAPY; y++)
        {
            for (var x = 0; x <= modTypes.MAX_MAPX; x++)
            {
                var tileInfo = mapData.Map.Tiles[x, y];

                modGameLogic.SaveMap.Tile[x, y].Ground = tileInfo.Ground;
                modGameLogic.SaveMap.Tile[x, y].Mask = tileInfo.Mask;
                modGameLogic.SaveMap.Tile[x, y].Anim = tileInfo.Anim;
                modGameLogic.SaveMap.Tile[x, y].Fringe = tileInfo.Fringe;
                modGameLogic.SaveMap.Tile[x, y].Type = (int) tileInfo.Type;
                modGameLogic.SaveMap.Tile[x, y].Data1 = tileInfo.Data1;
                modGameLogic.SaveMap.Tile[x, y].Data2 = tileInfo.Data2;
                modGameLogic.SaveMap.Tile[x, y].Data3 = tileInfo.Data3;
            }
        }

        for (var i = 1; i <= Limits.MaxMapNpcs; i++)
        {
            modGameLogic.SaveMap.Npc[i] = mapData.Map.NpcIds[i];
        }

        MapRepository.SaveLocalMap(mapData.Map.Id);

        if (!modGameLogic.InEditor)
        {
            return;
        }

        modGameLogic.InEditor = false;

        My.Forms.frmMirage.picMapEditor.Visible = false;

        Application.OpenForms.OfType<frmMapWarp>().FirstOrDefault()?.Close();
        Application.OpenForms.OfType<frmMapProperties>().FirstOrDefault()?.Close();
    }

    public static void HandleMapItemData(MapItemData mapItemData)
    {
        for (var slot = 1; slot <= Limits.MaxMapItems; slot++)
        {
            var mapItemInfo = mapItemData.Items[slot - 1];
            if (mapItemInfo is null)
            {
                // TODO: Clear slot...
                continue;
            }

            modGameLogic.SaveMapItem[slot].Num = mapItemInfo.ItemId;
            modGameLogic.SaveMapItem[slot].X = mapItemInfo.X;
            modGameLogic.SaveMapItem[slot].Y = mapItemInfo.Y;
        }
    }

    public static void HandleMapNpcData(MapNpcData mapNpcData)
    {
        for (var slot = 1; slot <= Limits.MaxMapNpcs; slot++)
        {
            var mapNpcInfo = mapNpcData.Npcs[slot - 1];

            modGameLogic.SaveMapNpc[slot].Num = mapNpcInfo.NpcId;
            modGameLogic.SaveMapNpc[slot].X = mapNpcInfo.X;
            modGameLogic.SaveMapNpc[slot].Y = mapNpcInfo.Y;
            modGameLogic.SaveMapNpc[slot].Dir = (int) mapNpcInfo.Direction;
        }
    }

    public static void HandleMapDone(MapDone mapDone)
    {
        modTypes.Map = modGameLogic.SaveMap;

        for (var slot = 1; slot <= Limits.MaxMapItems; slot++)
        {
            modTypes.MapItem[slot] = modGameLogic.SaveMapItem[slot];
        }

        for (var slot = 1; slot <= Limits.MaxMapNpcs; slot++)
        {
            modTypes.MapNpc[slot] = modGameLogic.SaveMapNpc[slot];
        }

        modGameLogic.GettingMap = false;

        SoundService.StopMusic();

        if (modTypes.Map.Music > 0)
        {
            SoundService.PlayOgg($"Music{modTypes.Map.Music}.ogg");
        }
    }

    public static void HandlePlayerMessage(PlayerMessage playerMessage)
    {
        modText.AddText(playerMessage.Message, playerMessage.Color);
    }

    public static void HandleSpawnItem(SpawnItem spawnItem)
    {
        var slot = spawnItem.Slot;

        modTypes.MapItem[slot].Num = spawnItem.ItemId;
        modTypes.MapItem[slot].X = spawnItem.X;
        modTypes.MapItem[slot].Y = spawnItem.Y;
    }

    public static void HandleOpenItemEditor()
    {
        modGameLogic.InItemsEditor = true;

        using var frmIndex = new frmIndex();

        for (var slot = 1; slot <= Limits.MaxInventory; slot++)
        {
            frmIndex.lstIndex.Items.Add($"{slot}: {modTypes.Item[slot].Name}");
        }

        frmIndex.lstIndex.SelectedIndex = 0;
        frmIndex.ShowDialog();
    }

    public static void HandleUpdateItem(UpdateItem updateItem)
    {
        var itemInfo = updateItem.ItemInfo;
        var itemId = itemInfo.Id;

        modTypes.Item[itemId].Name = itemInfo.Name;
        modTypes.Item[itemId].Pic = itemInfo.Sprite;
        modTypes.Item[itemId].Type = (int) itemInfo.Type;
        modTypes.Item[itemId].Data1 = 0;
        modTypes.Item[itemId].Data2 = 0;
        modTypes.Item[itemId].Data3 = 0;
    }

    public static void HandleEditItem(EditItem editItem)
    {
        var itemInfo = editItem.ItemInfo;
        var itemId = itemInfo.Id;

        modTypes.Item[itemId].Name = itemInfo.Name;
        modTypes.Item[itemId].Pic = itemInfo.Sprite;
        modTypes.Item[itemId].Type = (int) itemInfo.Type;
        modTypes.Item[itemId].Data1 = itemInfo.Data1;
        modTypes.Item[itemId].Data2 = itemInfo.Data2;
        modTypes.Item[itemId].Data3 = itemInfo.Data3;

        using var frmItemEditor = new frmItemEditor();

        frmItemEditor.ShowDialog();
    }

    public static void HandleSpawnNpc(SpawnNpc spawnNpc)
    {
        var slot = spawnNpc.Slot;

        modTypes.MapNpc[slot].Num = spawnNpc.NpcId;
        modTypes.MapNpc[slot].X = spawnNpc.X;
        modTypes.MapNpc[slot].Y = spawnNpc.Y;
        modTypes.MapNpc[slot].Dir = (int) spawnNpc.Direction;
        modTypes.MapNpc[slot].XOffset = 0;
        modTypes.MapNpc[slot].YOffset = 0;
        modTypes.MapNpc[slot].Moving = 0;
    }

    public static void HandleNpcDead(NpcDead npcDead)
    {
        var slot = npcDead.Slot;

        modTypes.MapNpc[slot].Num = 0;
        modTypes.MapNpc[slot].X = 0;
        modTypes.MapNpc[slot].Y = 0;
        modTypes.MapNpc[slot].Dir = 0;
        modTypes.MapNpc[slot].XOffset = 0;
        modTypes.MapNpc[slot].YOffset = 0;
        modTypes.MapNpc[slot].Moving = 0;
    }

    public static void HandleOpenNpcEditor()
    {
        modGameLogic.InNpcEditor = true;

        using var frmIndex = new frmIndex();

        for (var npcId = 1; npcId <= Limits.MaxNpcs; npcId++)
        {
            frmIndex.lstIndex.Items.Add($"{npcId}: {modTypes.Npc[npcId].Name}");
        }

        frmIndex.lstIndex.SelectedIndex = 0;
        frmIndex.ShowDialog();
    }

    public static void HandleUpdateNpc(UpdateNpc updateNpc)
    {
        var npcId = updateNpc.NpcId;

        modTypes.Npc[npcId].Name = updateNpc.Name;
        modTypes.Npc[npcId].AttackSay = "";
        modTypes.Npc[npcId].Sprite = updateNpc.Sprite;
        modTypes.Npc[npcId].SpawnSecs = 0;
        modTypes.Npc[npcId].Behavior = 0;
        modTypes.Npc[npcId].Range = 0;
        modTypes.Npc[npcId].DropChance = 0;
        modTypes.Npc[npcId].DropItem = 0;
        modTypes.Npc[npcId].DropItemValue = 0;
        modTypes.Npc[npcId].STR = 0;
        modTypes.Npc[npcId].DEF = 0;
        modTypes.Npc[npcId].SPEED = 0;
        modTypes.Npc[npcId].MAGI = 0;
    }

    public static void HandleEditNpc(EditNpc editNpc)
    {
        var npcInfo = editNpc.NpcInfo;
        var npcId = npcInfo.Id;

        modTypes.Npc[npcId].Name = npcInfo.Name;
        modTypes.Npc[npcId].AttackSay = npcInfo.AttackSay;
        modTypes.Npc[npcId].Sprite = npcInfo.Sprite;
        modTypes.Npc[npcId].SpawnSecs = npcInfo.SpawnSecs;
        modTypes.Npc[npcId].Behavior = (int) npcInfo.Behavior;
        modTypes.Npc[npcId].Range = npcInfo.Range;
        modTypes.Npc[npcId].DropChance = npcInfo.DropChance;
        modTypes.Npc[npcId].DropItem = npcInfo.DropItemId;
        modTypes.Npc[npcId].DropItemValue = npcInfo.DropItemQuantity;
        modTypes.Npc[npcId].STR = npcInfo.Strength;
        modTypes.Npc[npcId].DEF = npcInfo.Defense;
        modTypes.Npc[npcId].SPEED = npcInfo.Speed;
        modTypes.Npc[npcId].MAGI = npcInfo.Intelligence;

        using var frmNpcEditor = new frmNpcEditor();

        frmNpcEditor.ShowDialog();
    }

    public static void HandleMapKey(MapKey mapKey)
    {
        var x = mapKey.X;
        var y = mapKey.Y;

        modTypes.TempTile[x, y].DoorOpen = mapKey.Unlocked ? 1 : 0;
    }

    public static void HandleOpenMapEditor()
    {
        modGameLogic.EditorInit();
    }

    public static void HandleOpenShopEditor()
    {
        modGameLogic.InShopEditor = true;

        using var frmIndex = new frmIndex();

        for (var shopId = 1; shopId <= Limits.MaxShops; shopId++)
        {
            frmIndex.lstIndex.Items.Add($"{shopId}: {modTypes.Shop[shopId].Name}");
        }

        frmIndex.lstIndex.SelectedIndex = 0;
        frmIndex.ShowDialog();
    }

    public static void HandleUpdateShop(UpdateShop updateShop)
    {
        var shopId = updateShop.ShopId;

        modTypes.Shop[shopId].Name = updateShop.Name;
    }

    public static void HandleEditShop(EditShop editShop)
    {
        var shopInfo = editShop.ShopInfo;
        var shopId = shopInfo.Id;

        modTypes.Shop[shopId].Name = shopInfo.Name;
        modTypes.Shop[shopId].JoinSay = shopInfo.JoinSay;
        modTypes.Shop[shopId].LeaveSay = shopInfo.LeaveSay;
        modTypes.Shop[shopId].FixesItems = shopInfo.FixesItems ? 1 : 0;

        for (var slot = 1; slot <= Limits.MaxShopTrades; slot++)
        {
            var trade = shopInfo.Trades[slot];

            modTypes.Shop[shopId].TradeItem[slot].GiveItem = trade.GiveItemId;
            modTypes.Shop[shopId].TradeItem[slot].GiveValue = trade.GiveItemQuantity;
            modTypes.Shop[shopId].TradeItem[slot].GetItem = trade.GetItemId;
            modTypes.Shop[shopId].TradeItem[slot].GetValue = trade.GetItemQuantity;
        }

        using var frmShopEditor = new frmShopEditor();

        frmShopEditor.ShowDialog();
    }

    public static void HandleOpenSpellEditor()
    {
        modGameLogic.InSpellEditor = true;

        using var frmIndex = new frmIndex();

        for (var spellId = 1; spellId <= Limits.MaxSpells; spellId++)
        {
            frmIndex.lstIndex.Items.Add($"{spellId}: {modTypes.Spell[spellId].Name}");
        }

        frmIndex.lstIndex.SelectedIndex = 0;
        frmIndex.ShowDialog();
    }

    public static void HandleUpdateSpell(UpdateSpell updateSpell)
    {
        var spellId = updateSpell.SpellId;

        modTypes.Spell[spellId].Name = updateSpell.Name;
    }

    public static void HandleEditSpell(EditSpell editSpell)
    {
        var spellInfo = editSpell.SpellInfo;
        var spellId = spellInfo.Id;

        modTypes.Spell[spellId].Name = spellInfo.Name;
        modTypes.Spell[spellId].ClassReq = 0; // spellInfo.RequiredClassId;
        modTypes.Spell[spellId].LevelReq = spellInfo.RequiredLevel;
        modTypes.Spell[spellId].Type = (int) spellInfo.Type;
        modTypes.Spell[spellId].Data1 = spellInfo.Data1;
        modTypes.Spell[spellId].Data2 = spellInfo.Data2;
        modTypes.Spell[spellId].Data3 = spellInfo.Data3;

        using var frmSpellEditor = new frmSpellEditor();

        frmSpellEditor.ShowDialog();
    }

    public static void HandleTrade(Trade trade)
    {
        var shopId = trade.ShopId;

        using var frmTrade = new frmTrade();

        frmTrade.picFixItems.Visible = trade.FixesItems;

        for (var i = 1; i <= Limits.MaxShopTrades; i++)
        {
            var tradeInfo = trade.Trades[i];

            var giveItemId = tradeInfo.GiveItemId;
            var getItemId = tradeInfo.GetItemId;

            if (giveItemId > 0 && getItemId > 0)
            {
                frmTrade.lstTrade.Items.Add($"Give {modTypes.Shop[shopId].Name} {tradeInfo.GiveItemQuantity} {modTypes.Item[giveItemId].Name} for {tradeInfo.GetItemQuantity} {modTypes.Item[getItemId].Name.Trim()}");
            }
        }

        if (frmTrade.lstTrade.Items.Count > 0)
        {
            frmTrade.lstTrade.SelectedItem = 0;
        }

        frmTrade.ShowDialog();
    }

    public static void HandlePlayerSpells(PlayerSpells playerSpells)
    {
        My.Forms.frmMirage.picPlayerSpells.Visible = true;
        My.Forms.frmMirage.lstSpells.Items.Clear();

        for (var slot = 1; slot <= Limits.MaxPlayerSpells; slot++)
        {
            modTypes.Player[modGameLogic.MyIndex].Spell[slot] = playerSpells.SpellIds[slot];

            My.Forms.frmMirage.lstSpells.Items.Add(modTypes.Player[modGameLogic.MyIndex].Spell[slot] != 0
                ? $"{slot}: {modTypes.Spell[modTypes.Player[modGameLogic.MyIndex].Spell[slot]].Name}"
                : "<free spells slot>");
        }

        My.Forms.frmMirage.lstSpells.SelectedIndex = 0;
    }
}