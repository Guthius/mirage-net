using System.Diagnostics;
using System.Runtime.CompilerServices;
using Mirage.Game.Constants;
using Mirage.Game.Data;
using MongoDB.Driver;
using Serilog;

namespace Mirage.Server.Repositories;

public static class NpcRepository
{
    private static readonly NpcInfo[] Npcs = new NpcInfo[Limits.MaxNpcs + 1];

    private static IMongoCollection<NpcInfo> GetCollection()
    {
        return Database.GetCollection<NpcInfo>("npcs");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NpcInfo? Get(int npcId)
    {
        if (npcId is <= 0 or > Limits.MaxNpcs)
        {
            return null;
        }

        return Npcs[npcId];
    }

    public static void Update(int npcId, NpcInfo npcInfo)
    {
        if (npcId is <= 0 or > Limits.MaxNpcs)
        {
            return;
        }

        Npcs[npcId] = npcInfo;

        Save(npcId);
    }

    public static void Save(int npcId)
    {
        GetCollection().ReplaceOne(x => x.Id == npcId, Npcs[npcId], new ReplaceOptions
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
                Npcs[npcId] = npcInfos.FirstOrDefault(x => x.Id == npcId) ?? CreateNpc(npcId);
            }
        }
        finally
        {
            stopwatch.Stop();

            Log.Information("Loaded {Count} NPC's in {ElapsedMs}ms", Npcs.Length, stopwatch.ElapsedMilliseconds);
        }

        static NpcInfo CreateNpc(int npcId)
        {
            return new NpcInfo
            {
                Id = npcId
            };
        }
    }
}