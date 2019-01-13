using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace roleplay.Entities
{
    public class Player
    {
        public bool isLogged;

        public Client handle;
        public GlobalInfo globalInfo;
        public Character character;

        public Entities.Vehicle vehicle
        {
            get
            {
                if (handle.Vehicle == null)
                    return null;

                return Managers.VehicleManager.Instance().GetByHandle(handle.Vehicle);
            }
        }

        public void OutputMe(string action)
        {
            var players = NAPI.Player.GetPlayersInRadiusOfPosition(20, handle.Position);

            foreach (var player in players)
            {
                player.SendChatMessage(string.Format("!{{#C2A2DA}}*{0} {1}", handle.Name, action));
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
    }
}
