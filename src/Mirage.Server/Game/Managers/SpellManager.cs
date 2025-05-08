using System.Diagnostics;
using Mirage.Game.Constants;
using Mirage.Game.Data;
using Mirage.Server.Modules;
using MongoDB.Driver;
using Serilog;

namespace Mirage.Server.Game.Managers;

public static class SpellManager
{
    private static IMongoCollection<SpellInfo> GetCollection()
    {
        return DatabaseManager.GetCollection<SpellInfo>("spells");
    }

    public static SpellInfo? Get(int spellId)
    {
        if (spellId is <= 0 or > Limits.MaxSpells)
        {
            return null;
        }

        return modTypes.Spells[spellId];
    }

    public static void Update(int spellId, SpellInfo spellInfo)
    {
        if (spellId is <= 0 or > Limits.MaxSpells)
        {
            return;
        }

        modTypes.Spells[spellId] = spellInfo;

        Save(spellId);
    }

    private static void Save(int spellId)
    {
        GetCollection().ReplaceOne(x => x.Id == spellId, modTypes.Spells[spellId], new ReplaceOptions
        {
            IsUpsert = true
        });
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
                modTypes.Spells[spellId] = spellInfos.FirstOrDefault(x => x.Id == spellId) ?? CreateSpell(spellId);
            }
        }
        finally
        {
            stopwatch.Stop();

            Log.Information("Loaded {Count} spells in {ElapsedMs}ms", modTypes.Spells.Length, stopwatch.ElapsedMilliseconds);
        }

        return;

        static SpellInfo CreateSpell(int spellId)
        {
            return new SpellInfo
            {
                Id = spellId
            };
        }
    }
}