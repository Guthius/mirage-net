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
        Size = new Vector2i(800, 600);
        BackColor = new Color(0, 0, 0, 128);

        var window = new Window(TempStyle.Style)
        {
            Size = new Vector2i(340, 120),
            Text = SR.Confirm
        };

        window.Add(new Label(TempStyle.Style)
        {
            Position = new Vector2f(15, 35),
            Size = new Vector2i(310, 50),
            Text = message,
            HorizontalAlignment = HorizontalAlignment.Center
        });

        var confirmButton = new Button(TempStyle.Style)
        {
            Position = new Vector2f(155, window.Size.Y - 40),
            Size = new Vector2i(80, 25),
            Text = SR.Confirm
        };

        confirmButton.Click += () => Confirm?.Invoke(true);

        window.Add(confirmButton);

        var cancelButton = new Button(TempStyle.Style)
        {
            Position = new Vector2f(245, window.Size.Y - 40),
            Size = new Vector2i(80, 25),
            Text = SR.Cancel
        };

        cancelButton.Click += () => Confirm?.Invoke(false);

        window.Add(cancelButton);

        Add(window);

        window.MoveToCenter();
    }
}