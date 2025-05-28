namespace Mirage.Server.Maps;

public interface IMapService
{
    void Update(float dt);
    Map? GetByName(string mapName);
}