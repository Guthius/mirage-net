using Mirage.Game.Constants;
using Mirage.Game.Data;
using Mirage.Net.Protocol.FromClient;
using Mirage.Net.Protocol.FromClient.New;
using Mirage.Net.Protocol.FromServer;
using Mirage.Net.Protocol.FromServer.New;
using Mirage.Server.Game;
using Mirage.Server.Repositories;
using Serilog;
using static Mirage.Server.Net.Network;

namespace Mirage.Server.Net;

public static class NetworkHandlers
{
    public static void HandleAuth(GameSession session, AuthRequest request)
    {
        if (session.Account is not null)
        {
            return;
        }

        if (request.ProtocolVersion != ProtocolVersion)
        {
            session.Send(new AuthResponse(AuthResult.InvalidProtocolVersion));
            return;
        }

        var account = AccountRepository.Authenticate(request.AccountName, request.Password);
        if (account is null)
        {
            session.Send(new AuthResponse(AuthResult.InvalidAccountNameOrPassword));
            return;
        }

        if (GameState.IsAccountLoggedIn(request.AccountName))
        {
            session.Send(new AuthResponse(AuthResult.AlreadyLoggedIn));
            return;
        }

        session.Account = account;
        session.Send(new AuthResponse(AuthResult.Ok));
        session.Send(new UpdateJobListCommand(ClassRepository.GetAll()));
        session.Send(new UpdateCharacterListCommand(Limits.MaxCharacters, CharacterRepository.GetCharacterList(account.Id)));

        Log.Information("Account {AccountName} has logged in from {RemoteIp}", account.Name, GetIP(session.Id));
    }

    public static void HandleCreateCharacter(GameSession session, AccountInfo account, CreateCharacterRequest request)
    {
        var result = CharacterRepository.Create(account.Id, request.CharacterName, request.Gender, request.JobId);

        session.Send(new CreateCharacterResponse(result));
        if (result != CreateCharacterResult.Ok)
        {
            return;
        }

        Log.Information("Character {CharacterName} created by account {AccountName}", request.CharacterName, account.Name);

        session.Send(new UpdateCharacterListCommand(Limits.MaxCharacters, CharacterRepository.GetCharacterList(account.Id)));
    }

    public static void HandleDeleteCharacter(GameSession session, AccountInfo account, DeleteCharacterRequest request)
    {
        CharacterRepository.Delete(request.CharacterId, account.Id);

        Log.Information("Character deleted on account {AccountName}", account.Name);

        session.Send(new UpdateCharacterListCommand(Limits.MaxCharacters, CharacterRepository.GetCharacterList(account.Id)));
    }

    public static void HandleSelectCharacter(GameSession session, AccountInfo account, SelectCharacterRequest request)
    {
        var character = CharacterRepository.Get(request.CharacterId, account.Id);
        if (character is null)
        {
            session.Send(new SelectCharacterResponse(SelectCharacterResult.InvalidCharacter, -1));
            return;
        }

        session.Send(new SelectCharacterResponse(SelectCharacterResult.Ok, session.Id));
        session.CreatePlayer(character);

        Log.Information("Player {CharacterName} started playing {GameName} [Account: {AccountName}]",
            character.Name, Options.GameName, account.Name);
    }

    public static void HandleMove(GamePlayer player, MoveRequest request)
    {
        if (player.CastedSpell)
        {
            if (Environment.TickCount > player.AttackTimer + 1000)
            {
                player.CastedSpell = false;
            }
            else
            {
                player.Send(new PlayerPosition(player.Character.X, player.Character.Y));

                return;
            }
        }

        player.NewMap.Move(player, request.Direction, request.Movement);
    }

    //----

    public static void HandleCreateAccount(GameSession session, CreateAccountRequest request)
    {
        if (session.Account is not null)
        {
            return;
        }

        if (request.AccountName.Length < 3 || request.Password.Length < 3)
        {
            session.SendAlert("Account name and password must each contain at least 3 characters");
            return;
        }

        foreach (var ch in request.AccountName)
        {
            if (char.IsLetterOrDigit(ch) || ch == '_' || ch == ' ')
            {
                continue;
            }

            session.SendAlert("Invalid name, only letters, numbers, spaces, and _ allowed in names.");
            return;
        }

        if (AccountRepository.Exists(request.AccountName))
        {
            session.SendAlert("This account name is already taken. Please choose a different name.");

            return;
        }

        AccountRepository.Create(request.AccountName, request.Password);

        Log.Information("Account '{AccountName}' has been created.", request.AccountName);

        session.SendAlert("Your account has been created!");
    }

