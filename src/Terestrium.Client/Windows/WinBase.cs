using Terestrium.Client.UI.Controls;

namespace Terestrium.Client.Windows;

internal abstract class WinBase(Window window)
{
    public Window Window => window;
    
    protected TControl Get<TControl>(string name) where TControl : Control
    {
        return window.Get<TControl>(name);
    }

    public void Show()
    {
        window.Visible = true;

        OnShown();
    }

    protected virtual void OnShown()
    {
    }

    protected void Hide()
    {
        window.Visible = false;
    }

    protected void MoveToCenter()
    {
        window.MoveToCenter();
    }

    protected void MoveToFront()
    {
        window.MoveToFront();
    }
}