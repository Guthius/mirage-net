namespace Mirage.Server.Npcs.States;

public sealed class Dead(float respawnDelay) : State(updateRateInSeconds: respawnDelay)
{
    protected override IState OnUpdate(Npc npc, float dt)
    {
        npc.Respawn();

        return new Idle();
    }
}