using Mirage.Server.Players;
using Mirage.Shared.Data;

namespace Mirage.Server.Npcs.States;

public sealed class Hunt(Player target) : State(updateRateInSeconds: 0.5f)
{
    private static readonly int[,] Directions = {{1, 0}, {-1, 0}, {0, 1}, {0, -1}};

    protected override IState OnUpdate(Npc npc, float dt)
    {
        if (npc.IsAdjacentTo(target.Character.X, target.Character.Y))
        {
            return new Attack(target);
        }

        for (var i = 0; i < 4; i++)
        {
            var targetX = target.Character.X + Directions[i, 0];
            var targetY = target.Character.Y + Directions[i, 1];
            
            if (npc.NavigateTo(targetX, targetY, MovementType.Walking))
            {
                break;
            }
        }

        return this;
    }
}