namespace Mirage.Client.Scenes;

public interface ISceneManager
{
    IScene? Current { get; }
    void SwitchTo<TScene>() where TScene : class, IScene;
}