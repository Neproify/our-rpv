using System;
using GTANetworkAPI;
using MongoDB.Bson;

namespace roleplay.Admin
{
    public class VehicleCommands : Script
    {
        [Command("apojazd", GreedyArg = true)]
        public void AdminVehiclesCommand(Client client, string parameters)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.IsReady())
                return;

            if (!player.IsAdminOfLevel(AdminLevel.Supporter))
            {
                player.SendNoPermissionsToCommandNotification();
                return;
            }

            string[] args = parameters.Split(" ");

            if (args[0] == "stworz" || args[0] == "stwórz")
            {
                Entities.Vehicle createdVehicle = Managers.VehicleManager.Instance().CreateVehicle();
                player.SendChatMessage($"ID stworzonego pojazdu: {createdVehicle.vehicleData.UID}.");
                return;
            }

            if (args.Length < 2)
                goto Usage;

            if (!ObjectId.TryParse(args[0], out var vehicleID))
                goto Usage;

            Entities.Vehicle vehicle = Managers.VehicleManager.Instance().GetByID(vehicleID);

            if (vehicle == null)
            {
                player.SendVehicleNotFoundNotification();
                return;
            }

            if (args[1] == "napraw")
            {
                vehicle.Repair();
                vehicle.Save();
                return;
            }

            if (args[1] == "kolor")
            {
                if (args.Length != 4)
                    goto ColorUsage;

                if (!Int32.TryParse(args[2], out var color1) || !Int32.TryParse(args[3], out var color2))
                    goto ColorUsage;

                vehicle.primaryColor = color1;
                vehicle.secondaryColor = color2;

                vehicle.Save();

                return;
            }

            if (args[1] == "hp")
            {
                if (args.Length != 3)
                    goto HealthUsage;

                if (!Int32.TryParse(args[2], out var healthValue))
                    goto HealthUsage;

                vehicle.health = healthValue;

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
                    player.SendInvalidOwnerTypeNotification();
                    return;
                }

                if (ObjectId.TryParse(args[3], out var ownerID))
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
                vehicle.vehicleData.spawnPosition = vehicle.position;
                vehicle.vehicleData.spawnRotation = vehicle.rotation;

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

                player.SetPosition(vehicle.position);

                return;
            }

            if (args[1] == "tphere")
            {
                if (!vehicle.IsSpawned())
                    return;

                vehicle.position = player.GetPosition();

                return;
            }

            if (args[1] == "otworz" || args[1] == "otwórz")
            {
                if (!vehicle.IsSpawned())
                    return;

                vehicle.locked = false;
                return;
            }

            if(args[1] == "zamknij")
            {
                if (!vehicle.IsSpawned())
                    return;

                vehicle.locked = true;
                return;
            }

            if(args[1] == "model")
            {
                if (args.Length != 3)
                    goto ModelUsage;

                if (!UInt32.TryParse(args[2], out var modelHash))
                    goto ModelUsage;

                vehicle.model = modelHash;

                vehicle.Save();
                return;
            }

            if(args[1] == "paliwo")
            {
#warning Implement later.
                return;
            }

        Usage:
            player.SendUsageNotification("Użycie komendy: /apojazd [id pojazdu/stwórz] [napraw, hp, właściciel, parkuj, spawn, unspawn, tpto, tphere, otwórz, zamknij, model, paliwo]");
            return;
        ColorUsage:
            player.SendUsageNotification($"Użycie komendy: /apojazd {vehicleID} kolor [1 kolor] [2 kolor]");
            return;
        HealthUsage:
            player.SendUsageNotification($"Użycie komendy: /apojazd {vehicleID} hp [ilość]");
            return;
        OwnerUsage:
            player.SendUsageNotification($"Użycie komendy: /apojazd {vehicleID} właściciel [{Utils.GetOwnerTypes()}] [identyfikator właściciela]");
            return;
        ModelUsage:
            player.SendUsageNotification($"Użycie komendy: /apojazd {vehicleID} model [hash modelu]");
            return;
        }
    }
}
