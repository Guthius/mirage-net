using System.Diagnostics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mirage.Server.Maps;
using Mirage.Server.Repositories.Jobs;

namespace Mirage.Server.Services;

public sealed class GameService(ILogger<GameService> logger, IMapService mapService, IJobRepository jobRepository) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        LoadData();

        logger.LogInformation("Game logic service has started");

        var lastUpdate = DateTime.UtcNow;
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(10, stoppingToken);

            var now = DateTime.UtcNow;
            var dt = (float) (now - lastUpdate).TotalSeconds;

            lastUpdate = now;

            mapService.Update(dt);
        }

        logger.LogInformation("Game logic service has stopped");
    }

    private void LoadData()
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            jobRepository.Load();
        }
        finally
        {
            stopwatch.Stop();

            logger.LogInformation("Game data loaded in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
        }
    }
}