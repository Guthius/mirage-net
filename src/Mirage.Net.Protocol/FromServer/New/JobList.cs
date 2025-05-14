using Mirage.Game.Data;

namespace Mirage.Net.Protocol.FromServer.New;

public sealed record JobList(List<JobInfo> Jobs) : IPacket<JobList>
{
    public static string PacketId => nameof(JobList);

    public static JobList ReadFrom(PacketReader reader)
    {
        return new JobList(Jobs: reader.ReadList(() => new JobInfo
        {
            Id = reader.ReadString(),
            Name = reader.ReadString(),
            Sprite = reader.ReadInt32(),
            Strength = reader.ReadInt32(),
            Defense = reader.ReadInt32(),
            Speed = reader.ReadInt32(),
            Intelligence = reader.ReadInt32()
        }));
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteList(Jobs, job =>
        {
            writer.WriteString(job.Id);
            writer.WriteString(job.Name);
            writer.WriteInt32(job.Sprite);
            writer.WriteInt32(job.Strength);
            writer.WriteInt32(job.Defense);
            writer.WriteInt32(job.Speed);
            writer.WriteInt32(job.Intelligence);
        });
    }
}