using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace roleplay.Vehicles
{
    public class PlayerVehicles : Script
    {

        [Command("v", GreedyArg = true)]
        public void VehicleCommand(Client client, string arg)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);
            if (arg == "z" || arg == "zamek")
            {
                var vehicle = player.vehicle;
                if (vehicle == null)
                {
                    vehicle = player.GetClosestVehicle(5);
                    if (vehicle == null)
                    {
                        player.handle.SendNotification("~r~Nie znajdujesz się w pobliżu żadnego pojazdu!");
                        return;
                    }
                }

                if (!vehicle.CanBeAccessedBy(player))
                {
                    player.handle.SendNotification("~r~Nie masz kluczy do tego pojazdu!");
                    return;
                }

                if (vehicle.handle.Locked)
                {
                    player.handle.SendNotification("~g~Otworzyłeś pojazd.");
                    vehicle.handle.Locked = false;
                    player.OutputMe($"otwiera pojazd {vehicle.handle.DisplayName}.");
                }
                else
                {
                    player.handle.SendNotification("~g~Zamknąłeś pojazd.");
                    vehicle.handle.Locked = true;
                    player.OutputMe($"zamyka pojazd {vehicle.handle.DisplayName}.");
                }

                return;
            }

            if (arg == "silnik")
            {
                if (player.handle.Vehicle == null || player.handle.VehicleSeat != -1)
                {
                    player.handle.SendNotification("~r~Nie siedzisz w żadnym pojeździe lub nie jesteś kierowcą!");
                    return;
                }

                var vehicle = Managers.VehicleManager.Instance().GetByHandle(player.handle.Vehicle);

                if (!vehicle.CanBeAccessedBy(player))
                {
                    player.handle.SendNotification("~r~Nie masz kluczy do tego pojazdu!");
                    return;
                }

                if (vehicle.engineStatus == false)
                {
                    player.handle.SendNotification("~g~Uruchomiłeś silnik.");
                    vehicle.engineStatus = true;
                    player.OutputMe($"odpalił silnik w pojeździe {vehicle.handle.DisplayName}.");
                }
                else
                {
                    player.handle.SendNotification("~g~Zgasiłeś silnik.");
                    vehicle.engineStatus = false;
                    player.OutputMe($"zgasił silnik w pojeździe {vehicle.handle.DisplayName}.");
                }

                return;
            }

            if (arg == "parkuj")
            {
                if (player.handle.Vehicle == null || player.handle.VehicleSeat != -1)
                {
                    player.handle.SendNotification("~r~Nie siedzisz w żadnym pojeździe lub nie jesteś kierowcą!");
                    return;
                }

                var vehicle = Managers.VehicleManager.Instance().GetByHandle(player.handle.Vehicle);

                if (!vehicle.CanBeAccessedBy(player))
                {
                    player.handle.SendNotification("~r~Nie masz kluczy do tego pojazdu!");
                    return;
                }

                vehicle.vehicleData.spawnPosition = vehicle.handle.Position;
                vehicle.vehicleData.spawnRotation = vehicle.handle.Rotation;

                vehicle.Save();

                return;
            }

            client.SendNotification("Użycie komendy: /v [z(amek), silnik, parkuj]");
            return;
        }
    }
}
