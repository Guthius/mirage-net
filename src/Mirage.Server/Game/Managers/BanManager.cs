using Mirage.Game.Constants;
using Mirage.Net.Protocol.FromServer;
using Mirage.Server.Modules;
using Mirage.Server.Net;
using Serilog;

namespace Mirage.Server.Game.Managers;

public static class BanManager
{
    public static void BanIndex(int banPlayerIndex, int bannedByIndex)
    {
        var ip = Network.GetIP(banPlayerIndex);

        var dot = ip.LastIndexOf('.');
        if (dot == -1)
        {
            ip = ip[..dot];
        }

        using (var streamWriter = File.AppendText("Banlist.txt"))
        {
            streamWriter.WriteLine($"{ip},{modTypes.GetPlayerName(bannedByIndex)}");
        }

        Network.SendToAll(new GlobalMessage($"{modTypes.GetPlayerName(banPlayerIndex)} has been banned from {Options.GameName} by {modTypes.GetPlayerName(banPlayerIndex)}!", Color.White));
        
        Log.Information("{CharacterName} has banned {BannedCharacterName}", modTypes.GetPlayerName(bannedByIndex), modTypes.GetPlayerName(banPlayerIndex));
        
        Network.SendAlert(banPlayerIndex, $"You have been banned by {modTypes.GetPlayerName(bannedByIndex)}!");
    }
    
    public static bool IsBanned(string ip)
    {
        const string filename = "Banlist.txt";

        if (!File.Exists(filename))
        {
            return false;
        }

        using var streamReader = File.OpenText(filename);
        while (!streamReader.EndOfStream)
        {
            var ipToCheck = streamReader.ReadLine();
            if (ipToCheck is null)
            {
                continue;
            }

            var comma = ipToCheck.IndexOf(',');
            if (comma != -1)
            {
                ipToCheck = ipToCheck[..comma];
            }

            if (ipToCheck.Length >= ip.Length && ipToCheck[..ip.Length].Equals(ip, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    public static void Clear()
    {
        if (File.Exists("Banlist.txt"))
        {
            File.Delete("Banlist.txt");
        }
    }
}