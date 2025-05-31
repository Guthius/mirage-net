using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Mirage.Engine.UI.Controls;

public class Control : Transformable, Drawable
{
    private readonly List<Control> _children = [];
    private Control? _mouseTarget;
    private Control? _activeControl;
    private bool _hot;

    public int Width
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            field = value;

            OnSizeChanged();
        }
    }

    public int Height
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            field = value;

            OnSizeChanged();
        }
    }

    public bool Enabled { get; set; } = true;
    public Control? Parent { get; private set; }
    public Control? ActiveControl => Parent?.ActiveControl ?? _activeControl;
    public bool HasFocus => ActiveControl == this;
    public bool HasKeyboardFocus => ActiveControl is not null;
    public bool Visible { get; set; } = true;


    public Font? Font { get; set; } = new("Content/Fonts/Coolvetica Rg.otf");
    public int FontSize { get; set; } = 14;

    public TControl? Get<TControl>() where TControl : Control
    {
        var child = _children.OfType<TControl>().FirstOrDefault();
        if (child is not null)
        {
            return child;
        }

        foreach (var control in _children)
        {
            var result = control.Get<TControl>();
            if (result is not null)
            {
                return result;
            }
        }

        return null;
    }

    protected bool TabStop { get; set; }

    public void Add(Control control)
    {
        control.Parent = this;

        _children.Add(control);

        control.OnLayout();
    }

    public void Remove(Control control)
    {
        if (_children.Remove(control))
        {
            control.Parent = null;
        }
    }

    protected virtual void OnLayout()
    {
        foreach (var child in _children)
        {
            child.OnLayout();
        }
    }

    public virtual void Draw(RenderTarget target, RenderStates states)
    {
        if (!Visible)
        {
            return;
        }

        states.Transform *= Transform;

        DrawChildren(target, states);
    }

    protected void DrawChildren(RenderTarget target, RenderStates states)
    {
        foreach (var child in _children.Where(x => x.Visible))
        {
            child.Draw(target, states);
        }
    }

    public Control? GetChildAt(int x, int y)
    {
        return _children.FirstOrDefault(child => child.Contains(x, y));
    }

    public IEnumerable<T> Children<T>() where T : Control
    {
        return _children.OfType<T>();
    }

    protected void CaptureMouse()
    {
        SetMouseTarget(this);
    }

    protected void ReleaseMouse()
    {
        SetMouseTarget(null);
    }

    private void SetMouseTarget(Control? control)
    {
        if (Parent is null)
        {
            _mouseTarget = control;

            return;
        }

        Parent.SetMouseTarget(control);
    }

    protected bool Contains(int x, int y)
    {
        return x >= Position.X && x < Position.X + Width && y >= Position.Y && y < Position.Y + Height;
    }

    protected Vector2f GetGlobalPosition()
    {
        var pt = Position;

        for (var parent = Parent; parent is not null; parent = parent.Parent)
        {
            pt += parent.Position;
        }

        return pt;
    }

    private void FindFocusTarget(int x, int y)
    {
        if (Parent is not null) // Focus is handled by the root control
        {
            return;
        }

        var target = GetControlAt(this, x, y);

        Activate(target);

        static Control? GetControlAt(Control parent, int x, int y)
        {
            var child = parent._children.FirstOrDefault(control => control.Contains(x, y));
            if (child is null || !child.Visible)
            {
                return null;
            }

            var lx = x - (int) child.Position.X;
            var ly = y - (int) child.Position.Y;

            return GetControlAt(child, lx, ly) ?? child;
        }
    }

    public void HandleMouseButtonPressed(int x, int y, Mouse.Button button)
    {
        if (_mouseTarget is not null)
        {
            var pt = _mouseTarget.GetGlobalPosition();

            _mouseTarget.OnMousePressed(x - (int) pt.X, y - (int) pt.Y, button);
            return;
        }

        if (button == Mouse.Button.Left)
        {
            FindFocusTarget(x, y);
        }

        OnMousePressed(x - (int) Position.X, y - (int) Position.Y, button);

        var lx = x - (int) Position.X;
        var ly = y - (int) Position.Y;

        for (var i = _children.Count - 1; i >= 0; i--)
        {
            var child = _children[i];
            if (!child.Contains(lx, ly))
            {
                continue;
            }

            child.HandleMouseButtonPressed(lx, ly, button);
            break;
        }
    }

    public void HandleMouseButtonReleased(int x, int y, Mouse.Button button)
    {
        if (_mouseTarget is not null)
        {
            var pt = _mouseTarget.GetGlobalPosition();

            _mouseTarget.OnMouseReleased(x - (int) pt.X, y - (int) pt.Y, button);
            return;
        }

        OnMouseReleased(x, y, button);

        var lx = x - (int) Position.X;
        var ly = y - (int) Position.Y;

        for (var i = _children.Count - 1; i >= 0; i--)
        {
            var child = _children[i];
            if (!child.Contains(lx, ly))
            {
                continue;
            }

            child.HandleMouseButtonReleased(lx, ly, button);
            break;
        }
    }

    public void HandleMouseMoved(int x, int y)
    {
        if (_mouseTarget is not null)
        {
            var pt = _mouseTarget.GetGlobalPosition();

            _mouseTarget.OnMouseMove(x - (int) pt.X, y - (int) pt.Y);
            return;
        }

        var hot = Contains(x, y);
        if (hot != _hot)
        {
            switch (hot)
            {
                case true:
                    OnMouseEnter();
                    break;

                case false:
                    OnMouseLeave();
                    break;
            }
        }

        _hot = hot;
        if (_hot)
        {
            OnMouseMove(x, y);
        }

        var lx = x - (int) Position.X;
        var ly = y - (int) Position.Y;

        foreach (var child in _children)
        {
            child.HandleMouseMoved(lx, ly);
        }
    }

    protected virtual void OnMouseMove(int x, int y)
    {
    }

    protected virtual void OnMouseEnter()
    {
    }

    protected virtual void OnMouseLeave()
    {
    }

    protected virtual void OnMousePressed(int x, int y, Mouse.Button button)
    {
    }

    protected virtual void OnMouseReleased(int x, int y, Mouse.Button button)
    {
    }

    protected virtual void OnSizeChanged()
    {
        OnLayout();
    }

    private void Activate(Control? control)
    {
        if (Parent is not null)
        {
            Parent.Activate(control);

            return;
        }

        if (_activeControl == control)
        {
            return;
        }

        _activeControl?.OnBlur();
        _activeControl = control;
        _activeControl?.OnFocus();
    }

    public event Action? Blur;

    protected virtual void OnBlur()
    {
        Blur?.Invoke();
    }

    protected virtual void OnFocus()
    {
    }

    public void Focus()
    {
        Activate(this);
    }

    public void ClearFocus()
    {
        _activeControl?.OnBlur();
        _activeControl = null;
    }

    public void HandleKeyPressed(KeyEventArgs e)
    {
        if (e.Code == Keyboard.Key.Tab)
        {
            if (e.Shift)
            {
                MoveToPreviousTabStop();
            }
            else
            {
                MoveToNextTabStop();
            }

            return;
        }

        _activeControl?.OnKeyPressed(e.Control, e.Alt, e.Shift, e.Code);
    }

    public void HandleTextEntered(TextEventArgs e)
    {
        _activeControl?.OnTextEntered(e.Unicode);
    }

    protected virtual void OnTextEntered(string character)
    {
    }

    protected virtual void MoveToNextTabStop()
    {
        if (_activeControl is null)
        {
            return;
        }

        var parent = _activeControl.Parent;
        if (parent is null)
        {
            return;
        }

        var index = parent._children.IndexOf(_activeControl);
        if (index == -1)
        {
            return;
        }

        for (var i = index + 1; i < parent._children.Count; i++)
        {
            if (!parent._children[i].Visible || !parent._children[i].TabStop)
            {
                continue;
            }

            Activate(parent._children[i]);
            return;
        }

        for (var i = 0; i < index; i++)
        {
            if (!parent._children[i].TabStop)
            {
                continue;
            }

            Activate(parent._children[i]);
            return;
        }
    }

    protected virtual void MoveToPreviousTabStop()
    {
        if (_activeControl is null)
        {
            return;
        }

        var parent = _activeControl.Parent;
        if (parent is null)
        {
            return;
        }

        var index = parent._children.IndexOf(_activeControl);
        if (index == -1)
        {
            return;
        }

        for (var i = index - 1; i >= 0; i--)
        {
            if (!parent._children[i].Visible || !parent._children[i].TabStop)
            {
                continue;
            }

            Activate(parent._children[i]);
            return;
        }

        for (var i = parent._children.Count - 1; i > index; i--)
        {
            if (!parent._children[i].Visible || !parent._children[i].TabStop)
            {
                continue;
            }

            Activate(parent._children[i]);
            return;
        }
    }

    protected virtual void OnKeyPressed(bool control, bool alt, bool shift, Keyboard.Key key)
    {
    }
}