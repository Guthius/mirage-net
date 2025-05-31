using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Mirage.Server.Chat.Commands;

namespace Mirage.Server.Chat;

public static class DependencyInjection
{
    public static void AddChatCommands(this IServiceCollection services)
    {
        var commandType = typeof(Command);
        var commandTypes = new List<Type>();

        foreach (var type in commandType.Assembly.GetTypes())
        {
            if (type is {IsClass: true, IsAbstract: false, IsPublic: true} && type.IsAssignableTo(commandType))
            {
                commandTypes.Add(type);
            }
        }

        var serviceDescriptors = commandTypes
            .Select(implementationType => new ServiceDescriptor(
                commandType, implementationType, ServiceLifetime.Transient));

        services.TryAddEnumerable(serviceDescriptors);
    }
}