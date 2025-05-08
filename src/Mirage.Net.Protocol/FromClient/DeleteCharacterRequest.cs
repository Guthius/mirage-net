namespace Mirage.Net.Protocol.FromClient;

public sealed record DeleteCharacterRequest(int Slot) : IPacket<DeleteCharacterRequest>
{
    public static string PacketId => "delchar";

    public static DeleteCharacterRequest ReadFrom(PacketReader reader)
    {
        return new DeleteCharacterRequest(Slot: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(Slot);
    }
}