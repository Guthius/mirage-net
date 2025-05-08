using Mirage.Game.Constants;
using Mirage.Game.Data;

namespace Mirage.Net.Protocol.FromClient;

public sealed record UpdateMapRequest(MapInfo MapInfo) : IPacket<UpdateMapRequest>
{
    public static string PacketId => "mapdata";

    public static UpdateMapRequest ReadFrom(PacketReader reader)
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
            ShopId = reader.ReadInt32()
        };

        for (var y = 0; y <= Limits.MaxMapHeight; y++)
        {
            for (var x = 0; x <= Limits.MaxMapWidth; x++)
            {
                mapInfo.Tiles[x, y].Ground = reader.ReadInt32();
                mapInfo.Tiles[x, y].Mask = reader.ReadInt32();
                mapInfo.Tiles[x, y].Anim = reader.ReadInt32();
                mapInfo.Tiles[x, y].Fringe = reader.ReadInt32();
                mapInfo.Tiles[x, y].Type = reader.ReadEnum<TileType>();
                mapInfo.Tiles[x, y].Data1 = reader.ReadInt32();
                mapInfo.Tiles[x, y].Data2 = reader.ReadInt32();
                mapInfo.Tiles[x, y].Data3 = reader.ReadInt32();
            }
        }

        for (var mapNpcId = 1; mapNpcId <= Limits.MaxMapNpcs; mapNpcId++)
        {
            mapInfo.NpcIds[mapNpcId] = reader.ReadInt32();
        }

        return new UpdateMapRequest(mapInfo);
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteString(MapInfo.Name);
        writer.WriteInt32(MapInfo.Revision);
        writer.WriteEnum(MapInfo.Moral);
        writer.WriteInt32(MapInfo.Up);
        writer.WriteInt32(MapInfo.Down);
        writer.WriteInt32(MapInfo.Left);
        writer.WriteInt32(MapInfo.Right);
        writer.WriteInt32(MapInfo.Music);
        writer.WriteInt32(MapInfo.BootMapId);
        writer.WriteInt32(MapInfo.BootX);
        writer.WriteInt32(MapInfo.BootY);
        writer.WriteInt32(MapInfo.ShopId);

        for (var y = 0; y <= Limits.MaxMapHeight; y++)
        {
            for (var x = 0; x <= Limits.MaxMapWidth; x++)
            {
                writer.WriteInt32(MapInfo.Tiles[x, y].Ground);
                writer.WriteInt32(MapInfo.Tiles[x, y].Mask);
                writer.WriteInt32(MapInfo.Tiles[x, y].Anim);
                writer.WriteInt32(MapInfo.Tiles[x, y].Fringe);
                writer.WriteEnum(MapInfo.Tiles[x, y].Type);
                writer.WriteInt32(MapInfo.Tiles[x, y].Data1);
                writer.WriteInt32(MapInfo.Tiles[x, y].Data2);
                writer.WriteInt32(MapInfo.Tiles[x, y].Data3);
            }
        }

        for (var mapNpcId = 1; mapNpcId <= Limits.MaxMapNpcs; mapNpcId++)
        {
            writer.WriteInt32(MapInfo.NpcIds[mapNpcId]);
        }
    }
}