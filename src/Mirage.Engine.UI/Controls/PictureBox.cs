using SFML.Graphics;

namespace Mirage.Engine.UI.Controls;

public sealed class PictureBox : Control
{
    private readonly Sprite _sprite = new();
    private bool _mustUpdate;
    
    public string Image
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            field = value;

            _mustUpdate = true;
        }
    } = string.Empty;

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
        
        states.Transform *= Transform;
        
        target.Draw(_sprite, states);
    }

    private void Update()
    {
        if (string.IsNullOrEmpty(Image))
        {
            _sprite.Texture = null;
            return;
        }
        
        _sprite.Texture = new Texture(Image);
    }
}