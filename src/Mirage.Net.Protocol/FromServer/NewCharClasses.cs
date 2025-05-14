using Mirage.Game.Data;

namespace Mirage.Net.Protocol.FromServer;

public sealed record NewCharClasses(IReadOnlyList<JobInfo> Classes) : IPacket<NewCharClasses>
{
    public static string PacketId => "newcharclasses";

    public static NewCharClasses ReadFrom(PacketReader reader)
    {
        var classInfos = new List<JobInfo>();
        var classCount = reader.ReadInt32();

        for (var i = 0; i < classCount; i++)
        {
            var name = reader.ReadString();

            _ = reader.ReadInt32(); // Max HP
            _ = reader.ReadInt32(); // Max MP
            _ = reader.ReadInt32(); // Max SP

            var classInfo = new JobInfo
            {
                Name = name,
                Strength = reader.ReadInt32(),
                Defense = reader.ReadInt32(),
                Speed = reader.ReadInt32(),
                Intelligence = reader.ReadInt32()
            };

            classInfos.Add(classInfo);
        }

        return new NewCharClasses(classInfos);
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