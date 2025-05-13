namespace Mirage.Net.Protocol.FromClient;

public sealed record EditItemRequest(int ItemId) : IPacket<EditItemRequest>
{
    public static string PacketId => "edititem";

    public static EditItemRequest ReadFrom(PacketReader reader)
    {
        return new EditItemRequest(ItemId: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(ItemId);
    }
}