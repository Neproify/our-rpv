using GTANetworkAPI;

namespace roleplay.Buildings
{
    public class PlayerBuildings : Script
    {
        [Command("drzwi")]
        public void DoorCommand(Client client, string option = "przejdz")
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.isLogged || player.character == null)
                return;

            var building = player.building ?? player.GetClosestBuilding();

            if (building == null)
            {
                player.handle.SendNotification("~r~Nie znajdujesz się w pobliżu żadnych drzwi!");
                return;
            }

            if (option == "przejdz")
            {
                if (building.isLocked)
                {
                    player.handle.SendNotification("~r~Drzwi są zamknięte.");
                    return;
                }

                if (building != null)
                {
                    if (player.handle.Position.DistanceTo(building.exitPosition) <= 3f)
                    {
                        if (player.character.jailBuildingID != -1)
                        {
                            player.handle.SendNotification("~r~Jesteś uwięziony. Nie możesz wyjść.");
                            return;
                        }

                        player.handle.Position = building.enterPosition;
                        player.handle.Dimension = building.enterDimension;
                        player.building = null;

                        return;
                    }
                }

                player.handle.Position = building.exitPosition;
                player.handle.Dimension = building.exitDimension;
                player.building = building;

                return;
            }

            if (option == "zamek")
            {
                if (!building.CanBeAccessedBy(player))
                {
                    player.handle.SendNotification("~r~Nie posiadasz klucza do tych drzwi!");
                    return;
                }

                player.handle.SendNotification($"{(building.isLocked ? "Otworzyłeś" : "Zamknąłeś")} drzwi.");

                building.isLocked = !building.isLocked;

                return;
            }

            player.handle.SendNotification("Użyj: /drzwi [przejdz, zamek]");
        }
    }
}
