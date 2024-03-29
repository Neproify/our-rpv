﻿using GTANetworkAPI;
using MongoDB.Bson;

namespace roleplay.Buildings
{
    public class PlayerBuildings : Script
    {
        [Command("drzwi")]
        public void DoorCommand(Player client, string option = "przejdz")
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.IsReady())
                return;

            var building = player.building ?? player.GetClosestBuilding();

            if (building == null)
            {
                player.SendNotification("~r~Nie znajdujesz się w pobliżu żadnych drzwi!");
                return;
            }

            if (option == "przejdz")
            {
                if (building.isLocked)
                {
                    player.SendNotification("~r~Drzwi są zamknięte.");
                    return;
                }

                if (player.position.DistanceTo(building.exitPosition) <= 3f)
                {
                    if (player.character.jailBuildingID != ObjectId.Empty)
                    {
                        player.SendNotification("~r~Jesteś uwięziony. Nie możesz wyjść.");
                        return;
                    }

                    player.position = building.enterPosition;
                    player.SetDimension(building.enterDimension);
                    player.building = null;

                    return;
                }

                if (player.isInAnyVehicle)
                {
                    if (building.canBeEnteredByVehicle == false || !player.isDriver)
                    {
                        player.SendNotification("~r~Nie możesz wjechać do tego budynku pojazdem.");
                        return;
                    }
                }

                player.position = building.exitPosition;
                player.SetDimension(building.exitDimension);
                player.building = building;

                return;
            }

            if (option == "zamek")
            {
                if (!building.CanBeAccessedBy(player))
                {
                    player.SendNotification("~r~Nie posiadasz klucza do tych drzwi!");
                    return;
                }

                player.SendNotification($"{(building.isLocked ? "Otworzyłeś" : "Zamknąłeś")} drzwi.");

                building.isLocked = !building.isLocked;

                return;
            }

            player.SendUsageNotification("Użyj: /drzwi [przejdz, zamek]");
        }
    }
}
