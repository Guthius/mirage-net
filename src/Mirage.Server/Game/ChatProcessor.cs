using Mirage.Game.Constants;
using Mirage.Game.Data;
using Mirage.Net.Protocol.FromServer;
using Mirage.Server.Net;
using Mirage.Server.Repositories;
using Serilog;

namespace Mirage.Server.Game;

public static class ChatProcessor
{
    private static class Commands
    {
        public const string Help = "/help";
        public const string Info = "/info";
        public const string Who = "/who";
        public const string Stats = "/stats";
        public const string Trade = "/trade";
        public const string Party = "/party";
        public const string Join = "/join";
        public const string Leave = "/leave";
        public const string Admin = "/admin";
        public const string Kick = "/kick";
        public const string Location = "/loc";
        public const string MapEditor = "/mapeditor";
        public const string WarpMeTo = "/warpmeto";
        public const string WarpToMe = "/warptome";
        public const string WarpTo = "/warpto";
        public const string SetSprite = "/setsprite";
        public const string Respawn = "/respawn";
        public const string Motd = "/motd";
        public const string BanList = "/banlist";
        public const string Ban = "/ban";
        public const string EditItem = "/edititem";
        public const string EditNpc = "/editnpc";
        public const string EditShop = "/editshop";
        public const string EditSpell = "/editspell";
        public const string SetAccess = "/setaccess";
        public const string DestroyBanList = "/destroybanlist";
    }


    public static void Handle(GamePlayer player, ReadOnlySpan<char> message)
    {
        message = message.Trim();
        if (message.IsEmpty)
        {
            return;
        }

        Log.Information("[Map-{MapId}] {CharacterName}: '{Message}'",
            player.Character.MapId,
            player.Character.Name,
            new string(message));

        switch (message[0])
        {
            case '\'':
                HandleBroadcast(player, message[1..]);
                return;

            case '-':
                HandleEmote(player, message[1..]);
                return;

            case '!':
                HandleWhisper(player, message[1..]);
                return;
        }

        if (message.StartsWith(Commands.Help, StringComparison.OrdinalIgnoreCase))
        {
            HandleHelp(player);
            return;
        }

        if (message.StartsWith(Commands.Info, StringComparison.OrdinalIgnoreCase))
        {
            HandleInfo(player, message[6..].Trim());
            return;
        }

        if (message.StartsWith(Commands.Who, StringComparison.OrdinalIgnoreCase))
        {
            HandleWho(player);
            return;
        }

        if (message.StartsWith(Commands.Stats, StringComparison.OrdinalIgnoreCase))
        {
            HandleStats(player);
            return;
        }

        if (message.StartsWith(Commands.Trade, StringComparison.OrdinalIgnoreCase))
        {
            HandleTrade(player);
            return;
        }

        if (message.StartsWith(Commands.Party, StringComparison.OrdinalIgnoreCase))
        {
            HandlePartyInvite(player, message[7..].Trim());
            return;
        }

        if (message.StartsWith(Commands.Join, StringComparison.OrdinalIgnoreCase))
        {
            HandlePartyAccept(player);
            return;
        }

        if (message.StartsWith(Commands.Leave, StringComparison.OrdinalIgnoreCase))
        {
            HandlePartyDecline(player);
            return;
        }

        if (HandleAdmin(player, message))
        {
            return;
        }

        player.Map.SendMessage($"{player.Character.Name} says '{message}'", Color.SayColor);
    }

    private static void HandleBroadcast(GamePlayer player, ReadOnlySpan<char> message)
    {
        if (!message.IsEmpty)
        {
            Network.SendToAll(new PlayerMessage($"{player.Character.Name}: {message}", Color.BroadcastColor));
        }
    }

    private static void HandleEmote(GamePlayer player, ReadOnlySpan<char> emote)
    {
        if (!emote.IsEmpty)
        {
            player.Map.SendMessage($"{player.Character.Name} {emote}", Color.EmoteColor);
        }
    }

