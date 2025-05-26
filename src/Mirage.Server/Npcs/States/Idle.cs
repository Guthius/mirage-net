using Mirage.Shared.Data;

namespace Mirage.Server.Npcs.States;

public sealed class Idle() : State(updateRateInSeconds: 4f)
{
    protected override IState OnUpdate(Npc npc, float dt)
    {
        npc.NavigateTo(RandomDirection(), MovementType.Walking);

        return this;
    }

    private static Direction RandomDirection()
    {
        return (Direction) Random.Shared.Next(0, 4);
    }
}