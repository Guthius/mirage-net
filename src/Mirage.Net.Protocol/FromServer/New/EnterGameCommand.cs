namespace Mirage.Net.Protocol.FromServer.New;

public sealed record EnterGameCommand : IPacket<EnterGameCommand>
{
    public static string PacketId => nameof(EnterGameCommand);

    public static EnterGameCommand ReadFrom(PacketReader reader)
    {
        return EmptyPacket<EnterGameCommand>.Value;
    }

    public void WriteTo(PacketWriter writer)
    {
    }
}