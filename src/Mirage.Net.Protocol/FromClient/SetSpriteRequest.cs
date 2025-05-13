namespace Mirage.Net.Protocol.FromClient;

public sealed record SetSpriteRequest(int Sprite) : IPacket<SetSpriteRequest>
{
    public static string PacketId => "setsprite";

    public static SetSpriteRequest ReadFrom(PacketReader reader)
    {
        return new SetSpriteRequest(Sprite: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(Sprite);
    }
}