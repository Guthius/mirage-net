using AStarNavigator;
using Mirage.Net.Protocol.FromServer;
using Mirage.Server.Net;
using Mirage.Shared.Constants;
using Mirage.Shared.Data;

namespace Mirage.Server.Game;

public sealed class GameNpc(int slot, GameMap map, NpcInfo? npcInfo)
{
    private static readonly NpcInfo EmptyNpcInfo = new();

    private int _regenTimer;

    public int Slot { get; } = slot;
    public GameMap Map { get; } = map;
    public NpcInfo Info { get; private set; } = npcInfo ?? EmptyNpcInfo;
    public int AttackTimer { get; set; }
    public int SpawnWait { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public Direction Direction { get; set; }
    public int Target { get; set; }

    public int HP
    {
        get;
        set => field = Math.Clamp(value, 0, Info.MaxHP);
    }

    public int MP
    {
        get;
        set => field = Math.Clamp(value, 0, Info.MaxMP);
    }

    public int SP
    {
        get;
        set => field = Math.Clamp(value, 0, Info.MaxSP);
    }

    public bool Alive => HP > 0;

    public void Update(int tickCount)
    {
        if (!Alive)
        {
            if (tickCount > SpawnWait + Info.SpawnSecs * 1000)
            {
                Respawn();
            }

            return;
        }

        if (Target == 0 && Info.Behavior is NpcBehavior.AttackOnSight or NpcBehavior.Guard)
        {
            TryFindTarget();
        }

        if (Info.Behavior != NpcBehavior.Shopkeeper)
        {
            TryWalk();
        }

        TryAttackTarget();
        TryRegen();
    }

    private void TryFindTarget()
    {
        foreach (var player in GameState.OnlinePlayers())
        {
            if (player.Character.MapId != Map.Info.Id ||
                player.Character.AccessLevel > AccessLevel.Moderator)
            {
                continue;
            }

            var distX = Math.Abs(X - player.Character.X);
            var distY = Math.Abs(Y - player.Character.Y);

            if (distX > Info.Range || distY > Info.Range)
            {
                continue;
            }

            if (Info.Behavior != NpcBehavior.AttackOnSight && !player.Character.PlayerKiller)
            {
                continue;
            }

            if (!string.IsNullOrWhiteSpace(Info.AttackSay))
            {
                player.Tell($"{Info.Name} says, '{Info.AttackSay}' to you.", ColorCode.SayColor);
            }

            Target = player.Id;
        }
    }

    private void TryWalk()
    {
        if (Target == 0)
        {
            var movementDecision = Random.Shared.Next(4);
            if (movementDecision != 1)
            {
                return;
            }

            var direction = (Direction) Random.Shared.Next(4);
            if (CanMove(direction))
            {
                Move(direction, MovementType.Walking);
            }

            return;
        }

        var targetPlayer = GameState.GetPlayer(Target);
        if (targetPlayer is null || targetPlayer.Character.MapId != Map.Info.Id)
        {
            Target = 0;
            return;
        }

        var distX = Math.Abs(X - targetPlayer.Character.X);
        var distY = Math.Abs(Y - targetPlayer.Character.Y);
        if (distX > Info.Range || distY > Info.Range)
        {
            Target = 0;
            return;
        }

        if (distX + distY <= 1)
        {
            
            return;
        }

        MoveTo(targetPlayer.Character.X, targetPlayer.Character.Y);
    }

    private bool CanMove(Direction direction)
    {
        return direction switch
        {
            Direction.Up => IsTilePassable(this, X, Y - 1),
            Direction.Down => IsTilePassable(this, X, Y + 1),
            Direction.Left => IsTilePassable(this, X - 1, Y),
            Direction.Right => IsTilePassable(this, X + 1, Y),
            _ => false
        };

        static bool IsTilePassable(GameNpc npc, int x, int y)
        {
            if (x < 0 || y < 0 || x > Limits.MaxMapWidth || y > Limits.MaxMapHeight)
            {
                return false;
            }

            var tileType = npc.Map.Info.Tiles[x, y].Type;
            if (tileType != TileType.Walkable && tileType != TileType.Item)
            {
                return false;
            }

            if (GameState.OnlinePlayers().Any(player =>
                    player.Character.MapId == npc.Map.Info.Id &&
                    player.Character.X == x &&
                    player.Character.Y == y))
            {
                return false;
            }

            return npc.Map.AliveNpcs().All(otherNpc => otherNpc == npc || otherNpc.X != x || otherNpc.Y != y);
        }
    }

    private void MoveTo(int x, int y)
    {
        var path = Map.Navigator.Navigate(new Tile(X, Y), new Tile(x, y)).ToList();
        if (path.Count == 0)
        {
            return;
        }

        Move(GetDirection(X, Y, (int) path[0].X, (int) path[0].Y), MovementType.Walking);

        static Direction GetDirection(int sx, int sy, int dx, int dy)
        {
            if (Math.Abs(dx - sx) >= Math.Abs(dy - sy))
            {
                return dx > sx ? Direction.Right : Direction.Left;
            }

            return dy > sy ? Direction.Down : Direction.Up;
        }
    }

    private void Move(Direction direction, MovementType movementType)
    {
        Direction = direction;

        switch (direction)
        {
            case Direction.Up:
                Y -= 1;
                break;

            case Direction.Down:
                Y += 1;
                break;

            case Direction.Left:
                X -= 1;
                break;

            case Direction.Right:
                X += 1;
                break;
        }

        Map.Send(new NpcMove(Slot, X, Y, Direction, movementType));
    }

    public void Kill()
    {
        SpawnWait = Environment.TickCount;
        HP = 0;

        var dropChance = Random.Shared.Next(Info.DropChance) + 1;
        if (dropChance == 1)
        {
            Map.SpawnItem(X, Y, Info.DropItemId, Info.DropItemQuantity);
        }

        Map.Send(new NpcDead(Slot));
    }

    private void TryAttackTarget()
    {
        if (Target <= 0 || !Alive)
        {
            return;
        }

        if (Environment.TickCount < AttackTimer + 1000)
        {
            return;
        }

        var targetPlayer = GameState.GetPlayer(Target);
        if (targetPlayer is null || targetPlayer.Character.MapId != Map.Info.Id)
        {
            Target = 0;
            return;
        }

        if (targetPlayer.GettingMap)
        {
            return;
        }

        AttackTimer = Environment.TickCount;

        var distance = Math.Abs(targetPlayer.Character.X - X) + Math.Abs(targetPlayer.Character.Y - Y);
        if (distance != 1)
        {
            return;
        }

        if (targetPlayer.TryBlockHit(out var shieldInfo))
        {
            targetPlayer.Tell($"Your {shieldInfo.Name} blocks the {Info.Name}'s hit!", ColorCode.BrightCyan);
            return;
        }

        var damage = Info.Strength - targetPlayer.CalculateProtection();
        if (damage <= 0)
        {
            targetPlayer.Tell($"The {Info.Name}'s hit didn't even phase you!", ColorCode.BrightBlue);
            return;
        }

        AttackPlayer(targetPlayer, damage);
    }

    private void TryRegen()
    {
        if (Environment.TickCount <= _regenTimer + 10000)
        {
            return;
        }

        if (HP <= 0)
        {
            return;
        }

        HP += Info.HPRegen;

        _regenTimer = Environment.TickCount;
    }

    public void Reset(NpcInfo? newNpcInfo = null)
    {
        Info = newNpcInfo ?? Info;
        X = Y = 0;
        Target = 0;
        HP = MP = SP = 0;
        Direction = Direction.Up;
    }

    public void Respawn()
    {
        var spawned = false;
        var spawnX = 0;
        var spawnY = 0;

        for (var i = 1; i <= 100; i++)
        {
            var x = Random.Shared.Next(0, Limits.MaxMapWidth + 1);
            var y = Random.Shared.Next(0, Limits.MaxMapHeight + 1);

            if (Map.Info.Tiles[x, y].Type != TileType.Walkable)
            {
                continue;
            }

            spawnX = x;
            spawnY = y;
            spawned = true;

            break;
        }

        if (!spawned)
        {
            for (var y = 0; y <= Limits.MaxMapHeight; y++)
            {
                for (var x = 0; x <= Limits.MaxMapWidth; x++)
                {
                    if (Map.Info.Tiles[x, y].Type != TileType.Walkable)
                    {
                        continue;
                    }

                    spawnX = x;
                    spawnY = y;
                    spawned = true;

                    break;
                }
            }
        }

        if (!spawned)
        {
            return;
        }

        X = spawnX;
        Y = spawnY;
        Target = 0;
        HP = Info.MaxHP;
        MP = Info.MaxMP;
        SP = Info.MaxSP;
        Direction = (Direction) Random.Shared.Next(0, 4);

        Map.Send(new SpawnNpc(Slot, Info.Id, X, Y, Direction));
    }

    public void AttackPlayer(GamePlayer victim, int damage)
    {
        if (damage < 0)
        {
            return;
        }

        Map.Send(new NpcAttack(Slot));

        if (damage < victim.Character.HP)
        {
            victim.Character.HP -= damage;
            victim.SendHP();
            victim.Tell($"A {Info.Name} hit you for {damage} hit points.", ColorCode.BrightRed);

            return;
        }

        victim.Tell($"A {Info.Name} hit you for {damage} hit points.", ColorCode.BrightRed);

        Network.SendGlobalMessage($"{victim.Character.Name} has been killed by a {Info.Name}.", ColorCode.BrightRed);

        victim.DropItem(victim.Character.WeaponSlot);
        victim.DropItem(victim.Character.ArmorSlot);
        victim.DropItem(victim.Character.HelmetSlot);
        victim.DropItem(victim.Character.ShieldSlot);

        var exp = Math.Max(0, victim.Character.Exp / 3);
        if (exp == 0)
        {
            victim.Tell("You lost no experience points.", ColorCode.BrightRed);
        }
        else
        {
            victim.Character.Exp -= exp;
            victim.Tell($"You lost {exp} experience points.", ColorCode.BrightRed);
        }

        victim.WarpTo(Options.StartMapId, Options.StartX, Options.StartY);
        victim.Character.HP = victim.Character.MaxHP;
        victim.Character.MP = victim.Character.MaxMP;
        victim.Character.SP = victim.Character.MaxSP;
        victim.SendHP();
        victim.SendMP();
        victim.SendSP();

        Target = 0;

        if (!victim.Character.PlayerKiller)
        {
            return;
        }

        victim.Character.PlayerKiller = false;
        victim.SendPlayerData();
    }
}