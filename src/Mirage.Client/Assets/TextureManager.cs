using Microsoft.Xna.Framework.Graphics;

namespace Mirage.Client.Assets;

public sealed class TextureManager(GraphicsDevice graphicsDevice) : AssetManager<Texture2D>(new Texture2D(graphicsDevice, 64, 64))
{
    protected override Texture2D OnLoad(Stream stream)
    {
        return Texture2D.FromStream(graphicsDevice, stream);
    }
}