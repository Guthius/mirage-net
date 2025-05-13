using Mirage.Game.Data;

namespace Mirage.Net.Protocol.FromServer;

public sealed record ClassesData(List<ClassInfo> Classes) : IPacket<ClassesData>
{
    public static string PacketId => "classesdata";

    public static ClassesData ReadFrom(PacketReader reader)
    {
        var classInfos = new List<ClassInfo>();
        var classCount = reader.ReadInt32();

        for (var i = 0; i < classCount; i++)
        {
            var name = reader.ReadString();

            _ = reader.ReadInt32(); // Max HP
            _ = reader.ReadInt32(); // Max MP
            _ = reader.ReadInt32(); // Max SP

            var classInfo = new ClassInfo
            {
                Name = name,
                Strength = reader.ReadInt32(),
                Defense = reader.ReadInt32(),
                Speed = reader.ReadInt32(),
                Intelligence = reader.ReadInt32()
            };

            classInfos.Add(classInfo);
        }

        return new ClassesData(classInfos);
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(Classes.Count);

        foreach (var classInfo in Classes)
        {
            writer.WriteString(classInfo.Name);
            writer.WriteInt32(classInfo.MaxHP);
            writer.WriteInt32(classInfo.MaxMP);
            writer.WriteInt32(classInfo.MaxSP);
            writer.WriteInt32(classInfo.Strength);
            writer.WriteInt32(classInfo.Defense);
            writer.WriteInt32(classInfo.Speed);
            writer.WriteInt32(classInfo.Intelligence);
        }
    }
}