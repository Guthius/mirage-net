using System.Collections;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Mirage.Server.Maps;
using Mirage.Server.Net;
using Mirage.Shared.Data;

namespace Mirage.Server.Players;

public sealed class PlayerService(ILogger<PlayerService> logger, IMapService mapService, IServiceProvider serviceProvider) : IPlayerService
{
    private readonly ConcurrentDictionary<int, Player> _players = [];

    public Player? Create(NetworkConnection connection, CharacterInfo character)
    {
        var map = mapService.GetByName(character.Map);
        if (map is null)
        {
            logger.LogWarning(
                "Unable to create player instance for character {CharacterName} (map {MapName} not found)",
                character.Name, character.Map);

            return null;
        }

        var player = new Player(connection, character, map, serviceProvider);

        _players[player.Id] = player;

        return player;
    }

    public void Destroy(Player player)
    {
        if (_players.TryRemove(player.Id, out _))
        {
            player.Destroy();
        }
    }

    public Player? Find(ReadOnlySpan<char> characterName)
    {
        foreach (var player in _players.Values)
        {
            if (characterName.Equals(player.Character.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                return player;
            }
        }

        return null;
    }

    public IEnumerator<Player> GetEnumerator()
    {
        return _players.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}