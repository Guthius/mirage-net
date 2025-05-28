using System.Buffers;
using Microsoft.Extensions.Logging;
using Mirage.Net.Protocol.FromClient.New;
using Mirage.Net.Protocol.FromServer.New;
using Mirage.Server.Assets;
using Mirage.Server.Players;
using Mirage.Server.Repositories.Accounts;
using Mirage.Shared.Constants;

namespace Mirage.Server.Net;

public sealed partial class NetworkService
{
    private const int ProtocolVersion = 1;

    private void RegisterPackets()
    {
        // Authentication & Account Management
        _parser.Register<AuthRequest>(HandleAuth);
        _parser.Register<CreateAccountRequest>(HandleCreateAccount);
        _parser.Register<DeleteAccountRequest>(HandleDeleteAccount);

        // Character Management
        _parser.Register<CreateCharacterRequest>(HandleCreateCharacter);
        _parser.Register<DeleteCharacterRequest>(HandleDeleteCharacter);
        _parser.Register<SelectCharacterRequest>(HandleSelectCharacter);

        // Player Actions
        _parser.Register<MoveRequest>(HandleMove);
        _parser.Register<AttackRequest>(HandleAttack);
        _parser.Register<SetDirectionRequest>(HandleSetDirection);
        _parser.Register<SearchRequest>(HandleSearch);

        // Social
        _parser.Register<SayRequest>(HandleSay);

        // Asset Management
        _parser.Register<DownloadAssetRequest>(HandleDownloadAsset);
    }

    public void HandleAuth(NetworkConnection connection, AuthRequest request)
    {
        if (connection.Account is not null)
        {
            return;
        }

        if (request.ProtocolVersion != ProtocolVersion)
        {
            connection.Send(new AuthResponse(AuthResult.InvalidProtocolVersion));
            return;
        }

        var account = _accountRepository.Authenticate(request.AccountName, request.Password);
        if (account is null)
        {
            connection.Send(new AuthResponse(AuthResult.InvalidAccountNameOrPassword));
            return;
        }

        if (IsAccountLoggedIn(request.AccountName))
        {
            connection.Send(new AuthResponse(AuthResult.AlreadyLoggedIn));
            return;
        }

        connection.Account = account;
        connection.Send(new AuthResponse(AuthResult.Ok));
        connection.Send(new UpdateJobListCommand(_jobRepository.GetAll()));
        connection.Send(new UpdateCharacterListCommand(Limits.MaxCharacters, _characterRepository.GetCharacterList(account.Id)));

        _logger.LogInformation("Account {AccountName} has logged in from {RemoteIp}", account.Name, connection.Address);
    }

    public void HandleCreateAccount(NetworkConnection connection, CreateAccountRequest request)
    {
        if (connection.Account is not null)
        {
            return;
        }

        if (request.AccountName.Length < 3 || request.Password.Length < 3)
        {
            connection.Send(new CreateAccountResponse(CreateAccountResult.AccountNameOrPasswordTooShort));
            return;
        }

        foreach (var ch in request.AccountName)
        {
            if (char.IsLetterOrDigit(ch) || ch == '_' || ch == ' ')
            {
                continue;
            }

            connection.Send(new CreateAccountResponse(CreateAccountResult.AccountNameInvalid));
            return;
        }

        if (_accountRepository.Exists(request.AccountName))
        {
            connection.Send(new CreateAccountResponse(CreateAccountResult.AccountNameTaken));
            return;
        }

        var account = _accountRepository.Create(request.AccountName, request.Password);

        connection.Account = account;
        connection.Send(new CreateAccountResponse(CreateAccountResult.Ok));
        connection.Send(new UpdateJobListCommand(_jobRepository.GetAll()));
        connection.Send(new UpdateCharacterListCommand(Limits.MaxCharacters, _characterRepository.GetCharacterList(account.Id)));
    }

    public void HandleDeleteAccount(NetworkConnection connection, DeleteAccountRequest request)
    {
        if (connection.Account is not null)
        {
            return;
        }

        if (request.AccountName.Length < 3 || request.Password.Length < 3)
        {
            connection.Send(new DeleteAccountResponse(DeleteAccountResult.AccountNameOrPasswordTooShort));
            return;
        }

        var account = _accountRepository.Authenticate(request.AccountName, request.Password);
        if (account is null)
        {
            connection.Send(new DeleteAccountResponse(DeleteAccountResult.InvalidAccountNameOrPassword));
            return;
        }

        _accountRepository.Delete(account.Id);

        connection.Send(new DeleteAccountResponse(DeleteAccountResult.Ok));
    }

