using Mirage.Game.Data;

namespace Mirage.Net.Protocol.FromServer;

public sealed record UpdateItem(int ItemId, ItemInfo Item) : IPacket<UpdateItem>
{
    public static string PacketId => "updateitem";

    public static UpdateItem ReadFrom(PacketReader reader)
    {
        return new UpdateItem(
            ItemId: reader.ReadInt32(),
            Item: new ItemInfo
            {
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
        writer.WriteInt32(ItemId);
        writer.WriteString(Item.Name);
        writer.WriteInt32(Item.Sprite);
        writer.WriteEnum(Item.Type);
        writer.WriteInt32(Item.Data1);
        writer.WriteInt32(Item.Data2);
        writer.WriteInt32(Item.Data3);
    }
}