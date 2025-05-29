using System.Threading.Channels;
using Mirage.Net;
using Mirage.Net.Protocol.FromServer;
using Mirage.Server.Players;
using Mirage.Server.Repositories.Accounts;

namespace Mirage.Server.Net;

public sealed class NetworkConnection(int id, string address, Channel<byte[]> sendChannel)
{
    public int Id { get; } = id;
    public string Address { get; } = address;
    public AccountInfo? Account { get; set; }
    public Player? Player { get; set; }

    public void Send<TPacket>() where TPacket : IPacket<TPacket>, new()
    {
        Send(EmptyPacket<TPacket>.Value);
    }

    public void Send<TPacket>(TPacket packet) where TPacket : IPacket<TPacket>
    {
        var bytes = PacketSerializer.GetBytes(packet);

        Send(bytes);
    }

    public void Send(byte[] bytes)
    {
        sendChannel.Writer.TryWrite(bytes);
    }

    public void Disconnect(string message)
    {
        Send(new DisconnectCommand(message));

        Disconnect();
    }

    public void Disconnect()
    {
        sendChannel.Writer.TryComplete();
    }
}