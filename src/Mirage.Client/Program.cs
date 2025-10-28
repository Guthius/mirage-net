using Mirage.Modules;

namespace Mirage;

internal static class Program
{
    [STAThread]
    public static void Main()
    {
        ApplicationConfiguration.Initialize();

        My.Forms.frmSendGetData.Show();
        modGameLogic.SetStatus("Initializing TCP settings...");
        modClientTCP.TcpInit();
        My.Forms.frmMainMenu.Show();
        My.Forms.frmSendGetData.Hide();

        var app = new System.Windows.Application();
        
        app.Run(My.Forms.frmMainMenu);
    }
}