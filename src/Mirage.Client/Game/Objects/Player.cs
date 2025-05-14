using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mirage.Game.Data;

namespace Mirage.Client.Game.Objects;

public class Player(int id, CharacterInfo characterInfo) : GameObject(id)
{
    public int Sprite { get; set; } = characterInfo.Sprite;
    public int X { get; set; } = characterInfo.X;
    public int Y { get; set; } = characterInfo.Y;

    public override void Update(GameTime gameTime)
    {
    }

    public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
    }

    public void Move(Direction direction)
    {
    }
}