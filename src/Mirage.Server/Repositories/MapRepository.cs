using System.Runtime.CompilerServices;
using Mirage.Shared.Constants;
using Mirage.Shared.Data;

namespace Mirage.Server.Repositories;

public static class MapRepository
{
    private static readonly MapInfo[] Maps = new MapInfo[Limits.MaxMaps + 1];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MapInfo? Get(int mapId)
    {
        if (mapId <= 0)
        {
            return null;
        }

        return Maps[mapId];
    }
}