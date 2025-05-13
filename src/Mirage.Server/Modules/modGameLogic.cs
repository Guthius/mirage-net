using Mirage.Game.Data;
using Mirage.Net.Protocol.FromServer;
using Mirage.Server.Game;

namespace Mirage.Server.Modules;

public static class modGameLogic
{


    public static void NpcDir(GameNpc npc, Direction direction)
    {
        npc.Direction = direction;
        npc.Map.Send(new NpcDir(npc.Slot, direction));
    }
}