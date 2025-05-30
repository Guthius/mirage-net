namespace Mirage.Net.Protocol.FromServer;

public sealed record DeleteAccountResponse(DeleteAccountResult Result) : IPacket<DeleteAccountResponse>
{
    public static string PacketId => nameof(DeleteAccountResponse);

    public static DeleteAccountResponse ReadFrom(PacketReader reader)
    {
        return new DeleteAccountResponse(Result: reader.ReadEnum<DeleteAccountResult>());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteEnum(Result);
    }
}