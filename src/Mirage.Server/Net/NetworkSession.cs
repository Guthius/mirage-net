using Mirage.Net;
using Mirage.Net.Protocol.FromServer.New;
using Mirage.Server.Maps;
using Mirage.Server.Players;
using Mirage.Shared.Data;

namespace Mirage.Server.Net;

public sealed record NetworkSession(int Id) : IPacketRecipient
{
    public readonly byte[] Buffer = new byte[0xFFFF];

    public int BufferOffset { get; set; }
    public AccountInfo? Account { get; set; }
    public Player? Player { get; private set; }

    public void Send<TPacket>(TPacket packet) where TPacket : IPacket<TPacket>
    {
        var bytes = PacketSerializer.GetBytes(packet);

        Network.SendData(Id, bytes);
    }

    public void Disconnect(string message)
    {
        Send(new DisconnectCommand(message));

        Network.Disconnect(Id);
    }

    public void CreatePlayer(CharacterInfo character)
    {
        var map = MapManager.GetByName(character.Map);
        if (map is null)
        {
            Disconnect(
                "Your character is in an invalid state. " +
                "Please contact an administrator.");

            return;
        }

        Player = new Player(Id, this, character, map);
    }

    public void Destroy()
    {
        Player?.Destroy();
        Player = null;
    }
}