using System.Diagnostics;
using System.Runtime.CompilerServices;
using Mirage.Shared.Constants;
using Mirage.Shared.Data;
using MongoDB.Driver;
using Serilog;

namespace Mirage.Server.Repositories;

public static class SpellRepository
{
    private static readonly SpellInfo[] Spells = new SpellInfo[Limits.MaxSpells + 1];

    private static IMongoCollection<SpellInfo> GetCollection()
    {
        return Database.GetCollection<SpellInfo>("spells");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SpellInfo? Get(int spellId)
    {
        if (spellId is <= 0 or > Limits.MaxSpells)
        {
            return null;
        }

        return Spells[spellId];
    }

    public static void Load()
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var spellInfos = GetCollection()
                .Find(Builders<SpellInfo>.Filter.Empty)
                .ToList();

            for (var spellId = 1; spellId <= Limits.MaxSpells; spellId++)
            {
                Spells[spellId] = spellInfos.FirstOrDefault(x => x.Id == spellId) ?? CreateSpell(spellId);
            }
        }
        finally
        {
            stopwatch.Stop();

            Log.Information("Loaded {Count} spells in {ElapsedMs}ms", Spells.Length, stopwatch.ElapsedMilliseconds);
        }

        static SpellInfo CreateSpell(int spellId)
        {
            return new SpellInfo
            {
                Id = spellId
            };
        }
    }
}