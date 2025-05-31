using Mirage.Client.Assets;
using Mirage.Shared.Data;
using SFML.Graphics;
using SFML.System;

namespace Mirage.Client.Maps;

public sealed class MapRenderer
{
    private sealed record TileInfo(int Tileset, int U, int V, int W, int H);

    private sealed class Layer(Asset<Texture> texture, VertexArray vertices) : Drawable
    {
        public void Draw(RenderTarget target, RenderStates states)
        {
            states.Texture = texture.Instance;

            target.Draw(vertices, states);
        }
    }

    private readonly TextureManager _textureManager = new();
    private readonly List<Asset<Texture>> _textures = [];
    private readonly Dictionary<int, TileInfo> _tiles = [];
    private readonly List<Layer> _groundLayers = [];
    private readonly List<Layer> _skyLayers = [];
    private readonly Lock _updateLock = new();
    private bool _updating;
    
    public void Update(MapInfo map)
    {
        lock (_updateLock)
        {
            try
            {
                _updating = true;

                Reset();

                foreach (var tileset in map.Tilesets)
                {
                    BuildTiles(tileset);
                }

                foreach (var layer in map.Layers)
                {
                    BuildVertexArrays(map, layer);
                }
            }
            finally
            {
                _updating = false;
            }
        }
    }

    private void Reset()
    {
        _textures.Clear();
        _tiles.Clear();
        _groundLayers.Clear();
        _skyLayers.Clear();
    }

    private void BuildTiles(TilesetInfo tileset)
    {
        var texture = _textureManager.Get(tileset.Id);
        var textureId = _textures.Count;

        _textures.Add(texture);

        var rows = tileset.ImageHeight / tileset.TileHeight;
        var columns = tileset.ImageWidth / tileset.TileWidth;

        for (var y = 0; y < rows; y++)
        {
            for (var x = 0; x < columns; x++)
            {
                var id = tileset.FirstGid + y * columns + x;

                _tiles[id] = new TileInfo(textureId,
                    x * tileset.TileWidth,
                    y * tileset.TileHeight,
                    tileset.TileWidth,
                    tileset.TileHeight);
            }
        }
    }

    private void BuildVertexArrays(MapInfo mapInfo, MapLayerInfo mapLayerInfo)
    {
        var chunks = new Dictionary<int, List<Vertex>>();

        var tw = mapInfo.TileWidth;
        var th = mapInfo.TileHeight;
        
        for (var y = 0; y < mapInfo.Height; y++)
        {
            for (var x = 0; x < mapInfo.Width; x++)
            {
                var id = mapLayerInfo.Tiles[y * mapInfo.Width + x];
                if (!_tiles.TryGetValue(id, out var tile))
                {
                    continue;
                }

                if (!chunks.TryGetValue(tile.Tileset, out var vertices))
                {
                    chunks[tile.Tileset] = vertices = [];
                }

                var tx = x * tw;
                var ty = y * th;
                
                vertices.Add(new Vertex(new Vector2f(tx, ty), new Vector2f(tile.U, tile.V)));
                vertices.Add(new Vertex(new Vector2f(tx + tw, ty), new Vector2f(tile.U + tile.W, tile.V)));
                vertices.Add(new Vertex(new Vector2f(tx + tw, ty + th), new Vector2f(tile.U + tile.W, tile.V + tile.H)));
                vertices.Add(new Vertex(new Vector2f(tx, ty + th), new Vector2f(tile.U, tile.V + tile.H)));
            }
        }

        foreach (var (tileset, vertices) in chunks)
        {
            var vertexArray = new VertexArray(PrimitiveType.Quads, (uint) vertices.Count);
            for (var i = 0; i < vertices.Count; i++)
            {
                vertexArray[(uint) i] = vertices[i];
            }

            var layer = new Layer(_textures[tileset], vertexArray);
            if (mapLayerInfo.DrawOverActors)
            {
                _skyLayers.Add(layer);
            }
            else
            {
                _groundLayers.Add(layer);
            }
        }
    }
    
    public void DrawGround(RenderTarget target, RenderStates states)
    {
        if (_updating)
        {
            return;
        }
        
        foreach (var layer in _groundLayers)
        {
            target.Draw(layer, states);
        }
    }

    public void DrawSky(RenderTarget target, RenderStates states)
    {
        if (_updating)
        {
            return;
        }
        
        foreach (var layer in _skyLayers)
        {
            target.Draw(layer, states);
        }
    }
}