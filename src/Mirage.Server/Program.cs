using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mirage.Server.Chat;
using Mirage.Server.Maps;
using Mirage.Server.Net;
using Mirage.Server.Players;
using Mirage.Server.Repositories;
using Mirage.Server.Repositories.Accounts;
using Mirage.Server.Repositories.Bans;
using Mirage.Server.Repositories.Characters;
using Mirage.Server.Repositories.Jobs;
using Mirage.Server.Repositories.Maps;
using Mirage.Server.Services;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = Host.CreateApplicationBuilder(args);

    builder.Services.Configure<NetworkOptions>(options =>
    {
        options.Port = builder.Configuration.GetValue("Network:Port", 4000);
        options.MaxConnections = builder.Configuration.GetValue("Network:MaxConnections", 1000);
    });

    builder.Services.AddSerilog();

    /* Services */
    builder.Services.AddSingleton<IChatService, ChatService>();
    builder.Services.AddSingleton<IMapService, MapService>();
    builder.Services.AddSingleton<IPlayerService, PlayerService>();
    builder.Services.AddChatCommands();

    /* Repositories */
    builder.Services.AddSingleton<IAccountRepository, AccountRepository>();
    builder.Services.AddSingleton<IBanRepository, BanRepository>();
    builder.Services.AddSingleton<ICharacterRepository, CharacterRepository>();
    builder.Services.AddSingleton<IJobRepository, JobRepository>();
    builder.Services.AddSingleton<IMapRepository, MapRepository>();
    builder.Services.AddSingleton(typeof(IRepository<>), typeof(Repository<>));

    /* Background Services */
    builder.Services.AddHostedService<GameService>();
    builder.Services.AddHostedService<NetworkService>();

    await builder.Build().RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Server terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}