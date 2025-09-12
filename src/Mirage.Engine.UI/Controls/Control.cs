using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Mirage.Engine.UI.Controls;

public class Control : Drawable
{
    private readonly List<Control> _children = [];
    private Control? _activeControl;
    private Control? _mouseTarget;
    private bool _mouseOver;

    public string Name { get; set; } = string.Empty;
    public Vector2i Position { get; set; }
    public Vector2i Size { get; set; }
    public bool Enabled { get; set; } = true;
    public Control? Parent { get; private set; }
    public Control? ActiveControl => Parent?.ActiveControl ?? _activeControl;
    public bool HasFocus => ActiveControl == this;
    public bool HasKeyboardFocus => ActiveControl is not null;
    public bool Visible { get; set; } = true;
    public Font? Font { get; set; }
    public int FontSize { get; set; } = 14;
    public object? Tag { get; set; }

    protected bool TabStop { get; set; }

    public void Add(Control control)
    {
        control.Parent = this;

        _children.Add(control);
    }

    public void Remove(Control control)
    {
        if (_children.Remove(control))
        {
            control.Parent = null;
        }
    }

    public virtual void Draw(RenderTarget target, RenderStates states)
    {
        if (!Visible)
        {
            return;
        }

        states.Transform.Translate(Position.X, Position.Y);

        DrawChildren(target, states);
    }

    protected void DrawChildren(RenderTarget target, RenderStates states)
    {
        foreach (var child in _children.Where(x => x.Visible))
        {
            child.Draw(target, states);
        }
    }

    internal void MoveToFront(Control control)
    {
        if (_children.Count < 2)
        {
            return;
        }

        var index = _children.IndexOf(control);
        if (index == -1)
        {
            return;
        }

        _children.RemoveAt(index);
        _children.Add(control);
    }

    public Control? GetChildAt(int x, int y)
    {
        for (var i = _children.Count - 1; i >= 0; i--)
        {
            var child = _children[i];
            if (!child.Visible)
            {
                continue;
            }

            var lx = x - child.Position.X;
            var ly = y - child.Position.Y;
            if (child.Contains(lx, ly))
            {
                return child;
            }
        }

        return null;
    }

    public IEnumerable<T> GetChildrenOfType<T>() where T : Control
    {
        return _children.OfType<T>();
    }

    public TControl Get<TControl>(string name) where TControl : Control
    {
        return _children.OfType<TControl>().First(control => control.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
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
        // x,y are expected to be in the local coordinate space of this control
        return x >= 0 && x < Size.X && y >= 0 && y < Size.Y;
    }

    public Vector2i PointToLocal(Vector2i pt)
    {
        pt -= Position;

        for (var parent = Parent; parent is not null; parent = parent.Parent)
        {
            pt -= parent.Position;
        }

        return pt;
    }

    public Vector2i PointToGlobal(Vector2i pt)
    {
        pt += Position;

        for (var parent = Parent; parent is not null; parent = parent.Parent)
        {
            pt += parent.Position;
        }

        return pt;
    }

    private void FindFocusTarget(Vector2i pt)
    {
        if (Parent is not null) // Focus is handled by the root control
        {
            return;
        }

        var target = GetControlAt(this, pt);

        Activate(target);

        static Control? GetControlAt(Control parent, Vector2i pt)
        {
            for (var i = parent._children.Count - 1; i >= 0; i--)
            {
                var child = parent._children[i];
                if (!child.Visible)
                {
                    continue;
                }

                var lpt = pt - child.Position;
                if (!child.Contains(lpt.X, lpt.Y))
                {
                    continue;
                }

                return GetControlAt(child, lpt) ?? child;
            }

            return null;
        }
    }

    public bool HandleMouseButtonPressed(int x, int y, Mouse.Button button)
    {
        if (_mouseTarget is not null)
        {
            var pt = new Vector2i(x, y);

            pt = _mouseTarget.PointToLocal(pt);

            _mouseTarget.OnMousePressed(pt.X, pt.Y, button);
            return true;
        }

        if (button == Mouse.Button.Left)
        {
            FindFocusTarget(new Vector2i(x, y));
        }

        var handled = OnMousePressed(x - Position.X, y - Position.Y, button);

        var lx = x - Position.X;
        var ly = y - Position.Y;

        var children = _children.Where(c => c.Visible).ToArray();
        for (var i = children.Length - 1; i >= 0; i--)
        {
            var child = children[i];
            if (!child.Contains(lx - child.Position.X, ly - child.Position.Y))
            {
                continue;
            }

            handled = child.HandleMouseButtonPressed(lx, ly, button) || handled;
            break;
        }

        return handled;
    }

    public bool HandleMouseButtonReleased(int x, int y, Mouse.Button button)
    {
        if (_mouseTarget is not null)
        {
            var pt = new Vector2i(x, y);

            pt = _mouseTarget.PointToLocal(pt);

            _mouseTarget.OnMouseReleased(pt.X, pt.Y, button);
            return true;
        }

        var handled = OnMouseReleased(x - Position.X, y - Position.Y, button);

        var lx = x - Position.X;
        var ly = y - Position.Y;

        var children = _children.Where(c => c.Visible).ToArray();
        for (var i = children.Length - 1; i >= 0; i--)
        {
            var child = children[i];
            if (!child.Contains(lx - child.Position.X, ly - child.Position.Y))
            {
                continue;
            }

            handled = child.HandleMouseButtonReleased(lx, ly, button) || handled;
            break;
        }

        return handled;
    }

    public bool HandleMouseMoved(int x, int y)
    {
        if (_mouseTarget is not null)
        {
            var pt = new Vector2i(x, y);

            pt = _mouseTarget.PointToLocal(pt);

            _mouseTarget.OnMouseMove(pt.X, pt.Y);
            return true;
        }

        var lx = x - Position.X;
        var ly = y - Position.Y;

        var hot = Contains(lx, ly);
        if (hot != _mouseOver)
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

        _mouseOver = hot;
        
        var handled = false;
        if (_mouseOver)
        {
            handled = OnMouseMove(lx, ly) || handled;
        }

        handled = _children.Where(c => c.Visible).Aggregate(handled, (current, child) => child.HandleMouseMoved(lx, ly) || current);

        return handled;
    }

    protected virtual bool OnMouseMove(int x, int y)
    {
        return false;
    }

    protected virtual void OnMouseEnter()
    {
    }

    protected virtual void OnMouseLeave()
    {
    }

    protected virtual bool OnMousePressed(int x, int y, Mouse.Button button)
    {
        return false;
    }

    protected virtual bool OnMouseReleased(int x, int y, Mouse.Button button)
    {
        return false;
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

    public bool HandleKeyPressed(KeyEventArgs e)
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

            return true;
        }

        if (_activeControl is not null)
        {
            return _activeControl.OnKeyPressed(e.Control, e.Alt, e.Shift, e.Code);
        }

        return false;
    }

    public bool HandleTextEntered(TextEventArgs e)
    {
        if (_activeControl is not null)
        {
            return _activeControl.OnTextEntered(e.Unicode);
        }

        return false;
    }

    protected virtual bool OnTextEntered(string character)
    {
        return false;
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

    protected virtual bool OnKeyPressed(bool control, bool alt, bool shift, Keyboard.Key key)
    {
        return false;
    }
}