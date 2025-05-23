using Mirage.Shared.Data;

namespace Mirage.Net.Protocol.FromServer;

public sealed record UpdateItem(ItemInfo ItemInfo) : IPacket<UpdateItem>
{
    public static string PacketId => "updateitem";

    public static UpdateItem ReadFrom(PacketReader reader)
    {
        return new UpdateItem(ItemInfo: new ItemInfo
        {
            Id = reader.ReadInt32(),
            Name = reader.ReadString(),
            Sprite = reader.ReadInt32(),
            Type = reader.ReadEnum<ItemType>(),
            Data1 = reader.ReadInt32(),
            Data2 = reader.ReadInt32(),
            Data3 = reader.ReadInt32()
        });
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(ItemInfo.Id);
        writer.WriteString(ItemInfo.Name);
        writer.WriteInt32(ItemInfo.Sprite);
        writer.WriteEnum(ItemInfo.Type);
        writer.WriteInt32(ItemInfo.Data1);
        writer.WriteInt32(ItemInfo.Data2);
        writer.WriteInt32(ItemInfo.Data3);
    }
}