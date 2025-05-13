using Mirage.Game.Data;

namespace Mirage.Net.Protocol.FromClient;

public sealed record CreateCharacterRequest(string CharacterName, Gender Gender, int ClassId, int Slot) : IPacket<CreateCharacterRequest>
{
    public static string PacketId => "addchar";

    public static CreateCharacterRequest ReadFrom(PacketReader reader)
    {
        return new CreateCharacterRequest(
            CharacterName: reader.ReadString(),
            Gender: reader.ReadEnum<Gender>(),
            ClassId: reader.ReadInt32(),
            Slot: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteString(CharacterName);
        writer.WriteEnum(Gender);
        writer.WriteInt32(ClassId);
        writer.WriteInt32(Slot);
    }
}