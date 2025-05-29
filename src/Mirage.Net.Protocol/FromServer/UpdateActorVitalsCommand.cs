namespace Mirage.Net.Protocol.FromServer;

public sealed record UpdateActorVitalsCommand(int ActorId, int MaxHealth, int Health, int MaxMana, int Mana, int MaxStamina, int Stamina) : IPacket<UpdateActorVitalsCommand>
{
    public static string PacketId => nameof(UpdateActorVitalsCommand);

    public static UpdateActorVitalsCommand ReadFrom(PacketReader reader)
    {
        return new UpdateActorVitalsCommand(
            ActorId: reader.ReadInt32(),
            MaxHealth: reader.ReadInt32(),
            Health: reader.ReadInt32(),
            MaxMana: reader.ReadInt32(),
            Mana: reader.ReadInt32(),
            MaxStamina: reader.ReadInt32(),
            Stamina: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(ActorId);
        writer.WriteInt32(MaxHealth);
        writer.WriteInt32(Health);
        writer.WriteInt32(MaxMana);
        writer.WriteInt32(Mana);
        writer.WriteInt32(MaxStamina);
        writer.WriteInt32(Stamina);
    }
}