using System;
using GTANetworkAPI;

namespace roleplay.Admin
{
    public class BuildingCommands : Script
    {
        [Command("abudynek", GreedyArg = true)]
        public void AdminBuildingsCommand(Client client, string parameters)
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
                Entities.Building createdBuilding = Managers.BuildingManager.Instance().CreateBuilding();
                player.SendNotification($"ID stworzonego budynku: {createdBuilding.UID}.");
                return;
            }

            if (args.Length < 2)
                goto Usage;

            if (!Int32.TryParse(args[0], out var buildingID))
                goto Usage;

            Entities.Building building = Managers.BuildingManager.Instance().GetByID(buildingID);

            if (building == null)
            {
                player.SendNotification("~r~Nie znaleziono budynku o podanym identyfikatorze.");
                return;
            }

            if (args[1] == "nazwa")
            {
                if (args.Length < 3)
                {
                    goto NameUsage;
                }

                string name = string.Join(" ", args, 2, args.Length);

                building.name = name;

                building.Save();
                return;
            }

            if(args[1] == "opis")
            {
                if(args.Length < 3)
                {
                    goto DescriptionUsage;
                }

                string description = string.Join(" ", args, 2, args.Length);

                building.description = description;

                building.Save();
            }

            if (args[1] == "tpto")
            {
                player.SetPosition(building.enterPosition);
                return;
            }

            if(args[1] == "tphere")
            {
                building.enterPosition = player.GetPosition();
                building.Unspawn();
                building.Save();
                building.Spawn();
                return;
            }

            if(args[1] == "otworz" || args[1] == "otwórz")
            {
                building.isLocked = false;
                return;
            }

            if(args[1] == "zamknij")
            {
                building.isLocked = true;
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

                if (!Int32.TryParse(args[3], out var ownerID))
                {
                    goto OwnerUsage;
                }

                building.ownerType = type;
                building.ownerID = ownerID;

                building.Save();
                return;
            }

        Usage:
            player.SendUsageNotification("Użycie komendy: /abudynek [id budynku/stwórz] [nazwa, opis, tpto, tphere, otwórz, zamknij, właściciel]");
            return;
        NameUsage:
            player.SendUsageNotification($"Użycie komendy: /abudynek {building.UID} nazwa [wartość].");
            return;
        DescriptionUsage:
            player.SendUsageNotification($"Użycie komendy: /abudynek {building.UID} opis [wartość].");
            return;
        OwnerUsage:
            player.SendUsageNotification($"Użycie komendy: /abudynek {building.UID} właściciel [{Utils.GetOwnerTypes()}] [identyfikator własciciela].");
            return;
        }

    }
}
