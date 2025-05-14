using Microsoft.Xna.Framework;

namespace Mirage.Client.Scenes;

public interface IScene
{
    void Update(GameTime gameTime);
    void Show();
    void Hide();
    void Draw(GameTime gameTime);
    void DrawUI(GameTime gameTime);
}