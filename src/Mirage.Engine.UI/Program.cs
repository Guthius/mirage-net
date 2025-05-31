using Mirage.Engine.UI.Chat;
using Mirage.Engine.UI.Controls;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

var renderWindow = new RenderWindow(new VideoMode(800, 600), "UI Test");

var ui = new Control {Width = 800, Height = 600};

var chatWindow = new ChatPanel {Position = new Vector2f(25, 25), Width = 400, Height = 200};
for (var i = 0; i < 5; i++)
{
    chatWindow.AddChatMessage($"Message {i + 1}", Color.White);
}

chatWindow.ScrollToBottom();

ui.Add(chatWindow);

renderWindow.Closed += (_, _) => renderWindow.Close();
renderWindow.MouseButtonPressed += (_, e) => ui.HandleMouseButtonPressed(e.X, e.Y, e.Button);
renderWindow.MouseButtonReleased += (_, e) => ui.HandleMouseButtonReleased(e.X, e.Y, e.Button);
renderWindow.MouseMoved += (_, e) => ui.HandleMouseMoved(e.X, e.Y);
renderWindow.TextEntered += (_, e) => ui.HandleTextEntered(e);
renderWindow.KeyPressed += (_, e) => ui.HandleKeyPressed(e);

while (renderWindow.IsOpen)
{
    renderWindow.DispatchEvents();
    renderWindow.Clear();
    renderWindow.Draw(ui);
    renderWindow.Display();
}