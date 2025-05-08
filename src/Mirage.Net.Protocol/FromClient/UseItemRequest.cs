namespace Mirage.Net.Protocol.FromClient;

public sealed record UseItemRequest(int Slot) : IPacket<UseItemRequest>
{
    public static string PacketId => "useitem";

    public static UseItemRequest ReadFrom(PacketReader reader)
    {
        return new UseItemRequest(Slot: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(Slot);
    }
}