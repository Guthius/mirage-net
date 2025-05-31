using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Mirage.Client.Entities;
using Mirage.Client.Extensions;
using Mirage.Client.Inventory;
using Mirage.Client.Maps;
using Mirage.Client.Scenes;
using Mirage.Shared.Data;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Mirage.Client;

public sealed class Game
{
    private readonly ISceneManager _sceneManager;
    private readonly RenderWindow _renderWindow = new(new VideoMode(800, 600), "Mirage.NET", Styles.Close | Styles.Titlebar);
    private readonly Clock _clock = new();

    public static readonly Font Font = new("Content/Fonts/Coolvetica Rg.otf");

    public List<JobInfo> Jobs { get; set; } = [];
    public int MaxCharacters { get; set; }
    public List<CharacterSlotInfo> Characters { get; set; } = [];
    public Map Map { get; private set; }
    public bool GettingMap { get; set; }
    public int LocalPlayerId { get; set; }
    public Actor? LocalPlayer { get; set; }
    public bool ShowFps { get; set; } = true;
    public InventoryStore Inventory { get; } = new();

    private static void Main()
    {
        var services = new ServiceCollection();

        services.AddCore();
        services.AddSingleton<Game>();
        services.AddScenesFromAssemblyContaining<Game>();

        var serviceProvider = services.BuildServiceProvider();

        Ioc.Default.ConfigureServices(serviceProvider);

        var game = serviceProvider.GetRequiredService<Game>();

        game.Run();
    }

    public Game(ISceneManager sceneManager)
    {
        _sceneManager = sceneManager;

        _renderWindow.Closed += (_, _) => _renderWindow.Close();
        _renderWindow.MouseButtonPressed += (_, e) => _sceneManager.Current?.HandleMouseButtonPressed(e);
        _renderWindow.MouseButtonReleased += (_, e) => _sceneManager.Current?.HandleMouseButtonReleased(e);
        _renderWindow.MouseMoved += (_, e) => _sceneManager.Current?.HandleMouseMoved(e);
        _renderWindow.TextEntered += (_, e) => _sceneManager.Current?.HandleTextEntered(e);
        _renderWindow.KeyPressed += (_, e) => _sceneManager.Current?.HandleKeyPressed(e);
        _renderWindow.KeyReleased += (_, e) => _sceneManager.Current?.HandleKeyReleased(e);

        Map = new Map(this);
    }

    public void Run()
    {
        _sceneManager.SwitchTo<MainMenuScene>();

        _clock.Restart();

        while (_renderWindow.IsOpen)
        {
            var dt = _clock.Restart().AsSeconds();

            _renderWindow.DispatchEvents();
            _renderWindow.Clear();

            var scene = _sceneManager.Current;
            if (scene is null)
            {
                return;
            }

            scene.Update(dt);

            _renderWindow.Draw(scene);
            _renderWindow.Display();
        }
    }

    public void ConnectionLost()
    {
        _sceneManager.SwitchTo<MainMenuScene>();
        _sceneManager.Current?.ShowAlert("The connection with the server has been lost.");
    }

    public void ShowAlert(string alertMessage)
    {
        _sceneManager.Current?.ShowAlert(alertMessage);
    }
}