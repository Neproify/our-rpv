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

        public bool isBrutallyWounded = false;
        public int secondsToEndOfBrutallyWounded = 0;
        public bool isGagged = false;

        public Client handle;
        public GlobalInfo globalInfo;
        public Character character;
        public Groups.GroupDuty groupDuty;
        public Offers.OfferInfo offerInfo;
        public List<Penalties.Penalty> penalties = new List<Penalties.Penalty>();

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

        public void SetIsBrutallyWounded(bool isWounded)
        {
            if(isWounded)
            {
                isBrutallyWounded = true;
                secondsToEndOfBrutallyWounded = 60*5;

                NAPI.Player.SpawnPlayer(handle, handle.Position, handle.Heading);
                handle.Freeze(true);
                handle.PlayAnimation("combat@death@from_writhe", "death_a", 9);
                handle.SendNotification("~r~Straciłeś przytomność. Poczekaj aby ją odzyskać.");
            }
            else
            {
                isBrutallyWounded = false;
                secondsToEndOfBrutallyWounded = 0;
                handle.Freeze(false);
                handle.StopAnimation();
                handle.SendNotification("~g~Odzyskałeś przytomność. Pamiętaj o odegraniu obrażeń.");
            }
        }

        public void KillCharacter(string reason)
        {
            CreatePenalty(Penalties.PenaltyType.CharacterKill, reason, -1, DateTime.Now.AddYears(50));
            handle.Kick("Character Kill");
        }

        public Penalties.Penalty CreatePenalty(Penalties.PenaltyType type, string reason, int penaltiedBy, DateTime expireDate)
        {
            var command = Database.Instance().Connection.CreateCommand();
            command.CommandText = "INSERT INTO `rp_penalties` SET `globalID`=@globalID, `characterID`=@characterID, `type`=@type, `reason`=@reason, `penaltiedBy`=@penaltiedBy, `expireDate`=@expireDate";
            command.Prepare();

            command.Parameters.AddWithValue("@globalID", globalInfo.UID);
            command.Parameters.AddWithValue("@characterID", character.UID);
            command.Parameters.AddWithValue("@type", type);
            command.Parameters.AddWithValue("@reason", reason);
            command.Parameters.AddWithValue("@penaltiedBy", penaltiedBy);
            command.Parameters.AddWithValue("@expireDate", expireDate);

            command.ExecuteNonQuery();

            int UID = (int)command.LastInsertedId;
            Penalties.Penalty penalty = new Penalties.Penalty();
            penalty.Load(UID);
            penalties.Add(penalty);

            return penalty;
        }

        public void LoadPenalties()
        {
            penalties.Clear();

            var command = Database.Instance().Connection.CreateCommand();
            command.CommandText = "SELECT * FROM `rp_penalties` WHERE `globalID`=@globalID AND `expireDate` > @currentDate;";
            command.Prepare();

            command.Parameters.AddWithValue("@globalID", globalInfo.UID);
            command.Parameters.AddWithValue("@currentDate", DateTime.Now);

            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var penalty = new Penalties.Penalty();
                penalty.Load(reader);
                penalties.Add(penalty);
            }

            reader.Close();
        }

        public bool HaveActivePenaltyOfType(Penalties.PenaltyType type)
        {
            foreach(var penalty in penalties)
            {
                if(penalty.type == type)
                {
                    if(penalty.type == Penalties.PenaltyType.CharacterKill)
                    {
                        if (penalty.characterID != character.UID)
                            continue;
                    }

                    return true;
                }
            }

            return false;
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
            var item = Managers.ItemManager.Instance().GetByID(itemUID);

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

            if ((group.specialPermissions & (int)permission) == (int)permission)
                return true;

            return false;
        }

        public bool IsOnDutyOfGroupType(Groups.GroupType type)
        {
            if (groupDuty == null)
                return false;

            if (groupDuty.member.group.type == type)
                return true;

            return false;
        }

        public bool IsInBuildingOfHisGroup()
        {
            if (groupDuty == null)
                return false;

            if (building == null)
                return false;

            if (building.ownerType == OwnerType.Group && building.ownerID == groupDuty.member.groupID)
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
        public int health;

        public int jailBuildingID;
        public Vector3 jailPosition;

        public void Save()
        {
            var command = Database.Instance().Connection.CreateCommand();
            command.CommandText = "UPDATE `rp_characters` SET `model`=@model, `money`=@money, `health`=@health, `jailBuilding`=@jailBuildingID, " +
                "`jailPositionX`=@jailPositionX, `jailPositionY`=@jailPositionY, `jailPositionZ`=@jailPositionZ WHERE `UID`=@UID";
            command.Prepare();

            command.Parameters.AddWithValue("@model", model);
            command.Parameters.AddWithValue("@UID", UID);
            command.Parameters.AddWithValue("@money", money);
            command.Parameters.AddWithValue("@health", health);
            command.Parameters.AddWithValue("jailBuildingID", jailBuildingID);
            command.Parameters.AddWithValue("jailPositionX", jailPosition.X);
            command.Parameters.AddWithValue("jailPositionY", jailPosition.Y);
            command.Parameters.AddWithValue("jailPositionZ", jailPosition.Z);
            command.ExecuteNonQuery();
        }
    }
}
