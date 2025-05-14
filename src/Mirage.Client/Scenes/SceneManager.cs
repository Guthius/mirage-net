using Microsoft.Extensions.DependencyInjection;

namespace Mirage.Client.Scenes;

internal sealed class SceneManager(IServiceProvider serviceProvider) : ISceneManager
{
    private readonly Dictionary<int, IScene> _scenes = new();

    public IScene? Current { get; private set; }

    private IScene ConstructScene<TScene>() where TScene : IScene
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

    public void SwitchTo<TScene>() where TScene : class, IScene
    {
        var hash = HashCode.Combine(typeof(TScene).FullName);
        
        if (!_scenes.TryGetValue(hash, out var scene))
        {
            _scenes[hash] = scene = ConstructScene<TScene>();
        }

        Show(scene);
    }
}