namespace Mirage.Net.Protocol.FromServer.New;

public sealed record AuthResponse(AuthResult Result) : IPacket<AuthResponse>
{
    public static string PacketId => nameof(AuthResponse);

    public static AuthResponse ReadFrom(PacketReader reader)
    {
        return new AuthResponse(reader.ReadEnum<AuthResult>());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteEnum(Result);
    }
}