namespace Mirage.Client.Assets;

public sealed class Asset<T>(T instance)
{
    public T Instance { get; set; } = instance;
}