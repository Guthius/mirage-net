using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mirage.Client.Game;

public abstract class GameObject(int id)
{
    public int Id { get; } = id;
    
    public abstract void Update(GameTime gameTime);
    public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime);
}