using JetBrains.Annotations;
using Mirage.Server.Players;
using Mirage.Shared.Data;

namespace Mirage.Server.Chat.Commands;

/// <summary>
/// Base class for all chat commands.
/// </summary>
/// <param name="name">The name of the command.</param>
/// <param name="minimumAccessLevel">The minimum access level required to execute the command.</param>
[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public abstract class Command(string name, AccessLevel minimumAccessLevel)
{
    public string Name { get; } = name.ToLowerInvariant();
    public AccessLevel MinimumAccessLevel { get; } = minimumAccessLevel;

    public abstract void Execute(Player player, ReadOnlySpan<char> args);
}