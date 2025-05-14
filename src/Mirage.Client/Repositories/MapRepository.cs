using MessagePack;
using Mirage.Client.Modules;

namespace Mirage.Client.Repositories;

public static class MapRepository
{
    public static string GetMapPath()
    {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Mirage", "Maps");
    }

    public static bool MapExist(int mapNum)
    {
        var path = Path.Combine(GetMapPath(), $"Map{mapNum}.map");

        return File.Exists(path);
    }

    public static void SaveLocalMap(int mapNum)
    {
        var path = GetMapPath();
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        var bytes = MessagePackSerializer.Serialize(modGameLogic.SaveMap);

        path = Path.Combine(path, $"Map{mapNum}.map");

        File.WriteAllBytes(path, bytes);
    }

    public static void LoadMap(int mapNum)
    {
        var path = GetMapPath();
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        path = Path.Combine(path, $"Map{mapNum}.map");

        var mapBytes = File.ReadAllBytes(path);
        var map = MessagePackSerializer.Deserialize<modTypes.MapRec>(mapBytes);

        modGameLogic.SaveMap = map;
    }

    public static int GetMapRevision(int mapNum)
    {
        var path = GetMapPath();
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        path = Path.Combine(path, $"Map{mapNum}.map");

        var mapBytes = File.ReadAllBytes(path);
        var map = MessagePackSerializer.Deserialize<modTypes.MapRec>(mapBytes);

        return map.Revision;
    }
}