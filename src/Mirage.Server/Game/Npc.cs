using Mirage.Net.Protocol.FromServer.New;
using Mirage.Shared.Data;

namespace Mirage.Server.Game;

public sealed class Npc(Map map)
{
    private const float MoveIntervalInSeconds = 3f;
    
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Sprite { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public Direction Direction { get; set; }
    public int MaxHealth { get; set; }
    public int Health { get; set; }
    public bool Alive { get; set; }
    
    private float _moveTimer = Random.Shared.NextSingle() * (MoveIntervalInSeconds / 2);
    
    public void Update(float dt)
    {
        _moveTimer += dt;
        if (_moveTimer < MoveIntervalInSeconds)
        {
            return;
        }

        Move();

        _moveTimer -= MoveIntervalInSeconds;
    }

    private void Move()
    {
        var direction = (Direction) Random.Shared.Next(0, 4);

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
            return;
        }

        if (targetX < 0 || targetX > 30 || targetY < 0 || targetY > 30)
        {
            return;
        }

        X = targetX;
        Y = targetY;

        map.Send(new ActorMoveCommand(Id, direction, MovementType.Walking));
    }
}