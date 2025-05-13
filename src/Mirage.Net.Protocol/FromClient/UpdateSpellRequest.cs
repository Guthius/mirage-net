using Mirage.Game.Data;

namespace Mirage.Net.Protocol.FromClient;

public sealed record UpdateSpellRequest(SpellInfo SpellInfo) : IPacket<UpdateSpellRequest>
{
    public static string PacketId => "savespell";

    public static UpdateSpellRequest ReadFrom(PacketReader reader)
    {
        return new UpdateSpellRequest(
            SpellInfo: new SpellInfo
            {
                Id = reader.ReadInt32(),
                Name = reader.ReadString(),
                RequiredClassId = reader.ReadInt32(),
                RequiredLevel = reader.ReadInt32(),
                Type = reader.ReadEnum<SpellType>(),
                Data1 = reader.ReadInt32(),
                Data2 = reader.ReadInt32(),
                Data3 = reader.ReadInt32()
            });
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(SpellInfo.Id);
        writer.WriteString(SpellInfo.Name);
        writer.WriteInt32(SpellInfo.RequiredClassId);
        writer.WriteInt32(SpellInfo.RequiredLevel);
        writer.WriteEnum(SpellInfo.Type);
        writer.WriteInt32(SpellInfo.Data1);
        writer.WriteInt32(SpellInfo.Data2);
        writer.WriteInt32(SpellInfo.Data3);
    }
}