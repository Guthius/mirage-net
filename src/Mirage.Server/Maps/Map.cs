using AStarNavigator;
using AStarNavigator.Algorithms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mirage.Net;
using Mirage.Net.Protocol.FromServer.New;
using Mirage.Server.Maps.Pathfinding;
using Mirage.Server.Npcs;
using Mirage.Server.Players;
using Mirage.Server.Repositories;
using Mirage.Shared.Constants;
using Mirage.Shared.Data;

namespace Mirage.Server.Maps;

public sealed class Map
{
    public const float ItemLifetimeInSeconds = 30f;

    private readonly ILogger<Map> _logger;
    private readonly IRepository<ItemInfo> _itemRepository;
    private readonly List<Player> _players = [];
    private readonly List<Npc> _npcs = [];
    private readonly List<MapItem> _items = [];
    private readonly Lock _itemPickupLock = new();
    private readonly MapInfo _info;
    private readonly ITileNavigator _navigator;
    private int _nextItemId;

    public string Name => _info.Name;
    public string FileName { get; }

    public Map(string fileName, MapInfo info, IServiceProvider services)
    {
        _logger = services.GetRequiredService<ILogger<Map>>();
        _itemRepository = services.GetRequiredService<IRepository<ItemInfo>>();
        _info = info;
        _navigator = new TileNavigator(
            new BlockedProvider(this),
            new NeighborProvider(info.Width, info.Height),
            new PythagorasAlgorithm(),
            new ManhattanHeuristicAlgorithm());

        FileName = fileName;

        SpawnNpcs();
    }

    private void SpawnNpcs()
    {
        var npcInfo = new NpcInfo
        {
            Id = "abc",
            Name = "Drunken Sea Pirate",
            AttackSay = "Arrrrrrr...... ughhhh *puke*",
            Sprite = 7,
            SpawnSecs = 10,
            Behavior = NpcBehavior.AttackOnSight,
            Range = 1,
            LootTable =
            [
                new NpcLootInfo
                {
                    ItemId = "68234080be15fb3f0f2d4cb0",
                    DropRatePercentage = 100,
                    MinQuantity = 1,
                    MaxQuantity = 4
                }
            ],
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

    public void Update(float dt)
    {
        if (_players.Count == 0)
        {
            return;
        }

        UpdatePlayers(dt);
        UpdateNpcs(dt);
        UpdateItems(dt);
    }

    private void UpdatePlayers(float dt)
    {
        foreach (var player in _players)
        {
            player.Update(dt);
        }
    }

    private void UpdateNpcs(float dt)
    {
        foreach (var npc in _npcs)
        {
            npc.Update(dt);
        }
    }

    private void UpdateItems(float dt)
    {
        for (var i = _items.Count - 1; i >= 0; i--)
        {
            var item = _items[i];
            if (!item.Expires)
            {
                continue;
            }

            item.LifeTime -= dt;

            if (item.LifeTime <= 0)
            {
                RemoveItem(item);
            }
        }
    }

    public bool IsPassable(int x, int y)
    {
        if (!_info.IsPassable(x, y))
        {
            return false;
        }

        if (_players.Exists(player => player.Character.X == x && player.Character.Y == y))
        {
            return false;
        }

        if (_npcs.Exists(npc => npc.Alive && npc.X == x && npc.Y == y))
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

        foreach (var item in _items)
        {
            player.Send(new CreateItemCommand(
                item.Id,
                item.Info.Sprite,
                item.X,
                item.Y));
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

        var target = _npcs.Find(p => p.X == targetX && p.Y == targetY);
        if (target is null || !target.Alive)
        {
            return;
        }

        if (!target.IsAttackable)
        {
            player.Tell($"You cannot attack a {target.Info.Name}!", ColorCode.BrightBlue);
            return;
        }

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

    private Player? GetPlayerAt(int x, int y)
    {
        return _players.Find(p => p.Character.X == x && p.Character.Y == y);
    }

    private Npc? GetNpcAt(int x, int y)
    {
        return _npcs.Find(p => p.X == x && p.Y == y);
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

        // Check for items
        var items = _items.FindAll(item => item.X == x && item.Y == y);
        if (items.Count > 0)
        {
            var itemNames = _items.Select(item =>
            {
                if (item.Info.Type == ItemType.Currency)
                {
                    return item.Quantity + ' ' + item.Info.Name;
                }

                if (item.Quantity <= 1)
                {
                    return "a " + item.Info.Name;
                }

                return item.Quantity + ' ' + item.Info.Name + 's';
            });

            player.Tell($"You see {string.Join(",", itemNames)}", ColorCode.Yellow);
            return;
        }

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

    private int GetNextItemId()
    {
        var id = Interlocked.Increment(ref _nextItemId);

        return id;
    }

    public void SpawnItem(string itemId, int quantity, int x, int y)
    {
        if (!_info.InBounds(x, y) || quantity <= 0)
        {
            return;
        }

        var itemInfo = _itemRepository.Get(itemId);
        if (itemInfo is null)
        {
            _logger.LogWarning("[{Map}] Attempted to drop item that does not exist: {ItemId}", FileName, itemId);
            return;
        }

        var item = new MapItem(GetNextItemId(), itemInfo, quantity, x, y, true)
        {
            LifeTime = ItemLifetimeInSeconds
        };

        _items.Add(item);

        Send(new CreateItemCommand(
            item.Id,
            item.Info.Sprite,
            item.X,
            item.Y));
    }


    public void ItemPickup(Player player)
    {
        lock (_itemPickupLock)
        {
            var item = _items.Find(x => x.X == player.Character.X && x.Y == player.Character.Y);
            if (item is null)
            {
                return;
            }

            if (!player.Inventory.Give(item.Info, item.Quantity))
            {
                player.Tell("Your inventory is full!", ColorCode.Red);
                return;
            }

            RemoveItem(item);
        }
    }

    private void RemoveItem(MapItem item)
    {
        if (_items.Remove(item))
        {
            Send(new DestroyItemCommand(item.Id));
        }
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

        foreach (var player in _players.Where(predicate))
        {
            player.Send(bytes);
        }
    }

    public void SendMessage(string message, int color)
    {
        Send(new ChatCommand(message, color));
    }
}