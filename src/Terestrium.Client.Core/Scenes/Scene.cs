using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Terestrium.Client.UI;

namespace Terestrium.Client.Core.Scenes;

public abstract class Scene : IScene
{
    private bool _disposed;

    protected UserInterface UI { get; } = new("Default")
    {
        Size = new Vector2i(1280, 720)
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
        if (!UI.HandleMouseButtonPressed(e.X, e.Y, e.Button))
        {
            OnMouseButtonPressed(e.X, e.Y, e.Button);
        }
    }

    public void HandleMouseButtonReleased(MouseButtonEventArgs e)
    {
        if (!UI.HandleMouseButtonReleased(e.X, e.Y, e.Button))
        {
            OnMouseButtonReleased(e.X, e.Y, e.Button);
        }
    }

    public void HandleMouseMoved(MouseMoveEventArgs e)
    {
        if (!UI.HandleMouseMoved(e.X, e.Y))
        {
            OnMouseMoved(e.X, e.Y);
        }
    }

    public void HandleMouseWheelScrolled(MouseWheelScrollEventArgs e)
    {
        // UI does not currently handle wheel scroll; forward directly to scene.
        OnMouseWheelScrolled(e.X, e.Y, e.Delta);
    }

    public void HandleTextEntered(TextEventArgs e)
    {
        if (!UI.HandleTextEntered(e))
        {
            OnTextEntered(e.Unicode);
        }
    }

    public void HandleKeyPressed(KeyEventArgs e)
    {
        // Try UI first; if not handled, pass to scene.
        if (UI.HandleKeyPressed(e))
        {
            return;
        }

        OnKeyPressed(e.Code);
    }

    protected virtual void OnKeyPressed(Keyboard.Key key)
    {
    }

    protected virtual void OnMouseButtonPressed(int x, int y, Mouse.Button button)
    {
    }

    protected virtual void OnMouseButtonReleased(int x, int y, Mouse.Button button)
    {
    }

    protected virtual void OnMouseMoved(int x, int y)
    {
    }

    protected virtual void OnMouseWheelScrolled(int x, int y, float delta)
    {
    }

    protected virtual void OnTextEntered(string text)
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