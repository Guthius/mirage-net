using Mirage.Modules;

namespace Mirage;

internal static class Program
{
    private static readonly CancellationTokenSource CancellationTokenSource = new();

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
                modGeneral.PlayerSaveTimer();
                savePlayerTimeLeft += savePlayersInterval;
            }

            spawnMapItemsTimeLeft -= 500;
            if (spawnMapItemsTimeLeft <= 0)
            {
                modGeneral.CheckSpawnMapItems();
                spawnMapItemsTimeLeft += spawnMapItemsInterval;
            }

            Console.WriteLine("AI Tick");
            modGeneral.GameAI();
        }
    }

    public static void Main()
    {
        AppDomain.CurrentDomain.ProcessExit += (_, _) =>
        {
            CancellationTokenSource.Cancel();

            modGeneral.DestroyServer();
        };

        modGeneral.InitServer();

        if (modTypes.Class.Length == 0)
        {
            Console.WriteLine("There are no character classes setup");
            Console.WriteLine("The server will not function correctly without classes.");
            Console.WriteLine("Please configure atleast one class in the Classes.json file");
            return;
        }

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
                modDatabase.LoadClasses();
                Console.WriteLine("All classes reloaded.");
                continue;
            }

            modServerTCP.GlobalMsg(command, modText.White);
        }
    }
}