using AStarNavigator;
using Mirage.Net.Protocol.FromServer.New;
using Mirage.Server.Maps;
using Mirage.Server.Npcs.States;
using Mirage.Server.Players;
using Mirage.Shared.Constants;
using Mirage.Shared.Data;

namespace Mirage.Server.Npcs;

public sealed class Npc(Map map, NpcInfo info, ITileNavigator navigator)
{
    private const float RegenIntervalInSeconds = 10f;

    private float _regenTimer;
    private IState _state = new Idle();

    public Map Map { get; } = map;
    public NpcInfo Info { get; } = info;
    public int Id { get; init; }
    public int X { get; set; }
    public int Y { get; set; }
    public Direction Direction { get; private set; } = Direction.Down;
    public int Health { get; private set; } = info.MaxHealth;
    public bool Alive => _state is not Dead;
    public bool IsAttackable => Info.Behavior != NpcBehavior.Friendly && Info.Behavior != NpcBehavior.Shopkeeper;

    public void Update(float deltaTime)
    {
        UpdateHealth(deltaTime);

        _state = _state.Update(this, deltaTime);
    }

    private void UpdateHealth(float deltaTime)
    {
        _regenTimer += deltaTime;
        if (_regenTimer < RegenIntervalInSeconds)
        {
            return;
        }

        _regenTimer -= RegenIntervalInSeconds;

        Health = Math.Clamp(Health + Info.HealthRegen, 0, Info.MaxHealth);
    }

    public bool NavigateTo(Direction direction, MovementType movementType)
    {
        var targetX = X;
        var targetY = Y;

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

        if (targetX == X && targetY == Y)
        {
            return false;
        }

        if (targetX < 0 || targetX > 30 || targetY < 0 || targetY > 30)
        {
            return false;
        }

        return NavigateTo(targetX, targetY, movementType);
    }

    public bool NavigateTo(int x, int y, MovementType movementType)
    {
        var path = navigator.Navigate(new Tile(X, Y), new Tile(x, y))?.ToList();
        if (path is not {Count: > 0})
        {
            return false;
        }

        var targetX = (int) path[0].X;
        var targetY = (int) path[0].Y;

        Direction = GetDirection(X, Y, targetX, targetY);
        X = targetX;
        Y = targetY;

        Map.Send(new ActorMoveCommand(Id, Direction, movementType));

        return true;

        static Direction GetDirection(int sx, int sy, int dx, int dy)
        {
            if (Math.Abs(dx - sx) >= Math.Abs(dy - sy))
            {
                return dx > sx ? Direction.Right : Direction.Left;
            }

            return dy > sy ? Direction.Down : Direction.Up;
        }
    }

    public void Hurt(Player attacker, int damage)
    {
        if (damage < Health)
        {
            Health -= damage;

            attacker.Tell($"You hit a {Info.Name} for {damage} hit points.", ColorCode.White);

            if (_state is not Idle)
            {
                return;
            }

            attacker.Tell($"A {Info.Name} says, '{Info.AttackSay}' to you.", ColorCode.SayColor);

            _state = new Hunt(attacker);

            // TODO: Implement guard AI. When a guard is attacked, all other guards should target the attacker...

            return;
        }

        var experience = Math.Min(1, Info.Strength * Info.Defense * 2);

        attacker.Tell($"You hit a {Info.Name} for {damage} hit points, killing it.", ColorCode.BrightRed);
        attacker.GrantExperience(experience);

        Kill();
    }

    public bool Attack(Player target)
    {
        if (target.TryBlockHit(out var shieldInfo))
        {
            target.Tell($"Your {shieldInfo.Name} blocks the {Info.Name}'s hit!", ColorCode.BrightCyan);
            return false;
        }

        var damage = Info.Strength - target.CalculateProtection();
        if (damage <= 0)
        {
            target.Tell($"The {Info.Name}'s hit didn't even phase you!", ColorCode.BrightBlue);
            return false;
        }

        target.Tell($"A {Info.Name} hit you for {damage} hit points.", ColorCode.BrightRed);

        if (damage < target.Character.HP)
        {
            target.Character.HP -= damage;
            target.SendVitals();
            return false;
        }

        Map.Send(new ChatCommand($"{target.Character.Name} has been killed by a {Info.Name}.", ColorCode.BrightRed));

        target.Kill(Math.Max(0, target.Character.Exp / 10));

        if (!target.Character.PlayerKiller)
        {
            return true;
        }

        target.Character.PlayerKiller = false;
        // TODO: target.SendPlayerData(); - Need to tell map the player is no longer a pker

        return true;
    }

    private void Kill()
    {
        _state = new Dead(Info.SpawnSecs);

        Map.Send(new DestroyActorCommand(Id));

        DropLoot();
    }

    private void DropLoot()
    {
        foreach (var lootInfo in Info.LootTable)
        {
            var roll = Random.Shared.NextSingle() * 100.0f;
            if (roll > lootInfo.DropRatePercentage)
            {
                continue;
            }

            var quantity = Random.Shared.Next(lootInfo.MinQuantity, lootInfo.MaxQuantity + 1);
            if (quantity <= 0)
            {
                continue;
            }

            Map.SpawnItem(lootInfo.ItemId, quantity, X, Y);
        }
    }

    public void Respawn()
    {
        Health = Info.MaxHealth;

        Map.Send(new CreateActorCommand(
            Id, Info.Name, Info.Sprite,
            false, AccessLevel.None,
            X, Y, Direction,
            Info.MaxHealth,
            Health,
            0, 0, 0, 0));
    }

    public bool IsAdjacentTo(int x, int y)
    {
        var dx = Math.Abs(x - X);
        var dy = Math.Abs(y - Y);

        return (dx == 1 && dy == 0) ||
               (dx == 0 && dy == 1);
    }
}