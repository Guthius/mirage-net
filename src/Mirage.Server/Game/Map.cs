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
    public void Update()
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

        Send(new RemovePlayerCommand(player.Id));
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
}