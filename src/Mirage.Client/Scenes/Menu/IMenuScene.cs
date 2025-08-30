using Mirage.Shared.Data;
using SFML.Graphics;

namespace Mirage.Client.Scenes.Menu;

public interface IMenuScene : IScene
{
    void ShowAlert(string alertMessage);
    void ShowStatus(string status, Color? color = null);
    
    void ShowMainMenu();
    void ShowLogin();
    void ShowCreateAccount();
    void ShowDeleteAccount();
    void ShowCharacterSelect();
    void ShowCharacterSelect(List<CharacterSlotInfo> characterSlotInfos, int maxCharacters);
    void ShowCharacterCreation();
    void Quit();
    
    void Login(string accountName, string password);
    void CreateAccount(string accountName, string password);
    void DeleteAccount(string accountName, string password);
}