using Microsoft.Extensions.Hosting;
using Mirage.Game.Constants;
using Mirage.Server.Game.Repositories;
using Mirage.Server.Net;
using Serilog;

namespace Mirage.Server.Game;

public sealed class GameService : BackgroundService
{
    private static int _minPassed;
    private static int _spawnSeconds;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        const int savePlayersInterval = 60000;
        const int spawnMapItemsInterval = 1000;

        ClassRepository.Load();
        MapRepository.Load();
        ItemRepository.Load();
        NpcRepository.Load();
        ShopRepository.Load();
        SpellRepository.Load();

        SpawnAllMapsItems();
        SpawnAllMapNpcs();

        Network.Start();

        var savePlayerTimeLeft = savePlayersInterval;
        var spawnMapItemsTimeLeft = spawnMapItemsInterval;

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(500, stoppingToken);

            savePlayerTimeLeft -= 500;
            if (savePlayerTimeLeft <= 0)
            {
                _minPassed += 1;
                if (_minPassed < 10)
                {
                    return;
                }

                if (GameState.OnlinePlayerCount() > 0)
                {
                    Network.SendGlobalMessage("Saving all online players...", Color.Pink);

                    GameState.SavePlayers();
                }

                _minPassed = 0;

                savePlayerTimeLeft += savePlayersInterval;
            }

            spawnMapItemsTimeLeft -= 500;
            if (spawnMapItemsTimeLeft <= 0)
            {
                CheckSpawnMapItems();

                spawnMapItemsTimeLeft += spawnMapItemsInterval;
            }

            GameState.Update();
        }

        Log.Information("Shutting down server...");

        GameState.SavePlayers();
    }

    public static void SpawnAllMapsItems()
    {
        Log.Information("Spawning map items...");

        for (var i = 1; i <= Limits.MaxMaps; i++)
        {
            var map = GameState.GetMap(i);

            map.RespawnItems();
        }
    }

    public static void SpawnAllMapNpcs()
    {
        Log.Information("Spawning map npcs...");

        for (var mapId = 1; mapId <= Limits.MaxMaps; mapId++)
        {
            var map = GameState.GetMap(mapId);

            map.RespawnNpcs();
        }
    }

    public static void CheckSpawnMapItems()
    {
        _spawnSeconds += 1;
        if (_spawnSeconds < 120)
        {
            return;
        }

        for (var mapId = 1; mapId <= Limits.MaxMaps; mapId++)
        {
            var map = GameState.GetMap(mapId);
            if (map.PlayersOnMap)
            {
                continue;
            }

            map.RespawnItems();
        }

        _spawnSeconds = 0;
    }

    public static async Task RunTimedEvents(CancellationToken cancellationToken)
    {
    }
}