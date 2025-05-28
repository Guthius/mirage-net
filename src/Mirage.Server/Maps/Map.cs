using AStarNavigator;
using AStarNavigator.Algorithms;
using Mirage.Net;
using Mirage.Net.Protocol.FromServer.New;
using Mirage.Server.Maps.Pathfinding;
using Mirage.Server.Npcs;
using Mirage.Server.Players;
using Mirage.Shared.Constants;
using Mirage.Shared.Data;

namespace Mirage.Server.Maps;

public sealed class Map
{
    private readonly List<Player> _players = [];
    private readonly List<Npc> _npcs = [];
    private readonly MapInfo _info;
    private readonly ITileNavigator _navigator;

    public string Name => _info.Name;
    public string FileName { get; }

    public Map(string fileName, MapInfo info)
    {
        FileName = fileName;

        _info = info;
        _navigator = new TileNavigator(
            new BlockedProvider(this),
            new NeighborProvider(info.Width, info.Height),
            new PythagorasAlgorithm(),
            new ManhattanHeuristicAlgorithm());

        SpawnNpcs();
    }

    private void SpawnNpcs()
    {
        var npcInfo = new NpcInfo
        {
            Id = 1,
            Name = "Drunken Sea Pirate",
            AttackSay = "arrrrrrr...... ughhhh *puke*",
            Sprite = 7,
            SpawnSecs = 10,
            Behavior = NpcBehavior.AttackOnSight,
            Range = 1,
            DropChance = 1,
            DropItemId = 1,
            DropItemQuantity = 3,
            Strength = 4,
            Defense = 3,
            Speed = 5,
            Intelligence = 0
        };

        _npcs.Clear();
        _npcs.AddRange(
            new Npc(this, npcInfo, _navigator)
            {
                Id = MakeNpcId(1),
                X = 5,
                Y = 5,
            },
            new Npc(this, npcInfo, _navigator)
            {
                Id = MakeNpcId(2),
                X = 6,
                Y = 5,
            },
            new Npc(this, npcInfo, _navigator)
            {
                Id = MakeNpcId(3),
                X = 7,
                Y = 5
            });
    }

    private static int MakeNpcId(int slot)
    {
        return (slot & 0xffff) << 16;
    }

    public void Update(float deltaTime)
    {
        if (_players.Count == 0)
        {
            return;
        }

        UpdatePlayers(deltaTime);
        UpdateNpcs(deltaTime);

        // TODO: Implement: despawn items when they reach their expiry time...
    }

    private void UpdatePlayers(float deltaTime)
    {
        foreach (var player in _players)
        {
            player.Update(deltaTime);
        }
    }

    private void UpdateNpcs(float deltaTime)
    {
        foreach (var npc in _npcs)
        {
            npc.Update(deltaTime);
        }
    }

    public bool IsPassable(int x, int y)
    {
        if (!_info.IsPassable(x, y))
        {
            return false;
        }

        if (_players.Any(player => player.Character.X == x && player.Character.Y == y))
        {
            return false;
        }

        if (_npcs.Any(npc => npc.Alive && npc.X == x && npc.Y == y))
        {
            return false;
        }

        return true;
    }

    public void Add(Player player)
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
                npc.Info.Name,
                npc.Info.Sprite,
                false, AccessLevel.None,
                npc.X,
                npc.Y,
                npc.Direction,
                npc.Info.MaxHealth,
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

    public void Remove(Player player)
    {
        if (!_players.Remove(player))
        {
            return;
        }

        Send(new DestroyActorCommand(player.Id));
    }

    public void Move(Player player, Direction direction, MovementType movementType)
    {
        var targetX = player.Character.X;
        var targetY = player.Character.Y;

        switch (direction)
        {
            case Direction.Up:
                targetY--;
                break;

            case Direction.Down:
                targetY++;
                break;

            case Direction.Left:
                targetX--;
                break;

            case Direction.Right:
                targetX++;
                break;
        }

        player.Character.Direction = direction;

        if (!IsPassable(targetX, targetY))
        {
            player.Send(new SetActorPositionCommand(
                player.Id,
                player.Character.Direction,
                player.Character.X,
                player.Character.Y));

            return;
        }

        player.Character.X = targetX;
        player.Character.Y = targetY;

        Send(new ActorMoveCommand(player.Id, direction, movementType), recipient => recipient.Id != player.Id);
    }

