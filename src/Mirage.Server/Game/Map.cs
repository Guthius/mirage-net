using Mirage.Net;
using Mirage.Net.Protocol.FromServer.New;
using Mirage.Server.Net;
using Mirage.Shared.Data;

namespace Mirage.Server.Game;

public sealed class Map : IPacketRecipient
{
    private readonly List<GamePlayer> _players = [];
    private readonly List<Npc> _npcs;
    private readonly NewMapInfo _info;

    public Map(NewMapInfo info)
    {
        _info = info;
        _npcs =
        [
            new Npc(this)
            {
                Id = MakeNpcId(1),
                Name = "Test NPC 1",
                Sprite = 2,
                X = Random.Shared.Next(0, 10),
                Y = Random.Shared.Next(0, 10),
                Direction = Direction.Up,
                MaxHealth = 5,
                Health = 5,
                Alive = true
            },
            new Npc(this)
            {
                Id = MakeNpcId(2),
                Name = "Test NPC 2",
                Sprite = 2,
                X = Random.Shared.Next(0, 10),
                Y = Random.Shared.Next(0, 10),
                Direction = Direction.Up,
                MaxHealth = 5,
                Health = 5,
                Alive = true
            },
            new Npc(this)
            {
                Id = MakeNpcId(3),
                Name = "Test NPC 3",
                Sprite = 2,
                X = Random.Shared.Next(0, 10),
                Y = Random.Shared.Next(0, 10),
                Direction = Direction.Up,
                MaxHealth = 5,
                Health = 5,
                Alive = true
            }
        ];
    }

    private static int MakeNpcId(int slot)
    {
        return (slot & 0xffff) << 16;
    }

    public void Update(float deltaTime)
    {
        /* Don't bother updating the map if there are no players on it. */
        if (_players.Count == 0)
        {
            return;
        }

        /* Update all players on the map. */
        UpdatePlayers(deltaTime);

        /* Update all NPC's on the map. */
        UpdateNpcs(deltaTime);

        // TODO: Implement: despawn items when they reach their expiry time...
    }

    private void UpdatePlayers(float deltaTime)
    {
        // TODO: Player healh regen...
    }

    private void UpdateNpcs(float deltaTime)
    {
        foreach (var npc in _npcs)
        {
            npc.Update(deltaTime);
        }
    }

    /// <summary>
    /// Adds the specified <paramref name="player"/> to the map.
    /// </summary>
    /// <param name="player">The player to add to the map.</param>
    public void Add(GamePlayer player)
    {
        _players.Add(player);

        player.Send(new LoadMapCommand(_info.Id));

        foreach (var otherPlayer in _players)
        {
            player.Send(new CreateActorCommand(
                otherPlayer.Id,
                otherPlayer.Character.Name,
                otherPlayer.Character.Sprite,
                otherPlayer.Character.PlayerKiller,
                otherPlayer.Character.AccessLevel,
                otherPlayer.Character.X,
                otherPlayer.Character.Y,
                otherPlayer.Character.Direction,
                otherPlayer.Character.MaxHP,
                otherPlayer.Character.HP,
                otherPlayer.Character.MaxMP,
                otherPlayer.Character.MP,
                otherPlayer.Character.MaxSP,
                otherPlayer.Character.SP));
        }

        foreach (var npc in _npcs)
        {
            if (!npc.Alive)
            {
                continue;
            }

            player.Send(new CreateActorCommand(
                npc.Id,
                npc.Name,
                npc.Sprite,
                false, AccessLevel.None,
                npc.X,
                npc.Y,
                npc.Direction,
                npc.MaxHealth,
                npc.Health,
                0, 0, 0, 0));
        }

        Send(new CreateActorCommand(
            player.Id,
            player.Character.Name,
            player.Character.Sprite,
            player.Character.PlayerKiller,
            player.Character.AccessLevel,
            player.Character.X,
            player.Character.Y,
            player.Character.Direction,
            player.Character.MaxHP,
            player.Character.HP,
            player.Character.MaxMP,
            player.Character.MP,
            player.Character.MaxSP,
            player.Character.SP));
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

        Send(new DestroyActorCommand(player.Id));
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

        Send(new ActorMoveCommand(player.Id, direction, movementType), recipient => recipient.Id != player.Id);
    }

    /// <summary>
    /// Makes the player attack the actor on the tile in the direction the player is facing.
    /// </summary>
    /// <param name="player">The player that is attacking.</param>
    public void Attack(GamePlayer player)
    {
        Send(new ActorAttackCommand(player.Id), recipient => recipient.Id != player.Id);
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

    /// <summary>
    /// Sends a message to all players on the map.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="color">The message color.</param>
    public void SendMessage(string message, int color)
    {
        Send(new ChatCommand(message, color));
    }
}