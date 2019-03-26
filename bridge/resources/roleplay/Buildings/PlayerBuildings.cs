using GTANetworkAPI;

namespace roleplay.Buildings
{
    public class PlayerBuildings : Script
    {
        [Command("drzwi")]
        public void DoorCommand(Client client)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.isLogged || player.character == null)
                return;

            if(player.building != null)
            {
                if(player.handle.Position.DistanceTo(player.building.exitPosition) <= 3f)
                {
                    if(player.character.jailBuildingID != -1)
                    {
                        player.handle.SendNotification("~r~Jesteś uwięziony. Nie możesz wyjść.");
                        return;
                    }

                    player.handle.Position = player.building.enterPosition;
                    player.handle.Dimension = player.building.enterDimension;
                    player.building = null;

                    return;
                }
            }

            var building = player.GetClosestBuilding();

            if (building == null)
                return;

            player.handle.Position = building.exitPosition;
            player.handle.Dimension = building.exitDimension;
            player.building = building;

            return;
        }
    }
}
