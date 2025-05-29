namespace Mirage.Net.Protocol.FromServer;

public sealed record SelectCharacterResponse(SelectCharacterResult Result, int PlayerId) : IPacket<SelectCharacterResponse>
{
    public static string PacketId => nameof(SelectCharacterResponse);

    public static SelectCharacterResponse ReadFrom(PacketReader reader)
    {
        return new SelectCharacterResponse(
            Result: reader.ReadEnum<SelectCharacterResult>(),
            PlayerId: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteEnum(Result);
        writer.WriteInt32(PlayerId);
    }
}