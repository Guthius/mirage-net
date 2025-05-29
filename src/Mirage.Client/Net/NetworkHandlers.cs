using CommunityToolkit.Mvvm.DependencyInjection;
using Mirage.Client.Assets;
using Mirage.Client.Localization;
using Mirage.Client.Scenes;
using Mirage.Net.Protocol.FromServer.New;
using Mirage.Shared.Data;

namespace Mirage.Client.Net;

public static class NetworkHandlers
{
    private static readonly Game GameState = Ioc.Default.GetRequiredService<Game>();
    private static readonly ISceneManager SceneManager = Ioc.Default.GetRequiredService<ISceneManager>();

    public static void HandleCreateAccount(CreateAccountResponse response)
    {
        GameState.ClearStatus();

        switch (response.Result)
        {
            case CreateAccountResult.Ok:
                GameState.ShowAlert(SR.AccountCreated);
                SceneManager.SwitchTo<CharacterSelectScene>();
                break;

            case CreateAccountResult.AccountNameInvalid:
                GameState.ShowAlert("Invalid name, only letters, numbers, spaces, and _ allowed in names.");
                break;

            case CreateAccountResult.AccountNameOrPasswordTooShort:
                GameState.ShowAlert("Invalid account name, only letters, numbers, spaces, and _ allowed in names.");
                break;

            case CreateAccountResult.AccountNameTaken:
                GameState.ShowAlert("This account name is already taken. Please choose a different name.");
                break;

            default:
                GameState.ShowAlert(SR.UnknownError);
                break;
        }
    }

    public static void HandleDeleteAccount(DeleteAccountResponse response)
    {
        switch (response.Result)
        {
            case DeleteAccountResult.Ok:
                GameState.ShowAlert(SR.AccountDeleted);
                break;

            case DeleteAccountResult.InvalidAccountNameOrPassword:
                GameState.ShowAlert("Invalid account name or password.");
                break;

            case DeleteAccountResult.AccountNameOrPasswordTooShort:
                GameState.ShowAlert("Account name and password must each contain at least 3 characters");
                break;

            default:
                GameState.ShowAlert(SR.UnknownError);
                break;
        }

        SceneManager.SwitchTo<MainMenuScene>();
    }

    public static void HandleAuth(AuthResponse response)
    {
        GameState.ClearStatus();

        switch (response.Result)
        {
            case AuthResult.Ok:
                break;

            case AuthResult.InvalidAccountNameOrPassword:
                GameState.ShowAlert("Incorrect account name or password.");
                break;

            case AuthResult.InvalidProtocolVersion:
                GameState.ShowAlert("Your client is out of date. Please update your client and try again.");
                break;

            case AuthResult.AlreadyLoggedIn:
                GameState.ShowAlert("Account is already logged in.");
                break;

            default:
                GameState.ShowAlert(SR.UnknownError);
                break;
        }
    }

    public static void HandleUpdateJobList(UpdateJobListCommand command)
    {
        GameState.Jobs = command.Jobs;
    }

    public static void HandleUpdateCharacterList(UpdateCharacterListCommand command)
    {
        GameState.MaxCharacters = command.MaxCharacters;
        GameState.Characters = command.Characters;

        SceneManager.SwitchTo<CharacterSelectScene>();
    }

    public static void HandleCreateCharacter(CreateCharacterResponse response)
    {
        GameState.ClearStatus();

        switch (response.Result)
        {
            case CreateCharacterResult.Ok:
                SceneManager.SwitchTo<CharacterSelectScene>();
                break;

            case CreateCharacterResult.CharacterNameInvalid:
                GameState.ShowAlert("Invalid name, only letters, numbers, spaces, and _ allowed in names.");
                break;

            case CreateCharacterResult.CharacterNameTooShort:
                GameState.ShowAlert("Character name must be at least three characters in length.");
                break;

            case CreateCharacterResult.CharacterNameInUse:
                GameState.ShowAlert("Sorry, but that name is in use!");
                break;

            case CreateCharacterResult.InvalidJob:
                GameState.ShowAlert("Invalid character job.");
                break;

            default:
                GameState.ShowAlert(SR.UnknownError);
                break;
        }
    }

