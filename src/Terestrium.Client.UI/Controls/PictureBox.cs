using SFML.Graphics;

namespace Terestrium.Client.UI.Controls;

public sealed class PictureBox : Control
{
    private readonly Sprite _sprite = new();
    private string _image = string.Empty;
    private bool _mustUpdate;
    
    public string Image
    {
        get => _image;
        set
        {
            if (_image == value)
            {
                return;
            }

            _image = value;
            _mustUpdate = true;
        }
    }

    public override void Draw(RenderTarget target, RenderStates states)
    {
        if (_mustUpdate)
        {
            Update();

            _mustUpdate = false;
        }

        if (_sprite.Texture is null)
        {
            return;
        }
        
        states.Transform.Translate(Position.X, Position.Y);
        
        target.Draw(_sprite, states);
    }

    private void Update()
    {
        if (string.IsNullOrEmpty(Image))
        {
            _sprite.Texture = null;
            return;
        }
        
        try
        {
            if (!File.Exists(Image))
            {
                _sprite.Texture = null;
                return;
            }

            _sprite.Texture = new Texture(Image);
        }
        catch
        {
            // If loading fails for any reason, clear the texture so it simply renders nothing
            _sprite.Texture = null;
        }
    }
}