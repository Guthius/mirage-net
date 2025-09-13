using Terestrium.Client.UI.Controls;

namespace Terestrium.Client.Windows;

internal sealed class WinConfirm : WinBase
{
    private readonly Label _label;
    private Action? _action;

    public WinConfirm(Window window) : base(window)
    {
        var dialog = Get<Window>("ConfirmWindow");

        _label = dialog.Get<Label>("ConfirmLabel");

        dialog.Get<Button>("ConfirmButton").Click += (_, _) =>
        {
            _action?.Invoke();

            Hide();
        };

        dialog.Get<Button>("CancelButton").Click += (_, _) => Hide();
    }

    public void Show(string message, Action action)
    {
        _label.Text = message;
        _action = action;

        Show();
    }

    protected override void OnShown()
    {
        MoveToFront();
    }
}