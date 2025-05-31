using Mirage.Client.Maps;
using Mirage.Shared.Data;
using SFML.Graphics;
using SFML.System;

namespace Mirage.Client.Entities;

public sealed class Actor(Game game, int id, int x, int y) : Drawable
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

    private static readonly Texture Texture = new("Content/Sprites.png");

    private readonly Queue<object> _actionBuffer = [];
    private bool _moving;
    private double _moveDuration;
    private double _moveTimer;
    private Vector2f _moveFrom;
    private Vector2f _moveTo;
    private bool _attacking;
    private double _attackDuration;
    private double _attackTimer;
    private readonly Sprite _sprite = new(Texture);

    public bool IsLocalPlayer { get; } = game.LocalPlayerId == id;
    public required string Name { get; set; }
    public bool DisplayName { get; set; }
    public int Level { get; set; }
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

    public void Update(float dt)
    {
        UpdateMovement(dt);
        UpdateAttack(dt);
    }

    public void UpdateMovement(double dt)
    {
        if (!_moving)
        {
            return;
        }

        _moveTimer += dt;
        if (_moveTimer >= _moveDuration)
        {
            X = (int) _moveTo.X;
            Y = (int) _moveTo.Y;
            _moving = false;

            StartNextQueuedAction();
            return;
        }

        var moveFactor = (float) (_moveTimer / _moveDuration);
        var movePosition = new Vector2f(
            LerpPrecise(_moveFrom.X, _moveTo.X, moveFactor),
            LerpPrecise(_moveFrom.Y, _moveTo.Y, moveFactor));

        X = (int) movePosition.X;
        Y = (int) movePosition.Y;
    }

    public static float LerpPrecise(float value1, float value2, float amount)
    {
        return (1 - amount) * value1 + value2 * amount;
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

    public void Draw(RenderTarget target, RenderStates states)
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

        _sprite.Texture = Texture;
        _sprite.TextureRect = new IntRect(srcX, srcY, TileWidth, TileHeight);
        _sprite.Position = new Vector2f(destX, destY);

        target.Draw(_sprite, states);
    }

    public void DrawName(RenderTarget target, RenderStates states)
    {
        if (!DisplayName)
        {
            return;
        }
        
        var text = new Text();

        text.DisplayedString = Name;
        text.FillColor = GetNameColor(IsPlayerKiller, AccessLevel);
        text.Font = Game.Font;
        text.CharacterSize = 14;

        var size = text.GetLocalBounds();
        var x = X + 16 - (int) size.Width / 2;
        var y = Y - 24;

        text.Position = new Vector2f(x, y);

        target.Draw(text, states);
    }

    private static Color GetNameColor(bool playerKiller, AccessLevel accessLevel)
    {
        if (playerKiller)
        {
            return Color.Red;
        }

        return accessLevel switch
        {
            AccessLevel.None => Color.White,
            AccessLevel.Moderator => Color.Cyan,
            AccessLevel.Mapper => Color.Cyan,
            AccessLevel.Developer => Color.Blue,
            AccessLevel.Administrator => new Color(255, 105, 180),
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

        _moveTo = new Vector2f(targetX * TileWidth, targetY * TileHeight);
        _moveFrom = new Vector2f(X, Y);
        _moveDuration = GetMoveSpeed(movementType);
        _moveTimer = 0;

        return true;
    }

    public bool TryMoveMap(Direction direction)
    {
        var (moveX, moveY) = GetDirectionVector(direction);

        var targetX = TileX + (int) moveX;
        var targetY = TileY + (int) moveY;

        return direction switch
        {
            Direction.Up => targetY == -1,
            Direction.Down => targetY == Map.HeightInTiles,
            Direction.Left => targetX == -1,
            Direction.Right => targetX == Map.WidthInTiles,
            _ => false
        };
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
                _moveTo = new Vector2f(TileX * TileWidth, TileY * TileHeight);
                _moveFrom = new Vector2f(X, Y);
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

    private static Vector2f GetDirectionVector(Direction direction)
    {
        return direction switch
        {
            Direction.Up => new Vector2f(0, -1),
            Direction.Down => new Vector2f(0, 1),
            Direction.Left => new Vector2f(-1, 0),
            Direction.Right => new Vector2f(1, 0),
            _ => new Vector2f(0, 0)
        };
    }
}