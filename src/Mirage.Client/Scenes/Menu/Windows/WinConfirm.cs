using Mirage.Engine.UI.Controls;

namespace Mirage.Client.Scenes.Menu.Windows;

internal sealed class WinConfirm
{
    private readonly Window _window;
    private readonly Label _label;
    private Action? _action;

    public WinConfirm(Window window)
    {
        _window = window;
        
        var innerWindow = window.Get<Window>("ConfirmWindow");
        
        _label = innerWindow.Get<Label>("ConfirmLabel");

        innerWindow.Get<Button>("ConfirmButton").Click += (_, _) =>
        {
            _action?.Invoke();
            _window.Visible = false;
        };

        innerWindow.Get<Button>("CancelButton").Click += (_, _) => _window.Visible = false;
    }

    public void Show(string message, Action action)
    {
        _label.Text = message;
        _action = action;
        _window.Visible = true;
        _window.MoveToFront();
    }
}