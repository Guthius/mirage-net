using Mirage.Engine.UI.Controls;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Mirage.Client.Scenes;

public abstract class Scene : IScene
{
    private bool _disposed;

    public Control UI { get; } = new()
    {
        Size = new Vector2i(800, 600)
    };

    public void Show()
    {
        OnShow();
    }

    protected virtual void OnShow()
    {
    }

    public void Hide()
    {
        OnHide();
    }

    protected virtual void OnHide()
    {
    }

    public void ShowAlert(string alertMessage)
    {
        OnAlert(alertMessage);
    }

    protected virtual void OnAlert(string alertMessage)
    {
    }

    public void Update(float dt)
    {
        OnUpdate(dt);
    }

    protected virtual void OnUpdate(float dt)
    {
    }

    public void Draw(RenderTarget target, RenderStates states)
    {
        OnDraw(target, states);

        target.Draw(UI, states);
    }

    protected virtual void OnDraw(RenderTarget target, RenderStates states)
    {
    }

    public void HandleMouseButtonPressed(MouseButtonEventArgs e)
    {
        UI.HandleMouseButtonPressed(e.X, e.Y, e.Button);
    }

    public void HandleMouseButtonReleased(MouseButtonEventArgs e)
    {
        UI.HandleMouseButtonReleased(e.X, e.Y, e.Button);
    }

    public void HandleMouseMoved(MouseMoveEventArgs e)
    {
        UI.HandleMouseMoved(e.X, e.Y);
    }

    public void HandleTextEntered(TextEventArgs e)
    {
        UI.HandleTextEntered(e);
    }

    public void HandleKeyPressed(KeyEventArgs e)
    {
        if (UI.HasKeyboardFocus)
        {
            UI.HandleKeyPressed(e);

            return;
        }

        OnKeyPressed(e.Code);
    }

    protected virtual void OnKeyPressed(Keyboard.Key key)
    {
    }

    public void HandleKeyReleased(KeyEventArgs e)
    {
        if (UI.HasKeyboardFocus)
        {
            return;
        }

        OnKeyReleased(e.Code);
    }

    protected virtual void OnKeyReleased(Keyboard.Key key)
    {
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        GC.SuppressFinalize(this);

        _disposed = true;
    }
}