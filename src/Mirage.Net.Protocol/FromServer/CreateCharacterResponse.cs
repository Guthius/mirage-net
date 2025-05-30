namespace Mirage.Net.Protocol.FromServer;

public sealed record CreateCharacterResponse(CreateCharacterResult Result) : IPacket<CreateCharacterResponse>
{
    public static string PacketId => nameof(CreateCharacterResponse);

    public static CreateCharacterResponse ReadFrom(PacketReader reader)
    {
        return new CreateCharacterResponse(Result: reader.ReadEnum<CreateCharacterResult>());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteEnum(Result);
    }
}