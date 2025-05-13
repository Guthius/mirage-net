using Mirage.Game.Constants;
using Mirage.Net.Protocol.FromServer;
using Mirage.Server.Game;
using Mirage.Server.Game.Managers;
using Mirage.Server.Modules;
using Mirage.Server.Net;
using Serilog;

namespace Mirage.Server;

internal static class Program
{
    private static readonly CancellationTokenSource CancellationTokenSource = new();

    private static int _minPassed;
    
    // Used for respawning items
    public static int SpawnSeconds;
    
    public static void CheckSpawnMapItems()
    {
        SpawnSeconds += 1;
        if (SpawnSeconds < 120)
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

        SpawnSeconds = 0;
    }
    
    public static async Task RunTimedEvents(CancellationToken cancellationToken)
    {
        const int savePlayersInterval = 60000;
        const int spawnMapItemsInterval = 1000;

        var savePlayerTimeLeft = savePlayersInterval;
        var spawnMapItemsTimeLeft = spawnMapItemsInterval;

        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(500, cancellationToken);

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
                    Network.SendToAll(new GlobalMessage("Saving all online players...", Color.Pink));

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
    }

    public static void Main()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        AppDomain.CurrentDomain.ProcessExit += (_, _) =>
        {
            CancellationTokenSource.Cancel();

            Log.Information("Shutting down server...");

            GameState.SavePlayers();

            Environment.Exit(0);
        };
        
        ClassManager.Load();
        MapManager.Load();
        ItemManager.Load();
        NpcManager.Load();
        ShopManager.Load();
        SpellManager.Load();
        
        modGameLogic.SpawnAllMapsItems();
        modGameLogic.SpawnAllMapNpcs();

        Network.Start();

        SpawnSeconds = 0;
        
        // TODO: Start game AI timers...

        _ = Task.Run(async () => await RunTimedEvents(CancellationTokenSource.Token));

        while (true)
        {
            var command = Console.ReadLine();
            if (command is null)
            {
                break;
            }

            if (command.Equals("quit", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            if (command.Equals("reloadclasses", StringComparison.OrdinalIgnoreCase))
            {
                ClassManager.Load();

                Log.Information("All classes reloaded");

                continue;
            }

            Network.SendToAll(new GlobalMessage(command, Color.White));
        }
    }
}