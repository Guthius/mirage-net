using SFML.Graphics;

namespace Mirage.Client.Assets;

public sealed class TextureManager() : AssetManager<Texture>(new Texture(64, 64))
{
    protected override Texture OnLoad(Stream stream)
    {
        return new Texture(stream);
    }
}