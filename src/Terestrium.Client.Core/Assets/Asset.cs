namespace Terestrium.Client.Core.Assets;

public sealed class Asset<TAsset>(TAsset instance)
{
    public TAsset Instance { get; set; } = instance;
}