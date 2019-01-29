﻿using System;
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
        public Offers.OfferInfo offerInfo;

        public int money
        {
            get
            {
                if (character == null)
                    return 0;

                return character.money;
            }
            set
            {
                character.money = value;
                NAPI.ClientEvent.TriggerClientEvent(handle, "SetMoney", value);
            }
        }

        public Entities.Vehicle vehicle
        {
            get
            {
                if (handle.Vehicle == null)
                    return null;

                return Managers.VehicleManager.Instance().GetByHandle(handle.Vehicle);
            }
        }

        public Entities.Building building;

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

        public bool SendMoneyTo(Entities.Player player, int amount)
        {
            if (!this.isLogged || this.character == null)
                return false;

            if (!player.isLogged || player.character == null)
                return false;

            if(this.money < amount)
            {
                this.handle.SendNotification("~r~Masz za mało pieniędzy!");
                return false;
            }

            this.money -= amount;
            player.money += amount;

            return true;
        }

        public bool SendMoneyTo(Entities.Group group, int amount)
        {
            if (this.isLogged || this.character == null)
                return false;

            if(this.money < amount)
            {
                this.handle.SendNotification("~r~Masz za mało pieniędzy!");
                return false;
            }

            this.money -= amount;
            group.bank += amount;

            return true;
        }

        public List<Entities.Item> GetItems()
        {
            if (!isLogged)
                return null;

            return Managers.ItemManager.Instance().GetItemsOf(OwnerType.Character, character.UID);
        }

        public bool CanUseItem(Entities.Item item)
        {
            if (item == null)
                return false;

            if (item.ownerType == OwnerType.Character && item.ownerID == character.UID)
                return true;

            return false;
        }

        public bool CanUseItem(int itemUID)
        {
            var item = Managers.ItemManager.Instance().GetItem(itemUID);

            return CanUseItem(item);
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

        public Entities.Building GetClosestBuilding(float maxDistance = 3f)
        {
            return Managers.BuildingManager.Instance().GetClosestBuilding(handle.Position, maxDistance);
        }

        public Entities.Item GetClosestItem(float maxDistance = 5f)
        {
            return Managers.ItemManager.Instance().GetClosestItem(handle.Position, maxDistance);
        }

        public List<Entities.Group> GetGroups()
        {
            return Managers.GroupManager.Instance().GetPlayerGroups(this);
        }

        public bool HasSpecialPermissionInGroup(Groups.GroupSpecialPermission permission)
        {
            var group = groupDuty?.member.group;

            if (group == null)
                return false;

            if ((group.specialPermissions & (int)permission) == 1)
                return true;

            return false;
        }

        public bool IsOnDutyOfGroupType(Groups.GroupType type)
        {
            if (groupDuty == null)
                return false;

            if (groupDuty.member.group.type == (int)type)
                return true;

            return false;
        }

        public bool IsAdminOfLevel(Admin.AdminLevel level)
        {
            if (globalInfo == null)
                return false;

            return globalInfo.adminLevel >= (int)level;
        }
    }

    public class GlobalInfo
    {
        public int UID;
        public string name;
        public int score;
        public int adminLevel;
        public int adminPermissions;
    }

    public class Character
    {
        public int UID;
        public int GID;
        public string name;
        public uint model;
        public int money;

        public void Save()
        {
            var command = Database.Instance().Connection.CreateCommand();
            command.CommandText = "UPDATE `rp_characters` SET `model`=@model, `money`=@money WHERE `UID`=@UID";
            command.Prepare();

            command.Parameters.AddWithValue("@model", model);
            command.Parameters.AddWithValue("@UID", UID);
            command.Parameters.AddWithValue("@money", money);
            command.ExecuteNonQuery();
        }
    }
}
