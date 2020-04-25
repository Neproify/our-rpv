using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace roleplay.Scripts
{
    public class AdminContact : Script
    {
        [Command("report", GreedyArg = true)]
        public void ReportCommand(Player client, string message)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if(!player.IsReady())
            {
                return;
            }

            var onlineAdmins = Managers.PlayerManager.Instance().GetAll().FindAll(x => x.globalInfo?.adminLevel > 0);

            if(onlineAdmins.Count == 0)
            {
                player.SendNotification("~r~Przepraszamy, obecnie nie ma żadnych administratorów na serwerze.");
                return;
            }

            onlineAdmins.ForEach(x => x.SendNotification($"Report(ID {player.GetGameID()}: {message}"));
        }
    }
}
