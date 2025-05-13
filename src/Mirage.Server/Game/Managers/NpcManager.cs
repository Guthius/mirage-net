using System.Diagnostics;
using Mirage.Game.Constants;
using Mirage.Game.Data;
using Mirage.Server.Modules;
using MongoDB.Driver;
using Serilog;

namespace Mirage.Server.Game.Managers;

public static class NpcManager
{
    private static IMongoCollection<NpcInfo> GetCollection()
    {
        return DatabaseManager.GetCollection<NpcInfo>("npcs");
    }

    public static NpcInfo? Get(int npcId)
    {
        if (npcId is <= 0 or > Limits.MaxNpcs)
        {
            return null;
        }

        return modTypes.Npcs[npcId];
    }

    public static void Update(int npcId, NpcInfo npcInfo)
    {
        if (npcId is <= 0 or > Limits.MaxNpcs)
        {
            return;
        }

        modTypes.Npcs[npcId] = npcInfo;

        Save(npcId);
    }

    public static void Save(int npcId)
    {
        GetCollection().ReplaceOne(x => x.Id == npcId, modTypes.Npcs[npcId], new ReplaceOptions
        {
            IsUpsert = true
        });
    }

    public static void Load()
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var npcInfos = GetCollection()
                .Find(Builders<NpcInfo>.Filter.Empty)
                .ToList();

            for (var npcId = 1; npcId <= Limits.MaxNpcs; npcId++)
            {
                modTypes.Npcs[npcId] = npcInfos.FirstOrDefault(x => x.Id == npcId) ?? CreateNpc(npcId);
            }
        }
        finally
        {
            stopwatch.Stop();

            Log.Information("Loaded {Count} NPC's in {ElapsedMs}ms", modTypes.Npcs.Length, stopwatch.ElapsedMilliseconds);
        }

        return;

        static NpcInfo CreateNpc(int npcId)
        {
            return new NpcInfo
            {
                Id = npcId
            };
        }
    }
}