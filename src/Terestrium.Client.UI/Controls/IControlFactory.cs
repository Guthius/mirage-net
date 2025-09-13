using System.Xml;

namespace Terestrium.Client.UI.Controls;

public interface IControlFactory<out TControl> where TControl : Control
{
    TControl Create(XmlReader xmlReader, Control? parent);
}