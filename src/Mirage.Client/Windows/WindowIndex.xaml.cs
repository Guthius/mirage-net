using System.Windows;
using Mirage.Modules;

namespace Mirage.Windows;

public partial class WindowIndex
{
    public WindowIndex()
    {
        InitializeComponent();
    }

    private void OnAccept(object sender, RoutedEventArgs e)
    {
        modGameLogic.EditorIndex = Items.SelectedIndex + 1;

        if (modGameLogic.InItemsEditor)
        {
            modClientTCP.SendData("EDITITEM" + modTypes.SEP_CHAR + modGameLogic.EditorIndex + modTypes.SEP_CHAR);
        }

        if (modGameLogic.InNpcEditor)
        {
            modClientTCP.SendData("EDITNPC" + modTypes.SEP_CHAR + modGameLogic.EditorIndex + modTypes.SEP_CHAR);
        }

        if (modGameLogic.InShopEditor)
        {
            modClientTCP.SendData("EDITSHOP" + modTypes.SEP_CHAR + modGameLogic.EditorIndex + modTypes.SEP_CHAR);
        }

        if (modGameLogic.InSpellEditor)
        {
            modClientTCP.SendData("EDITSPELL" + modTypes.SEP_CHAR + modGameLogic.EditorIndex + modTypes.SEP_CHAR);
        }

        Close();
    }

    private void OnCancel(object sender, RoutedEventArgs e)
    {
        modGameLogic.InItemsEditor = false;
        modGameLogic.InNpcEditor = false;
        modGameLogic.InShopEditor = false;
        modGameLogic.InSpellEditor = false;

        Close();
    }
}