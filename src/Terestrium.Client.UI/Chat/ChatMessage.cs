using SFML.Graphics;

namespace Terestrium.Client.UI.Chat;

public sealed class ChatMessage : Drawable
{
    private static readonly Font Font = new("Content/Skins/Default/Fonts/LiberationSans-Regular.ttf");
    
    private readonly Text _text;

    public int Height { get; }

    public ChatMessage(string message, Color color)
    {
        _text = new Text();
        _text.Font = Font;
        _text.DisplayedString = message;
        _text.CharacterSize = 14;
        _text.FillColor = color;

        Height = 15;
    }

    public void Draw(RenderTarget target, RenderStates states)
    {
        _text.Draw(target, states);
    }
}