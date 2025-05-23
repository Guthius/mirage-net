using Mirage.Shared.Constants;
using Mirage.Shared.Data;

namespace Mirage.Net.Protocol.FromServer;

public sealed record MapItemData(MapItemInfo?[] Items) : IPacket<MapItemData>
{
    public static string PacketId => "mapitemdata";

    public static MapItemData ReadFrom(PacketReader reader)
    {
        var mapItemInfos = new MapItemInfo?[Limits.MaxMapItems + 1];

        for (var i = 1; i <= Limits.MaxMapItems; i++)
        {
            var mapItemInfo = new MapItemInfo(
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32());

            mapItemInfos[i] = mapItemInfo.ItemId == 0 ? null : mapItemInfo;
        }

        return new MapItemData(mapItemInfos);
    }

    public void WriteTo(PacketWriter writer)
    {
        foreach (var mapItemInfo in Items)
        {
            writer.WriteInt32(mapItemInfo?.ItemId ?? 0);
            writer.WriteInt32(mapItemInfo?.Value ?? 0);
            writer.WriteInt32(mapItemInfo?.Dur ?? 0);
            writer.WriteInt32(mapItemInfo?.X ?? 0);
            writer.WriteInt32(mapItemInfo?.Y ?? 0);
        }
    }
}