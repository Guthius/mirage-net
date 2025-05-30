using Mirage.Server.Players;

namespace Mirage.Server.Npcs.States;

public sealed class Attack(Player target) : State(updateRateInSeconds: 1f)
{
    public override IState Update(Npc npc, float dt)
    {
        if (!npc.IsAdjacentTo(target.Character.X, target.Character.Y))
        {
            return new Hunt(target);
        }

        return base.Update(npc, dt);
    }

    protected override IState OnUpdate(Npc npc, float dt)
    {
        if (npc.Attack(target))
        {
            return new Idle();
        }

        return this;
    }
}