using Mirage.Game.Constants;
using Mirage.Game.Data;
using Mirage.Net.Protocol.FromClient;
using Mirage.Net.Protocol.FromServer;
using Mirage.Server.Game;
using Mirage.Server.Game.Managers;
using Mirage.Server.Modules;
using Serilog;
using static Mirage.Server.Net.Network;

namespace Mirage.Server.Net;

public static class NetworkHandlers
{
    public static void HandleGetClasses(GameSession session, GetClassesRequest request)
    {
        if (session.Player is null)
        {
            session.Send(new NewCharClasses(modTypes.Classes));
        }
    }

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

        if (AccountManager.Exists(request.AccountName))
        {
            session.SendAlert("This account name is already taken. Please choose a different name.");

            return;
        }

        AccountManager.Create(request.AccountName, request.Password);

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

        var account = AccountManager.Authenticate(request.AccountName, request.Password);
        if (account is null)
        {
            session.SendAlert("Invalid account name or password.");
            return;
        }

        AccountManager.Delete(account.Id);

        session.SendAlert("Your account has been deleted!");
    }

    public static void HandleLogin(GameSession session, LoginRequest request)
    {
        if (session.Account is not null)
        {
            return;
        }

        if (request.Version.Major != Options.VersionMajor ||
            request.Version.Minor != Options.VersionMinor ||
            request.Version.Build != Options.VersionBuild)
        {
            session.SendAlert("Version outdated, please visit https://github.com/guthius/mirage-net/");
            return;
        }

        if (request.AccountName.Length < 3 || request.Password.Length < 3)
        {
            session.SendAlert("Account name and password must each contain at least 3 characters");
            return;
        }

        var account = AccountManager.Authenticate(request.AccountName, request.Password);
        if (account is null)
        {
            session.SendAlert("Invalid account name or password.");
            return;
        }

        if (GameState.IsAccountLoggedIn(request.AccountName))
        {
            session.SendAlert("Multiple account logins is not authorized.");
            return;
        }

        session.Account = account;

        Log.Information("{AccountName} has logged in from {RemoteIp}", account.Name, GetIP(session.Id));

        var characterSlotInfos = CharacterManager.GetCharacterSlots(account.Id);

        var emptyCharacterSlot = new CharacterSlotInfo();

        var characterSlots = Enumerable
            .Range(1, Limits.MaxCharacters)
            .Select(slot => { return characterSlotInfos.FirstOrDefault(c => c.Slot == slot) ?? emptyCharacterSlot; })
            .ToList();

        session.Send(new CharacterList(characterSlots));
    }

    public static void HandleCreateCharacter(GameSession session, AccountInfo account, CreateCharacterRequest request)
    {
        if (request.CharacterName.Length < 3)
        {
            session.SendAlert("Character name must be at least three characters in length.");
            return;
        }

        foreach (var ch in request.CharacterName)
        {
            if (char.IsLetterOrDigit(ch) || ch == '_' || ch == ' ')
            {
                continue;
            }

            session.SendAlert("Invalid name, only letters, numbers, spaces, and _ allowed in names.");
            return;
        }

        var (_, errorMessage) = CharacterManager.Create(account.Id, request.CharacterName, request.Gender, request.ClassId, request.Slot);
        if (errorMessage is not null)
        {
            ReportHackAttempt(session.Id, errorMessage);
            return;
        }

        Log.Information("Character '{CharacterName}' added to account '{AccountName}'", request.CharacterName, account.Name);

        session.SendAlert("Character has been created!");
    }

    public static void HandleDeleteCharacter(GameSession session, AccountInfo account, DeleteCharacterRequest request)
    {
        CharacterManager.Delete(account.Id, request.Slot);

        Log.Information("Character deleted on account '{AccountName}'", account.Name);

        session.SendAlert("Character has been deleted!");
    }

    public static void HandleSelectCharacter(GameSession session, AccountInfo account, SelectCharacterRequest request)
    {
        var character = CharacterManager.Get(account.Id, request.Slot);
        if (character is null)
        {
            session.SendAlert("Character does not exist!");
            return;
        }

        session.CreatePlayer(character);

        Log.Information("{AccountName}/{CharacterName} has began playing {GameName}", account.Name, character.Name, Options.GameName);
    }

    public static void HandleSay(GamePlayer player, SayRequest request)
    {
        foreach (var ch in request.Message)
        {
            if (ch >= 32 && ch <= 126)
            {
                continue;
            }

            ReportHackAttempt(player.Id, "Say Text Modification");
            return;
        }

        Log.Information("Map #{MapId}: {CharacterName} says '{Message}'",
            player.Character.MapId,
            player.Character.Name,
            request.Message);

        player.Map.SendMessage($"{player.Character.Name} says '{request.Message}'", Color.SayColor);
    }

    public static void HandleEmote(GamePlayer player, EmoteRequest request)
    {
        foreach (var ch in request.Message)
        {
            if (ch >= 32 && ch <= 126)
            {
                continue;
            }

            ReportHackAttempt(player.Id, "Emote Text Modification");
            return;
        }

        Log.Information("Map #{MapId}: {CharacterName} {Message}",
            player.Character.MapId,
            player.Character.Name,
            request.Message);

        player.Map.SendMessage($"{player.Character.Name} {request.Message}", Color.EmoteColor);
    }

    public static void HandleBroadcast(GamePlayer player, BroadcastRequest request)
    {
        foreach (var ch in request.Message)
        {
            if (ch >= 32 && ch <= 126)
            {
                continue;
            }

            ReportHackAttempt(player.Id, "Broadcast Text Modification");
            return;
        }

        Log.Information("{CharacterName}: {Message}", player.Character.Name, request.Message);

        SendToAll(new GlobalMessage($"{player.Character.Name}: {request.Message}", Color.BroadcastColor));
    }

    public static void HandleGlobalMessage(GamePlayer player, GlobalMessageRequest request)
    {
        foreach (var ch in request.Message)
        {
            if (ch >= 32 && ch <= 126)
            {
                continue;
            }

            ReportHackAttempt(player.Id, "Global Text Modification");
            return;
        }

        if (player.Character.AccessLevel <= 0)
        {
            return;
        }

        Log.Information("(global) {CharacterName}: {Message}", player.Character.Name, request.Message);

        SendToAll(new GlobalMessage($"(global) {player.Character.Name}: {request.Message}", Color.GlobalColor));
    }

    public static void HandleAdminMessage(GamePlayer player, AdminMessageRequest request)
    {
        foreach (var ch in request.Message)
        {
            if (ch >= 32 && ch <= 126)
            {
                continue;
            }

            ReportHackAttempt(player.Id, "Admin Text Modification");
            return;
        }

        if (player.Character.AccessLevel <= 0)
        {
            return;
        }

        Log.Information("(admin {CharacterName}) {Message}", player.Character.Name, request.Message);

        SendToAll(new GlobalMessage($"(admin {player.Character.Name}) {request.Message}", Color.AdminColor));
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

        player.Move(request.Direction, request.Movement);
    }

    public static void HandleSetDirection(GamePlayer player, SetDirectionRequest request)
    {
        player.Character.Direction = request.Direction;
        player.Map.Send(player.Id, new PlayerDir(player.Id, player.Character.Direction));
    }

    public static void HandleUseItem(GamePlayer player, UseItemRequest request)
    {
        player.UseItem(request.Slot);
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

    public static void HandlePlayerInfoRequest(GamePlayer player, PlayerInfoRequest request)
    {
        var targetPlayer = GameState.FindPlayer(request.TargetName);
        if (targetPlayer is null)
        {
            player.Tell("Player is not online.", Color.White);
            return;
        }

        player.Tell($"Account: {GameState.Sessions[targetPlayer.Id]?.Account?.Name}, Name: {targetPlayer.Character.Name}", Color.BrightGreen);
        if (player.Character.AccessLevel <= AccessLevel.Moderator)
        {
            return;
        }

        player.Tell($"-=- Stats for {targetPlayer.Character.Name} -=-", Color.BrightGreen);
        player.Tell($"Level: {targetPlayer.Character.Level}  Exp: {targetPlayer.Character.Exp}/{targetPlayer.Character.RequiredExp}", Color.BrightGreen);
        player.Tell($"HP: {targetPlayer.Character.HP}/{targetPlayer.Character.MaxHP}  MP: {targetPlayer.Character.MP}/{targetPlayer.Character.MaxMP}  SP: {targetPlayer.Character.SP}/{targetPlayer.Character.MaxSP}", Color.BrightGreen);
        player.Tell($"STR: {targetPlayer.Character.Strength}  DEF: {targetPlayer.Character.Defense}  MAGI: {targetPlayer.Character.Intelligence}  SPEED: {targetPlayer.Character.Speed}", Color.BrightGreen);

        player.Tell($"Critical Hit Chance: {targetPlayer.Character.CriticalHitRate}%, Block Chance: {targetPlayer.Character.BlockRate}%", Color.BrightGreen);
    }

    public static void HandleWarpMeTo(GamePlayer player, WarpMeToRequest request)
    {
        var targetPlayer = GameState.FindPlayer(request.TargetName);
        if (targetPlayer is null)
        {
            player.Tell("Player is not online.", Color.White);
            return;
        }

        if (targetPlayer == player)
        {
            player.Tell("You cannot warp to yourself!", Color.White);
            return;
        }

        player.WarpTo(targetPlayer.Character.MapId, targetPlayer.Character.X, targetPlayer.Character.Y);

        Log.Information("{CharacterName} has warped to {TargetCharacterName}, map #{MapId}.",
            player.Character.Name,
            targetPlayer.Character.Name,
            targetPlayer.Character.MapId);

        targetPlayer.Tell($"{player.Character.Name} has warped to you.", Color.BrightBlue);

        player.Tell($"You have been warped to {targetPlayer.Character.Name}.", Color.BrightBlue);
    }

    public static void HandleWarpToMe(GamePlayer player, WarpToMeRequest request)
    {
        var targetPlayer = GameState.FindPlayer(request.TargetName);
        if (targetPlayer is null)
        {
            player.Tell("Player is not online.", Color.White);
            return;
        }

        if (targetPlayer == player)
        {
            player.Tell("You cannot warp yourself to yourself!", Color.White);
            return;
        }

        targetPlayer.WarpTo(player.Character.MapId, player.Character.X, player.Character.Y);

        Log.Information("{CharacterName} has warped {TargetCharacterName} to self, map #{MapId}.",
            player.Character.Name,
            targetPlayer.Character.Name,
            player.Character.MapId);

        targetPlayer.Tell($"You have been summoned by {player.Character.Name}.", Color.BrightBlue);

        player.Tell($"{targetPlayer.Character.Name} has been summoned.", Color.BrightBlue);
    }

    public static void HandleWarpTo(GamePlayer player, WarpToRequest request)
    {
        var mapInfo = MapManager.Get(request.MapId);
        if (mapInfo is null)
        {
            ReportHackAttempt(player.Id, "Invalid map");
            return;
        }

        player.WarpTo(request.MapId, player.Character.X, player.Character.Y);
        player.Tell($"You have been warped to map #{request.MapId}", Color.BrightBlue);

        Log.Information("{CharacterName} warped to map #{MapId}", player.Character.Name, request.MapId);
    }

    public static void HandleSetSprite(GamePlayer player, SetSpriteRequest request)
    {
        player.Character.Sprite = request.Sprite;
        player.SendPlayerData();
    }

    public static void HandleGetStats(GamePlayer player, GetStatsRequest request)
    {
        player.Tell($"-=- Stats for {player.Character.Name} -=-", Color.White);
        player.Tell($"Level: {player.Character.Level}  Exp: {player.Character.Exp}/{player.Character.RequiredExp}", Color.White);
        player.Tell($"HP: {player.Character.HP}/{player.Character.MaxHP}  MP: {player.Character.MP}/{player.Character.MaxMP}  SP: {player.Character.SP}/{player.Character.MaxSP}", Color.White);
        player.Tell($"STR: {player.Character.Strength}  DEF: {player.Character.Defense}  MAGI: {player.Character.Intelligence}  SPEED: {player.Character.Speed}", Color.White);
        player.Tell($"Critical Hit Chance: {player.Character.CriticalHitRate}%, Block Chance: {player.Character.BlockRate}%", Color.White);
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

    public static void HandleMapRespawn(GamePlayer player, MapRespawnRequest request)
    {
        var mapId = player.Character.MapId;

        for (var slot = 1; slot <= Limits.MaxMapItems; slot++)
        {
            player.Map.ClearItem(slot);
        }

        player.Map.RespawnItems();
        player.Map.RespawnNpcs();

        player.Tell("Map respawned.", Color.Blue);

        Log.Information("{CharacterName} has respawned map #{MapId}", player.Character.Name, mapId);
    }

    public static void HandleMapReport(GamePlayer player, MapReportRequest request)
    {
        var mapStart = 1;
        var mapEnd = 1;

        var ranges = new List<string>();
        for (var mapId = 1; mapId <= Limits.MaxMaps; mapId++)
        {
            if (string.IsNullOrWhiteSpace(modTypes.Maps[mapId].Name))
            {
                mapEnd++;
            }
            else
            {
                if (mapEnd - mapStart > 0)
                {
                    ranges.Add($"{mapStart}-{mapEnd - 1}");
                }

                mapStart = mapId + 1;
                mapEnd = mapId + 1;
            }
        }

        ranges.Add($"{mapStart}-{mapEnd - 1}");

        player.Tell($"Free Maps: {string.Join(", ", ranges)}.", Color.Brown);
    }

    public static void HandleKickPlayer(GamePlayer player, KickPlayerRequest request)
    {
        var targetPlayer = GameState.FindPlayer(request.TargetName);
        if (targetPlayer is null)
        {
            player.Tell("Player is not online.", Color.White);
            return;
        }

        if (targetPlayer == player)
        {
            player.Tell("You cannot kick yourself!", Color.White);
            return;
        }

        if (targetPlayer.Character.AccessLevel > player.Character.AccessLevel)
        {
            player.Tell("That is a higher access admin then you!", Color.White);
            return;
        }

        SendGlobalMessage($"{targetPlayer.Character.Name} has been kicked from {Options.GameName} by {player.Character.Name}!", Color.White);

        Log.Information("{CharacterName} has kicked {TargetCharacterName}.", player.Character.Name, targetPlayer.Character.Name);

        targetPlayer.SendAlert($"You have been kicked by {player.Character.Name}!");
    }

    public static void HandleBanList(GamePlayer player, BanListRequest request)
    {
        if (!File.Exists("Banlist.txt"))
        {
            return;
        }

        var lineNumber = 1;

        using var reader = File.OpenText("Banlist.txt");
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (line is null)
            {
                continue;
            }

            var comma = line.IndexOf(',');
            if (comma == -1)
            {
                continue;
            }

            var ip = line[..comma];
            var name = line[(comma + 1)..];

            player.Tell($"{lineNumber}: Banned IP {ip} by {name}", Color.White);

            lineNumber++;
        }
    }

    public static void HandleBanDestroy(GamePlayer player, BanDestroyRequest request)
    {
        BanManager.Clear();

        player.Tell("Ban list destroyed.", Color.White);
    }

    public static void HandleBanPlayer(GamePlayer player, BanPlayerRequest request)
    {
        var targetPlayer = GameState.FindPlayer(request.TargetName);
        if (targetPlayer is null)
        {
            player.Tell("Player is not online.", Color.White);
            return;
        }

        if (targetPlayer == player)
        {
            player.Tell("You cannot ban yourself!", Color.White);
            return;
        }

        if (targetPlayer.Character.AccessLevel > player.Character.AccessLevel)
        {
            player.Tell("That is a higher access admin then you!", Color.White);
            return;
        }

        BanManager.BanIndex(targetPlayer.Id, player.Id);
    }

    public static void HandleOpenMapEditor(GamePlayer player, OpenManEditorRequest request)
    {
        player.Send<OpenMapEditor>();
    }

    public static void HandleOpenItemEditor(GamePlayer player, OpenItemEditorRequest request)
    {
        player.Send<OpenItemEditor>();
    }

    public static void HandleEditItem(GamePlayer player, EditItemRequest request)
    {
        var itemInfo = ItemManager.Get(request.ItemId);
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
        var itemInfo = ItemManager.Get(request.ItemInfo.Id);
        if (itemInfo is null)
        {
            ReportHackAttempt(player.Id, "Invalid Item Index");
            return;
        }

        ItemManager.Update(request.ItemInfo.Id, request.ItemInfo);

        Log.Information("{CharacterName} saved item #{ItemId}.", player.Character.Name, request.ItemInfo.Id);

        SendToAll(new UpdateItem(request.ItemInfo.Id, request.ItemInfo));
    }

    public static void HandleOpenNpcEditor(GamePlayer player, OpenNpcEditorRequest request)
    {
        player.Send<OpenNpcEditor>();
    }

    public static void HandleEditNpc(GamePlayer player, EditNpcRequest request)
    {
        var npcInfo = NpcManager.Get(request.NpcId);
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
        var npcInfo = NpcManager.Get(request.NpcInfo.Id);
        if (npcInfo is null)
        {
            ReportHackAttempt(player.Id, "Invalid NPC Index");
            return;
        }

        NpcManager.Update(request.NpcInfo.Id, request.NpcInfo);

        Log.Information("{CharacterName} saved npc #{NpcId}.", player.Character.Name, request.NpcInfo.Id);

        SendToAll(new UpdateNpc(request.NpcInfo.Id, request.NpcInfo.Name, request.NpcInfo.Sprite));
    }

    public static void HandleSetAccessLevel(GamePlayer player, SetAccessLevelRequest request)
    {
        var targetPlayer = GameState.FindPlayer(request.TargetName);
        if (targetPlayer is null)
        {
            player.Tell("Player is not online.", Color.White);
            return;
        }

        if (targetPlayer.Character.AccessLevel <= AccessLevel.Player)
        {
            SendGlobalMessage($"{targetPlayer.Character.Name} has been blessed with administrative access.", Color.BrightBlue);
        }

        targetPlayer.Character.AccessLevel = request.AccessLevel;

        Log.Information("{CharacterName} has modified {TargetCharacterName}'s access..", player.Character.Name, targetPlayer.Character.Name);

        targetPlayer.Map.Send(new PlayerData(
            targetPlayer.Id,
            targetPlayer.Character.Name,
            targetPlayer.Character.Sprite,
            targetPlayer.Character.MapId,
            targetPlayer.Character.X,
            targetPlayer.Character.Y,
            targetPlayer.Character.Direction,
            targetPlayer.Character.AccessLevel,
            targetPlayer.Character.PlayerKiller));
    }

    public static void HandleSetMotd(GamePlayer player, SetMotdRequest request)
    {
        File.WriteAllText("Motd.txt", request.Motd);

        Log.Information("{CharacterName} changed MOTD to: {NewMotd}", player.Character.Name, request.Motd);

        SendToAll(new GlobalMessage($"MOTD changed to: {request.Motd}", Color.BrightCyan));
    }

    public static void HandleOpenShopEditor(GamePlayer player, OpenShopEditorRequest request)
    {
        player.Send<OpenShopEditor>();
    }

    public static void HandleEditShop(GamePlayer player, EditShopRequest request)
    {
        var shopInfo = ShopManager.Get(request.ShopId);
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
        var shopInfo = ShopManager.Get(request.ShopInfo.Id);
        if (shopInfo is null)
        {
            ReportHackAttempt(player.Id, "Invalid Shop Index");
            return;
        }

        ShopManager.Update(request.ShopInfo.Id, request.ShopInfo);

        Log.Information("{CharacterName} saving shop #{ShopId}", player.Character.Name, request.ShopInfo.Id);

        SendToAll(new UpdateShop(request.ShopInfo.Id, request.ShopInfo.Name));
    }

    public static void OpenSpellEditor(GamePlayer player, OpenSpellEditorRequest request)
    {
        player.Send<OpenSpellEditor>();
    }

    public static void HandleEditSpell(GamePlayer player, EditSpellRequest request)
    {
        var spellInfo = SpellManager.Get(request.SpellId);
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
        var spellInfo = SpellManager.Get(request.SpellInfo.Id);
        if (spellInfo is null)
        {
            ReportHackAttempt(player.Id, "Invalid Spell Index");
            return;
        }

        SpellManager.Update(request.SpellInfo.Id, request.SpellInfo);

        Log.Information("{CharacterName} saving spell #{SpellId}.", player.Character.Name, request.SpellInfo.Id);

        SendToAll(new UpdateSpell(request.SpellInfo.Id, request.SpellInfo.Name));
    }

    public static void HandleWhosOnline(GamePlayer player, WhosOnlineRequest request)
    {
        player.SendWhosOnline();
    }

    public static void HandleShop(GamePlayer player, ShopRequest request)
    {
        var mapInfo = MapManager.Get(player.Character.MapId);
        if (mapInfo is null)
        {
            return;
        }

        var shopInfo = ShopManager.Get(mapInfo.ShopId);
        if (shopInfo is null)
        {
            player.Tell("There is no shop here.", Color.BrightRed);
            return;
        }

        foreach (var tradeInfo in shopInfo.Trades)
        {
            var itemInfo = ItemManager.Get(tradeInfo.GetItemId);
            if (itemInfo is null || itemInfo.Type != ItemType.Spell)
            {
                continue;
            }

            var spellInfo = SpellManager.Get(itemInfo.Data1);
            if (spellInfo is null)
            {
                continue;
            }

            player.Tell(spellInfo.RequiredClassId == 0
                    ? $"{itemInfo.Name} can be used by all classes."
                    : $"{itemInfo.Name} can only be used by a {ClassManager.GetName(spellInfo.RequiredClassId - 1)};",
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

        var mapInfo = MapManager.Get(player.Character.MapId);
        if (mapInfo is null)
        {
            return;
        }

        var shopInfo = ShopManager.Get(mapInfo.ShopId);
        if (shopInfo is null)
        {
            return;
        }

        var tradeInfo = shopInfo.Trades[request.Slot];

        var getItemInfo = ItemManager.Get(tradeInfo.GetItemId);
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

        var itemInfo = ItemManager.Get(slotInfo.ItemId);
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
            var itemInfo = ItemManager.Get(item.ItemId);
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

    public static void HandleParty(GamePlayer player, PartyRequest request)
    {
        if (player.InParty)
        {
            player.Tell("You are already in a party!", Color.Pink);
            return;
        }

        var targetPlayer = GameState.FindPlayer(request.TargetName);
        if (targetPlayer is null)
        {
            player.Tell("Player is not online.", Color.White);
            return;
        }

        if (player.Character.AccessLevel > AccessLevel.Moderator)
        {
            player.Tell("You can't join a party, you are an admin!", Color.BrightBlue);
            return;
        }

        if (targetPlayer.Character.AccessLevel > AccessLevel.Moderator)
        {
            player.Tell("Admins cannot join parties!", Color.BrightBlue);
            return;
        }

        var levelDifference = Math.Abs(player.Character.Level - targetPlayer.Character.Level);
        if (levelDifference > 5)
        {
            player.Tell("There is more then a 5 level gap between you two, party failed.", Color.Pink);
            return;
        }

        if (targetPlayer.InParty)
        {
            player.Tell("Player is already in a party!", Color.Pink);
            return;
        }

        player.Tell($"Party request has been sent to {targetPlayer.Character.Name}.", Color.Pink);
        targetPlayer.Tell($"{player.Character.Name} wants you to join their party.  Type /join to join, or /leave to decline.", Color.Pink);

        player.IsPartyStarter = true;
        player.PartyMember = targetPlayer;

        targetPlayer.PartyMember = player;
    }

    public static void HandleJoinParty(GamePlayer player, JoinPartyRequest request)
    {
        if (player.PartyMember is null || player.IsPartyStarter)
        {
            player.Tell("You have not been invited into a party!", Color.Pink);
            return;
        }

        if (player.PartyMember.PartyMember != player)
        {
            player.Tell("Party failed.", Color.Pink);
            return;
        }

        player.InParty = true;
        player.Tell($"You have joined {player.PartyMember.Character.Name}'s party!", Color.Pink);

        player.PartyMember.InParty = true;
        player.PartyMember.Tell($"{player.Character.Name} has joined your party!", Color.Pink);
    }

    public static void HandleLeaveParty(GamePlayer player, LeavePartyRequest request)
    {
        if (player.PartyMember is null)
        {
            player.Tell("You are not in a party!", Color.Pink);
            return;
        }

        if (player.InParty)
        {
            player.Tell("You have left the party.", Color.Pink);
            player.PartyMember.Tell($"{player.Character.Name} has left the party.", Color.Pink);
        }
        else
        {
            player.Tell("Declined party request.", Color.Pink);
            player.PartyMember.Tell($"{player.Character.Name} declined your request.", Color.Pink);
        }

        player.PartyMember.PartyMember = null;
        player.PartyMember.IsPartyStarter = false;
        player.PartyMember.InParty = false;

        player.PartyMember = null;
        player.IsPartyStarter = false;
        player.InParty = false;
    }

    public static void HandleSpells(GamePlayer player, SpellsRequest request)
    {
        player.Send(new PlayerSpells(player.Character.SpellIds));
    }

    public static void HandleCast(GamePlayer player, CastRequest request)
    {
        player.Cast(request.SpellSlot);
    }

    public static void HandleLocation(GamePlayer player, LocationRequest request)
    {
        player.Tell($"Map: {player.Character.MapId}, X: {player.Character.X}, Y: {player.Character.Y}", Color.Pink);
    }
}