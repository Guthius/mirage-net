using Mirage.Game.Data;

namespace Mirage.Net.Protocol.FromServer;

public sealed record PlayerData(int PlayerId, string Name, int Sprite, int MapId, int X, int Y, Direction Dir, AccessLevel Access, bool PlayerKiller) : IPacket<PlayerData>
{
    public static string PacketId => "playerdata";

    public static PlayerData ReadFrom(PacketReader reader)
    {
        return new PlayerData(
            PlayerId: reader.ReadInt32(),
            Name: reader.ReadString(),
            Sprite: reader.ReadInt32(),
            MapId: reader.ReadInt32(),
            X: reader.ReadInt32(),
            Y: reader.ReadInt32(),
            Dir: reader.ReadEnum<Direction>(),
            Access: reader.ReadEnum<AccessLevel>(),
            PlayerKiller: reader.ReadInt32() != 0);
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(PlayerId);
        writer.WriteString(Name);
        writer.WriteInt32(Sprite);
        writer.WriteInt32(MapId);
        writer.WriteInt32(X);
        writer.WriteInt32(Y);
        writer.WriteEnum(Dir);
        writer.WriteEnum(Access);
        writer.WriteInt32(PlayerKiller ? 1 : 0);
    }

    /// <summary>
    /// Creates a player data packet with all attributes cleared.
    /// </summary>
    /// <param name="playerId">The ID of the player.</param>
    /// <returns>A empty player data packet.</returns>
    public static PlayerData Cleared(int playerId)
    {
        return new PlayerData(playerId, "", 0, 0, 0, 0, Direction.Up, 0, false);
    }

    /// <summary>
    /// Creates a player data packet for the specified character with the map field set to zero.
    /// </summary>
    /// <param name="playerId">The ID of the player.</param>
    /// <param name="characterInfo">The character info.</param>
    /// <returns>A player data packet.</returns>
    public static PlayerData ClearMap(int playerId, CharacterInfo characterInfo)
    {
        return new PlayerData(playerId,
            characterInfo.Name,
            characterInfo.Sprite,
            0,
            characterInfo.X,
            characterInfo.Y,
            characterInfo.Direction,
            characterInfo.AccessLevel,
            characterInfo.PlayerKiller);
    }
}