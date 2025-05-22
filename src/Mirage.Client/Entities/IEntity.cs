using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mirage.Client.Entities;

/// <summary>
/// Represents an entity that exists in the game world.
/// </summary>
public interface IEntity
{
    void Update(GameTime gameTime);
    void Draw(SpriteBatch spriteBatch);
}