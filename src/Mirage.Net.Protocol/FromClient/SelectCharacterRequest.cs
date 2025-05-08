namespace Mirage.Net.Protocol.FromClient;

public sealed record SelectCharacterRequest(int Slot) : IPacket<SelectCharacterRequest>
{
    public static string PacketId => "usechar";
    
    public static SelectCharacterRequest ReadFrom(PacketReader reader)
    {
        return new SelectCharacterRequest(Slot: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(Slot);
    }
}