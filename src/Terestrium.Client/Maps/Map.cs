using System.Collections.Concurrent;
using Mirage.Shared.Data;
using SFML.Graphics;
using SFML.System;
using Terestrium.Client.Entities;

namespace Terestrium.Client.Maps;

public sealed class Map(Game game) : Drawable
{
    private static readonly Texture Items = new("Content/Items.png");

    private readonly MapManager _mapManager = new();
    private readonly ConcurrentDictionary<int, Actor> _actors = new();
    private readonly ConcurrentDictionary<int, Item> _items = [];
    private readonly MapRenderer _renderer = new();
    private MapInfo? _info;

    public int WidthInPixels { get; private set; }
    public int WidthInTiles => _info?.Width ?? 0;
    public int HeightInPixels { get; private set; }
    public int HeightInTiles => _info?.Height ?? 0;
    public MapInfo? Info => _info;
    
    public void Load(string mapId)
    {
        Clear();

        try
        {
            _mapManager.Get(mapId, info =>
            {
                _renderer.Update(info);

                game.GettingMap = false;

                _info = info;

                WidthInPixels = _info.Width * _info.TileWidth;
                HeightInPixels = _info.Height * _info.TileHeight;
            });
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to load map {mapId}: {ex.Message}", ex);
        }
    }

    private void Clear()
    {
        _actors.Clear();
    }

    public void Update(float dt)
    {
        foreach (var gameObject in _actors.Values)
        {
            gameObject.Update(dt);
        }
    }

    public void Draw(RenderTarget target, RenderStates states)
    {
        var mapInfo = _info;
        if (mapInfo is null)
        {
            return;
        }

        _renderer.DrawGround(target, states);

        DrawActors(target, states);

        _renderer.DrawSky(target, states);

        DrawActorNames(target, states);
    }

    private void DrawActors(RenderTarget target, RenderStates states)
    {
        var tileWidth = _info?.TileWidth ?? 32;
        var tileHeight = _info?.TileHeight ?? 32;

        foreach (var item in _items.Values)
        {
            DrawItem(target, states, item, tileWidth, tileHeight);
        }

        foreach (var actor in _actors.Values)
        {
            target.Draw(actor, states);
        }
    }

    private static void DrawItem(RenderTarget target, RenderStates states, Item item, int tileWidth, int tileHeight)
    {
        var y = item.Sprite * 32;

        var sprite = new Sprite();

        sprite.Position = new Vector2f(item.X * tileWidth, item.Y * tileHeight);
        sprite.Texture = Items;
        sprite.TextureRect = new IntRect(0, y, 32, 32);

        target.Draw(sprite, states);
    }

    private void DrawActorNames(RenderTarget target, RenderStates states)
    {
        foreach (var actor in _actors.Values)
        {
            actor.DrawName(target, states);
        }
    }
    
    public Actor? GetActor(int actorId)
    {
        return _actors.GetValueOrDefault(actorId);
    }

    public Actor CreateActor(int actorId, string name, int sprite, bool isPlayerKiller, AccessLevel accessLevel, int x, int y, Direction direction, int maxHealth, int health, int maxMana, int mana, int maxStamina, int stamina)
    {
        var actor = new Actor(game, actorId, x, y)
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

    public void CreateItem(int id, int sprite, int x, int y)
    {
        _items[id] = new Item(id, sprite, x, y);
    }

    public void DestroyActor(int actorId)
    {
        _actors.TryRemove(actorId, out _);
    }

    public void DestroyItem(int itemId)
    {
        _items.TryRemove(itemId, out _);
    }

    public bool IsPassable(int x, int y)
    {
        if (_actors.Values.Any(actor => actor.TileX == x && actor.TileY == y))
        {
            return false;
        }

        return _info?.IsPassable(x, y) ?? false;
    }

    public TileTypes GetTileType(int x, int y)
    {
        return _info?.GetTileType(x, y) ?? TileTypes.None;
    }
}