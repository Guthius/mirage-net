using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mirage.Game.Data;
using MonoGame.ImGuiNet;
using Vector2 = System.Numerics.Vector2;

namespace Mirage.Client.Scenes;

public sealed class MapEditorScene : Scene
{
    private readonly SpriteBatch _spriteBatch;
    private readonly Texture2D _tileset;
    private readonly IntPtr _tilesetId;

    public MapEditorScene(GraphicsDevice graphicsDevice, ImGuiRenderer imGuiRenderer)
    {
        _spriteBatch = new SpriteBatch(graphicsDevice);
        _tileset = Texture2D.FromFile(graphicsDevice, "Assets/Tiles.png");
        _tilesetId = imGuiRenderer.BindTexture(_tileset);
    }

    public override void DrawUI(GameTime gameTime)
    {
        ShowEditor();
    }

    private int _tileX = 1;
    private int _tileY = 0;

    private List<MapLayerInfo> _layers = [new() {Name = "Ground", Tiles = new int[25 * 25]}];
    private int _selectedLayer;
    
    private void ShowEditor()
    {
        var tilesetSize = new Vector2(_tileset.Width, _tileset.Height);

        ImGui.Begin("Map Editor", ImGuiWindowFlags.AlwaysAutoResize);
        
        ImGui.SeparatorText("Layers");
        ImGui.SetNextItemWidth(240);
        ImGui.ListBox("##Layers", ref _selectedLayer, _layers.Select(c => c.Name).ToArray(), _layers.Count, 5);

        ImGui.SeparatorText("Tileset");
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(0, 0));
        ImGui.BeginChild("TilesetScroll", new Vector2(240, 300), ImGuiChildFlags.FrameStyle, ImGuiWindowFlags.HorizontalScrollbar);
        ImGui.Image(_tilesetId, tilesetSize);
        if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
        {
        }

        ImGui.EndChild();
        ImGui.PopStyleVar();

        var topLeft = new Vector2(_tileX * 32, _tileY * 32);
        var bottomRight = new Vector2(topLeft.X + 32, topLeft.Y + 32);

        ImGui.Image(_tilesetId, new Vector2(32, 32), topLeft / tilesetSize, bottomRight / tilesetSize);
        ImGui.End();
    }
}