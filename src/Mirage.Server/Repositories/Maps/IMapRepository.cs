using Mirage.Shared.Data;

namespace Mirage.Server.Repositories.Maps;

public interface IMapRepository
{
    IEnumerable<KeyValuePair<string, MapInfo>> Load();
}