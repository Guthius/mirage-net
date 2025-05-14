using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework.Graphics;
using Mirage.Client;
using Mirage.Client.Game;
using MonoGame.ImGuiNet;

var services = new ServiceCollection();

services.AddCore();
services.AddSingleton<GameClient>();
services.AddSingleton<IGameState>(provider => provider.GetRequiredService<GameClient>());
services.AddScenesFromAssemblyContaining<Program>();
services.AddSingleton<GraphicsDevice>(provider => provider.GetRequiredService<GameClient>().GraphicsDevice);
services.AddSingleton<ImGuiRenderer>(provider => provider.GetRequiredService<GameClient>().ImGuiRenderer);

var serviceProvider = services.BuildServiceProvider();

Ioc.Default.ConfigureServices(serviceProvider);

using var client = serviceProvider.GetRequiredService<GameClient>();

client.Run();