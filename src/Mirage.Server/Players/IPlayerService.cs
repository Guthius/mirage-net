using Mirage.Server.Net;
using Mirage.Server.Repositories.Characters.Data;

namespace Mirage.Server.Players;

public interface IPlayerService : IEnumerable<Player>
{
    Player? Create(NetworkConnection connection, CharacterInfo character);
    void Destroy(Player player);
    Player? Find(ReadOnlySpan<char> characterName);
}