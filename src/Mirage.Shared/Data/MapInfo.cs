namespace Mirage.Shared.Data;

public sealed record MapInfo
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool PvpEnabled { get; set; }
    public int TileWidth { get; set; }
    public int TileHeight { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public List<TilesetInfo> Tilesets { get; set; } = [];
    public List<MapLayerInfo> Layers { get; set; } = [];
    public TileInfo[] Tiles { get; set; } = [];

    public void WriteTo(Stream stream)
    {
        using var binaryWriter = new BinaryWriter(stream);

        binaryWriter.Write(Name);
        binaryWriter.Write(PvpEnabled);
        binaryWriter.Write(TileWidth);
        binaryWriter.Write(TileHeight);
        binaryWriter.Write(Width);
        binaryWriter.Write(Height);

        binaryWriter.Write(Tilesets.Count);
        foreach (var tileset in Tilesets)
        {
            binaryWriter.Write(tileset.Id);
            binaryWriter.Write(tileset.FirstGid);
            binaryWriter.Write(tileset.TileWidth);
            binaryWriter.Write(tileset.TileHeight);
        }

        binaryWriter.Write(Layers.Count);
        foreach (var layer in Layers)
        {
            binaryWriter.Write(layer.DrawOverActors);
            binaryWriter.Write(layer.Tiles.Length);
            foreach (var tile in layer.Tiles)
            {
                binaryWriter.Write(tile);
            }
        }

        binaryWriter.Write(Tiles.Length);
        foreach (var tile in Tiles)
        {
            binaryWriter.Write((int) tile.Type);
        }
    }

    public static MapInfo ReadFrom(Stream stream)
    {
        using var reader = new BinaryReader(stream);

        return new MapInfo
        {
            Name = reader.ReadString(),
            PvpEnabled = reader.ReadBoolean(),
            TileWidth = reader.ReadInt32(),
            TileHeight = reader.ReadInt32(),
            Width = reader.ReadInt32(),
            Height = reader.ReadInt32(),
            Tilesets = ReadTilesets(),
            Layers = ReadLayers(),
            Tiles = ReadTiles()
        };

        List<TilesetInfo> ReadTilesets()
        {
            var count = reader.ReadInt32();
            var tilesets = new List<TilesetInfo>(count);

            for (var i = 0; i < count; i++)
            {
                tilesets.Add(new TilesetInfo
                {
                    Id = reader.ReadString(),
                    FirstGid = reader.ReadInt32(),
                    TileWidth = reader.ReadInt32(),
                    TileHeight = reader.ReadInt32()
                });
            }

            return tilesets;
        }

        List<MapLayerInfo> ReadLayers()
        {
            var count = reader.ReadInt32();
            var layers = new List<MapLayerInfo>(count);

            for (var i = 0; i < count; i++)
            {
                layers.Add(new MapLayerInfo
                {
                    DrawOverActors = reader.ReadBoolean(),
                    Tiles = ReadLayerTiles()
                });
            }

            return layers;
        }

        int[] ReadLayerTiles()
        {
            var count = reader.ReadInt32();
            var tiles = new int[count];
            for (var i = 0; i < count; i++)
            {
                tiles[i] = reader.ReadInt32();
            }

            return tiles;
        }

        TileInfo[] ReadTiles()
        {
            var count = reader.ReadInt32();
            var tiles = new TileInfo[count];

            for (var i = 0; i < count; i++)
            {
                tiles[i] = new TileInfo
                {
                    Type = (TileTypes) reader.ReadInt32()
                };
            }

            return tiles;
        }
    }

    public bool InBounds(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    public TileTypes GetTileType(int x, int y)
    {
        return InBounds(x, y) ? Tiles[y * Width + x].Type : TileTypes.None;
    }

    public bool IsPassable(int x, int y)
    {
        return InBounds(x, y) && GetTileType(x, y) == TileTypes.None;
    }
}