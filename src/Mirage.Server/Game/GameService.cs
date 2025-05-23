using Microsoft.Extensions.Hosting;
using Mirage.Server.Net;
using Mirage.Server.Repositories;
using Mirage.Shared.Constants;
using Serilog;

namespace Mirage.Server.Game;

public sealed class GameService : BackgroundService
{
    private int _minPassed;
    private int _spawnSeconds;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        const int savePlayersInterval = 60000;
        const int spawnMapItemsInterval = 1000;

        ClassRepository.Load();
        MapRepository.Load();
        NewMapRepository.Load();
        ItemRepository.Load();
        NpcRepository.Load();
        ShopRepository.Load();
        SpellRepository.Load();

        MapManager.Initialize();

        Network.Start();
        
        var lastUpdateTime = DateTime.UtcNow;

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(10, stoppingToken);
            
            var currentTime = DateTime.UtcNow;
            var deltaTime = (float)(currentTime - lastUpdateTime).TotalSeconds;
            
            lastUpdateTime = currentTime;

            MapManager.Update(deltaTime);
        }

        Log.Information("Shutting down server...");

        GameState.SavePlayers();
    }

    public void CheckSpawnMapItems()
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
}