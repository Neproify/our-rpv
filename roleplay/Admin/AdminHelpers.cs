using MongoDB.Bson;

namespace roleplay.Admin
{
    public static class AdminHelpers
    {
        public static ObjectId GetObjectId(string input, EntityType entityType = EntityType.None, Entities.Player player = null)
        {
            ObjectId objectId;

            if (ObjectId.TryParse(input, out objectId))
            {
                return objectId;
            }

            if (input == "pusty")
            {
                return ObjectId.Empty;
            }

            if (input == "wybrany" || input == "wybrana")
            {
                switch (entityType)
                {
                    case EntityType.None:
                        return ObjectId.Empty;
                    case EntityType.Character:
                        return player?.selectedEntities.selectedPlayer != null ? player.selectedEntities.selectedPlayer.character.UID : ObjectId.Empty;
                    case EntityType.Vehicle:
                        return player?.selectedEntities.selectedVehicle != null ? player.selectedEntities.selectedVehicle.vehicleData.UID : ObjectId.Empty;
                    case EntityType.Building:
                        return player?.selectedEntities.selectedBuilding != null ? player.selectedEntities.selectedBuilding.UID : ObjectId.Empty;
                    case EntityType.Group:
                        return player?.selectedEntities.selectedGroup != null ? player.selectedEntities.selectedGroup.UID : ObjectId.Empty;
                    case EntityType.Item:
                        return player?.selectedEntities.selectedItem != null ? player.selectedEntities.selectedItem.UID : ObjectId.Empty;
                    case EntityType.Object:
                        return player?.selectedEntities.selectedObject != null ? player.selectedEntities.selectedObject.UID : ObjectId.Empty;
                }
            }

            if (input == "ja")
            {
                if (entityType == EntityType.Character)
                {
                    if (player != null) return player.character.UID;
                }
            }

            if (input == "najblizszy")
            {
                if (player == null)
                {
                    return ObjectId.Empty;
                }

                switch (entityType)
                {
                    case EntityType.Character:
                        var nearestPlayer = player.GetClosestPlayer();
                        if (nearestPlayer != null)
                            return nearestPlayer.character.UID;
                        break;
                    case EntityType.Vehicle:
                        var nearestVehicle = player.GetClosestVehicle();
                        if (nearestVehicle != null)
                            return nearestVehicle.vehicleData.UID;
                        break;
                    case EntityType.Building:
                        var nearestBuilding = player.GetClosestBuilding();
                        if (nearestBuilding != null)
                            return nearestBuilding.UID;
                        break;
                    case EntityType.Item:
                        var nearestItem = player.GetClosestItem();
                        if (nearestItem != null)
                            return nearestItem.UID;
                        break;
                    case EntityType.Object:
                        var nearestObject = player.GetClosestObject();
                        if (nearestObject != null)
                            return nearestObject.UID;
                        break;
                }
            }

            if(input == "obecny")
            {
                if(player == null)
                {
                    return ObjectId.Empty;
                }

                switch(entityType)
                {
                    case EntityType.Vehicle:
                        var vehicle = player.GetVehicle();
                        if (vehicle != null)
                            return vehicle.vehicleData.UID;
                        break;
                    case EntityType.Building:
                        var building = player.building;
                        if (building != null)
                            return building.UID;
                        break;
                    case EntityType.Group:
                        var group = player.groupDuty?.member.group;
                        if (group != null)
                            return group.UID;
                        break;
                }
            }

            return ObjectId.Empty;
        }
    }
}
