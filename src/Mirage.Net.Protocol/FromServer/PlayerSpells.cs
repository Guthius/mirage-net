using Mirage.Shared.Constants;

namespace Mirage.Net.Protocol.FromServer;

public sealed record PlayerSpells(int[] SpellIds) : IPacket<PlayerSpells>
{
    public static string PacketId => "spells";

    public static PlayerSpells ReadFrom(PacketReader reader)
    {
        var spellIds = new int[Limits.MaxPlayerSpells + 1];

        for (var slot = 1; slot <= Limits.MaxPlayerSpells; slot++)
        {
            spellIds[slot] = reader.ReadInt32();
        }

        return new PlayerSpells(spellIds);
    }

    public void WriteTo(PacketWriter writer)
    {
        for (var slot = 1; slot <= Limits.MaxPlayerSpells; slot++)
        {
            writer.WriteInt32(SpellIds[slot]);
        }
    }
}