    public static void HandleDeleteAccount(GameSession session, DeleteAccountRequest request)
    {
        if (session.Account is not null)
        {
            return;
        }

        if (request.AccountName.Length < 3 || request.Password.Length < 3)
        {
            session.SendAlert("Account name and password must each contain at least 3 characters");
            return;
        }

        var account = AccountRepository.Authenticate(request.AccountName, request.Password);
        if (account is null)
        {
            session.SendAlert("Invalid account name or password.");
            return;
        }

        AccountRepository.Delete(account.Id);

        session.SendAlert("Your account has been deleted!");
    }

    public static void HandleSay(GamePlayer player, SayRequest request)
    {
        ChatProcessor.Handle(player, request.Message);
    }
    
    public static void HandlePlayerMessage(GamePlayer player, PlayerMessageRequest request)
    {
        var targetPlayerId = GameState.FindPlayer(request.TargetName);
        if (targetPlayerId is null)
        {
            player.Tell("Player is not online.", Color.White);
            return;
        }

        foreach (var ch in request.Message)
        {
            if (ch >= 32 && ch <= 126)
            {
                continue;
            }

            ReportHackAttempt(player.Id, "Player Msg Text Modification");
            return;
        }

        if (targetPlayerId == player)
        {
            Log.Information("Map #{MapId}: {CharacterName} begins to mumble to himself, what a wierdo...",
                player.Character.MapId,
                player.Character.Name);

            player.Map.SendMessage($"{player.Character.Name} begins to mumble to himself, what a wierdo...", Color.Green);

            return;
        }

        Log.Information("{FromCharacterName} tells {ToCharacterName}, '{Message}'",
            player.Character.Name,
            targetPlayerId.Character.Name,
            request.Message);

        targetPlayerId.Tell($"{player.Character.Name} tells you, '{request.Message}'", Color.TellColor);

        player.Tell($"You tell {targetPlayerId.Character.Name}, '{request.Message}'", Color.TellColor);
    }

    public static void HandleSetDirection(GamePlayer player, SetDirectionRequest request)
    {
        player.Character.Direction = request.Direction;
        player.Map.Send(player.Id, new PlayerDir(player.Id, player.Character.Direction));
    }

    public static void HandleUseItem(GamePlayer player, UseItemRequest request)
    {
        player.UseItem(request.InventorySlot);
    }

    public static void HandleAttack(GamePlayer player, AttackRequest request)
    {
        foreach (var otherPlayer in GameState.OnlinePlayers())
        {
            if (otherPlayer == player)
            {
                continue;
            }

            if (!player.CanAttackPlayer(otherPlayer))
            {
                continue;
            }

            if (otherPlayer.TryBlockHit(out var shieldInfo))
            {
                player.Tell($"{otherPlayer.Character.Name}'s {shieldInfo.Name} has blocked your hit!", Color.BrightCyan);

                otherPlayer.Tell($"Your {shieldInfo.Name} has blocked {player.Character.Name}'s hit!", Color.BrightRed);

                return;
            }

            var damage = player.CalculateDamage();
            if (player.TryCriticalHit())
            {
                damage += Random.Shared.Next(damage / 2) + 1 - otherPlayer.CalculateProtection();

                player.Tell("You feel a surge of energy upon swinging!", Color.BrightCyan);

                otherPlayer.Tell($"{player.Character.Name} swings with enormous might!", Color.BrightCyan);
            }
            else
            {
                damage -= otherPlayer.CalculateProtection();
            }

            if (damage > 0)
            {
                player.AttackPlayer(otherPlayer, damage);
            }
            else
            {
                player.Tell("Your attack does nothing.", Color.BrightRed);
            }

            return;
        }

        foreach (var npc in player.Map.AliveNpcs())
        {
            if (!player.CanAttackNpc(npc))
            {
                continue;
            }

            int damage;
            if (!player.TryCriticalHit())
            {
                damage = player.CalculateDamage() - npc.Info.Defense / 2;
            }
            else
            {
                var n = player.CalculateDamage();

                damage = n + Random.Shared.Next(n / 2) + 1 - npc.Info.Defense / 2;

                player.Tell("You feel a surge of energy upon swinging!", Color.BrightCyan);
            }

            if (damage > 0)
            {
                player.AttackNpc(npc, damage);
            }
            else
            {
                player.Tell("Your attack does nothing.", Color.BrightRed);
            }
        }
    }

