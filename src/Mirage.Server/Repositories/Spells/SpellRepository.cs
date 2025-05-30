using System.Runtime.CompilerServices;
using Mirage.Shared.Constants;
using Mirage.Shared.Data;

namespace Mirage.Server.Repositories.Spells;

public static class SpellRepository
{
    private static readonly SpellInfo[] Spells = new SpellInfo[Limits.MaxSpells + 1];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SpellInfo? Get(int spellId)
    {
        if (spellId is <= 0 or > Limits.MaxSpells)
        {
            return null;
        }

        return Spells[spellId];
    }
}