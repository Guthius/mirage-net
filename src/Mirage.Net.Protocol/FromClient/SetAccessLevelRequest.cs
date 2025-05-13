using Mirage.Game.Data;

namespace Mirage.Net.Protocol.FromClient;

public sealed record SetAccessLevelRequest(string TargetName, AccessLevel AccessLevel) : IPacket<SetAccessLevelRequest>
{
    public static string PacketId => "setaccess";

    public static SetAccessLevelRequest ReadFrom(PacketReader reader)
    {
        return new SetAccessLevelRequest(
            TargetName: reader.ReadString(),
            AccessLevel: reader.ReadEnum<AccessLevel>());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteString(TargetName);
        writer.WriteEnum(AccessLevel);
    }
}