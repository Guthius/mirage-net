using Mirage.Game.Data;
using Mirage.Net;
using Mirage.Net.Protocol.FromServer;
using Mirage.Server.Net;

namespace Mirage.Server.Game;

public sealed record GameSession(int Id)
{
    public readonly byte[] Buffer = new byte[0xFFFF];
    public int BufferPos;
    public int DataTimer;
    public int DataBytes;
    public int DataPackets;

    public AccountInfo? Account { get; set; }
    public GamePlayer? Player { get; private set; }

    public void Send<TPacket>(TPacket packet) where TPacket : IPacket<TPacket>
    {
        var bytes = PacketSerializer.GetBytes(packet);

        Network.SendData(Id, bytes);
    }

    public void SendAlert(string alertMessage)
    {
        Send(new AlertMessage(alertMessage));

        Network.Disconnect(Id);
    }

    public void CreatePlayer(CharacterInfo character)
    {
        Player = new GamePlayer(Id, this, character);
    }

    public void Destroy()
    {
        Player?.Destroy();
        Player = null;
    }
}