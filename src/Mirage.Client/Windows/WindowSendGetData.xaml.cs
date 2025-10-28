using System.Windows.Input;
using Mirage.Modules;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace Mirage.Windows;

public partial class WindowSendGetData
{
    public WindowSendGetData()
    {
        InitializeComponent();
    }

    private void OnKeyUp(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            modGameLogic.GameDestroy();
        }
    }

    public void SetStatus(string status)
    {
        TextBlockInfo.Text = status;
    }
}