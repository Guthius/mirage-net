using Mirage.Client.Localization;
using Mirage.Engine.UI.Controls;
using SFML.Graphics;
using SFML.System;

namespace Mirage.Client.UI;

public sealed class ConfirmWindow : Frame
{
    public event Action<bool>? Confirm;

    public ConfirmWindow(string message)
    {
        Width = 800;
        Height = 600;
        BackColor = new Color(0, 0, 0, 128);

        var window = new Window(TempStyle.Style)
        {
            Width = 340,
            Height = 120,
            Text = SR.Confirm
        };

        window.Add(new Label(TempStyle.Style)
        {
            Position = new Vector2f(15, 35),
            Width = 310,
            Height = 50,
            Text = message,
            HorizontalAlignment = HorizontalAlignment.Center
        });

        var confirmButton = new Button(TempStyle.Style)
        {
            Position = new Vector2f(155, window.Height - 40),
            Width = 80,
            Height = 25,
            Text = SR.Confirm
        };

        confirmButton.Click += () => Confirm?.Invoke(true);

        window.Add(confirmButton);

        var cancelButton = new Button(TempStyle.Style)
        {
            Position = new Vector2f(245, window.Height - 40),
            Width = 80,
            Height = 25,
            Text = SR.Cancel
        };

        cancelButton.Click += () => Confirm?.Invoke(false);

        window.Add(cancelButton);

        Add(window);

        window.MoveToCenter();
    }
}