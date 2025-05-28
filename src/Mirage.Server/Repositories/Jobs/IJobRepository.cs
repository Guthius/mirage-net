using Mirage.Shared.Data;

namespace Mirage.Server.Repositories.Jobs;

public interface IJobRepository
{
    JobInfo? Get(string classId);
    List<JobInfo> GetAll();
    string GetName(string classId);
    void Load();
}