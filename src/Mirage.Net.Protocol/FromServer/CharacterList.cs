using Mirage.Game.Constants;
using Mirage.Game.Data;

namespace Mirage.Net.Protocol.FromServer;

public sealed record CharacterList(List<CharacterSlotInfo> Slots) : IPacket<CharacterList>
{
    public static string PacketId => "allchars";

    public static CharacterList ReadFrom(PacketReader reader)
    {
        var slots = new List<CharacterSlotInfo>();

        for (var slot = 1; slot < Limits.MaxCharacters + 1; slot++)
        {
            slots.Add(new CharacterSlotInfo
            {
                Slot = slot,
                Name = reader.ReadString(),
                ClassName = reader.ReadString(),
                Level = reader.ReadInt32()
            });
        }

        return new CharacterList(slots);
    }

    public void WriteTo(PacketWriter writer)
    {
        foreach (var slotInfo in Slots)
        {
            writer.WriteString(slotInfo.Name);
            writer.WriteString(slotInfo.ClassName);
            writer.WriteInt32(slotInfo.Level);
        }
    }
}