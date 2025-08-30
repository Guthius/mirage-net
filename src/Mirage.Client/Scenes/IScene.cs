using SFML.Graphics;
using SFML.Window;

namespace Mirage.Client.Scenes;

public interface IScene : Drawable, IDisposable
{
    void Update(float dt);
    void Show();
    void Hide();

    void HandleMouseButtonPressed(MouseButtonEventArgs e);
    void HandleMouseButtonReleased(MouseButtonEventArgs e);
    void HandleMouseMoved(MouseMoveEventArgs e);
    void HandleTextEntered(TextEventArgs e);
    void HandleKeyPressed(KeyEventArgs e);
    void HandleKeyReleased(KeyEventArgs e);
}