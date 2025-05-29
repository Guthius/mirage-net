using System.Globalization;
using CommunityToolkit.Mvvm.DependencyInjection;
using ImGuiNET;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mirage.Client.Entities;
using Mirage.Client.Extensions;
using Mirage.Client.Maps;
using Mirage.Client.Scenes;
using Mirage.Shared.Data;
using MonoGame.ImGuiNet;
using ImGuiVec2 = System.Numerics.Vector2;

namespace Mirage.Client;

public sealed class Game : Microsoft.Xna.Framework.Game
{
    private const int MaxChatHistory = 500;
    
    private readonly ISceneManager _sceneManager;
    private string _status = string.Empty;
    private string _alertMessage = string.Empty;
    private readonly List<ChatInfo> _chatHistory = [];
    private volatile List<ChatInfo> _chatHistorySnapshot = [];
    private readonly Lock _chatLock = new();

    public List<JobInfo> Jobs { get; set; } = [];
    public int MaxCharacters { get; set; }
    public List<CharacterSlotInfo> Characters { get; set; } = [];
    public InventorySlotInfo[] Inventory { get; set; } = [];
    public Map Map { get; private set; } = null!;
    public bool GettingMap { get; set; }
    public int LocalPlayerId { get; set; }
    public Actor? LocalPlayer { get; set; }
    public ImGuiRenderer ImGuiRenderer { get; private set; } = null!;
    public bool ShowFps { get; set; } = true;
    public IReadOnlyList<ChatInfo> ChatHistory => _chatHistorySnapshot;
    public bool ChatHistoryUpdated { get; set; }

    private static void Main()
    {
        var services = new ServiceCollection();

        services.AddCore();
        services.AddSingleton<Game>();
        services.AddScenesFromAssemblyContaining<Game>();
        services.AddSingleton<GraphicsDevice>(provider => provider.GetRequiredService<Game>().GraphicsDevice);
        services.AddSingleton<ImGuiRenderer>(provider => provider.GetRequiredService<Game>().ImGuiRenderer);

        var serviceProvider = services.BuildServiceProvider();

        Ioc.Default.ConfigureServices(serviceProvider);

        using var client = serviceProvider.GetRequiredService<Game>();

        client.Run();
    }

    public Game(ISceneManager sceneManager)
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

        ImGuiRenderer = new ImGuiRenderer(this);
        ImGuiRenderer.RebuildFontAtlas();

        Textures.Font = Content.Load<SpriteFont>("PixelOperator");

        _sceneManager.SwitchTo<MainMenuScene>();

        Map = new Map(this, GraphicsDevice);

        base.Initialize();
    }

    public void ConnectionLost()
    {
        _sceneManager.SwitchTo<MainMenuScene>();

        ShowAlert("The connection with the server was lost.");
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

        ImGuiRenderer.BeginLayout(gameTime);

        scene.DrawUI(gameTime);

        DrawStatus();
        DrawAlert();

        ImGuiRenderer.EndLayout();

        DrawFps(gameTime);
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

    private void DrawFps(GameTime gameTime)
    {
        if (!ShowFps)
        {
            return;
        }

        var frameRate = 1 / gameTime.ElapsedGameTime.TotalSeconds;
        var frameRateStr = "FPS: " + frameRate.ToString("0.00", CultureInfo.InvariantCulture);

        var spriteBatch = new SpriteBatch(GraphicsDevice);

        spriteBatch.Begin();
        spriteBatch.DrawString(Textures.Font, frameRateStr, new Vector2(6, 6), Color.Black);
        spriteBatch.DrawString(Textures.Font, frameRateStr, new Vector2(5, 5), Color.White);
        spriteBatch.End();
    }

    public void ClearChatHistory()
    {
        lock (_chatLock)
        {
            _chatHistory.Clear();
            _chatHistorySnapshot = [];
        }
    }

    public void AddChat(ChatInfo chatInfo)
    {
        lock (_chatLock)
        {
            _chatHistory.Add(chatInfo);
            if (_chatHistory.Count > MaxChatHistory)
            {
                _chatHistory.RemoveAt(0);
            }

            _chatHistorySnapshot = _chatHistory.ToList();

            ChatHistoryUpdated = true;
        }
    }
}