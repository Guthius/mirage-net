using Mirage.Game.Data;

namespace Mirage.Net.Protocol.FromServer;

public sealed record PlayerDir(int PlayerId, Direction Direction) : IPacket<PlayerDir>
{
    public static string PacketId => "playerdir";

    public static PlayerDir ReadFrom(PacketReader reader)
    {
        return new PlayerDir(
            PlayerId: reader.ReadInt32(),
            Direction: reader.ReadEnum<Direction>());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(PlayerId);
        writer.WriteEnum(Direction);
    }
}