using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace Mirage.Client.Scenes;

public abstract class Scene : IScene
{
    private KeyboardState _oldKeyState;
    private KeyboardState _currentKeyState;

    protected bool IsKeyPressed(Keys key)
    {
        return _currentKeyState.IsKeyDown(key) && _oldKeyState.IsKeyUp(key);
    }
    
    public void Update(GameTime gameTime)
    {
        _currentKeyState = Keyboard.GetState();
        
        OnUpdate(gameTime);
        
        _oldKeyState =  Keyboard.GetState();
    }
    
    public void Show()
    {
        OnShow();
    }

    public void Hide()
    {
        OnHide();
    }
    
    public virtual void Draw(GameTime gameTime)
    {
    }

    public virtual void DrawUI(GameTime gameTime)
    {
    }

    protected virtual void OnShow()
    {
    }

    protected virtual void OnHide()
    {
    }

    protected virtual void OnUpdate(GameTime gameTime)
    {
    }
}