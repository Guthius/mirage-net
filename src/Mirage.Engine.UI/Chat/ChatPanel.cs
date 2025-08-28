using Mirage.Engine.UI.Controls;
using Mirage.Engine.UI.Styles;
using SFML.Graphics;
using SFML.System;

namespace Mirage.Engine.UI.Chat;

public sealed class ChatPanel : Control
{
    private const int MaxMessages = 1000;
    private const int ScrollbarWidth = 20;
    private const float ScrollSpeed = 20f;

    private readonly VScroll _scrollBar;
    private readonly Lock _messageLock = new();
    private readonly LinkedList<ChatMessage> _messages = [];
    private RenderTexture? _messageTexture;
    private int _messageHeight;

    public ChatPanel()
    {
        _scrollBar = new VScroll(Style.Empty);

        Add(_scrollBar);
    }

    protected override void OnLayout()
    {
        _scrollBar.Position = new Vector2f(Size.X - ScrollbarWidth, 0);
        _scrollBar.Size = new Vector2i(ScrollbarWidth, Size.Y);
    }

    public override void Draw(RenderTarget target, RenderStates states)
    {
        _scrollBar.MaxValue = _messageHeight - Size.Y;
        _scrollBar.Enabled = _scrollBar.MaxValue > 0;

        DrawMessagesToTexture();

        target.Draw(
            new Sprite(_messageTexture!.Texture)
            {
                Position = Position
            },
            states);

        base.Draw(target, states);
    }

    private void DrawMessagesToTexture()
    {
        _messageTexture ??= new RenderTexture((uint) (Size.X - ScrollbarWidth), (uint) Size.Y);
        _messageTexture.Clear(Color.Transparent);

        var states = RenderStates.Default;

        states.Transform.Translate(0, -_scrollBar.Value);

        for (var node = _messages.First; node != null; node = node.Next)
        {
            var message = node.Value;

            message.Draw(_messageTexture, states);

            states.Transform.Translate(0, message.Height);
        }

        _messageTexture.Display();
    }

    public void AddChatMessage(string message, Color color)
    {
        lock (_messageLock)
        {
            while (_messages.Count >= MaxMessages)
            {
                _messages.RemoveFirst();
            }

            _messages.AddLast(new ChatMessage(message, color));
            _messageHeight = _messages.Sum(x => x.Height);
        }
    }

    public void HandleScroll(float delta)
    {
        _scrollBar.Value = (int) (_scrollBar.Value - delta * ScrollSpeed);
    }

    public void ScrollToTop()
    {
        _scrollBar.Value = 0;
    }

    public void ScrollToBottom()
    {
        _scrollBar.Value = _scrollBar.MaxValue;
    }

    public void ScrollToPosition(int position)
    {
        _scrollBar.Value = position;
    }
}