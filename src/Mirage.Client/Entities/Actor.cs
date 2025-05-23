using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mirage.Shared.Data;

namespace Mirage.Client.Entities;

public sealed class Actor(GameState gameState, int id, int x, int y)
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

    public bool IsLocalPlayer { get; } = gameState.LocalPlayerId == id;
    public required string Name { get; set; }
    public required int Sprite { get; set; }
    public required bool IsPlayerKiller { get; set; }
    public required AccessLevel AccessLevel { get; set; }
    public required Map Map { get; set; }
    public required int X { get; set; }
    public required int Y { get; set; }
    public required Direction Direction { get; set; }
    public required int MaxHealth { get; set; }
    public required int Health { get; set; }
    public required int MaxMana { get; set; }
    public required int Mana { get; set; }
    public required int MaxStamina { get; set; }
    public required int Stamina { get; set; }
    public bool Busy => _moving || _attacking;

    private int _currentX = x * TileWidth;
    private int _currentY = y * TileHeight;
    private readonly Queue<object> _actionBuffer = [];
    private bool _moving;
    private double _moveDuration;
    private double _moveTimer;
    private Vector2 _moveFrom;
    private Vector2 _moveTo;
    private bool _attacking;
    private double _attackDuration;
    private double _attackTimer;

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
            _currentX = (int) _moveTo.X;
            _currentY = (int) _moveTo.Y;
            _moving = false;

            StartNextQueuedAction();
            return;
        }

        var moveFactor = (float) (_moveTimer / _moveDuration);
        var movePosition = Vector2.LerpPrecise(_moveFrom, _moveTo, moveFactor);

        _currentX = (int) movePosition.X;
        _currentY = (int) movePosition.Y;
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

        var destX = _currentX;
        var destY = _currentY - 4;
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
        var x = _currentX + 16 - width / 2;
        var y = _currentY - 24;

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

        var targetX = X + (int) moveX;
        var targetY = Y + (int) moveY;

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
        X = targetX;
        Y = targetY;

        _moveTo = new Vector2(targetX * TileWidth, targetY * TileHeight);
        _moveFrom = new Vector2(_currentX, _currentY);
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
                X += (int) directionVector.X;
                Y += (int) directionVector.Y;

                _moving = true;
                _moveTo = new Vector2(X * TileWidth, Y * TileHeight);
                _moveFrom = new Vector2(_currentX, _currentY);
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