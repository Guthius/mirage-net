using System.Collections.Concurrent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mirage.Client.Game;

public static class GameObjectManager
{
    private static readonly ConcurrentDictionary<int, GameObject> Objects = new();

    public static void Add(GameObject gameObject)
    {
        Objects.TryAdd(gameObject.Id, gameObject);
    }

    public static void Remove(GameObject gameObject)
    {
        Objects.TryRemove(gameObject.Id, out _);
    }

    public static void Reset()
    {
        Objects.Clear();
    }

    public static void Update(GameTime gameTime)
    {
        foreach (var gameObject in Objects.Values)
        {
            gameObject.Update(gameTime);
        }
    }

    public static void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        foreach (var gameObject in Objects.Values)
        {
            gameObject.Draw(spriteBatch, gameTime);
        }
    }
}