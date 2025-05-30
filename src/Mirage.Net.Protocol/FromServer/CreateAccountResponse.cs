namespace Mirage.Net.Protocol.FromServer;

public sealed record CreateAccountResponse(CreateAccountResult Result) : IPacket<CreateAccountResponse>
{
    public static string PacketId => nameof(CreateAccountResponse);

    public static CreateAccountResponse ReadFrom(PacketReader reader)
    {
        return new CreateAccountResponse(Result: reader.ReadEnum<CreateAccountResult>());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteEnum(Result);
    }
}