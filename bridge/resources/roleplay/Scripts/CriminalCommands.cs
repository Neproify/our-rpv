using GTANetworkAPI;

namespace roleplay.Scripts
{
    public class CriminalCommands : Script
    {
        [Command("knebel")]
        public void GagPlayer(Client client, int playerID)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.IsReady())
                return;

            var targetPlayer = Managers.PlayerManager.Instance().GetByID(playerID);

            if(targetPlayer == null)
            {
                player.SendPlayerNotFoundNotification();
                return;
            }

            if(!player.IsOnDutyOfGroupType(GroupType.Gang) && !player.IsOnDutyOfGroupType(GroupType.Mafia))
            {
                player.SendNoPermissionsToCommandNotification();
                return;
            }

            if(player.position.DistanceTo(targetPlayer.position) > 5)
            {
                player.SendNotification("~r~Jesteś za daleko od gracza.");
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
