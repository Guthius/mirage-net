namespace Terestrium.Client.Core.Scenes;

public interface ISceneManager
{
    IScene? Current { get; }
    TScene SwitchTo<TScene>() where TScene : class, IScene;
}