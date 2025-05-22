using Mirage.Game.Data;

namespace Mirage.Net.Protocol.FromServer.New;

/// <summary>
/// Tells the client to create a new player object with the specified attributes.
/// </summary>
/// <param name="PlayerId">The ID of the player object.</param>
/// <param name="Name">The name of the player.</param>
/// <param name="JobId">The job ID of the player.</param>
/// <param name="Sprite">The player sprite.</param>
/// <param name="IsPlayerKiller">Whether the player is a player killer.</param>
/// <param name="AccessLevel">The access level of the player.</param>
/// <param name="X">The X position of the player.</param>
/// <param name="Y">The Y position of the player.</param>
/// <param name="Direction">The direction the player is facing.</param>
public sealed record CreatePlayerCommand(int PlayerId, string Name, string JobId, int Sprite, bool IsPlayerKiller, AccessLevel AccessLevel, int X, int Y, Direction Direction) : IPacket<CreatePlayerCommand>
{
    public static string PacketId => nameof(CreatePlayerCommand);

    public static CreatePlayerCommand ReadFrom(PacketReader reader)
    {
        return new CreatePlayerCommand(
            PlayerId: reader.ReadInt32(),
            Name: reader.ReadString(),
            JobId: reader.ReadString(),
            Sprite: reader.ReadInt32(),
            IsPlayerKiller: reader.ReadBoolean(),
            AccessLevel: reader.ReadEnum<AccessLevel>(),
            X: reader.ReadInt32(),
            Y: reader.ReadInt32(),
            Direction: reader.ReadEnum<Direction>());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(PlayerId);
        writer.WriteString(Name);
        writer.WriteString(JobId);
        writer.WriteInt32(Sprite);
        writer.WriteBoolean(IsPlayerKiller);
        writer.WriteEnum(AccessLevel);
        writer.WriteInt32(X);
        writer.WriteInt32(Y);
        writer.WriteEnum(Direction);
    }
}