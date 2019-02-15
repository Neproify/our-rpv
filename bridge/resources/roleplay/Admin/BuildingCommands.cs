using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace roleplay.Admin
{
    public class BuildingCommands : Script
    {
        public void AdminItemsCommand(Client client, string parameters)
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
                Entities.Building createdBuilding = Managers.BuildingManager.Instance().CreateBuilding();
                player.handle.SendNotification($"ID stworzonego budynku: {createdBuilding.UID}.");
                return;
            }

            int buildingID;

            if (args.Length < 2)
                goto Usage;

            if (!Int32.TryParse(args[0], out buildingID))
                goto Usage;

            Entities.Building building = Managers.BuildingManager.Instance().GetByID(buildingID);

            if (building == null)
            {
                player.handle.SendNotification("~r~Nie znaleziono budynku o podanym identyfikatorze.");
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
                player.handle.Position = building.enterPosition;
                return;
            }

            if(args[1] == "tphere")
            {
                building.enterPosition = player.handle.Position;
                building.Unspawn();
                building.Save();
                building.Spawn();
                return;
            }

            if(args[1] == "otworz" || args[1] == "otwórz")
            {
#warning Implement later.
                return;
            }

            if(args[1] == "zamknij")
            {
#warning Implement later.
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

                building.ownerType = type;
                building.ownerID = ownerID;

                building.Save();
                return;
            }

        Usage:
            player.handle.SendNotification("Użycie komendy: /abudynek [id budynku/stwórz] [nazwa, opis, tpto, tphere, otwórz, zamknij, właściciel]");
            return;
        NameUsage:
            player.handle.SendNotification($"Użycie komendy: /abudynek {building.UID} nazwa [wartość].");
            return;
        DescriptionUsage:
            player.handle.SendNotification($"Użycie komendy: /abudynek {building.UID} opis [wartość].");
            return;
        OwnerUsage:
            player.handle.SendNotification($"Użycie komendy: /abudynek {building.UID} właściciel [{Utils.GetOwnerTypes()}] [identyfikator własciciela].");
            return;
        }

    }
}
