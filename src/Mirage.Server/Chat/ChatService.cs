using Mirage.Net.Protocol.FromServer;
using Mirage.Net.Protocol.FromServer.New;
using Mirage.Server.Maps;
using Mirage.Server.Players;
using Mirage.Server.Repositories;
using Mirage.Shared.Constants;
using Mirage.Shared.Data;
using Serilog;

namespace Mirage.Server.Chat;

public sealed class ChatService(IPlayerService playerService, IMapService mapService) : IChatService
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
        public const string WarpMeTo = "/warpmeto";
        public const string WarpToMe = "/warptome";
        public const string WarpTo = "/warpto";
        public const string SetSprite = "/setsprite";
        public const string Motd = "/motd";
        public const string BanList = "/banlist";
        public const string Ban = "/ban";
        public const string SetAccess = "/setaccess";
        public const string DestroyBanList = "/destroybanlist";
    }

    public void Handle(Player player, ReadOnlySpan<char> message)
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

        if (message.StartsWith(Commands.Info, StringComparison.OrdinalIgnoreCase) && message.Length > Commands.Info.Length)
        {
            HandleInfo(player, message[Commands.Info.Length..].Trim());
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

        if (message.StartsWith(Commands.Party, StringComparison.OrdinalIgnoreCase) && message.Length > Commands.Party.Length)
        {
            HandlePartyInvite(player, message[Commands.Party.Length..].Trim());
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

        player.Map.SendMessage($"{player.Character.Name} says '{message}'", ColorCode.SayColor);
    }

    private void HandleBroadcast(Player player, ReadOnlySpan<char> message)
    {
        if (!message.IsEmpty)
        {
            playerService.SendToAll(new ChatCommand($"{player.Character.Name}: {message}", ColorCode.BroadcastColor));
        }
    }

    private static void HandleEmote(Player player, ReadOnlySpan<char> emote)
    {
        if (!emote.IsEmpty)
        {
            player.Map.SendMessage($"{player.Character.Name} {emote}", ColorCode.EmoteColor);
        }
    }

    private void HandleWhisper(Player player, ReadOnlySpan<char> message)
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
            player.Tell("Usage: !playername msghere", ColorCode.AlertColor);
            return;
        }

        var targetPlayerId = playerService.Find(targetName);
        if (targetPlayerId is null)
        {
            player.Tell("Player is not online.", ColorCode.White);
            return;
        }

        if (targetPlayerId == player)
        {
            player.Map.SendMessage($"{player.Character.Name} begins to mumble to himself, what a wierdo...", ColorCode.Green);

            return;
        }

        Log.Information("{FromCharacterName} tells {ToCharacterName}, '{Message}'", player.Character.Name, targetPlayerId.Character.Name, new string(chatMesage));

        targetPlayerId.Tell($"{player.Character.Name} tells you, '{chatMesage}'", ColorCode.TellColor);

        player.Tell($"You tell {targetPlayerId.Character.Name}, '{chatMesage}'", ColorCode.TellColor);
    }

    private static void HandleHelp(Player player)
    {
        player.Tell("Social commands:", ColorCode.HelpColor);
        player.Tell("'msghere = Broadcast Message", ColorCode.HelpColor);
        player.Tell("-msghere = Emote Message", ColorCode.HelpColor);
        player.Tell("!namehere msghere = Player Message", ColorCode.HelpColor);
        player.Tell("Available Commands: /help, /info, /who, /fps, /inv, /stats, /train, /trade, /party, /join, /leave", ColorCode.HelpColor);
    }

    private void HandleInfo(Player player, ReadOnlySpan<char> targetName)
    {
        if (targetName.IsEmpty)
        {
            return;
        }

        var targetPlayer = playerService.Find(targetName);
        if (targetPlayer is null)
        {
            player.Tell("Player is not online.", ColorCode.White);
            return;
        }

        player.Tell($"Name: {targetPlayer.Character.Name}", ColorCode.BrightGreen);
        if (player.Character.AccessLevel <= AccessLevel.Moderator)
        {
            return;
        }

        player.Tell($"-=- Stats for {targetPlayer.Character.Name} -=-", ColorCode.BrightGreen);
        player.Tell($"Level: {targetPlayer.Character.Level}  Exp: {targetPlayer.Character.Exp}/{targetPlayer.Character.RequiredExp}", ColorCode.BrightGreen);
        player.Tell($"HP: {targetPlayer.Character.HP}/{targetPlayer.Character.MaxHP}  MP: {targetPlayer.Character.MP}/{targetPlayer.Character.MaxMP}  SP: {targetPlayer.Character.SP}/{targetPlayer.Character.MaxSP}", ColorCode.BrightGreen);
        player.Tell($"STR: {targetPlayer.Character.Strength}  DEF: {targetPlayer.Character.Defense}  MAGI: {targetPlayer.Character.Intelligence}  SPEED: {targetPlayer.Character.Speed}", ColorCode.BrightGreen);

        player.Tell($"Critical Hit Chance: {targetPlayer.Character.CriticalHitRate}%, Block Chance: {targetPlayer.Character.BlockRate}%", ColorCode.BrightGreen);
    }

    private static void HandleWho(Player player)
    {
        player.SendWhosOnline();
    }

    private static void HandleStats(Player player)
    {
        player.Tell($"-=- Stats for {player.Character.Name} -=-", ColorCode.White);
        player.Tell($"Level: {player.Character.Level}  Exp: {player.Character.Exp}/{player.Character.RequiredExp}", ColorCode.White);
        player.Tell($"HP: {player.Character.HP}/{player.Character.MaxHP}  MP: {player.Character.MP}/{player.Character.MaxMP}  SP: {player.Character.SP}/{player.Character.MaxSP}", ColorCode.White);
        player.Tell($"STR: {player.Character.Strength}  DEF: {player.Character.Defense}  MAGI: {player.Character.Intelligence}  SPEED: {player.Character.Speed}", ColorCode.White);
        player.Tell($"Critical Hit Chance: {player.Character.CriticalHitRate}%, Block Chance: {player.Character.BlockRate}%", ColorCode.White);
    }

    private static void HandleTrade(Player player)
    {
        // TODO:
        // var mapInfo = MapRepository.Get(player.Character.MapId);
        // if (mapInfo is null)
        // {
        //     return;
        // }
        //
        // var shopInfo = ShopRepository.Get(mapInfo.ShopId);
        // if (shopInfo is null)
        // {
        //     player.Tell("There is no shop here.", ColorCode.BrightRed);
        //     return;
        // }
        //
        // foreach (var tradeInfo in shopInfo.Trades)
        // {
        //     var itemInfo = ItemRepository.Get(tradeInfo.GetItemId);
        //     if (itemInfo is null || itemInfo.Type != ItemType.Spell)
        //     {
        //         continue;
        //     }
        //
        //     var spellInfo = SpellRepository.Get(itemInfo.Data1);
        //     if (spellInfo is null)
        //     {
        //         continue;
        //     }
        //
        //     player.Tell(!string.IsNullOrEmpty(spellInfo.RequiredClassId)
        //             ? $"{itemInfo.Name} can be used by all classes."
        //             : $"{itemInfo.Name} can only be used by a {JobRepository.GetName(spellInfo.RequiredClassId)};",
        //         ColorCode.Yellow);
        // }
        //
        // player.Send(new Trade(shopInfo.Id, shopInfo.FixesItems, shopInfo.Trades));
    }

    private void HandlePartyInvite(Player player, ReadOnlySpan<char> targetName)
    {
        if (targetName.IsEmpty)
        {
            player.Tell("Usage: /party playernamehere", ColorCode.AlertColor);
            return;
        }

        if (player.InParty)
        {
            player.Tell("You are already in a party!", ColorCode.Pink);
            return;
        }

        var targetPlayer = playerService.Find(targetName);
        if (targetPlayer is null)
        {
            player.Tell("Player is not online.", ColorCode.White);
            return;
        }

        if (player.Character.AccessLevel > AccessLevel.Moderator)
        {
            player.Tell("You can't join a party, you are an admin!", ColorCode.BrightBlue);
            return;
        }

        if (targetPlayer.Character.AccessLevel > AccessLevel.Moderator)
        {
            player.Tell("Admins cannot join parties!", ColorCode.BrightBlue);
            return;
        }

        var levelDifference = Math.Abs(player.Character.Level - targetPlayer.Character.Level);
        if (levelDifference > 5)
        {
            player.Tell("There is more then a 5 level gap between you two, party failed.", ColorCode.Pink);
            return;
        }

        if (targetPlayer.InParty)
        {
            player.Tell("Player is already in a party!", ColorCode.Pink);
            return;
        }

        player.Tell($"Party request has been sent to {targetPlayer.Character.Name}.", ColorCode.Pink);
        targetPlayer.Tell($"{player.Character.Name} wants you to join their party.  Type /join to join, or /leave to decline.", ColorCode.Pink);

        player.IsPartyStarter = true;
        player.PartyMember = targetPlayer;

        targetPlayer.PartyMember = player;
    }

    private static void HandlePartyAccept(Player player)
    {
        if (player.PartyMember is null || player.IsPartyStarter)
        {
            player.Tell("You have not been invited into a party!", ColorCode.Pink);
            return;
        }

        if (player.PartyMember.PartyMember != player)
        {
            player.Tell("Party failed.", ColorCode.Pink);
            return;
        }

        player.InParty = true;
        player.Tell($"You have joined {player.PartyMember.Character.Name}'s party!", ColorCode.Pink);

        player.PartyMember.InParty = true;
        player.PartyMember.Tell($"{player.Character.Name} has joined your party!", ColorCode.Pink);
    }

    private static void HandlePartyDecline(Player player)
    {
        if (player.PartyMember is null)
        {
            player.Tell("You are not in a party!", ColorCode.Pink);
            return;
        }

        if (player.InParty)
        {
            player.Tell("You have left the party.", ColorCode.Pink);
            player.PartyMember.Tell($"{player.Character.Name} has left the party.", ColorCode.Pink);
        }
        else
        {
            player.Tell("Declined party request.", ColorCode.Pink);
            player.PartyMember.Tell($"{player.Character.Name} declined your request.", ColorCode.Pink);
        }

        player.PartyMember.PartyMember = null;
        player.PartyMember.IsPartyStarter = false;
        player.PartyMember.InParty = false;

        player.PartyMember = null;
        player.IsPartyStarter = false;
        player.InParty = false;
    }

    private bool HandleAdmin(Player player, ReadOnlySpan<char> chatText)
    {
        var access = player.Character.AccessLevel;
        if (access <= AccessLevel.None)
        {
            return false;
        }

        if (chatText.StartsWith(Commands.Admin, StringComparison.OrdinalIgnoreCase))
        {
            player.Tell("Social Commands:", ColorCode.HelpColor);
            player.Tell("\"msghere = Global Admin Message", ColorCode.HelpColor);
            player.Tell("=msghere = Private Admin Message", ColorCode.HelpColor);
            player.Tell("Available Commands: /admin, /loc, /warpmeto, /warptome, /warpto, /setsprite, /kick, /ban, /motd, /ban", ColorCode.HelpColor);
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
                player.Tell($"Map: {player.Character.MapId}, X: {player.Character.X}, Y: {player.Character.Y}", ColorCode.Pink);
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

    private void HandleKick(Player player, ReadOnlySpan<char> targetName)
    {
        if (targetName.IsEmpty)
        {
            return;
        }

        var targetPlayer = playerService.Find(targetName);
        if (targetPlayer is null)
        {
            player.Tell("Player is not online.", ColorCode.White);
            return;
        }

        if (targetPlayer == player)
        {
            player.Tell("You cannot kick yourself!", ColorCode.White);
            return;
        }

        if (targetPlayer.Character.AccessLevel > player.Character.AccessLevel)
        {
            player.Tell("That is a higher access admin then you!", ColorCode.White);
            return;
        }

        playerService.SendToAll(new ChatCommand($"{targetPlayer.Character.Name} has been kicked by {player.Character.Name}!", ColorCode.White));

        Log.Information("{CharacterName} has kicked {TargetCharacterName}.", player.Character.Name, targetPlayer.Character.Name);

        targetPlayer.Disconnect($"You have been kicked by {player.Character.Name}!");
    }

    private void HandleGlobalMessage(Player player, ReadOnlySpan<char> message)
    {
        if (!message.IsEmpty)
        {
            playerService.SendToAll(new ChatCommand($"(global) {player.Character.Name}: {message}", ColorCode.GlobalColor));
        }
    }

    private void HandleAdminMessage(Player player, ReadOnlySpan<char> message)
    {
        if (!message.IsEmpty)
        {
            playerService.SendToAll(new ChatCommand($"(admin {player.Character.Name}) {message}", ColorCode.AdminColor));
        }
    }

    private void HandleWarpMeTo(Player player, ReadOnlySpan<char> targetName)
    {
        if (targetName.IsEmpty)
        {
            return;
        }

        var targetPlayer = playerService.Find(targetName);
        if (targetPlayer is null)
        {
            player.Tell("Player is not online.", ColorCode.White);
            return;
        }

        if (targetPlayer == player)
        {
            player.Tell("You cannot warp to yourself!", ColorCode.White);
            return;
        }

        player.WarpTo(targetPlayer.Map, targetPlayer.Character.X, targetPlayer.Character.Y);

        Log.Information("{CharacterName} has warped to {TargetCharacterName}, map #{MapId}.",
            player.Character.Name,
            targetPlayer.Character.Name,
            targetPlayer.Character.MapId);

        targetPlayer.Tell($"{player.Character.Name} has warped to you.", ColorCode.BrightBlue);

        player.Tell($"You have been warped to {targetPlayer.Character.Name}.", ColorCode.BrightBlue);
    }

    private void HandleWarpToMe(Player player, ReadOnlySpan<char> targetName)
    {
        if (targetName.IsEmpty)
        {
            return;
        }

        var targetPlayer = playerService.Find(targetName);
        if (targetPlayer is null)
        {
            player.Tell("Player is not online.", ColorCode.White);
            return;
        }

        if (targetPlayer == player)
        {
            player.Tell("You cannot warp yourself to yourself!", ColorCode.White);
            return;
        }

        targetPlayer.WarpTo(player.Map, player.Character.X, player.Character.Y);

        Log.Information("{CharacterName} has warped {TargetCharacterName} to self, map #{MapId}.",
            player.Character.Name,
            targetPlayer.Character.Name,
            player.Character.MapId);

        targetPlayer.Tell($"You have been summoned by {player.Character.Name}.", ColorCode.BrightBlue);

        player.Tell($"{targetPlayer.Character.Name} has been summoned.", ColorCode.BrightBlue);
    }

    private void HandleWarpTo(Player player, ReadOnlySpan<char> targetMapName)
    {
        if (targetMapName.IsEmpty)
        {
            return;
        }

        var map = mapService.GetByName(new string(targetMapName));
        if (map is null)
        {
            player.Tell("The specified map does not exist.", ColorCode.Red);
            return;
        }

        player.WarpTo(map, player.Character.X, player.Character.Y);
        player.Tell($"You have been warped to {map.Name}", ColorCode.BrightBlue);

        Log.Information("{CharacterName} warped to {MapName}", player.Character.Name, map.FileName);
    }

    private static void HandleSetSprite(Player player, ReadOnlySpan<char> spriteStr)
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

    private void HandleSetMotd(Player player, ReadOnlySpan<char> motd)
    {
        if (motd.IsEmpty)
        {
            return;
        }

        File.WriteAllText("Motd.txt", motd);

        Log.Information("{CharacterName} changed MOTD to: {NewMotd}", player.Character.Name, new string(motd));

        playerService.SendToAll(new ChatCommand($"MOTD changed to: {motd}", ColorCode.BrightCyan));
    }

    private static void HandleBanList(Player player)
    {
        var bans = BanRepository.GetAll();
        if (bans.Count == 0)
        {
            return;
        }

        var lineNumber = 1;

        foreach (var x in bans)
        {
            player.Tell($"{lineNumber}: Banned IP {x.Ip} by {x.BannedBy}", ColorCode.White);

            lineNumber++;
        }
    }

    private void HandleBan(Player player, ReadOnlySpan<char> targetName)
    {
        if (targetName.IsEmpty)
        {
            return;
        }

        var targetPlayer = playerService.Find(targetName);
        if (targetPlayer is null)
        {
            player.Tell("Player is not online.", ColorCode.White);
            return;
        }

        if (targetPlayer == player)
        {
            player.Tell("You cannot ban yourself!", ColorCode.White);
            return;
        }

        if (targetPlayer.Character.AccessLevel > player.Character.AccessLevel)
        {
            player.Tell("That is a higher access admin then you!", ColorCode.White);
            return;
        }

        BanRepository.AddBan(targetPlayer.Address, player.Character.Name);

        playerService.SendToAll(new ChatCommand($"{targetPlayer.Character.Name} has been banned by {player.Character.Name}!", ColorCode.White));

        Log.Information("{CharacterName} has banned {BannedCharacterName}",
            targetPlayer.Character.Name, player.Character);

        targetPlayer.Disconnect($"You have been banned by {player.Character.Name}!");
    }

    private void HandleSetAccessLevel(Player player, ReadOnlySpan<char> args)
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

        var targetPlayer = playerService.Find(targetName);
        if (targetPlayer is null)
        {
            player.Tell("Player is not online.", ColorCode.White);
            return;
        }

        if (targetPlayer.Character.AccessLevel <= AccessLevel.None)
        {
            playerService.SendToAll(new ChatCommand($"{targetPlayer.Character.Name} has been blessed with administrative access.", ColorCode.BrightBlue));
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

    private static void HandleDestroyBanList(Player player)
    {
        BanRepository.Clear();

        player.Tell("Ban list destroyed.", ColorCode.White);
    }
}