namespace Mirage.Net.Protocol.FromClient;

public sealed record SelectCharacterRequest(string CharacterId) : IPacket<SelectCharacterRequest>
{
    public static string PacketId => nameof(SelectCharacterRequest);

    public static SelectCharacterRequest ReadFrom(PacketReader reader)
    {
        return new SelectCharacterRequest(CharacterId: reader.ReadString());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteString(CharacterId);
    }
}