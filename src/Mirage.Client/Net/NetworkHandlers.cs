using CommunityToolkit.Mvvm.DependencyInjection;
using Mirage.Client.Assets;
using Mirage.Client.Inventory;
using Mirage.Client.Localization;
using Mirage.Client.Scenes;
using Mirage.Net.Protocol.FromServer;
using SFML.Graphics;

namespace Mirage.Client.Net;

public static class NetworkHandlers
{
    private static readonly Game Game = Ioc.Default.GetRequiredService<Game>();
    private static readonly ISceneManager SceneManager = Ioc.Default.GetRequiredService<ISceneManager>();

    public static void HandleCreateAccount(CreateAccountResponse response)
    {
        switch (response.Result)
        {
            case CreateAccountResult.Ok:
                Game.ShowAlert(SR.AccountCreated);
                SceneManager.SwitchTo<CharacterSelectScene>();
                break;

            case CreateAccountResult.AccountNameInvalid:
                Game.ShowAlert("Invalid name, only letters, numbers, spaces, and _ allowed in names.");
                break;

            case CreateAccountResult.AccountNameOrPasswordTooShort:
                Game.ShowAlert("Invalid account name, only letters, numbers, spaces, and _ allowed in names.");
                break;

            case CreateAccountResult.AccountNameTaken:
                Game.ShowAlert("This account name is already taken. Please choose a different name.");
                break;

            default:
                Game.ShowAlert(SR.UnknownError);
                break;
        }
    }

    public static void HandleDeleteAccount(DeleteAccountResponse response)
    {
        switch (response.Result)
        {
            case DeleteAccountResult.Ok:
                Game.ShowAlert(SR.AccountDeleted);
                break;

            case DeleteAccountResult.InvalidAccountNameOrPassword:
                Game.ShowAlert("Invalid account name or password.");
                break;

            case DeleteAccountResult.AccountNameOrPasswordTooShort:
                Game.ShowAlert("Account name and password must each contain at least 3 characters");
                break;

            default:
                Game.ShowAlert(SR.UnknownError);
                break;
        }

        SceneManager.SwitchTo<MainMenuScene>();
    }

    public static void HandleAuth(AuthResponse response)
    {
        switch (response.Result)
        {
            case AuthResult.Ok:
                break;

            case AuthResult.InvalidAccountNameOrPassword:
                Game.ShowAlert("Incorrect account name or password.");
                break;

            case AuthResult.InvalidProtocolVersion:
                Game.ShowAlert("Your client is out of date. Please update your client and try again.");
                break;

            case AuthResult.AlreadyLoggedIn:
                Game.ShowAlert("Account is already logged in.");
                break;

            default:
                Game.ShowAlert(SR.UnknownError);
                break;
        }
    }

    public static void HandleUpdateJobList(UpdateJobListCommand command)
    {
        Game.Jobs = command.Jobs;
    }

    public static void HandleUpdateCharacterList(UpdateCharacterListCommand command)
    {
        Game.MaxCharacters = command.MaxCharacters;
        Game.Characters = command.Characters;

        SceneManager.SwitchTo<CharacterSelectScene>();
    }

    public static void HandleCreateCharacter(CreateCharacterResponse response)
    {
        switch (response.Result)
        {
            case CreateCharacterResult.Ok:
                SceneManager.SwitchTo<CharacterSelectScene>();
                break;

            case CreateCharacterResult.CharacterNameInvalid:
                Game.ShowAlert("Invalid name, only letters, numbers, spaces, and _ allowed in names.");
                break;

            case CreateCharacterResult.CharacterNameTooShort:
                Game.ShowAlert("Character name must be at least three characters in length.");
                break;

            case CreateCharacterResult.CharacterNameInUse:
                Game.ShowAlert("Sorry, but that name is in use!");
                break;

            case CreateCharacterResult.CharacterLimitReached:
                Game.ShowAlert("You have reached the maximum number of characters.");
                break;

            case CreateCharacterResult.InvalidJob:
                Game.ShowAlert("Invalid character job.");
                break;

            default:
                Game.ShowAlert(SR.UnknownError);
                break;
        }
    }

    public static void HandleSelectCharacter(SelectCharacterResponse response)
    {
        switch (response.Result)
        {
            case SelectCharacterResult.Ok:
                Game.LocalPlayerId = response.PlayerId;
                SceneManager.SwitchTo<LoadingScene>();
                return;

            case SelectCharacterResult.InvalidCharacter:
                Game.ShowAlert("Invalid character.");
                return;

            default:
                Game.ShowAlert(SR.UnknownError);
                return;
        }
    }

