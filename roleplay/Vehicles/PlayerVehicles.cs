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
                        player.SendNotification("~r~Nie znajdujesz się w pobliżu żadnego pojazdu!");
                        return;
                    }
                }

                if (!vehicle.CanBeAccessedBy(player))
                {
                    player.SendNotification("~r~Nie masz kluczy do tego pojazdu!");
                    return;
                }

                if (vehicle.locked)
                {
                    player.SendNotification("~g~Otworzyłeś pojazd.");
                    vehicle.locked = false;
                    player.OutputMe($"otwiera pojazd {vehicle.displayName}.");
                }
                else
                {
                    player.SendNotification("~g~Zamknąłeś pojazd.");
                    vehicle.locked = true;
                    player.OutputMe($"zamyka pojazd {vehicle.displayName}.");
                }

                return;
            }

            if (arg == "silnik")
            {
                if (player.GetVehicleSeat() != (int)VehicleSeat.Driver)
                {
                    player.SendNotADriverNotification();
                    return;
                }

                var vehicle = player.GetVehicle();

                if (!vehicle.CanBeAccessedBy(player))
                {
                    player.SendNotification("~r~Nie masz kluczy do tego pojazdu!");
                    return;
                }

                if (vehicle.engineStatus == false)
                {
                    player.SendNotification("~g~Uruchomiłeś silnik.");
                    vehicle.engineStatus = true;
                    player.OutputMe($"odpalił silnik w pojeździe {vehicle.displayName}.");
                }
                else
                {
                    player.SendNotification("~g~Zgasiłeś silnik.");
                    vehicle.engineStatus = false;
                    player.OutputMe($"zgasił silnik w pojeździe {vehicle.displayName}.");
                }

                return;
            }

            if (arg == "parkuj")
            {
                if (player.GetVehicleSeat() != (int)VehicleSeat.Driver)
                {
                    player.SendNotADriverNotification();
                    return;
                }

                var vehicle = player.GetVehicle();

                if (!vehicle.CanBeAccessedBy(player))
                {
                    player.SendNotification("~r~Nie masz kluczy do tego pojazdu!");
                    return;
                }

                vehicle.vehicleData.spawnPosition = vehicle.position;
                vehicle.vehicleData.spawnRotation = vehicle.rotation;

                vehicle.Save();

                return;
            }

            player.SendUsageNotification("Użycie komendy: /v [z(amek), silnik, parkuj]");
        }
    }
}
