using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace roleplay.Scripts
{
    public class CriminalCommands : Script
    {
        [Command("knebel")]
        public void GagPlayer(Client client, int playerID)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.isLogged || player.character == null)
                return;

            var targetPlayer = Managers.PlayerManager.Instance().GetByID(playerID);

            if(targetPlayer == null)
            {
                player.handle.SendNotification("~r~Podałeś nieprawidłowy identyfikator gracza!");
                return;
            }

            if(!player.IsOnDutyOfGroupType(Groups.GroupType.Gang) && !player.IsOnDutyOfGroupType(Groups.GroupType.Mafia))
            {
                player.handle.SendNotification("~r~Nie posiadasz uprawnień do wykonania tej komendy.");
                return;
            }

            if(player.handle.Position.DistanceTo(targetPlayer.handle.Position) > 5)
            {
                player.handle.SendNotification("~r~Jesteś za daleko od gracza.");
                return;
            }

            if(targetPlayer.isGagged)
            {
                targetPlayer.isGagged = false;
                player.OutputMe($"ściąga knebel z {targetPlayer.formattedName}.");
                return;
            }
            else
            {
                targetPlayer.isGagged = true;
                player.OutputMe($"zakłada knebel {targetPlayer.formattedName}.");
                return;
            }
        }
    }
}