    public static void HandleClearInventorySlot(ClearInventorySlotCommand command)
    {
        Game.Inventory.Clear(command.Slot);
    }

    public static void HandleUpdateEquipment(UpdateEquipmentCommand command)
    {
        Game.Inventory.Weapon = ToSlot(command.Weapon);
        Game.Inventory.Armor = ToSlot(command.Armor);
        Game.Inventory.Helmet = ToSlot(command.Helmet);
        Game.Inventory.Shield = ToSlot(command.Shield);

        static EquipmentSlot? ToSlot(UpdateEquipmentCommand.Slot? slot)
        {
            if (slot is null)
            {
                return null;
            }

            return new EquipmentSlot(
                slot.Sprite,
                slot.ItemName,
                slot.Damage,
                slot.Protection);
        }
    }

    public static void HandleUpdateInventory(UpdateInventoryCommand command)
    {
        Game.Inventory.Size = command.InventorySize;
    }

    public static void HandleUpdateInventorySlot(UpdateInventorySlotCommand command)
    {
        Game.Inventory.Update(
            command.SlotIndex,
            command.Type,
            command.Sprite,
            command.ItemName,
            command.Quantity);
    }

    public static void HandleUpdateInventorySlotQuantity(UpdateInventorySlotQuantityCommand command)
    {
        Game.Inventory.UpdateQuantity(
            command.SlotIndex,
            command.Quantity);
    }

    public static void HandleLoadMap(LoadMapCommand command)
    {
        Game.Map.Load(command.MapId);
    }

    public static void HandleEnterGame(EnterGameCommand command)
    {
        SceneManager.SwitchTo<GameScene>();
    }

    public static void HandleMoveMap(MoveMapResponse response)
    {
        if (response.Result == MoveMapResult.Failed)
        {
            Game.GettingMap = false;
        }
    }

    public static void HandleCreateActor(CreateActorCommand command)
    {
        var player = Game.Map.CreateActor(
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
            Game.LocalPlayer = player;
        }
    }

    public static void HandleDestroyActor(DestroyActorCommand command)
    {
        Game.Map.DestroyActor(command.ActorId);
    }

    public static void HandleUpdateActorVitals(UpdateActorVitalsCommand command)
    {
        var actor = Game.Map.GetActor(command.ActorId);
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
        var actor = Game.Map.GetActor(command.ActorId);

        actor?.QueueMove(command.Direction, command.MovementType);
    }

    public static void HandleActorAttack(ActorAttackCommand command)
    {
        var actor = Game.Map.GetActor(command.ActorId);

        actor?.QueueAttack();
    }

    public static void HandleSetActorAccessLevel(SetActorAccessLevelCommand command)
    {
        var actor = Game.Map.GetActor(command.ActorId);
        if (actor is null)
        {
            return;
        }

        actor.AccessLevel = command.AccessLevel;
    }

    public static void HandleSetActorDirection(SetActorDirectionCommand command)
    {
        var actor = Game.Map.GetActor(command.ActorId);

        actor?.SetDirection(command.Direction);
    }

    public static void HandleSetActorPlayerKiller(SetActorPlayerKillerCommand command)
    {
        var actor = Game.Map.GetActor(command.ActorId);
        if (actor is null)
        {
            return;
        }

        actor.IsPlayerKiller = command.PlayerKiller;
    }

    public static void HandleSetActorPosition(SetActorPositionCommand command)
    {
        var actor = Game.Map.GetActor(command.ActorId);

        actor?.SetPosition(
            command.Direction,
            command.X,
            command.Y);
    }

    public static void HandleSetActorSprite(SetActorSpriteCommand command)
    {
        var actor = Game.Map.GetActor(command.ActorId);
        if (actor is null)
        {
            return;
        }

        actor.Sprite = command.Sprite;
    }

    public static void HandleCreateItem(CreateItemCommand command)
    {
        Game.Map.CreateItem(
            command.Id,
            command.Sprite,
            command.X,
            command.Y);
    }

    public static void HandleDestroyItem(DestroyItemCommand command)
    {
        Game.Map.DestroyItem(command.Id);
    }

    public static void HandleChat(ChatCommand command)
    {
        if (SceneManager.Current is not GameScene gameScene)
        {
            return;
        }

        var chatMessage = command.Message;
        var chatMessageColor = new Color(
            command.Color.R,
            command.Color.G,
            command.Color.B,
            command.Color.A);


        gameScene.AddChatMessage(chatMessage, chatMessageColor);
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

        Game.ShowAlert(command.Message);

        SceneManager.SwitchTo<MainMenuScene>();
    }
}