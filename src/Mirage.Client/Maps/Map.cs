using System.Collections.Concurrent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mirage.Client.Assets;
using Mirage.Client.Entities;
using Mirage.Shared.Data;

namespace Mirage.Client.Maps;

public sealed class Map(Game gameState, GraphicsDevice graphicsDevice)
{
    private readonly TextureManager _textureManager = new(graphicsDevice);
    private readonly MapManager _mapManager = new();
    private readonly ConcurrentDictionary<int, Actor> _actors = new();
    private readonly Dictionary<int, Asset<Texture2D>> _tilesets = [];
    private NewMapInfo? _info;

    public void Load(string mapId)
    {
        Clear();

        try
        {
            _mapManager.Get(mapId, info =>
            {
                _info = info;

                LoadTilesets(_info);
            });
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to load map {mapId}: {ex.Message}", ex);
        }
    }

    private void LoadTilesets(NewMapInfo map)
    {
        _tilesets.Clear();

        foreach (var tileset in map.Tilesets)
        {
            _tilesets[tileset.FirstGid] = _textureManager.Get(tileset.Id);
        }
    }

    private void Clear()
    {
        _actors.Clear();
    }

    public void Update(GameTime gameTime)
    {
        foreach (var gameObject in _actors.Values)
        {
            gameObject.Update(gameTime);
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        DrawGroundLayers(spriteBatch);
        DrawActors(spriteBatch);
        DrawSkyLayers(spriteBatch);
        DrawActorNames(spriteBatch);
    }

    public void DrawUI(SpriteBatch spriteBatch)
    {
        DrawMapName(spriteBatch);
    }

    private void DrawLayer(SpriteBatch spriteBatch, NewMapInfo mapInfo, MapLayerInfo layerInfo)
    {
        for (var y = 0; y < mapInfo.Height; y++)
        {
            for (var x = 0; x < mapInfo.Width; x++)
            {
                var tile = layerInfo.Tiles[y * mapInfo.Width + x];
                if (tile == 0)
                {
                    continue;
                }

                // Find the right tileset for this gid
                var tilesetGid = mapInfo.Tilesets.Where(t => t.FirstGid <= tile).Max(t => t.FirstGid);
                var tileset = mapInfo.Tilesets.First(t => t.FirstGid == tilesetGid);
                var tilesetTexture = _tilesets[tilesetGid].Instance;

                // Get the source rectangle from the tileset
                var tileId = tile - tileset.FirstGid;
                var tilesetWidth = tilesetTexture.Width / tileset.TileWidth;
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
                    tilesetTexture,
                    destinationRectangle,
                    sourceRectangle,
                    Color.White);
            }
        }
    }

    private void DrawGroundLayers(SpriteBatch spriteBatch)
    {
        if (_info is null)
        {
            return;
        }

        foreach (var layerInfo in _info.Layers.Where(x => !x.DrawOverActors))
        {
            DrawLayer(spriteBatch, _info, layerInfo);
        }
    }

    private void DrawSkyLayers(SpriteBatch spriteBatch)
    {
        if (_info is null)
        {
            return;
        }

        foreach (var layerInfo in _info.Layers.Where(x => x.DrawOverActors))
        {
            DrawLayer(spriteBatch, _info, layerInfo);
        }
    }

    private void DrawActors(SpriteBatch spriteBatch)
    {
        foreach (var actor in _actors.Values)
        {
            actor.Draw(spriteBatch);
        }
    }

    private void DrawActorNames(SpriteBatch spriteBatch)
    {
        foreach (var actor in _actors.Values)
        {
            actor.DrawName(spriteBatch);
        }
    }

    private void DrawMapName(SpriteBatch spriteBatch)
    {
        if (_info is null || string.IsNullOrEmpty(_info.Name))
        {
            return;
        }

        var color = _info.PvpEnabled ? Color.Red : Color.White;
        var x = graphicsDevice.Viewport.Width + (int) Textures.Font.MeasureString(_info.Name).X;

        spriteBatch.DrawString(Textures.Font, _info.Name, new Vector2((int) (x * 0.5f), 10), color);
    }

    public Actor? GetActor(int actorId)
    {
        return _actors.GetValueOrDefault(actorId);
    }

    public Actor CreateActor(int actorId, string name, int sprite, bool isPlayerKiller, AccessLevel accessLevel, int x, int y, Direction direction, int maxHealth, int health, int maxMana, int mana, int maxStamina, int stamina)
    {
        var actor = new Actor(gameState, actorId, x, y)
        {
            Name = name,
            Sprite = sprite,
            IsPlayerKiller = isPlayerKiller,
            AccessLevel = accessLevel,
            Map = this,
            TileX = x,
            TileY = y,
            Direction = direction,
            MaxHealth = maxHealth,
            Health = health,
            MaxMana = maxMana,
            Mana = mana,
            MaxStamina = maxStamina,
            Stamina = stamina
        };

        _actors[actorId] = actor;

        return actor;
    }

    public void DestroyActor(int actorId)
    {
        _actors.TryRemove(actorId, out _);
    }

    public bool IsPassable(int x, int y)
    {
        if (_actors.Values.Any(actor => actor.TileX == x && actor.TileY == y))
        {
            return false;
        }

        return _info?.IsPassable(x, y) ?? false;
    }

    public TileType GetTileType(int x, int y)
    {
        return _info?.GetTileType(x, y) ?? TileType.Walkable;
    }
}