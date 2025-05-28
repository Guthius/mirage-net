using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Mirage.Server.Chat.Commands;

namespace Mirage.Server.Chat;

public static class DependencyInjection
{
    public static void AddChatCommands(this IServiceCollection services)
    {
        var serviceType = typeof(Command);
        var commandTypes = new List<Type>();

        foreach (var type in serviceType.Assembly.GetTypes())
        {
            if (type is {IsClass: true, IsAbstract: false, IsPublic: true} && type.IsAssignableTo(serviceType))
            {
                commandTypes.Add(type);
            }
        }

        services.TryAddEnumerable(commandTypes.Select(implementationType =>
            new ServiceDescriptor(serviceType, implementationType, ServiceLifetime.Transient)));
    }
}