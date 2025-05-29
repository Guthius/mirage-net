namespace Mirage.Net.Protocol.FromClient;

public sealed record DeleteCharacterRequest(string CharacterId) : IPacket<DeleteCharacterRequest>
{
    public static string PacketId => nameof(DeleteCharacterRequest);

    public static DeleteCharacterRequest ReadFrom(PacketReader reader)
    {
        return new DeleteCharacterRequest(CharacterId: reader.ReadString());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteString(CharacterId);
    }
}