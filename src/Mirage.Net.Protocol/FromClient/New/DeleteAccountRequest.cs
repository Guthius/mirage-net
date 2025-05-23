namespace Mirage.Net.Protocol.FromClient.New;

public sealed record DeleteAccountRequest(string AccountName, string Password) : IPacket<DeleteAccountRequest>
{
    public static string PacketId => nameof(DeleteAccountRequest);

    public static DeleteAccountRequest ReadFrom(PacketReader reader)
    {
        return new DeleteAccountRequest(
            AccountName: reader.ReadString(),
            Password: reader.ReadString());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteString(AccountName);
        writer.WriteString(Password);
    }
}