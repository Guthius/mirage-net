using Mirage.Game.Data;
using Mirage.Net;
using Mirage.Net.Protocol.FromServer.New;
using Mirage.Server.Net;

namespace Mirage.Server.Game;

public sealed class Map(NewMapInfo info) : IPacketRecipient
{
    private readonly List<GamePlayer> _players = [];

    /// <summary>
    /// Updates the map and all objects on the map.
    /// </summary>
    public void Update(float dt)
    {
        // TODO: Implement me: regen player health, regen NPC health, move NPCs, respawn NPCS, despawn items
    }

    /// <summary>
    /// Adds the specified <paramref name="player"/> to the map.
    /// </summary>
    /// <param name="player">The player to add to the map.</param>
    public void Add(GamePlayer player)
    {
        _players.Add(player);

        player.Send(new LoadMapCommand(info.Name, info.Revision));

        foreach (var otherPlayer in _players)
        {
            player.Send(new CreatePlayerCommand(
                otherPlayer.Id,
                otherPlayer.Character.Name,
                otherPlayer.Character.JobId,
                otherPlayer.Character.Sprite,
                otherPlayer.Character.PlayerKiller,
                otherPlayer.Character.AccessLevel,
                otherPlayer.Character.X,
                otherPlayer.Character.Y,
                otherPlayer.Character.Direction));
        }

        // TODO: Sync NPC's and items with the new player...

        Send(new CreatePlayerCommand(
            player.Id,
            player.Character.Name,
            player.Character.JobId,
            player.Character.Sprite,
            player.Character.PlayerKiller,
            player.Character.AccessLevel,
            player.Character.X,
            player.Character.Y,
            player.Character.Direction));
    }

    /// <summary>
    /// Removes the specified <paramref name="player"/> from the map.
    /// </summary>
    /// <param name="player">The player to remove from the map.</param>
    public void Remove(GamePlayer player)
    {
        if (!_players.Remove(player))
        {
            return;
        }

        Send(new DestroyPlayerCommand(player.Id));
    }

    /// <summary>
    /// Moves the specified <paramref name="player"/> in the specified <paramref name="direction"/>.
    /// </summary>
    /// <param name="player">The player to move.</param>
    /// <param name="direction">The direction to move in.</param>
    /// <param name="movementType">The movement type.</param>
    public void Move(GamePlayer player, Direction direction, MovementType movementType)
    {
        // TODO: Check to ensure the move is valid...

        switch (direction)
        {
            case Direction.Up:
                player.Character.Y--;
                break;

            case Direction.Down:
                player.Character.Y++;
                break;

            case Direction.Left:
                player.Character.X--;
                break;

            case Direction.Right:
                player.Character.X++;
                break;
        }

        Send(new MovePlayerCommand(player.Id, direction, movementType), recipient => recipient.Id != player.Id);
    }

    /// <summary>
    /// Sends the specified <paramref name="packet"/> to all players on the map.
    /// </summary>
    /// <param name="packet">The packet to send.</param>
    /// <typeparam name="TPacket">The packet type.</typeparam>
    public void Send<TPacket>(TPacket packet) where TPacket : IPacket<TPacket>
    {
        var bytes = PacketSerializer.GetBytes(packet);

        foreach (var player in _players)
        {
            Network.SendData(player.Id, bytes);
        }
    }

    /// <summary>
    /// Sends the specified <paramref name="packet"/> to all players on the map.
    /// </summary>
    /// <param name="packet">The packet to send.</param>
    /// <param name="predicate"></param>
    /// <typeparam name="TPacket">The packet type.</typeparam>
    public void Send<TPacket>(TPacket packet, Func<GamePlayer, bool> predicate) where TPacket : IPacket<TPacket>
    {
        var bytes = PacketSerializer.GetBytes(packet);

        foreach (var player in _players)
        {
            if (predicate(player))
            {
                Network.SendData(player.Id, bytes);
            }
        }
    }
}