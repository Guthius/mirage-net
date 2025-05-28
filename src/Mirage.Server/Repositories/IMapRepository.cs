using Mirage.Shared.Data;

namespace Mirage.Server.Repositories;

public interface IMapRepository
{
    IEnumerable<KeyValuePair<string, MapInfo>> Load();
}