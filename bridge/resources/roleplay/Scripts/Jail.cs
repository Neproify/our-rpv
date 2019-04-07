using GTANetworkAPI;

namespace roleplay.Scripts
{
    public class Jail : Script
    {
        [Command("przetrzymaj")]
        public void JailCommand(Client client, int playerID)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.IsReady())
                return;

            var targetPlayer = Managers.PlayerManager.Instance().GetByID(playerID);

            if (targetPlayer == null)
            {
                player.handle.SendNotification("~r~Podałeś nieprawidłowy identyfikator gracza!");
                return;
            }

            if (!player.IsOnDutyOfGroupType(GroupType.Gang) && !player.IsOnDutyOfGroupType(GroupType.Mafia)
                && !player.IsOnDutyOfGroupType(GroupType.Police))
            {
                player.handle.SendNotification("~r~Nie posiadasz uprawnień do wykonania tej komendy.");
                return;
            }

            if (player.handle.Position.DistanceTo(targetPlayer.handle.Position) > 5 && player.handle.Dimension == targetPlayer.handle.Dimension)
            {
                player.handle.SendNotification("~r~Jesteś za daleko od gracza.");
                return;
            }

            if(!player.IsInBuildingOfHisGroup())
            {
                player.handle.SendNotification("~r~Nie jesteś w budynku swojej grupy!");
                return;
            }

            if(targetPlayer.character.jailBuildingID == -1)
            {
                targetPlayer.character.jailBuildingID = player.building.UID;
                targetPlayer.character.jailPosition = targetPlayer.handle.Position;
                targetPlayer.character.Save();
                player.OutputMe($" więzi {targetPlayer.formattedName}.");
            }
            else
            {
                targetPlayer.character.jailBuildingID = -1;
                targetPlayer.character.Save();
                player.OutputMe($" uwalnia {targetPlayer.formattedName}.");
            }
        }
    }
}
