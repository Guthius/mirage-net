using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mirage.Client.Game;
using Mirage.Game.Data;

namespace Mirage.Client.Entities;

public sealed class Player(GameState gameState, int id, string name, string jobId, int sprite, bool isPlayerKiller, AccessLevel accessLevel, Map map, int x, int y, Direction direction) : IEntity
{
    private const int TileWidth = 32;
    private const int TileHeight = 32;

    private sealed record MoveInfo(Direction Direction, double Duration);

    private static class MovementSpeed
    {
        public const float Walking = 0.25f;
        public const float Running = 0.15f;
    }

    public int Id { get; } = id;
    public bool IsLocalPlayer { get; } = gameState.LocalPlayerId == id;
    public string Name { get; set; } = name;
    public string JobId { get; set; } = jobId;
    public int Sprite { get; set; } = sprite;
    public bool IsPlayerKiller { get; set; } = isPlayerKiller;
    public AccessLevel AccessLevel { get; set; } = accessLevel;
    public Map Map { get; set; } = map;
    public int X { get; set; } = x;
    public int Y { get; set; } = y;
    public Direction Direction { get; set; } = direction;
    public bool Moving => _moving;

    private int _currentX = x * TileWidth;
    private int _currentY = y * TileHeight;
    private readonly Queue<MoveInfo> _moveBuffer = [];
    private bool _moving;
    private double _moveDuration;
    private Vector2 _moveFrom;
    private Vector2 _moveTo;
    private double _moveTime;
    private bool _attacking;
    private int _attackTimer;

    public void Update(GameTime gameTime)
    {
        UpdateMovement(gameTime);
    }

    public void UpdateMovement(GameTime gameTime)
    {
        if (!_moving)
        {
            return;
        }

        _moveTime += gameTime.ElapsedGameTime.TotalSeconds;
        if (_moveTime >= _moveDuration)
        {
            _moving = false;
            _currentX = (int) _moveTo.X;
            _currentY = (int) _moveTo.Y;
            return;
        }

        var moveFactor = (float) (_moveTime / _moveDuration);
        var movePosition = Vector2.LerpPrecise(_moveFrom, _moveTo, moveFactor);

        _currentX = (int) movePosition.X;
        _currentY = (int) movePosition.Y;
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        var anim = 0;

        if (_moving)
        {
            var moveFactor = (float) (_moveTime / _moveDuration);
            if (moveFactor > 0.5f)
            {
                anim = 1;
            }
        }

        // TODO: Use attack frame (2) when attacking...

        if (_attackTimer + 1000 < Environment.TickCount)
        {
            _attacking = false;
            _attackTimer = 0;
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
        var width = Textures.Font.MeasureString(Name).X;
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
            AccessLevel.Player => Color.White,
            AccessLevel.Moderator => Color.DimGray,
            AccessLevel.Mapper => Color.Cyan,
            AccessLevel.Developer => Color.Blue,
            AccessLevel.Administrator => Color.HotPink,
            _ => Color.White
        };
    }

    public bool TryMoveNow(Direction direction, MovementType movementType)
    {
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
        _moveDuration = GetMoveDuration(movementType);
        _moveTime = 0;

        return true;
    }

    public void Move(Direction direction, MovementType movementType)
    {
        if (IsLocalPlayer)
        {
            return;
        }

        var moveDuration = GetMoveDuration(movementType);
        var moveInfo = new MoveInfo(direction, moveDuration);

        _moveBuffer.Enqueue(moveInfo);

        var moving = Interlocked.Exchange(ref _moving, true);
        if (moving)
        {
            return;
        }

        MoveNext();
    }

    private void MoveNext()
    {
        if (!_moveBuffer.TryDequeue(out var move))
        {
            _moving = false;
            return;
        }

        var directionVector = GetDirectionVector(move.Direction);

        Direction = move.Direction;
        X += (int) directionVector.X;
        Y += (int) directionVector.Y;

        _moveTo = new Vector2(X * TileWidth, Y * TileHeight);
        _moveFrom = new Vector2(_currentX, _currentY);
        _moveDuration = move.Duration;
        _moveTime = 0;
    }

    private static float GetMoveDuration(MovementType movementType)
    {
        return movementType switch
        {
            MovementType.Walking => MovementSpeed.Walking,
            MovementType.Running => MovementSpeed.Running,
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