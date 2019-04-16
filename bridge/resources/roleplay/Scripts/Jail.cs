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
                player.SendPlayerNotFoundNotification();
                return;
            }

            if (!player.IsOnDutyOfGroupType(GroupType.Gang) && !player.IsOnDutyOfGroupType(GroupType.Mafia)
                && !player.IsOnDutyOfGroupType(GroupType.Police))
            {
                player.SendNoPermissionsToCommandNotification();
                return;
            }

            if (player.GetPosition().DistanceTo(targetPlayer.GetPosition()) > 5 && player.GetDimension() == targetPlayer.GetDimension())
            {
                player.SendNotification("~r~Jesteś za daleko od gracza.");
                return;
            }

            if(!player.IsInBuildingOfHisGroup())
            {
                player.SendNotification("~r~Nie jesteś w budynku swojej grupy!");
                return;
            }

            if(targetPlayer.character.jailBuildingID == MongoDB.Bson.ObjectId.Empty)
            {
                targetPlayer.character.jailBuildingID = player.building.UID;
                targetPlayer.character.jailPosition = targetPlayer.GetPosition();
                targetPlayer.character.Save();
                player.OutputMe($" więzi {targetPlayer.formattedName}.");
            }
            else
            {
                targetPlayer.character.jailBuildingID = MongoDB.Bson.ObjectId.Empty;
                targetPlayer.character.Save();
                player.OutputMe($" uwalnia {targetPlayer.formattedName}.");
            }
        }
    }
}
