namespace Mirage.Net.Protocol.FromClient;

public sealed record AuthRequest(int ProtocolVersion, string AccountName, string Password) : IPacket<AuthRequest>
{
    public static string PacketId => nameof(AuthRequest);

    public static AuthRequest ReadFrom(PacketReader reader)
    {
        return new AuthRequest(
            ProtocolVersion: reader.ReadInt32(),
            AccountName: reader.ReadString(),
            Password: reader.ReadString());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(ProtocolVersion);
        writer.WriteString(AccountName);
        writer.WriteString(Password);
    }
}