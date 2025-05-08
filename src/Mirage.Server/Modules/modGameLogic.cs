using Mirage.Game.Constants;
using Mirage.Game.Data;
using Mirage.Net.Protocol.FromServer;
using Mirage.Server.Game;
using Mirage.Server.Game.Managers;
using Serilog;

namespace Mirage.Server.Modules;

public static class modGameLogic
{
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

    public static (int X, int Y) GetAdjacentPosition(int x, int y, Direction direction)
    {
        return direction switch
        {
            Direction.Up => (x, y - 1),
            Direction.Down => (x, y + 1),
            Direction.Left => (x - 1, y),
            Direction.Right => (x + 1, y),
            _ => (x, y)
        };
    }

    public static void NpcDir(GameNpc npc, Direction direction)
    {
        npc.Direction = direction;
        npc.Map.Send(new NpcDir(npc.Slot, direction));
    }


}