    private static void HandleWhisper(GamePlayer player, ReadOnlySpan<char> message)
    {
        var space = message.IndexOf(' ');
        if (space == -1)
        {
            return;
        }

        var targetName = message[..space].Trim();
        var chatMesage = message[(space + 1)..].Trim();

        if (targetName.IsEmpty || chatMesage.IsEmpty)
        {
            player.Tell("Usage: !playername msghere", Color.AlertColor);
            return;
        }

        var targetPlayerId = GameState.FindPlayer(targetName);
        if (targetPlayerId is null)
        {
            player.Tell("Player is not online.", Color.White);
            return;
        }

        if (targetPlayerId == player)
        {
            player.NewMap.SendMessage($"{player.Character.Name} begins to mumble to himself, what a wierdo...", Color.Green);

            return;
        }

        Log.Information("{FromCharacterName} tells {ToCharacterName}, '{Message}'", player.Character.Name, targetPlayerId.Character.Name, new string(chatMesage));

        targetPlayerId.Tell($"{player.Character.Name} tells you, '{chatMesage}'", Color.TellColor);

        player.Tell($"You tell {targetPlayerId.Character.Name}, '{chatMesage}'", Color.TellColor);
    }

    private static void HandleHelp(GamePlayer player)
    {
        player.Tell("Social commands:", Color.HelpColor);
        player.Tell("'msghere = Broadcast Message", Color.HelpColor);
        player.Tell("-msghere = Emote Message", Color.HelpColor);
        player.Tell("!namehere msghere = Player Message", Color.HelpColor);
        player.Tell("Available Commands: /help, /info, /who, /fps, /inv, /stats, /train, /trade, /party, /join, /leave", Color.HelpColor);
    }

