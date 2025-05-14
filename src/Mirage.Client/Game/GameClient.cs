using ImGuiNET;
using Microsoft.Xna.Framework;
using Mirage.Client.Scenes;
using Mirage.Game.Data;
using MonoGame.ImGuiNet;
using Color = Microsoft.Xna.Framework.Color;
using ImGuiVec2 = System.Numerics.Vector2;

namespace Mirage.Client.Game;

public sealed class GameClient : Microsoft.Xna.Framework.Game, IGameState
{
    private readonly ISceneManager _sceneManager;
    private ImGuiRenderer _imGuiRenderer = null!;
    private string _status = string.Empty;
    private string _alertMessage = string.Empty;

    public List<JobInfo> Jobs { get; set; } = [];
    public int MaxCharacters { get; set; }
    public List<CharacterSlotInfo> Characters { get; set; } = [];
    public int? SelectedCharacterSlot { get; set; }
    public InventorySlotInfo[] Inventory { get; set; } = [];
    public List<Chat> ChatHistory { get; set; } = [];
    public bool ChatHistoryUpdated { get; set; }
    public Map? Map { get; set; }

    public ImGuiRenderer ImGuiRenderer => _imGuiRenderer;

    public GameClient(ISceneManager sceneManager)
    {
        _sceneManager = sceneManager;

        var graphics = new GraphicsDeviceManager(this);

        graphics.PreferredBackBufferWidth = 1280;
        graphics.PreferredBackBufferHeight = 720;

        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        Window.AllowUserResizing = false;
    }

    protected override void Initialize()
    {
        Window.Title = "Mirage.NET";

        _imGuiRenderer = new ImGuiRenderer(this);
        _imGuiRenderer.RebuildFontAtlas();

        _sceneManager.SwitchTo<MapEditorScene>();

        base.Initialize();
    }

    protected override void Update(GameTime gameTime)
    {
        _sceneManager.Current?.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        var scene = _sceneManager.Current;
        if (scene is null)
        {
            return;
        }

        scene.Draw(gameTime);

        _imGuiRenderer.BeginLayout(gameTime);

        scene.DrawUI(gameTime);

        DrawStatus();
        DrawAlert();

        _imGuiRenderer.EndLayout();
    }

    public void SetStatus(string status)
    {
        _status = status;
    }

    public void ClearStatus()
    {
        _status = string.Empty;
    }

    public void ShowAlert(string alertMessage)
    {
        _alertMessage = alertMessage;
    }

    private void DrawStatus()
    {
        if (string.IsNullOrEmpty(_status))
        {
            return;
        }

        var io = ImGui.GetIO();
        var size = ImGui.CalcTextSize(_status);

        ImGui.SetNextWindowPos(new ImGuiVec2(0, 0), ImGuiCond.Always);
        ImGui.SetNextWindowSize(io.DisplaySize, ImGuiCond.Always);
        ImGui.Begin("status", ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoMouseInputs | ImGuiWindowFlags.NoDecoration);
        ImGui.SetCursorPosX(10);
        ImGui.SetCursorPosY(io.DisplaySize.Y - size.Y - 10);
        ImGui.Text(_status);
        ImGui.End();
    }

    private void DrawAlert()
    {
        if (string.IsNullOrEmpty(_alertMessage))
        {
            return;
        }

        var center = ImGui.GetMainViewport().GetCenter();


        ImGui.OpenPopup("Alert");
        ImGui.SetNextWindowSize(new ImGuiVec2(380, 120), ImGuiCond.Always);
        ImGui.SetNextWindowPos(new ImGuiVec2(center.X - 190, center.Y - 60), ImGuiCond.Appearing);

        if (!ImGui.BeginPopupModal("Alert"))
        {
            return;
        }

        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new ImGuiVec2(10, 10));
        ImGui.Spacing();
        ImGui.Text(_alertMessage);
        ImGui.Spacing();
        ImGui.Separator();

        var buttonsWidth = 70 + ImGui.GetStyle().ItemSpacing.X;

        ImGui.SetCursorPosX((ImGui.GetWindowSize().X - buttonsWidth) * 0.5f);

        if (ImGui.Button("OK", new ImGuiVec2(70, 26)))
        {
            _alertMessage = string.Empty;
        }

        ImGui.PopStyleVar();
        ImGui.EndPopup();
    }
}