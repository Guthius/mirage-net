namespace Mirage.Client.Scenes;

public interface ISceneManager
{
    IScene? Current { get; }
    TScene SwitchTo<TScene>() where TScene : class, IScene;
}