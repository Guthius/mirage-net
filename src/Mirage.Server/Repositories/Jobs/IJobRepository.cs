using Mirage.Shared.Data;

namespace Mirage.Server.Repositories.Jobs;

public interface IJobRepository
{
    JobInfo? Get(string jobId);
    List<JobInfo> GetAll();
    string GetName(string jobId);
    void Load();
}