using Mirage.Client.Modules;
using Mirage.Client.Net;
using Mirage.Game.Data;
using Mirage.Net.Protocol.FromClient;

namespace Mirage.Client.Game;

public static class GameChat
{
    public static void Handle(string chatText)
    {
        chatText = chatText.Trim();
        if (chatText.Length == 0)
        {
            return;
        }

        switch (chatText[0])
        {
            case '\'':
                HandleBroadcast(chatText[1..]);
                return;

            case '-':
                HandleEmote(chatText[1..]);
                return;

            case '!':
                HandlePlayerMessage(chatText[1..]);
                return;
        }

        if (chatText.StartsWith("/help", StringComparison.OrdinalIgnoreCase))
        {
            HandleHelp();
            return;
        }

        if (chatText.StartsWith("/info", StringComparison.OrdinalIgnoreCase))
        {
            HandleInfo(chatText[6..].Trim());
            return;
        }

        if (chatText.StartsWith("/who", StringComparison.OrdinalIgnoreCase))
        {
            HandleWho();
            return;
        }

        if (chatText.StartsWith("/fps", StringComparison.OrdinalIgnoreCase))
        {
            HandleFps();
            return;
        }

        if (chatText.StartsWith("/stats", StringComparison.OrdinalIgnoreCase))
        {
            HandleStats();
            return;
        }

        if (chatText.StartsWith("/train", StringComparison.OrdinalIgnoreCase))
        {
            HandleTrain();
            return;
        }

        if (chatText.StartsWith("/trade", StringComparison.OrdinalIgnoreCase))
        {
            HandleTrade();
            return;
        }

        if (chatText.StartsWith("/party", StringComparison.OrdinalIgnoreCase))
        {
            HandlePartyInvite(chatText[7..].Trim());
            return;
        }

        if (chatText.StartsWith("/join", StringComparison.OrdinalIgnoreCase))
        {
            HandlePartyAccept();
            return;
        }

        if (chatText.StartsWith("/leave", StringComparison.OrdinalIgnoreCase))
        {
            HandlePartyDecline();
            return;
        }

        if (HandleAdmin(chatText))
        {
            return;
        }

        if (chatText.Trim().Length > 0)
        {
            Network.Send(new SayRequest(chatText));
        }
    }

    private static void HandleBroadcast(string message)
    {
        if (message.Length > 0)
        {
            Network.Send(new BroadcastRequest(message));
        }
    }

    private static void HandleEmote(string emote)
    {
        if (emote.Length > 0)
        {
            Network.Send(new EmoteRequest(emote));
        }
    }

    private static void HandlePlayerMessage(string chatText)
    {
        var space = chatText.IndexOf(' ');
        if (space == -1)
        {
            return;
        }

        var name = chatText[..space];
        var message = chatText[(space + 1)..].Trim();

        if (name.Length > 0 && message.Length > 0)
        {
            Network.Send(new PlayerMessageRequest(name, message));
            return;
        }

        modText.AddText("Usage: !playername msghere", modText.AlertColor);
    }

    private static void HandleHelp()
    {
        modText.AddText("Social commands:", modText.HelpColor);
        modText.AddText("'msghere = Broadcast Message", modText.HelpColor);
        modText.AddText("-msghere = Emote Message", modText.HelpColor);
        modText.AddText("!namehere msghere = Player Message", modText.HelpColor);
        modText.AddText("Available Commands: /help, /info, /who, /fps, /inv, /stats, /train, /trade, /party, /join, /leave", modText.HelpColor);
    }

    private static void HandleInfo(string playerName)
    {
        if (playerName.Length > 0)
        {
            Network.Send(new PlayerInfoRequest(playerName));
        }
    }

    private static void HandleWho()
    {
        Network.Send<WhosOnlineRequest>();
    }

    private static void HandleFps()
    {
        modText.AddText($"FPS: {modGameLogic.GameFPS}", modText.Pink);
    }

    private static void HandleStats()
    {
        Network.Send<GetStatsRequest>();
    }

    private static void HandleTrain()
    {
        // using var frmTraining = new frmTraining();
        // frmTraining.ShowDialog();
    }

    private static void HandleTrade()
    {
        Network.Send<ShopRequest>();
    }

    private static void HandlePartyInvite(string targetName)
    {
        if (targetName.Length == 0)
        {
            modText.AddText("Usage: /party playernamehere", modText.AlertColor);
            return;
        }

        Network.Send(new PartyRequest(targetName));
    }

    private static void HandlePartyAccept()
    {
        Network.Send<JoinPartyRequest>();
    }

    private static void HandlePartyDecline()
    {
        Network.Send<LeavePartyRequest>();
    }

