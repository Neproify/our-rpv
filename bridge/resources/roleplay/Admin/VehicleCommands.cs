using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace roleplay.Admin
{
    public class VehicleCommands : Script
    {
        [Command("apojazd", GreedyArg = true)]
        public void AdminVehiclesCommand(Client client, string parameters)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.isLogged || player.character == null)
                return;

            if (!player.IsAdminOfLevel(AdminLevel.Supporter))
            {
                player.handle.SendNotification("~r~Nie masz uprawnień do użycia tej komendy!");
                return;
            }

            string[] args = parameters.Split(" ");

            if (args[0] == "stworz" || args[0] == "stwórz")
            {
                Entities.Vehicle createdVehicle = Managers.VehicleManager.Instance().CreateVehicle();
                player.handle.SendNotification($"ID stworzonego pojazdu: {createdVehicle.vehicleData.UID}.");
                return;
            }

            int vehicleID;

            if (args.Length < 2)
                goto Usage;

            if (!Int32.TryParse(args[0], out vehicleID))
                goto Usage;

            Entities.Vehicle vehicle = Managers.VehicleManager.Instance().GetVehicle(vehicleID);
            if (vehicle == null)
            {
                player.handle.SendNotification("~r~Nie znaleziono pojazdu o podanym identyfikatorze.");
                return;
            }

            if (args[1] == "napraw")
            {
                vehicle.handle.Repair();
                vehicle.Save();
                return;
            }

            if (args[1] == "kolor")
            {
                if (args.Length != 4)
                    goto ColorUsage;

                int color1, color2;

                if (!Int32.TryParse(args[2], out color1) || !Int32.TryParse(args[3], out color2))
                    goto ColorUsage;

                vehicle.vehicleData.color1 = color1;
                vehicle.vehicleData.color2 = color2;
                vehicle.handle.PrimaryColor = color1;
                vehicle.handle.SecondaryColor = color2;

                vehicle.Save();

                return;
            }

            if (args[1] == "hp")
            {
                if (args.Length != 3)
                    goto HealthUsage;

                int healthValue;

                if (!Int32.TryParse(args[2], out healthValue))
                    goto HealthUsage;

                vehicle.handle.Health = healthValue;

                vehicle.Save();

                return;
            }

            if (args[1] == "wlasciciel" || args[1] == "właściciel")
            {
                if (args.Length != 4)
                {
                    goto OwnerUsage;
                }

                OwnerType type = Utils.GetOwnerTypeByName(args[2]);

                if (type == OwnerType.Invalid)
                {
                    player.handle.SendNotification("~r~Podałeś nieprawidłowy typ właściciela.");
                    return;
                }

                int ownerID;

                if (!Int32.TryParse(args[3], out ownerID))
                {
                    goto OwnerUsage;
                }

                vehicle.vehicleData.ownerType = type;
                vehicle.vehicleData.ownerID = ownerID;

                vehicle.Save();
                return;
            }

            if (args[1] == "parkuj")
            {
                vehicle.vehicleData.spawnPosition = vehicle.handle.Position;
                vehicle.vehicleData.spawnRotation = vehicle.handle.Rotation;

                vehicle.Save();

                return;
            }

            if (args[1] == "spawn")
            {
                vehicle.Spawn();

                return;
            }

            if (args[1] == "unspawn")
            {
                vehicle.Unspawn();

                return;
            }

            if (args[1] == "tpto")
            {
                if (!vehicle.IsSpawned())
                    return;

                player.handle.Position = vehicle.handle.Position;

                return;
            }

            if (args[1] == "tphere")
            {
                if (!vehicle.IsSpawned())
                    return;

                vehicle.handle.Position = player.handle.Position;

                return;
            }

            if (args[1] == "otworz" || args[1] == "otwórz")
            {
                if (!vehicle.IsSpawned())
                    return;

                vehicle.handle.Locked = false;
                return;
            }

            if(args[1] == "zamknij")
            {
                if (!vehicle.IsSpawned())
                    return;

                vehicle.handle.Locked = true;
                return;
            }

            if(args[1] == "model")
            {
                if (args.Length != 3)
                    goto ModelUsage;

                uint modelHash;

                if (!UInt32.TryParse(args[2], out modelHash))
                    goto ModelUsage;

                vehicle.vehicleData.model = modelHash;
                NAPI.Entity.SetEntityModel(vehicle.handle, modelHash);

                vehicle.Save();
                return;
            }

            if(args[1] == "paliwo")
            {
#warning Implement this.
                return;
            }

        Usage:
            player.handle.SendNotification("Użycie komendy: /apojazd [id pojazdu/stwórz] [napraw, hp, właściciel, parkuj, spawn, unspawn, tpto, tphere, otwórz, zamknij, model, paliwo]");
            return;
        ColorUsage:
            player.handle.SendNotification($"Użycie komendy: /apojazd {vehicleID} kolor [1 kolor] [2 kolor]");
            return;
        HealthUsage:
            player.handle.SendNotification($"Użycie komendy: /apojazd {vehicleID} hp [ilość]");
            return;
        OwnerUsage:
            player.handle.SendNotification($"Użycie komendy: /apojazd {vehicleID} właściciel [{Utils.GetOwnerTypes()}] [identyfikator właściciela]");
            return;
        ModelUsage:
            player.handle.SendNotification($"Użycie komendy: /apojazd {vehicleID} model [hash modelu]");
            return;
        }
    }
}