    public static void HandleUseStatPoint(GamePlayer player, UseStatPointRequest request)
    {
        if (player.Character.StatPoints <= 0)
        {
            player.Tell("You have no skill points to train with!", Color.BrightRed);
            return;
        }

        player.Character.StatPoints--;
        switch (request.PointType)
        {
            case StatType.Strength:
                player.Character.Strength++;
                player.Tell("You have gained more strength!", Color.White);
                break;

            case StatType.Defense:
                player.Character.Defense++;
                player.Tell("You have gained more defense!", Color.White);
                break;

            case StatType.Intelligence:
                player.Character.Intelligence++;
                player.Tell("You have gained more magic abilities!", Color.White);
                break;

            case StatType.Speed:
                player.Character.Speed++;
                player.Tell("You have gained more speed!", Color.White);
                break;
        }

        player.SendStats();
    }
    
    public static void HandleGetStats(GamePlayer player, GetStatsRequest request)
    {
    }

    public static void HandleNewMap(GamePlayer player, NewMapRequest request)
    {
        player.Move(request.Direction, MovementType.Walking);
    }

    public static void HandleUpdateMap(GamePlayer player, UpdateMapRequest request)
    {
        var mapId = request.MapInfo.Id;

        player.Map.UpdateInfo(request.MapInfo);

        foreach (var otherPlayer in GameState.OnlinePlayers())
        {
            if (otherPlayer.Character.MapId == mapId)
            {
                otherPlayer.WarpTo(mapId, otherPlayer.Character.X, otherPlayer.Character.Y);
            }
        }

        Log.Information("{CharacterName} saved map #{ItemId}.", player.Character.Name, request.MapInfo.Id);
    }

    public static void HandleNeedMap(GamePlayer player, NeedMapRequest request)
    {
        if (request.NeedMap)
        {
            player.Send(new MapData(player.Map.Info));
        }

        player.Send(new MapItemData(player.Map.GetItemData()));
        player.Send(new MapNpcData(player.Map.GetNpcData()));

        player.SendJoinMap();

        player.GettingMap = false;

        player.Send(new MapDone());
    }

    public static void HandlePickupItem(GamePlayer player, PickupItemRequest request)
    {
        player.PickupItem();
    }

    public static void HandleDropItem(GamePlayer player, DropItemRequest request)
    {
        if (request.InventorySlot is < 1 or > Limits.MaxInventory)
        {
            ReportHackAttempt(player.Id, "Invalid InvNum");
            return;
        }

        var slotInfo = player.Character.Inventory[request.InventorySlot];
        if (request.Quantity > slotInfo.Quantity)
        {
            ReportHackAttempt(player.Id, "Item amount modification");
            return;
        }

        player.DropItem(request.InventorySlot, request.Quantity);
    }
    
    public static void HandleEditItem(GamePlayer player, EditItemRequest request)
    {
        var itemInfo = ItemRepository.Get(request.ItemId);
        if (itemInfo is null)
        {
            ReportHackAttempt(player.Id, "Invalid Item Index");
            return;
        }

        Log.Information("{CharacterName} editing item #{ItemId}", player.Character.Name, request.ItemId);

        player.Send(new EditItem(itemInfo));
    }

    public static void HandleUpdateItem(GamePlayer player, UpdateItemRequest request)
    {
        var itemInfo = ItemRepository.Get(request.ItemInfo.Id);
        if (itemInfo is null)
        {
            ReportHackAttempt(player.Id, "Invalid Item Index");
            return;
        }

        ItemRepository.Update(request.ItemInfo.Id, request.ItemInfo);

        Log.Information("{CharacterName} saved item #{ItemId}.", player.Character.Name, request.ItemInfo.Id);

        SendToAll(new UpdateItem(request.ItemInfo));
    }
    
    public static void HandleEditNpc(GamePlayer player, EditNpcRequest request)
    {
        var npcInfo = NpcRepository.Get(request.NpcId);
        if (npcInfo is null)
        {
            ReportHackAttempt(player.Id, "Invalid NPC Index");
            return;
        }

        Log.Information("{CharacterName} editing npc #{NpcId}.", player.Character.Name, request.NpcId);

        player.Send(new EditNpc(npcInfo));
    }

    public static void HandleUpdateNpc(GamePlayer player, UpdateNpcRequest request)
    {
        var npcInfo = NpcRepository.Get(request.NpcInfo.Id);
        if (npcInfo is null)
        {
            ReportHackAttempt(player.Id, "Invalid NPC Index");
            return;
        }

        NpcRepository.Update(request.NpcInfo.Id, request.NpcInfo);

        Log.Information("{CharacterName} saved npc #{NpcId}.", player.Character.Name, request.NpcInfo.Id);

        SendToAll(new UpdateNpc(request.NpcInfo.Id, request.NpcInfo.Name, request.NpcInfo.Sprite));
    }
    
