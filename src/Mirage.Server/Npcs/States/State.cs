namespace Mirage.Server.Npcs.States;

public abstract class State(float updateRateInSeconds) : IState
{
    private float _elapsed;
    
    public virtual IState Update(Npc npc, float dt)
    {
        _elapsed += dt;
        if (_elapsed < updateRateInSeconds)
        {
            return this;
        }

        _elapsed -= updateRateInSeconds;
        
        return OnUpdate(npc, dt);
    }
    
    protected abstract IState OnUpdate(Npc npc, float dt);
}