using Microsoft.Extensions.DependencyInjection;

namespace Mirage.Client.Scenes;

internal sealed class SceneManager(IServiceProvider serviceProvider) : ISceneManager
{
    public IScene? Current { get; private set; }

    private TScene ConstructScene<TScene>() where TScene : IScene
    {
        return serviceProvider.GetRequiredService<TScene>();
    }

    private void Show(IScene scene)
    {
        if (Current == scene)
        {
            return;
        }

        Current?.Hide();
        Current = scene;
        Current.Show();
    }

    public TScene SwitchTo<TScene>() where TScene : class, IScene
    {
        var scene = ConstructScene<TScene>();

        Show(scene);

        return scene;
    }
}