    public void HandleCreateCharacter(NetworkConnection connection, AccountInfo account, CreateCharacterRequest request)
    {
        var result = _characterRepository.Create(account.Id, request.CharacterName, request.Gender, request.JobId);

        connection.Send(new CreateCharacterResponse(result));
        if (result != CreateCharacterResult.Ok)
        {
            return;
        }

        _logger.LogInformation("Character {CharacterName} created by account {AccountName}", request.CharacterName, account.Name);

        connection.Send(new UpdateCharacterListCommand(Limits.MaxCharacters, _characterRepository.GetCharacterList(account.Id)));
    }

    public void HandleDeleteCharacter(NetworkConnection connection, AccountInfo account, DeleteCharacterRequest request)
    {
        _characterRepository.Delete(request.CharacterId, account.Id);

        _logger.LogInformation("Character deleted on account {AccountName}", account.Name);

        connection.Send(new UpdateCharacterListCommand(Limits.MaxCharacters, _characterRepository.GetCharacterList(account.Id)));
    }

    public void HandleSelectCharacter(NetworkConnection connection, AccountInfo account, SelectCharacterRequest request)
    {
        var character = _characterRepository.Get(request.CharacterId, account.Id);
        if (character is null)
        {
            connection.Send(new SelectCharacterResponse(SelectCharacterResult.InvalidCharacter, -1));
            return;
        }

        connection.Send(new SelectCharacterResponse(SelectCharacterResult.Ok, connection.Id));
        connection.Player = _playerService.Create(connection, character);
        if (connection.Player is null)
        {
            _logger.LogWarning(
                "Character {CharacterName} could not enter the game [Account: {AccountName}]",
                character.Name, account.Name);

            connection.Disconnect(
                "Your character is in an invalid state. " +
                "Please contact an administrator.");

            return;
        }

        _logger.LogInformation("Character {CharacterName} entered the game [Account: {AccountName}]",
            character.Name, account.Name);
    }

    public static void HandleMove(Player player, MoveRequest request)
    {
        if (player.CastedSpell)
        {
            if (Environment.TickCount > player.AttackTimer + 1000)
            {
                player.CastedSpell = false;
            }
            else
            {
                player.Send(new SetActorPositionCommand(
                    player.Id,
                    player.Character.Direction,
                    player.Character.X,
                    player.Character.Y));

                return;
            }
        }

        player.Map.Move(player, request.Direction, request.Movement);
    }

    public static void HandleAttack(Player player, AttackRequest request)
    {
        player.Map.Attack(player);
    }

    public static void HandleSetDirection(Player player, SetDirectionRequest request)
    {
        player.Character.Direction = request.Direction;
        player.Map.Send(new SetActorDirectionCommand(player.Id, player.Character.Direction),
            p => p.Id != player.Id);
    }

    public static void HandleSearch(Player player, SearchRequest request)
    {
        player.Map.LookAt(player, request.X, request.Y);
    }

    public void HandleSay(Player player, SayRequest request)
    {
        _chatService.Handle(player, request.Message);
    }

    public void HandleDownloadAsset(Player player, DownloadAssetRequest request)
    {
        const int chunkSize = 1024;

        var asset = AssetManager.Get(request.Hash);
        if (asset is null)
        {
            player.Send(new DownloadAssetResponse(request.Handle, DownloadAssetResult.NotFound));
            return;
        }

        _ = Task.Run(async () =>
        {
            var buffer = ArrayPool<byte>.Shared.Rent(chunkSize);
            try
            {
                int bytesRead;

                await using var stream = asset.OpenRead();
                while ((bytesRead = await stream.ReadAsync(buffer)) > 0)
                {
                    player.Send(new DownloadAssetChunkCommand(request.Handle, buffer.AsSpan(0, bytesRead).ToArray()));
                }

                player.Send(new DownloadAssetResponse(request.Handle, DownloadAssetResult.Ok));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending {Hash} to client [Hash: {Sha1Hash}]",
                    asset.Path, asset.Id);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        });
    }
}