    public static void HandleEditShop(GamePlayer player, EditShopRequest request)
    {
        var shopInfo = ShopRepository.Get(request.ShopId);
        if (shopInfo is null)
        {
            ReportHackAttempt(player.Id, "Invalid Shop Index");
            return;
        }

        Log.Information("{CharacterName} editing shop #{ShopId}", player.Character.Name, request.ShopId);

        player.Send(new EditShop(shopInfo));
    }

    public static void HandleUpdateShop(GamePlayer player, UpdateShopRequest request)
    {
        var shopInfo = ShopRepository.Get(request.ShopInfo.Id);
        if (shopInfo is null)
        {
            ReportHackAttempt(player.Id, "Invalid Shop Index");
            return;
        }

        ShopRepository.Update(request.ShopInfo.Id, request.ShopInfo);

        Log.Information("{CharacterName} saving shop #{ShopId}", player.Character.Name, request.ShopInfo.Id);

        SendToAll(new UpdateShop(request.ShopInfo.Id, request.ShopInfo.Name));
    }
    
    public static void HandleEditSpell(GamePlayer player, EditSpellRequest request)
    {
        var spellInfo = SpellRepository.Get(request.SpellId);
        if (spellInfo is null)
        {
            ReportHackAttempt(player.Id, "Invalid Spell Index");
            return;
        }

        Log.Information("{CharacterName} editing spell #{SpellId}", player.Character.Name, request.SpellId);

        player.Send(new EditSpell(spellInfo));
    }

    public static void HandleUpdateSpell(GamePlayer player, UpdateSpellRequest request)
    {
        var spellInfo = SpellRepository.Get(request.SpellInfo.Id);
        if (spellInfo is null)
        {
            ReportHackAttempt(player.Id, "Invalid Spell Index");
            return;
        }

        SpellRepository.Update(request.SpellInfo.Id, request.SpellInfo);

        Log.Information("{CharacterName} saving spell #{SpellId}.", player.Character.Name, request.SpellInfo.Id);

        SendToAll(new UpdateSpell(request.SpellInfo.Id, request.SpellInfo.Name));
    }

    public static void HandleShop(GamePlayer player, ShopRequest request)
    {
        var mapInfo = MapRepository.Get(player.Character.MapId);
        if (mapInfo is null)
        {
            return;
        }

        var shopInfo = ShopRepository.Get(mapInfo.ShopId);
        if (shopInfo is null)
        {
            player.Tell("There is no shop here.", Color.BrightRed);
            return;
        }

        foreach (var tradeInfo in shopInfo.Trades)
        {
            var itemInfo = ItemRepository.Get(tradeInfo.GetItemId);
            if (itemInfo is null || itemInfo.Type != ItemType.Spell)
            {
                continue;
            }

            var spellInfo = SpellRepository.Get(itemInfo.Data1);
            if (spellInfo is null)
            {
                continue;
            }

            player.Tell(!string.IsNullOrEmpty(spellInfo.RequiredClassId)
                    ? $"{itemInfo.Name} can be used by all classes."
                    : $"{itemInfo.Name} can only be used by a {ClassRepository.GetName(spellInfo.RequiredClassId)};",
                Color.Yellow);
        }

        player.Send(new Trade(shopInfo.Id, shopInfo.FixesItems, shopInfo.Trades));
    }

    public static void HandleShopTrade(GamePlayer player, ShopTradeRequest request)
    {
        if (request.Slot is < 0 or > Limits.MaxShopTrades)
        {
            ReportHackAttempt(player.Id, "Invalid Trade Index");
            return;
        }

        var mapInfo = MapRepository.Get(player.Character.MapId);
        if (mapInfo is null)
        {
            return;
        }

        var shopInfo = ShopRepository.Get(mapInfo.ShopId);
        if (shopInfo is null)
        {
            return;
        }

        var tradeInfo = shopInfo.Trades[request.Slot];

        var getItemInfo = ItemRepository.Get(tradeInfo.GetItemId);
        if (getItemInfo is null)
        {
            return;
        }

        var inventorySlot = player.GetFreeInventorySlot(getItemInfo);
        if (inventorySlot == 0)
        {
            player.Tell("Trade unsuccessful, inventory full.", Color.BrightRed);
            return;
        }

        if (player.GetItemQuantity(tradeInfo.GiveItemId) < tradeInfo.GiveItemQuantity)
        {
            player.Tell("Trade unsuccessful.", Color.BrightRed);
            return;
        }

        player.TakeItem(tradeInfo.GiveItemId, tradeInfo.GiveItemQuantity);
        player.GiveItem(tradeInfo.GetItemId, tradeInfo.GetItemQuantity);

        player.Tell("The trade was successful!", Color.Yellow);
    }

