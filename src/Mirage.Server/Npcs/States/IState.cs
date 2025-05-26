namespace Mirage.Server.Npcs.States;

public interface IState
{
    public IState Update(Npc npc, float dt);
}