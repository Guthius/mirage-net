using Mirage.Shared.Constants;
using Mirage.Shared.Data;

namespace Mirage.Net.Protocol.FromServer;


public sealed record MapNpcData(MapNpcInfo[] Npcs) : IPacket<MapNpcData>
{
    public static string PacketId => "mapnpcdata";

    public static MapNpcData ReadFrom(PacketReader reader)
    {
        var npcs = new MapNpcInfo[Limits.MaxMapNpcs];

        for (var i = 0; i < Limits.MaxMapNpcs; i++)
        {
            npcs[i] = new MapNpcInfo(
                NpcId: reader.ReadInt32(),
                X: reader.ReadInt32(),
                Y: reader.ReadInt32(),
                Direction: reader.ReadEnum<Direction>());
        }

        return new MapNpcData(npcs);
    }

    public void WriteTo(PacketWriter writer)
    {
        foreach (var npc in Npcs)
        {
            writer.WriteInt32(npc.NpcId);
            writer.WriteInt32(npc.X);
            writer.WriteInt32(npc.Y);
            writer.WriteEnum(npc.Direction);
        }
    }
}
