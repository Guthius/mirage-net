using Mirage.Shared.Data;

namespace Mirage.Net.Protocol.FromClient.New;

public sealed record CreateCharacterRequest(string CharacterName, Gender Gender, string JobId) : IPacket<CreateCharacterRequest>
{
    public static string PacketId => nameof(CreateCharacterRequest);

    public static CreateCharacterRequest ReadFrom(PacketReader reader)
    {
        return new CreateCharacterRequest(
            CharacterName: reader.ReadString(),
            Gender: reader.ReadEnum<Gender>(),
            JobId: reader.ReadString());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteString(CharacterName);
        writer.WriteEnum(Gender);
        writer.WriteString(JobId);
    }
}