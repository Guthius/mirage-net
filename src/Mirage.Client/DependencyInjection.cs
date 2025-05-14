using Microsoft.Extensions.DependencyInjection;
using Mirage.Client.Scenes;

namespace Mirage.Client;

public static class DependencyInjection
{
    public static void AddCore(this IServiceCollection services)
    {
        services.AddSingleton<ISceneManager, SceneManager>();
    }

    public static void AddScenesFromAssemblyContaining<T>(this IServiceCollection services)
    {
        foreach (var type in typeof(T).Assembly.GetTypes())
        {
            if (type is {IsClass: true, IsAbstract: false, IsPublic: true} && type.IsAssignableTo(typeof(IScene)))
            {
                services.AddSingleton(type);
            }
        }
    }
}