    public void Attack(Player player)
    {
        Send(new ActorAttackCommand(player.Id), recipient => recipient.Id != player.Id);

        var (targetX, targetY) = GetAdjacentPosition(
            player.Character.Direction,
            player.Character.X,
            player.Character.Y);

        var target = _npcs.FirstOrDefault(p => p.X == targetX && p.Y == targetY);
        if (target is null || !target.Alive)
        {
            return;
        }

        // TODO:
        // if (npc.Info.Behavior != NpcBehavior.Friendly &&
        //     npc.Info.Behavior != NpcBehavior.Shopkeeper)
        // {
        //     return true;
        // }
        //
        // Tell($"You cannot attack a {npc.Info.Name}!", ColorCode.BrightBlue);

        player.Attack(target);
    }

    private static (int, int) GetAdjacentPosition(Direction direction, int x, int y)
    {
        return direction switch
        {
            Direction.Up => (x, y - 1),
            Direction.Down => (x, y + 1),
            Direction.Left => (x - 1, y),
            Direction.Right => (x + 1, y),
            _ => (x, y)
        };
    }

    public Player? GetPlayerAt(int x, int y)
    {
        return _players.FirstOrDefault(p => p.Character.X == x && p.Character.Y == y);
    }

    public Npc? GetNpcAt(int x, int y)
    {
        return _npcs.FirstOrDefault(p => p.X == x && p.Y == y);
    }

    public void LookAt(Player player, int x, int y)
    {
        if (x < 0 || x >= _info.Width || y < 0 || y >= _info.Height)
        {
            return;
        }

        // Check for a player.
        var otherPlayer = player.Map.GetPlayerAt(x, y);
        if (otherPlayer is not null && otherPlayer != player)
        {
            var levelDifference = otherPlayer.Character.Level - player.Character.Level;
            switch (levelDifference)
            {
                case >= 5:
                    player.Tell("You wouldn't stand a chance.", ColorCode.BrightRed);
                    break;

                case > 0:
                    player.Tell("This one seems to have an advantage over you.", ColorCode.Yellow);
                    break;

                case <= -5:
                    player.Tell("You could slaughter that player.", ColorCode.BrightBlue);
                    break;

                case < 0:
                    player.Tell("You would have an advantage over that player.", ColorCode.Yellow);
                    break;

                default:
                    player.Tell("This would be an even fight.", ColorCode.White);
                    break;
            }

            player.TargetPlayer = otherPlayer;
            player.TargetNpc = null;
            player.Tell($"Your target is now {otherPlayer.Character.Name}.", ColorCode.Yellow);
            return;
        }

        // TODO: Check for an item
        // var item = player.Map.GetItemAt(request.X, request.Y);
        // if (item is not null)
        // {
        //     var itemInfo = ItemRepository.Get(item.ItemId);
        //     if (itemInfo is null)
        //     {
        //         return;
        //     }
        //
        //     player.Tell($"You see a {itemInfo.Name}.", ColorCode.Yellow);
        //     return;
        // }

        // Check for an NPC
        var npc = player.Map.GetNpcAt(x, y);
        if (npc is null)
        {
            return;
        }

        player.TargetPlayer = null;
        player.TargetNpc = npc;
        player.Tell($"Your target is now a {npc.Info.Name}.", ColorCode.Yellow);
    }

    public void Send<TPacket>(TPacket packet) where TPacket : IPacket<TPacket>
    {
        var bytes = PacketSerializer.GetBytes(packet);

        foreach (var player in _players)
        {
            player.Send(bytes);
        }
    }

    public void Send<TPacket>(TPacket packet, Func<Player, bool> predicate) where TPacket : IPacket<TPacket>
    {
        var bytes = PacketSerializer.GetBytes(packet);

        foreach (var player in _players)
        {
            if (predicate(player))
            {
                player.Send(bytes);
            }
        }
    }

    public void SendMessage(string message, int color)
    {
        Send(new ChatCommand(message, color));
    }
}