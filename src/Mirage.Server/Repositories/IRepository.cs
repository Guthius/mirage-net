using Mirage.Shared.Data;

namespace Mirage.Server.Repositories;

public interface IRepository<out T> where T : ObjectInfo
{
    T? Get(string id);
}