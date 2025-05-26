using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mirage.Client.Maps;
using Mirage.Shared.Data;

namespace Mirage.Client.Entities;

public sealed class Actor(Game game, int id, int x, int y)
{
    private const int TileWidth = 32;
    private const int TileHeight = 32;
    private const float WalkDuration = 0.25f;
    private const float RunDuration = 0.15f;
    private const double AttackSpeed = 0.3f;

    private sealed record MoveAction(Direction Direction, double Duration);

    private sealed record AttackAction
    {
        public static readonly AttackAction Instance = new();
    }

    private readonly Queue<object> _actionBuffer = [];
    private bool _moving;
    private double _moveDuration;
    private double _moveTimer;
    private Vector2 _moveFrom;
    private Vector2 _moveTo;
    private bool _attacking;
    private double _attackDuration;
    private double _attackTimer;

    public bool IsLocalPlayer { get; } = game.LocalPlayerId == id;
    public required string Name { get; set; }
    public required int Sprite { get; set; }
    public required bool IsPlayerKiller { get; set; }
    public required AccessLevel AccessLevel { get; set; }
    public required Map Map { get; set; }
    public required int TileX { get; set; }
    public required int TileY { get; set; }
    public int X { get; private set; } = x * TileWidth;
    public int Y { get; private set; } = y * TileHeight;
    public required Direction Direction { get; set; }
    public required int MaxHealth { get; set; }
    public required int Health { get; set; }
    public required int MaxMana { get; set; }
    public required int Mana { get; set; }
    public required int MaxStamina { get; set; }
    public required int Stamina { get; set; }
    public bool Busy => _moving || _attacking;

    public void Update(GameTime gameTime)
    {
        var deltaTime = gameTime.ElapsedGameTime.TotalSeconds;

        UpdateMovement(deltaTime);
        UpdateAttack(deltaTime);
    }

    public void UpdateMovement(double deltaTime)
    {
        if (!_moving)
        {
            return;
        }

        _moveTimer += deltaTime;
        if (_moveTimer >= _moveDuration)
        {
            X = (int) _moveTo.X;
            Y = (int) _moveTo.Y;
            _moving = false;

            StartNextQueuedAction();
            return;
        }

        var moveFactor = (float) (_moveTimer / _moveDuration);
        var movePosition = Vector2.LerpPrecise(_moveFrom, _moveTo, moveFactor);

        X = (int) movePosition.X;
        Y = (int) movePosition.Y;
    }

    private void UpdateAttack(double deltaTime)
    {
        if (!_attacking)
        {
            return;
        }

        _attackTimer += deltaTime;
        if (_attackTimer < _attackDuration)
        {
            return;
        }

        _attacking = false;

        StartNextQueuedAction();
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var anim = 0;

        if (_moving)
        {
            var moveFactor = (float) (_moveTimer / _moveDuration);
            if (moveFactor > 0.5f)
            {
                anim = 1;
            }
        }

        if (_attacking)
        {
            var attackFactor = (float) (_attackTimer / _attackDuration);
            if (attackFactor < 0.8f)
            {
                anim = 2;
            }
        }

        var destX = X;
        var destY = Y - 4;
        if (destY < 0)
        {
            destY = 0;
        }

        var srcX = ((int) Direction * 3 + anim) * TileWidth;
        var srcY = Sprite * TileHeight;

        spriteBatch.Draw(Textures.Sprites,
            new Vector2(destX, destY),
            new Rectangle(srcX, srcY, TileWidth, TileHeight),
            Color.White);
    }

    public void DrawName(SpriteBatch spriteBatch)
    {
        var color = GetNameColor(IsPlayerKiller, AccessLevel);
        var width = (int) Textures.Font.MeasureString(Name).X;
        var x = X + 16 - width / 2;
        var y = Y - 24;

        spriteBatch.DrawString(Textures.Font, Name, new Vector2(x, y), color);
    }

