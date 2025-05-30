namespace Mirage.Server.Assets;

public sealed record Asset(string Path, string Id)
{
    public Stream OpenRead()
    {
        return File.OpenRead(Path);
    }
}