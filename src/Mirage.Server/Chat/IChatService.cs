using Mirage.Server.Players;

namespace Mirage.Server.Chat;

public interface IChatService
{
    void Handle(Player player, ReadOnlySpan<char> message);
}