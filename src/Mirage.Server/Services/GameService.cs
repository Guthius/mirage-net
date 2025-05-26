using Microsoft.Extensions.Hosting;
using Mirage.Server.Maps;
using Mirage.Server.Net;
using Mirage.Server.Repositories;
using Serilog;

namespace Mirage.Server.Services;

public sealed class GameService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        JobRepository.Load();
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
            var deltaTime = (float) (currentTime - lastUpdateTime).TotalSeconds;

            lastUpdateTime = currentTime;

            MapManager.Update(deltaTime);
        }

        Log.Information("Shutting down server...");

        Network.SavePlayers();
    }
}