using SFML.Audio;

namespace Mirage.Client.Services;

public static class SoundService
{
    private static Music? _music;

    public static void PlayOgg(string song)
    {
        StopMusic();

        var path = Path.Combine("Assets", "Music", song);
        if (!File.Exists(path))
        {
            return;
        }

        _music = new Music(path);
        _music.Volume = 40;
        _music.Play();
    }

    public static void StopMusic()
    {
        _music?.Stop();
        _music = null;
    }
}