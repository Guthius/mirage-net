using Mirage.Game.Data;
using Mirage.Net;
using Mirage.Net.Protocol.FromServer;
using Mirage.Server.Net;

namespace Mirage.Server.Game;

public sealed record GameSession(int Id) : IPacketRecipient
{
    public readonly byte[] Buffer = new byte[0xFFFF];

    public int BufferOffset { get; set; }
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
        var map = MapManager.GetMap(character.Map);
        if (map is null)
        {
            throw new NotImplementedException(); // TODO: Map not available...
        }
        
        Player = new GamePlayer(Id, this, character, map);
    }

    public void Destroy()
    {
        Player?.Destroy();
        Player = null;
    }
}