    private static void HandleInfo(GamePlayer player, ReadOnlySpan<char> targetName)
    {
        if (targetName.IsEmpty)
        {
            return;
        }

        var targetPlayer = GameState.FindPlayer(targetName);
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

    private static void HandleWho(GamePlayer player)
    {
        player.SendWhosOnline();
    }

    private static void HandleStats(GamePlayer player)
    {
        player.Tell($"-=- Stats for {player.Character.Name} -=-", Color.White);
        player.Tell($"Level: {player.Character.Level}  Exp: {player.Character.Exp}/{player.Character.RequiredExp}", Color.White);
        player.Tell($"HP: {player.Character.HP}/{player.Character.MaxHP}  MP: {player.Character.MP}/{player.Character.MaxMP}  SP: {player.Character.SP}/{player.Character.MaxSP}", Color.White);
        player.Tell($"STR: {player.Character.Strength}  DEF: {player.Character.Defense}  MAGI: {player.Character.Intelligence}  SPEED: {player.Character.Speed}", Color.White);
        player.Tell($"Critical Hit Chance: {player.Character.CriticalHitRate}%, Block Chance: {player.Character.BlockRate}%", Color.White);
    }

    private static void HandleTrade(GamePlayer player)
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

    private static void HandlePartyInvite(GamePlayer player, ReadOnlySpan<char> targetName)
    {
        if (targetName.IsEmpty)
        {
            player.Tell("Usage: /party playernamehere", Color.AlertColor);
            return;
        }

        if (player.InParty)
        {
            player.Tell("You are already in a party!", Color.Pink);
            return;
        }

        var targetPlayer = GameState.FindPlayer(targetName);
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

    private static void HandlePartyAccept(GamePlayer player)
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

    private static void HandlePartyDecline(GamePlayer player)
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

    private static bool HandleAdmin(GamePlayer player, ReadOnlySpan<char> chatText)
    {
        var access = player.Character.AccessLevel;
        if (access <= AccessLevel.Player)
        {
            return false;
        }

        if (chatText.StartsWith(Commands.Admin, StringComparison.OrdinalIgnoreCase))
        {
            player.Tell("Social Commands:", Color.HelpColor);
            player.Tell("\"msghere = Global Admin Message", Color.HelpColor);
            player.Tell("=msghere = Private Admin Message", Color.HelpColor);
            player.Tell("Available Commands: /admin, /loc, /mapeditor, /warpmeto, /warptome, /warpto, /setsprite, /kick, /ban, /edititem, /respawn, /editnpc, /motd, /editshop, /ban, /editspell", Color.HelpColor);
            return true;
        }

        if (chatText.StartsWith(Commands.Kick, StringComparison.OrdinalIgnoreCase))
        {
            HandleKick(player, chatText[6..].Trim());
            return true;
        }

        switch (chatText[0])
        {
            case '"':
                HandleGlobalMessage(player, chatText[1..].Trim());
                return true;

            case '=':
                HandleAdminMessage(player, chatText[1..].Trim());
                return true;
        }

        if (access >= AccessLevel.Mapper)
        {
            if (chatText.StartsWith(Commands.Location, StringComparison.OrdinalIgnoreCase))
            {
                player.Tell($"Map: {player.Character.MapId}, X: {player.Character.X}, Y: {player.Character.Y}", Color.Pink);
                return true;
            }

            if (chatText.StartsWith(Commands.MapEditor, StringComparison.OrdinalIgnoreCase))
            {
                player.Send<OpenMapEditor>();
                return true;
            }

            if (chatText.StartsWith(Commands.WarpMeTo, StringComparison.OrdinalIgnoreCase))
            {
                HandleWarpMeTo(player, chatText[Commands.WarpMeTo.Length..].Trim());
                return true;
            }

            if (chatText.StartsWith(Commands.WarpToMe, StringComparison.OrdinalIgnoreCase))
            {
                HandleWarpToMe(player, chatText[Commands.WarpToMe.Length..].Trim());
                return true;
            }

            if (chatText.StartsWith(Commands.WarpTo, StringComparison.OrdinalIgnoreCase))
            {
                HandleWarpTo(player, chatText[Commands.WarpTo.Length..].Trim());
                return true;
            }

            if (chatText.StartsWith(Commands.SetSprite, StringComparison.OrdinalIgnoreCase))
            {
                HandleSetSprite(player, chatText[Commands.SetSprite.Length..].Trim());
                return true;
            }

            if (chatText.StartsWith(Commands.Respawn, StringComparison.OrdinalIgnoreCase))
            {
                HandleRespawn(player);
                return true;
            }

            if (chatText.StartsWith(Commands.Motd))
            {
                HandleSetMotd(player, chatText[Commands.Motd.Length..].Trim());
                return true;
            }

            if (chatText.StartsWith(Commands.BanList, StringComparison.OrdinalIgnoreCase))
            {
                HandleBanList(player);
                return true;
            }

            if (chatText.StartsWith(Commands.Ban))
            {
                HandleBan(player, chatText[Commands.Ban.Length..].Trim());
                return true;
            }
        }

        if (access >= AccessLevel.Developer)
        {
            if (chatText.StartsWith(Commands.EditItem))
            {
                player.Send<OpenItemEditor>();
                return true;
            }

            if (chatText.StartsWith(Commands.EditNpc))
            {
                player.Send<OpenNpcEditor>();
                return true;
            }

            if (chatText.StartsWith(Commands.EditShop))
            {
                player.Send<OpenShopEditor>();
                return true;
            }

            if (chatText.StartsWith(Commands.EditSpell))
            {
                player.Send<OpenSpellEditor>();
                return true;
            }
        }

        if (access < AccessLevel.Administrator)
        {
            return false;
        }

        if (chatText.StartsWith(Commands.SetAccess))
        {
            HandleSetAccessLevel(player, chatText[Commands.SetAccess.Length..]);
            return true;
        }

        if (chatText.StartsWith(Commands.DestroyBanList))
        {
            HandleDestroyBanList(player);
            return true;
        }

        return false;
    }

    private static void HandleKick(GamePlayer player, ReadOnlySpan<char> targetName)
    {
        if (targetName.IsEmpty)
        {
            return;
        }

        var targetPlayer = GameState.FindPlayer(targetName);
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

        Network.SendGlobalMessage($"{targetPlayer.Character.Name} has been kicked from {Options.GameName} by {player.Character.Name}!", Color.White);

        Log.Information("{CharacterName} has kicked {TargetCharacterName}.", player.Character.Name, targetPlayer.Character.Name);

        targetPlayer.SendAlert($"You have been kicked by {player.Character.Name}!");
    }

    private static void HandleGlobalMessage(GamePlayer player, ReadOnlySpan<char> message)
    {
        if (!message.IsEmpty)
        {
            Network.SendToAll(new PlayerMessage($"(global) {player.Character.Name}: {message}", Color.GlobalColor));
        }
    }

    private static void HandleAdminMessage(GamePlayer player, ReadOnlySpan<char> message)
    {
        if (!message.IsEmpty)
        {
            Network.SendToAll(new PlayerMessage($"(admin {player.Character.Name}) {message}", Color.AdminColor));
        }
    }

    private static void HandleWarpMeTo(GamePlayer player, ReadOnlySpan<char> targetName)
    {
        if (targetName.IsEmpty)
        {
            return;
        }

        var targetPlayer = GameState.FindPlayer(targetName);
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

    private static void HandleWarpToMe(GamePlayer player, ReadOnlySpan<char> targetName)
    {
        if (targetName.IsEmpty)
        {
            return;
        }

        var targetPlayer = GameState.FindPlayer(targetName);
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

    private static void HandleWarpTo(GamePlayer player, ReadOnlySpan<char> targetMapId)
    {
        if (targetMapId.IsEmpty)
        {
            return;
        }

        if (!int.TryParse(targetMapId, out var mapId))
        {
            return;
        }

        if (mapId is <= 0 or > Limits.MaxMaps)
        {
            player.Tell("Invalid map number.", Color.Red);
            return;
        }

        var mapInfo = MapRepository.Get(mapId);
        if (mapInfo is null)
        {
            // TODO: ReportHackAttempt(player.Id, "Invalid map");
            return;
        }

        player.WarpTo(mapId, player.Character.X, player.Character.Y);
        player.Tell($"You have been warped to map #{mapId}", Color.BrightBlue);

        Log.Information("{CharacterName} warped to map #{MapId}", player.Character.Name, mapId);
    }

    private static void HandleSetSprite(GamePlayer player, ReadOnlySpan<char> spriteStr)
    {
        if (spriteStr.IsEmpty)
        {
            return;
        }

        if (!int.TryParse(spriteStr, out var sprite))
        {
            return;
        }

        player.Character.Sprite = sprite;
        player.SendPlayerData();
    }

    private static void HandleRespawn(GamePlayer player)
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

    private static void HandleSetMotd(GamePlayer player, ReadOnlySpan<char> motd)
    {
        if (motd.IsEmpty)
        {
            return;
        }

        File.WriteAllText("Motd.txt", motd);

        Log.Information("{CharacterName} changed MOTD to: {NewMotd}", player.Character.Name, new string(motd));

        Network.SendToAll(new PlayerMessage($"MOTD changed to: {motd}", Color.BrightCyan));
    }

    private static void HandleBanList(GamePlayer player)
    {
        var bans = BanRepository.GetAll();
        if (bans.Count == 0)
        {
            return;
        }

        var lineNumber = 1;

        foreach (var x in bans)
        {
            player.Tell($"{lineNumber}: Banned IP {x.Ip} by {x.BannedBy}", Color.White);

            lineNumber++;
        }
    }

    private static void HandleBan(GamePlayer player, ReadOnlySpan<char> targetName)
    {
        if (targetName.IsEmpty)
        {
            return;
        }

        var targetPlayer = GameState.FindPlayer(targetName);
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

        BanRepository.AddBan(Network.GetIP(targetPlayer.Id), player.Character.Name);

        Network.SendGlobalMessage($"{targetPlayer.Character.Name} has been banned from {Options.GameName} by {player.Character.Name}!", Color.White);

        Log.Information("{CharacterName} has banned {BannedCharacterName}",
            targetPlayer.Character.Name, player.Character);

        targetPlayer.SendAlert($"You have been banned by {player.Character.Name}!");
    }

    private static void HandleSetAccessLevel(GamePlayer player, ReadOnlySpan<char> args)
    {
        var space = args.IndexOf(' ');
        if (space == -1)
        {
            return;
        }

        if (int.TryParse(args[..space], out var accessLevel) || !Enum.IsDefined(typeof(AccessLevel), accessLevel))
        {
            return;
        }

        var targetName = args[(space + 1)..];
        if (targetName.IsEmpty)
        {
            return;
        }

        var targetPlayer = GameState.FindPlayer(targetName);
        if (targetPlayer is null)
        {
            player.Tell("Player is not online.", Color.White);
            return;
        }

        if (targetPlayer.Character.AccessLevel <= AccessLevel.Player)
        {
            Network.SendGlobalMessage($"{targetPlayer.Character.Name} has been blessed with administrative access.", Color.BrightBlue);
        }

        targetPlayer.Character.AccessLevel = (AccessLevel) accessLevel;

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

    private static void HandleDestroyBanList(GamePlayer player)
    {
        BanRepository.Clear();

        player.Tell("Ban list destroyed.", Color.White);
    }
}