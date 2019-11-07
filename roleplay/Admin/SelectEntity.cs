using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using MongoDB.Bson;

namespace roleplay.Admin
{
    public class SelectEntity : Script
    {
        [Command("awybierz")]
        public void SelectCommand(Client client, string type, string name)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.IsReady())
                return;

            if (!player.IsAdminOfLevel(AdminLevel.Supporter))
            {
                player.SendNoPermissionsToCommandNotification();
                return;
            }

            EntityType entityType = Utils.GetEntityTypeByName(type);

            if (entityType == EntityType.None)
                goto Usage;

            switch (entityType)
            {
                case EntityType.Character:
                    var playerToSelect = Managers.PlayerManager.Instance().GetAll().Find(x => x.character.UID == AdminHelpers.GetObjectId(name, entityType, player));
                    if(playerToSelect != null)
                    {
                        player.selectedEntities.selectedPlayer = playerToSelect;
                        player.SendNotification($"~g~Wybrano gracza {playerToSelect.formattedName}.");
                        return;
                    }
                    break;
                case EntityType.Vehicle:
                    var vehicleToSelect = Managers.VehicleManager.Instance().GetAll().Find(x => x.vehicleData.UID == AdminHelpers.GetObjectId(name, entityType, player));
                    if(vehicleToSelect != null)
                    {
                        player.selectedEntities.selectedVehicle = vehicleToSelect;
                        player.SendNotification($"~g~Wybrano pojazd {vehicleToSelect.displayName}.");
                        return;
                    }
                    break;
                case EntityType.Building:
                    var buildingToSelect = Managers.BuildingManager.Instance().GetAll().Find(x => x.UID == AdminHelpers.GetObjectId(name, entityType, player));
                    if(buildingToSelect != null)
                    {
                        player.selectedEntities.selectedBuilding = buildingToSelect;
                        player.SendNotification($"~g~Wybrano budynek {buildingToSelect.name}.");
                        return;
                    }
                    break;
                case EntityType.Group:
                    var groupToSelect = Managers.GroupManager.Instance().GetAll().Find(x => x.UID == AdminHelpers.GetObjectId(name, entityType, player));
                    if(groupToSelect != null)
                    {
                        player.selectedEntities.selectedGroup = groupToSelect;
                        player.SendNotification($"~g~Wybrano grupę {groupToSelect.name}.");
                        return;
                    }
                    break;
                case EntityType.Item:
                    var itemToSelect = Managers.ItemManager.Instance().GetAll().Find(x => x.UID == AdminHelpers.GetObjectId(name, entityType, player));
                    if(itemToSelect != null)
                    {
                        player.selectedEntities.selectedItem = itemToSelect;
                        player.SendNotification($"~g~Wybrano przedmiot {itemToSelect.name}.");
                        return;
                    }
                    break;
                case EntityType.Object:
                    var objectToSelect = Managers.ObjectManager.Instance().GetAll().Find(x => x.UID == AdminHelpers.GetObjectId(name, entityType, player));
                    if(objectToSelect != null)
                    {
                        player.selectedEntities.selectedObject = objectToSelect;
                        player.SendNotification($"~g~Wybrano obiekt {objectToSelect.model}.");
                        return;
                    }
                    break;
            }

            player.SendNotification("~r~Nie wybrano żadnego obiektu.");

        Usage:
            player.SendUsageNotification($"Użyj: /awybierz [{Utils.GetEntityTypes()}] [ID])");
            return;
        }
    }
}
