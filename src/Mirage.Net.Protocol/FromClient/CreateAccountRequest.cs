namespace Mirage.Net.Protocol.FromClient;

public sealed record CreateAccountRequest(string AccountName, string Password) : IPacket<CreateAccountRequest>
{
    public static string PacketId => "newaccount";

    public static CreateAccountRequest ReadFrom(PacketReader reader)
    {
        return new CreateAccountRequest(
            AccountName: reader.ReadString(),
            Password: reader.ReadString());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteString(AccountName);
        writer.WriteString(Password);
    }
}