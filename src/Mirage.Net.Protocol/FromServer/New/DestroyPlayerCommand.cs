namespace Mirage.Net.Protocol.FromServer.New;

/// <summary>
/// Tells the client to destroy the player object with the specified ID.
/// </summary>
/// <param name="PlayerId">The ID of the player object to destroy.</param>
public sealed record DestroyPlayerCommand(int PlayerId) : IPacket<DestroyPlayerCommand>
{
    public static string PacketId => nameof(DestroyPlayerCommand);

    public static DestroyPlayerCommand ReadFrom(PacketReader reader)
    {
        return new DestroyPlayerCommand(PlayerId: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(PlayerId);
    }
}