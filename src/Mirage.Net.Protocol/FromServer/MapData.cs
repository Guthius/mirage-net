using Mirage.Game.Constants;
using Mirage.Game.Data;

namespace Mirage.Net.Protocol.FromServer;

public sealed record MapData(MapInfo Map) : IPacket<MapData>
{
    public static string PacketId => "mapdata";

    public static MapData ReadFrom(PacketReader reader)
    {
        var mapInfo = new MapInfo
        {
            Id = reader.ReadInt32(),
            Name = reader.ReadString(),
            Revision = reader.ReadInt32(),
            Moral = reader.ReadEnum<MapMoral>(),
            Up = reader.ReadInt32(),
            Down = reader.ReadInt32(),
            Left = reader.ReadInt32(),
            Right = reader.ReadInt32(),
            Music = reader.ReadInt32(),
            BootMapId = reader.ReadInt32(),
            BootX = reader.ReadInt32(),
            BootY = reader.ReadInt32(),
            ShopId = reader.ReadInt32(),
            Tiles = new TileInfo[Limits.MaxMapWidth + 1, Limits.MaxMapHeight + 1],
            NpcIds = new int[Limits.MaxMapNpcs + 1]
        };

        for (var y = 0; y <= Limits.MaxMapHeight; y++)
        {
            for (var x = 0; x <= Limits.MaxMapWidth; x++)
            {
                mapInfo.Tiles[x, y] = new TileInfo
                {
                    Ground = reader.ReadInt32(),
                    Mask = reader.ReadInt32(),
                    Anim = reader.ReadInt32(),
                    Fringe = reader.ReadInt32(),
                    Type = reader.ReadEnum<TileType>(),
                    Data1 = reader.ReadInt32(),
                    Data2 = reader.ReadInt32(),
                    Data3 = reader.ReadInt32()
                };
            }
        }

        for (var slot = 1; slot <= Limits.MaxMapNpcs; slot++)
        {
            mapInfo.NpcIds[slot] = reader.ReadInt32();
        }

        return new MapData(mapInfo);
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(Map.Id);
        writer.WriteString(Map.Name);
        writer.WriteInt32(Map.Revision);
        writer.WriteEnum(Map.Moral);
        writer.WriteInt32(Map.Up);
        writer.WriteInt32(Map.Down);
        writer.WriteInt32(Map.Left);
        writer.WriteInt32(Map.Right);
        writer.WriteInt32(Map.Music);
        writer.WriteInt32(Map.BootMapId);
        writer.WriteInt32(Map.BootX);
        writer.WriteInt32(Map.BootY);
        writer.WriteInt32(Map.ShopId);

        for (var y = 0; y <= Limits.MaxMapHeight; y++)
        {
            for (var x = 0; x <= Limits.MaxMapWidth; x++)
            {
                var tile = Map.Tiles[x, y];

                writer.WriteInt32(tile.Ground);
                writer.WriteInt32(tile.Mask);
                writer.WriteInt32(tile.Anim);
                writer.WriteInt32(tile.Fringe);
                writer.WriteEnum(tile.Type);
                writer.WriteInt32(tile.Data1);
                writer.WriteInt32(tile.Data2);
                writer.WriteInt32(tile.Data3);
            }
        }

        for (var slot = 1; slot <= Limits.MaxMapNpcs; slot++)
        {
            writer.WriteInt32(Map.NpcIds[slot]);
        }
    }
}