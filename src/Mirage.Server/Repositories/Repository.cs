using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Logging;
using Mirage.Shared.Data;
using MongoDB.Driver;

namespace Mirage.Server.Repositories;

public sealed class Repository<T> : IRepository<T> where T : ObjectInfo
{
    private readonly string _collectionName = GetCollectionName(typeof(T).Name);
    private readonly Dictionary<string, T> _items = [];

    public Repository(ILogger<Repository<T>> logger)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var items = Database
                .GetCollection<T>(_collectionName)
                .Find(Builders<T>.Filter.Empty)
                .ToList();

            foreach (var item in items)
            {
                _items[item.Id] = item;
            }
        }
        finally
        {
            stopwatch.Stop();

            logger.LogInformation("Loaded {Count} items from {CollectionName} in {ElapsedMs}ms",
                _items.Count, _collectionName, stopwatch.ElapsedMilliseconds);
        }
    }

    public T? Get(string id)
    {
        return _items.GetValueOrDefault(id);
    }

    private static string GetCollectionName(ReadOnlySpan<char> collectionName)
    {
        if (collectionName.EndsWith("Info", StringComparison.OrdinalIgnoreCase))
        {
            collectionName = collectionName[..^4];
        }

        var stringBuilder = new StringBuilder();
        foreach (var ch in collectionName)
        {
            stringBuilder.Append(char.ToLowerInvariant(ch));
            if (char.IsUpper(ch))
            {
                stringBuilder.Append('_');
            }
        }

        stringBuilder.Append('s');

        return stringBuilder.ToString();
    }
}