    public static void HandleFixItem(GamePlayer player, FixItemRequest request)
    {
        const int goldId = 1;

        var slotInfo = player.Character.Inventory[request.InventorySlot];

        var itemInfo = ItemRepository.Get(slotInfo.ItemId);
        if (itemInfo is null)
        {
            return;
        }

        if (itemInfo.Type is < ItemType.Weapon or > ItemType.Shield)
        {
            player.Tell("You can only fix weapons, armors, helmets, and shields.", Color.BrightRed);
            return;
        }

        var pointsToRepair = itemInfo.Data1 - slotInfo.Durability;
        if (pointsToRepair <= 0)
        {
            player.Tell("This item is in perfect condition!", Color.White);
            return;
        }

        var costPerPoint = Math.Min(1, itemInfo.Data2 / 5);
        var costTotal = Math.Min(1, pointsToRepair * costPerPoint);

        var availableGold = player.GetItemQuantity(goldId);
        if (availableGold < costPerPoint)
        {
            player.Tell("Insufficient gold to fix this item!", Color.BrightRed);
            return;
        }

        if (availableGold >= costTotal)
        {
            player.TakeItem(goldId, costTotal);

            slotInfo.Durability = itemInfo.Data1;

            player.Tell($"Item has been totally restored for {costTotal} gold!", Color.BrightBlue);
            return;
        }

        pointsToRepair = availableGold / costPerPoint;
        if (pointsToRepair <= 0)
        {
            return;
        }

        var cost = pointsToRepair * costPerPoint;

        player.TakeItem(goldId, cost);

        slotInfo.Durability += pointsToRepair;

        player.Tell($"Item has been partially fixed for {cost} gold!", Color.BrightBlue);
    }

    public static void HandleSearch(GamePlayer player, SearchRequest request)
    {
        if (request.X < 0 || request.X > Limits.MaxMapWidth ||
            request.Y < 0 || request.Y > Limits.MaxMapHeight)
        {
            return;
        }

        var mapId = player.Character.MapId;

        for (var otherId = 1; otherId <= Limits.MaxPlayers; otherId++)
        {
            var otherPlayer = GameState.GetPlayer(otherId);
            if (otherPlayer is null)
            {
                continue;
            }

            if (otherPlayer.Character.MapId != mapId ||
                otherPlayer.Character.X != request.X ||
                otherPlayer.Character.Y != request.Y)
            {
                continue;
            }

            var levelDifference = otherPlayer.Character.Level - player.Character.Level;
            switch (levelDifference)
            {
                case >= 5:
                    player.Tell("You wouldn't stand a chance.", Color.BrightRed);
                    break;

                case > 0:
                    player.Tell("This one seems to have an advantage over you.", Color.Yellow);
                    break;

                case <= -5:
                    player.Tell("You could slaughter that player.", Color.BrightBlue);
                    break;

                case < 0:
                    player.Tell("You would have an advantage over that player.", Color.Yellow);
                    break;

                default:
                    player.Tell("This would be an even fight.", Color.White);
                    break;
            }

            player.Target = otherId;
            player.TargetType = TargetType.Player;

            player.Tell($"Your target is now {otherPlayer.Character.Name}.", Color.Yellow);
            return;
        }

        // Check for an item
        var item = player.Map.GetItemAt(request.X, request.Y);
        if (item is not null)
        {
            var itemInfo = ItemRepository.Get(item.ItemId);
            if (itemInfo is null)
            {
                return;
            }

            player.Tell($"You see a {itemInfo.Name}.", Color.Yellow);
            return;
        }

        // Check for an NPC
        foreach (var npc in player.Map.AliveNpcs())
        {
            if (npc.X != request.X || npc.Y != request.Y)
            {
                continue;
            }

            player.Target = npc.Slot;
            player.TargetType = TargetType.Npc;

            player.Tell($"Your target is now a {npc.Info.Name}.", Color.Yellow);
            return;
        }
    }

    public static void HandleSpells(GamePlayer player, SpellsRequest request)
    {
        player.Send(new PlayerSpells(player.Character.SpellIds));
    }

    public static void HandleCast(GamePlayer player, CastRequest request)
    {
        player.Cast(request.SpellSlot);
    }
}