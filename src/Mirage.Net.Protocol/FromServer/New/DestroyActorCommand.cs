namespace Mirage.Net.Protocol.FromServer.New;

/// <summary>
/// Tells the client to destroy the actor object with the specified ID.
/// </summary>
/// <param name="ActorId">The ID of the actor object to destroy.</param>
public sealed record DestroyActorCommand(int ActorId) : IPacket<DestroyActorCommand>
{
    public static string PacketId => nameof(DestroyActorCommand);

    public static DestroyActorCommand ReadFrom(PacketReader reader)
    {
        return new DestroyActorCommand(ActorId: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(ActorId);
    }
}