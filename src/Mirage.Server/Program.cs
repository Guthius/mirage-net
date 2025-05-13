using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mirage.Server.Game;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    var builder = Host.CreateApplicationBuilder(args);

    builder.Services.AddSerilog();
    builder.Services.AddHostedService<GameService>();

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