    public static void HandleSelectCharacter(SelectCharacterResponse response)
    {
        GameState.ClearStatus();

        switch (response.Result)
        {
            case SelectCharacterResult.Ok:
                GameState.SetStatus("Entering game...");
                GameState.LocalPlayerId = response.PlayerId;
                SceneManager.SwitchTo<LoadingScene>();
                return;

            case SelectCharacterResult.InvalidCharacter:
                GameState.ShowAlert("Invalid character.");
                return;

            default:
                GameState.ShowAlert(SR.UnknownError);
                return;
        }
    }

    public static void HandleLoadMap(LoadMapCommand command)
    {
        GameState.Map.Load(command.MapId);
    }

    public static void HandleEnterGame(EnterGameCommand command)
    {
        SceneManager.SwitchTo<GameScene>();
    }

    public static void HandleCreateActor(CreateActorCommand command)
    {
        var player = GameState.Map.CreateActor(
            command.ActorId,
            command.Name,
            command.Sprite,
            command.IsPlayerKiller,
            command.AccessLevel,
            command.X,
            command.Y,
            command.Direction,
            command.MaxHealth,
            command.Health,
            command.MaxMana,
            command.Mana,
            command.MaxStamina,
            command.Stamina);

        if (player.IsLocalPlayer)
        {
            GameState.LocalPlayer = player;
        }
    }

    public static void HandleDestroyActor(DestroyActorCommand command)
    {
        GameState.Map.DestroyActor(command.ActorId);
    }

    public static void HandleUpdateActorVitals(UpdateActorVitalsCommand command)
    {
        var actor = GameState.Map.GetActor(command.ActorId);
        if (actor is null)
        {
            return;
        }

        actor.MaxHealth = command.MaxHealth;
        actor.Health = command.Health;
        actor.MaxMana = command.MaxMana;
        actor.Mana = command.Mana;
        actor.MaxStamina = command.MaxStamina;
        actor.Stamina = command.Stamina;
    }

    public static void HandleActorMove(ActorMoveCommand command)
    {
        var actor = GameState.Map.GetActor(command.ActorId);

        actor?.QueueMove(command.Direction, command.MovementType);
    }

    public static void HandleActorAttack(ActorAttackCommand command)
    {
        var actor = GameState.Map.GetActor(command.ActorId);

        actor?.QueueAttack();
    }

    public static void HandleSetActorAccessLevel(SetActorAccessLevelCommand command)
    {
        var actor = GameState.Map.GetActor(command.ActorId);
        if (actor is null)
        {
            return;
        }

        actor.AccessLevel = command.AccessLevel;
    }

    public static void HandleSetActorDirection(SetActorDirectionCommand command)
    {
        var actor = GameState.Map.GetActor(command.ActorId);

        actor?.SetDirection(command.Direction);
    }

    public static void HandleSetActorPosition(SetActorPositionCommand command)
    {
        var actor = GameState.Map.GetActor(command.ActorId);

        actor?.SetPosition(
            command.Direction,
            command.X,
            command.Y);
    }

    public static void HandleSetActorSprite(SetActorSpriteCommand command)
    {
        var actor = GameState.Map.GetActor(command.ActorId);
        if (actor is null)
        {
            return;
        }

        actor.Sprite = command.Sprite;
    }

    public static void HandleCreateItem(CreateItemCommand command)
    {
        GameState.Map.CreateItem(
            command.Id,
            command.Sprite,
            command.X,
            command.Y);
    }

    public static void HandleDestroyItem(DestroyItemCommand command)
    {
        GameState.Map.DestroyItem(command.Id);
    }

    public static void HandleChat(ChatCommand command)
    {
        GameState.ChatHistory.Add(new ChatInfo(command.Message, command.Color));
        GameState.ChatHistoryUpdated = true;
    }

    public static void HandleDownloadAssetChunk(DownloadAssetChunkCommand command)
    {
        AssetDownloader.WriteChunk(command.Handle, command.Data);
    }

    public static void HandleDownloadAssetResponse(DownloadAssetResponse response)
    {
        AssetDownloader.End(response.Handle);
    }

    public static void HandleDisconnect(DisconnectCommand command)
    {
        Network.Disconnect();

        GameState.ShowAlert(command.Message);
        GameState.ClearStatus();

        SceneManager.SwitchTo<MainMenuScene>();
    }
}