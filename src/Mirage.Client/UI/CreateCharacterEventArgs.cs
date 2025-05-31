using Mirage.Shared.Data;

namespace Mirage.Client.UI;

public sealed record CreateCharacterEventArgs(string Name, Gender Gender, string JobId);