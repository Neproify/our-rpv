using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace roleplay.Entities
{
    public class Player
    {
        public bool isLogged;
        public string formattedName
        {
            get
            {
                return handle.Name.Replace("_", " ");
            }
        }

        public Client handle;
        public GlobalInfo globalInfo;
        public Character character;
        public Groups.GroupDuty groupDuty;

        public Entities.Vehicle vehicle
        {
            get
            {
                if (handle.Vehicle == null)
                    return null;

                return Managers.VehicleManager.Instance().GetByHandle(handle.Vehicle);
            }
        }

        public void Save()
        {
            character.Save();
        }

        public void OutputMe(string action)
        {
            var players = NAPI.Player.GetPlayersInRadiusOfPosition(20, handle.Position);

            foreach (var player in players)
            {
                player.SendChatMessage($"!{{#C2A2DA}}*{formattedName} {action}");
            }
        }

        public List<Entities.Item> GetItems()
        {
            if (!isLogged)
                return null;

            return Managers.ItemManager.Instance().GetItemsOf(OwnerType.Character, character.UID);
        }

        public bool CanUseItem(int itemUID)
        {
            var item = Managers.ItemManager.Instance().GetItem(itemUID);

            if (item == null)
                return false;

            if (item.ownerType == OwnerType.Character && item.ownerID == character.UID)
                return true;

            return false;
        }

        public Entities.Vehicle GetClosestVehicle()
        {
            var vehicles = NAPI.Pools.GetAllVehicles();

            GTANetworkAPI.Vehicle returnedVehicle = null;
            float distance = 99999;

            foreach(var vehicle in vehicles)
            {
                var tempDistance = handle.Position.DistanceTo(vehicle.Position);
                if(tempDistance < distance)
                {
                    distance = tempDistance;
                    returnedVehicle = vehicle;
                }
            }

            if (returnedVehicle == null)
                return null;

            return Managers.VehicleManager.Instance().GetByHandle(returnedVehicle);
        }

        public Entities.Vehicle GetClosestVehicle(float maxDistance)
        {
            var vehicle = GetClosestVehicle();

            if (vehicle == null)
                return null;

            if (handle.Position.DistanceTo(vehicle.handle.Position) >= maxDistance)
                return null;

            return vehicle;
        }

        public List<Entities.Group> GetGroups()
        {
            return Managers.GroupManager.Instance().GetPlayerGroups(this);
        }
    }

    public class GlobalInfo
    {
        public int UID;
        public string name;
    }

    public class Character
    {
        public int UID;
        public int GID;
        public string name;
        public uint model;

        public void Save()
        {
#warning Save characters when there are more things to save
            var command = Database.Instance().Connection.CreateCommand();
            command.CommandText = "UPDATE `rp_characters` SET `model`=@model WHERE `UID`=@UID";
            command.Prepare();

            command.Parameters.AddWithValue("@model", model);
            command.Parameters.AddWithValue("@UID", UID);
            command.ExecuteNonQuery();
        }
    }
}
