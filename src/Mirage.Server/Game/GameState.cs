using Mirage.Game.Constants;
using Mirage.Game.Data;
using Mirage.Server.Game.Repositories;
using Serilog;

namespace Mirage.Server.Game;

public static class GameState
{
    public static GameSession?[] Sessions { get; } = new GameSession[Limits.MaxPlayers + 1];

    private static readonly GameMap[] Maps = new GameMap[Limits.MaxMaps + 1];

    private static int _regenTimer;

    static GameState()
    {
        for (var mapId = 1; mapId <= Limits.MaxMaps; mapId++)
        {
            var mapInfo = MapRepository.Get(mapId) ?? new MapInfo();
            var map = new GameMap(mapInfo);

            Maps[mapId] = map;
            
            map.RespawnItems();
            map.RespawnNpcs();
        }
    }
    
    public static void Update()
    {
        CheckGiveHP();

        foreach (var map in Maps)
        {
            map?.Update();
        }
    }

    public static void CheckGiveHP()
    {
        if (Environment.TickCount <= _regenTimer + 10000)
        {
            return;
        }

        foreach (var player in OnlinePlayers())
        {
            player.Character.HP += player.Character.HPRegen;
            player.Character.MP += player.Character.MPRegen;
            player.Character.SP += player.Character.SPRegen;

            player.SendHP();
            player.SendMP();
            player.SendSP();
        }

        _regenTimer = Environment.TickCount;
    }

    public static GameSession? GetSession(int playerId)
    {
        if (playerId is <= 0 or > Limits.MaxPlayers)
        {
            return null;
        }

        return Sessions[playerId];
    }

    public static GameMap GetMap(int mapId)
    {
        if (mapId is <= 0 or > Limits.MaxMaps)
        {
            throw new InvalidOperationException("Invalid map");
        }

        return Maps[mapId];
    }

    public static GamePlayer? GetPlayer(int playerId)
    {
        return GetSession(playerId)?.Player;
    }

    public static void CreateSession(int playerId)
    {
        Sessions[playerId] = new GameSession(playerId);
    }

    public static void DestroySession(int playerId)
    {
        Sessions[playerId]?.Destroy();
        Sessions[playerId] = null;
    }

    public static bool IsAccountLoggedIn(string accountName)
    {
        for (var playerId = 1; playerId <= Limits.MaxPlayers; playerId++)
        {
            var accountInfo = Sessions[playerId]?.Account;
            if (accountInfo is null)
            {
                continue;
            }

            if (accountInfo.Name.Equals(accountName, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    public static GamePlayer? FindPlayer(ReadOnlySpan<char> characterName)
    {
        characterName = characterName.Trim();

        for (var playerId = 1; playerId <= Limits.MaxPlayers; playerId++)
        {
            var player = Sessions[playerId]?.Player;
            if (player is null)
            {
                continue;
            }

            var character = player.Character;
            if (character.Name.Length < characterName.Length)
            {
                continue;
            }

            if (character.Name.AsSpan()[..characterName.Length].Equals(characterName, StringComparison.CurrentCultureIgnoreCase))
            {
                return player;
            }
        }

        return null;
    }

    public static int OnlinePlayerCount()
    {
        return Sessions.Count(session => session?.Player is not null);
    }

    public static int OnlinePlayerCount(int mapId)
    {
        return Sessions.Count(session => session?.Player is not null && session.Player.Map.Info.Id == mapId);
    }

    public static IEnumerable<GamePlayer> OnlinePlayers()
    {
        return Sessions.Where(session => session?.Player is not null).Select(session => session!.Player!);
    }

    public static void SavePlayers()
    {
        Log.Information("Saving all online players...");

        for (var playerId = 1; playerId <= Limits.MaxPlayers; playerId++)
        {
            var session = Sessions[playerId];

            if (session?.Player != null)
            {
                CharacterRepository.Save(session.Player.Character);
            }
        }
    }
}