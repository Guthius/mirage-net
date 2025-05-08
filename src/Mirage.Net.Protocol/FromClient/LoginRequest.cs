namespace Mirage.Net.Protocol.FromClient;

public sealed record LoginRequest(string AccountName, string Password, Version Version) : IPacket<LoginRequest>
{
    public static string PacketId => "login";

    public static LoginRequest ReadFrom(PacketReader reader)
    {
        return new LoginRequest(
            AccountName: reader.ReadString(),
            Password: reader.ReadString(),
            new Version(
                reader.ReadInt32(), 
                reader.ReadInt32(), 
                reader.ReadInt32()));
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteString(AccountName);
        writer.WriteString(Password);
        writer.WriteInt32(Version.Major);
        writer.WriteInt32(Version.Minor);
        writer.WriteInt32(Version.Build);
    }
}