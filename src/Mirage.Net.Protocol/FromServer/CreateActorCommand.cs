using Mirage.Shared.Data;

namespace Mirage.Net.Protocol.FromServer;

public sealed record CreateActorCommand(
    int ActorId,
    string Name,
    int Sprite,
    bool IsPlayerKiller,
    AccessLevel AccessLevel,
    int X,
    int Y,
    Direction Direction,
    int MaxHealth,
    int Health,
    int MaxMana,
    int Mana,
    int MaxStamina,
    int Stamina) : IPacket<CreateActorCommand>
{
    public static string PacketId => nameof(CreateActorCommand);

    public static CreateActorCommand ReadFrom(PacketReader reader)
    {
        return new CreateActorCommand(
            ActorId: reader.ReadInt32(),
            Name: reader.ReadString(),
            Sprite: reader.ReadInt32(),
            IsPlayerKiller: reader.ReadBoolean(),
            AccessLevel: reader.ReadEnum<AccessLevel>(),
            X: reader.ReadInt32(),
            Y: reader.ReadInt32(),
            Direction: reader.ReadEnum<Direction>(),
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
        writer.WriteString(Name);
        writer.WriteInt32(Sprite);
        writer.WriteBoolean(IsPlayerKiller);
        writer.WriteEnum(AccessLevel);
        writer.WriteInt32(X);
        writer.WriteInt32(Y);
        writer.WriteEnum(Direction);
        writer.WriteInt32(MaxHealth);
        writer.WriteInt32(Health);
        writer.WriteInt32(MaxMana);
        writer.WriteInt32(Mana);
        writer.WriteInt32(MaxStamina);
        writer.WriteInt32(Stamina);
    }
}