    private static Color GetNameColor(bool playerKiller, AccessLevel accessLevel)
    {
        if (playerKiller)
        {
            return Color.OrangeRed;
        }

        return accessLevel switch
        {
            AccessLevel.None => Color.White,
            AccessLevel.Moderator => Color.DimGray,
            AccessLevel.Mapper => Color.Cyan,
            AccessLevel.Developer => Color.Blue,
            AccessLevel.Administrator => Color.HotPink,
            _ => Color.White
        };
    }

    public bool TryMove(Direction direction, MovementType movementType)
    {
        if (Busy)
        {
            return false;
        }

        var (moveX, moveY) = GetDirectionVector(direction);

        var targetX = TileX + (int) moveX;
        var targetY = TileY + (int) moveY;

        var passable = Map.IsPassable(targetX, targetY);
        if (!passable)
        {
            return false;
        }

        if (Interlocked.Exchange(ref _moving, true))
        {
            return false;
        }

        Direction = direction;
        TileX = targetX;
        TileY = targetY;

        _moveTo = new Vector2(targetX * TileWidth, targetY * TileHeight);
        _moveFrom = new Vector2(X, Y);
        _moveDuration = GetMoveSpeed(movementType);
        _moveTimer = 0;

        return true;
    }

    public bool TryAttack()
    {
        if (Busy)
        {
            return false;
        }

        if (Interlocked.Exchange(ref _attacking, true))
        {
            return false;
        }

        _attackDuration = AttackSpeed;
        _attackTimer = 0;

        return true;
    }

    public void QueueMove(Direction direction, MovementType movementType)
    {
        if (IsLocalPlayer)
        {
            return;
        }

        var moveDuration = GetMoveSpeed(movementType);
        var moveInfo = new MoveAction(direction, moveDuration);

        _actionBuffer.Enqueue(moveInfo);

        if (_moving || _attacking)
        {
            return;
        }

        StartNextQueuedAction();
    }

    public void SetPosition(Direction direction, int x, int y)
    {
        _actionBuffer.Clear();
        _moving = false;
        _attacking = false;

        Direction = direction;
        TileX = x;
        TileY = y;
        X = x * TileWidth;
        Y = y * TileHeight;
    }

    public void SetDirection(Direction direction)
    {
        Direction = direction;
    }
    
    public void QueueAttack()
    {
        if (IsLocalPlayer)
        {
            return;
        }

        _actionBuffer.Enqueue(AttackAction.Instance);

        if (_moving || _attacking)
        {
            return;
        }

        StartNextQueuedAction();
    }

    private void StartNextQueuedAction()
    {
        if (IsLocalPlayer || !_actionBuffer.TryDequeue(out var action))
        {
            return;
        }

        _moving = _attacking = false;

        switch (action)
        {
            case MoveAction move:
                var directionVector = GetDirectionVector(move.Direction);

                Direction = move.Direction;
                TileX += (int) directionVector.X;
                TileY += (int) directionVector.Y;

                _moving = true;
                _moveTo = new Vector2(TileX * TileWidth, TileY * TileHeight);
                _moveFrom = new Vector2(X, Y);
                _moveDuration = move.Duration;
                _moveTimer = 0;
                return;

            case AttackAction:
                _attacking = true;
                _attackDuration = AttackSpeed;
                _attackTimer = 0;
                return;
        }
    }

    private static float GetMoveSpeed(MovementType movementType)
    {
        return movementType switch
        {
            MovementType.Walking => WalkDuration,
            MovementType.Running => RunDuration,
            _ => 0
        };
    }

    private static Vector2 GetDirectionVector(Direction direction)
    {
        return direction switch
        {
            Direction.Up => -Vector2.UnitY,
            Direction.Down => Vector2.UnitY,
            Direction.Left => -Vector2.UnitX,
            Direction.Right => Vector2.UnitX,
            _ => Vector2.Zero
        };
    }
}