using Mirage.Game.Data;

namespace Mirage.Net.Protocol.FromServer.New;

public sealed record CharacterList(int MaxCharacters, List<CharacterSlotInfo> Characters) : IPacket<CharacterList>
{
    public static string PacketId => nameof(CharacterList);

    public static CharacterList ReadFrom(PacketReader reader)
    {
        return new CharacterList(
            MaxCharacters: reader.ReadInt32(),
            Characters: reader.ReadList(() => new CharacterSlotInfo
            {
                CharacterId = reader.ReadString(),
                Name = reader.ReadString(),
                JobId = reader.ReadString(),
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
            writer.WriteString(character.JobId);
            writer.WriteInt32(character.Level);
        });
    }
}