    private static bool HandleAdmin(string chatText)
    {
        var access = modTypes.GetPlayerAccess(modGameLogic.MyIndex);
        if (access <= 0)
        {
            return false;
        }

        if (chatText.StartsWith("/admin", StringComparison.OrdinalIgnoreCase))
        {
            modText.AddText("Social Commands:", modText.HelpColor);
            modText.AddText("\"msghere = Global Admin Message", modText.HelpColor);
            modText.AddText("=msghere = Private Admin Message", modText.HelpColor);
            modText.AddText("Available Commands: /admin, /loc, /mapeditor, /warpmeto, /warptome, /warpto, /setsprite, /mapreport, /kick, /ban, /edititem, /respawn, /editnpc, /motd, /editshop, /ban, /editspell", modText.HelpColor);
            return true;
        }

        if (chatText.StartsWith("/kick", StringComparison.OrdinalIgnoreCase))
        {
            if (chatText.Length > 6)
            {
                var targetName = chatText[6..].Trim();
                Network.Send(new KickPlayerRequest(targetName));
            }

            return true;
        }

        switch (chatText[0])
        {
            case '"':
            {
                chatText = chatText[1..].Trim();
                if (chatText.Length > 0)
                {
                    Network.Send(new GlobalMessageRequest(chatText));
                }

                return true;
            }

            // Admin Message
            case '=':
            {
                chatText = chatText[1..].Trim();
                if (chatText.Length > 0)
                {
                    Network.Send(new AdminMessageRequest(chatText));
                }

                return true;
            }
        }

        // Mapper Admin Commands
        if (access >= modTypes.ADMIN_MAPPER)
        {
            // Location
            if (chatText.StartsWith("/loc", StringComparison.OrdinalIgnoreCase))
            {
                Network.Send<LocationRequest>();
                return true;
            }

            // Map Editor
            if (chatText.StartsWith("/mapeditor", StringComparison.OrdinalIgnoreCase))
            {
                Network.Send<OpenMapEditorRequest>();
                return true;
            }

            // Warping to a player
            if (chatText.StartsWith("/warpmeto", StringComparison.OrdinalIgnoreCase))
            {
                if (chatText.Length > 10)
                {
                    var name = chatText[10..].Trim();
                    if (name.Length > 0)
                    {
                        Network.Send(new WarpMeToRequest(name));
                    }
                }

                return true;
            }

            // Warping a player to you
            if (chatText.StartsWith("/warptome", StringComparison.OrdinalIgnoreCase))
            {
                if (chatText.Length > 10)
                {
                    var name = chatText[10..].Trim();
                    if (name.Length > 0)
                    {
                        Network.Send(new WarpToMeRequest(name));
                    }
                }

                return true;
            }

            // Warping to a map
            if (chatText.StartsWith("/warpto", StringComparison.OrdinalIgnoreCase))
            {
                if (chatText.Length > 8)
                {
                    var str = chatText[8..].Trim();
                    if (int.TryParse(str, out var mapId))
                    {
                        // Check to make sure its a valid map #
                        if (mapId is > 0 and <= modTypes.MAX_MAPS)
                        {
                            Network.Send(new WarpToRequest(mapId));
                        }
                        else
                        {
                            modText.AddText("Invalid map number.", modText.Red);
                        }
                    }
                }

                return true;
            }

            // Setting sprite
            if (chatText.StartsWith("/setsprite", StringComparison.OrdinalIgnoreCase))
            {
                if (chatText.Length > 11)
                {
                    var str = chatText[11..].Trim();
                    if (int.TryParse(str, out var sprite))
                    {
                        Network.Send(new SetSpriteRequest(sprite));
                    }
                }

                return true;
            }

            // Map report
            if (chatText.StartsWith("/mapreport", StringComparison.OrdinalIgnoreCase))
            {
                Network.Send<MapReportRequest>();
                return true;
            }

            // Respawn request
            if (chatText.StartsWith("/respawn", StringComparison.OrdinalIgnoreCase))
            {
                Network.Send<MapRespawnRequest>();
                return true;
            }

            // MOTD change
            if (chatText.StartsWith("/motd"))
            {
                if (chatText.Length > 6)
                {
                    var motd = chatText[6..].Trim();
                    if (motd.Length > 0)
                    {
                        Network.Send(new SetMotdRequest(motd));
                    }
                }

                return true;
            }

            // Check the ban list
            if (chatText.StartsWith("/banlist", StringComparison.OrdinalIgnoreCase))
            {
                Network.Send<BanListRequest>();
                return true;
            }

            // Banning a player
            if (chatText.StartsWith("/ban"))
            {
                if (chatText.Length > 5)
                {
                    var targetName = chatText[5..].Trim();
                    if (targetName.Length > 0)
                    {
                        Network.Send(new BanPlayerRequest(targetName));
                    }
                }

                return true;
            }
        }

        if (access >= modTypes.ADMIN_DEVELOPER)
        {
            // Editing item request
            if (chatText.StartsWith("/edititem"))
            {
                Network.Send<OpenItemEditorRequest>();
                return true;
            }

            // Editing npc request
            if (chatText.StartsWith("/editnpc"))
            {
                Network.Send<OpenNpcEditorRequest>();
                return true;
            }

            // Editing shop request
            if (chatText.StartsWith("/editshop"))
            {
                Network.Send<OpenShopEditorRequest>();
                return true;
            }

            // Editing spell request
            if (chatText.StartsWith("/editspell"))
            {
                Network.Send<OpenSpellEditorRequest>();
                return true;
            }
        }

        if (access < modTypes.ADMIN_CREATOR)
        {
            return false;
        }

        if (chatText.StartsWith("/setaccess"))
        {
            var newAccessLevel = int.Parse(chatText.Substring(11, 1));

            chatText = chatText[13..];

            Network.Send(new SetAccessLevelRequest(chatText, (AccessLevel) newAccessLevel));
            return true;
        }

        if (chatText.StartsWith("/destroybanlist"))
        {
            Network.Send<BanDestroyRequest>();
            return true;
        }

        return false;
    }
}