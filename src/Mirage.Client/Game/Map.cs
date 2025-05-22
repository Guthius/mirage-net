using System.Collections.Concurrent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mirage.Client.Entities;
using Mirage.Client.Net;
using Mirage.Game.Data;
using Mirage.Net.Protocol.FromClient.New;
using TiledSharp;

namespace Mirage.Client.Game;

public sealed class Map(GameState gameState, GraphicsDevice graphicsDevice)
{
    private const string MapName = "Map Name";

    private readonly ConcurrentDictionary<int, IEntity> _objects = new();
    private readonly Dictionary<int, Texture2D> _tilesets = [];
    private TmxMap _map;

    public void Load(string mapName, int revision)
    {
        Clear();
        
        try
        {
            var path = Path.Combine("Content", mapName);

            _map = new TmxMap(path);

            LoadTilesets(_map);
        }
        catch (FileNotFoundException)
        {
            Network.Send(new DownloadMapRequest(mapName));
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to load map {mapName}: {ex.Message}", ex);
        }
    }

    private void LoadTilesets(TmxMap map)
    {
        _tilesets.Clear();

        foreach (var tileset in map.Tilesets)
        {
            using var stream = File.OpenRead(tileset.Image.Source);

            _tilesets[tileset.FirstGid] = Texture2D.FromStream(graphicsDevice, stream);
        }
    }

    private void Clear()
    {
        _objects.Clear();
    }

    public void Update(GameTime gameTime)
    {
        foreach (var gameObject in _objects.Values)
        {
            gameObject.Update(gameTime);
        }
    }

    public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        DrawGroundLayers(spriteBatch);

        foreach (var gameObject in _objects.Values)
        {
            gameObject.Draw(spriteBatch);
        }

        DrawSkyLayers(spriteBatch);
        DrawPlayerNames(spriteBatch);
        DrawMapName(spriteBatch);
    }

    private void DrawGroundLayers(SpriteBatch spriteBatch)
    {
        foreach (var layer in _map.Layers)
        {
            for (var y = 0; y < _map.Height; y++)
            {
                for (var x = 0; x < _map.Width; x++)
                {
                    var tile = layer.Tiles[y * _map.Width + x];
                    if (tile.Gid == 0)
                    {
                        continue;
                    }

                    // Find the right tileset for this gid
                    var tilesetGid = _map.Tilesets.Where(t => t.FirstGid <= tile.Gid).Max(t => t.FirstGid);
                    var tileset = _map.Tilesets.First(t => t.FirstGid == tilesetGid);
                    
                    // Get the source rectangle from the tileset
                    var tileId = tile.Gid - tileset.FirstGid;
                    var tilesetWidth = tileset.Image.Width.Value / tileset.TileWidth;
                    var tilesetX = tileId % tilesetWidth * tileset.TileWidth;
                    var tilesetY = tileId / tilesetWidth * tileset.TileHeight;

                    var sourceRectangle = new Rectangle(
                        tilesetX, 
                        tilesetY, 
                        tileset.TileWidth, 
                        tileset.TileHeight);

                    var destinationRectangle = new Rectangle(
                        x * tileset.TileWidth, 
                        y * tileset.TileHeight, 
                        tileset.TileWidth, 
                        tileset.TileHeight);

                    spriteBatch.Draw(
                        _tilesets[tilesetGid],
                        destinationRectangle,
                        sourceRectangle,
                        Color.White);
                }
            }
        }
    }

    private void DrawSkyLayers(SpriteBatch spriteBatch)
    {
        // TODO: Implement me
    }

    private void DrawPlayerNames(SpriteBatch spriteBatch)
    {
        foreach (var player in _objects.Values.OfType<Player>())
        {
            player.DrawName(spriteBatch);
        }
    }

    private static void DrawMapName(SpriteBatch spriteBatch)
    {
        // TODO: Draw the map name in red if PvP is allowed on the map
        // TODO: Draw the map name centered at the top of the window

        spriteBatch.DrawString(Textures.Font, MapName, new Vector2(10, 10), Color.White);
    }

    public Player CreatePlayer(int playerId, string name, string jobId, int sprite, bool isPlayerKiller, AccessLevel accessLevel, int x, int y, Direction direction)
    {
        var player = new Player(gameState, playerId, name, jobId, sprite, isPlayerKiller, accessLevel, this, x, y, direction);

        _objects[playerId] = player;

        return player;
    }

    public void MovePlayer(int playerId, Direction direction, MovementType movementType)
    {
        if (_objects.TryGetValue(playerId, out var gameObject) && gameObject is Player player)
        {
            player.Move(direction, movementType);
        }
    }

    public void DestroyPlayer(int playerId)
    {
        _objects.TryRemove(playerId, out _);
    }

    public bool IsPassable(int x, int y)
    {
        // TODO: Implement me

        return true;
    }

    public TileType GetTileType(int x, int y)
    {
        // TODO: Implement me

        return TileType.Walkable;
    }
}