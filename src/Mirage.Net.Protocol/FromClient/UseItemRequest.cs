namespace Mirage.Net.Protocol.FromClient;

public sealed record UseItemRequest(int SlotIndex) : IPacket<UseItemRequest>
{
    public static string PacketId => nameof(UseItemRequest);

    public static UseItemRequest ReadFrom(PacketReader reader)
    {
        return new UseItemRequest(SlotIndex: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(SlotIndex);
    }
}