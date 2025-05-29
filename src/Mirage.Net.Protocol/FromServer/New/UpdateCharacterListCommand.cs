using Mirage.Shared.Data;

namespace Mirage.Net.Protocol.FromServer.New;

public sealed record UpdateCharacterListCommand(int MaxCharacters, List<CharacterSlotInfo> Characters) : IPacket<UpdateCharacterListCommand>
{
    public static string PacketId => nameof(UpdateCharacterListCommand);

    public static UpdateCharacterListCommand ReadFrom(PacketReader reader)
    {
        return new UpdateCharacterListCommand(
            MaxCharacters: reader.ReadInt32(),
            Characters: reader.ReadList(() => new CharacterSlotInfo
            {
                CharacterId = reader.ReadString(),
                Name = reader.ReadString(),
                JobName = reader.ReadString(),
                Level = reader.ReadInt32()
            }));
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(MaxCharacters);
        writer.WriteList(Characters, character =>
        {
            writer.WriteString(character.CharacterId);
            writer.WriteString(character.Name);
            writer.WriteString(character.JobName);
            writer.WriteInt32(character.Level);
        });
    }
}