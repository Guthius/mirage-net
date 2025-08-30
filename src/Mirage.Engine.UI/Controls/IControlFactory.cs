using System.Xml;

namespace Mirage.Engine.UI.Controls;

public interface IControlFactory<out TControl> where TControl : Control
{
    TControl Create(XmlReader xmlReader, Control? parent);
}