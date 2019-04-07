using System;
using System.Collections.Generic;
using GTANetworkAPI;
using Newtonsoft.Json;

namespace roleplay.Entities
{
	public class Player
	{
		private bool isLogged;
		public string formattedName => handle.Name.Replace("_", " ");

		public bool isBrutallyWounded = false;
		public int secondsToEndOfBrutallyWounded = 0;
		public bool isGagged = false;

		public Client handle;
		public GlobalInfo globalInfo;
		public Character character;
		public GroupDuty groupDuty;
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

		public Vehicle vehicle
		{
			get
			{
				if (handle.Vehicle == null)
					return null;

				return Managers.VehicleManager.Instance().GetByHandle(handle.Vehicle);
			}
		}

		public Building building;
		public Items.ItemType.Phone activePhone = null;
		public Items.ItemType.PhoneCall phoneCall = null;

		public void Save()
		{
			character.Save();
		}

        public bool IsLoggedIn() => isLogged;

        public void SetIsLoggedIn(bool logged) => isLogged = logged;

        public bool IsReady() => IsLoggedIn() && character != null;

		public void OutputMe(string action)
		{
			var players = NAPI.Player.GetPlayersInRadiusOfPosition(20, handle.Position);

			foreach (var player in players)
			{
				player.SendChatMessage($"!{{#C2A2DA}}*{formattedName} {action}");
			}
		}

		public void OutputDo(string action)
		{
			var players = NAPI.Player.GetPlayersInRadiusOfPosition(20, handle.Position);

			foreach (var nearPlayer in players)
			{
				nearPlayer.SendChatMessage($"!{{#9A9CCD}}* {action} (({formattedName}))");
			}
		}

		public bool SendMoneyTo(Player player, int amount)
		{
			if (!isLogged || character == null)
				return false;

			if (!player.IsReady())
				return false;

			if (money < amount)
			{
				handle.SendNotification("~r~Masz za mało pieniędzy!");
				return false;
			}

			money -= amount;
			player.money += amount;

			return true;
		}

		public bool SendMoneyTo(Group group, int amount)
		{
			if (isLogged || character == null)
				return false;

			if (money < amount)
			{
				handle.SendNotification("~r~Masz za mało pieniędzy!");
				return false;
			}

			money -= amount;
			group.bank += amount;

			return true;
		}

		public void SetIsBrutallyWounded(bool isWounded)
		{
			if (isWounded)
			{
				isBrutallyWounded = true;
				secondsToEndOfBrutallyWounded = 60 * 5;

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
			var command = Database.Instance().connection.CreateCommand();
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

			var command = Database.Instance().connection.CreateCommand();
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
			foreach (var penalty in penalties)
			{
				if (penalty.type == type)
				{
					if (penalty.type == Penalties.PenaltyType.CharacterKill)
					{
						if (penalty.characterID != character.UID)
							continue;
					}

					return true;
				}
			}

			return false;
		}

		public List<Item> GetItems() => isLogged ? Managers.ItemManager.Instance().GetItemsOf(OwnerType.Character, character.UID) : null;

		public bool CanUseItem(Item item) => item?.ownerType == OwnerType.Character && item?.ownerID == character.UID;

        public bool CanUseItem(int itemUID) => CanUseItem(Managers.ItemManager.Instance().GetByID(itemUID));

        public bool IsUsingItemOfType(ItemType type) => GetItems().Find(x => x.type == type && x.isUsed == true) != null;

        public void ShowItems()
		{
			List<ItemInfo> items = new List<ItemInfo>();

			foreach (var item in GetItems())
			{
				items.Add(new ItemInfo
				{
					UID = item.UID,
					name = item.name
				});

            }

			var output = JsonConvert.SerializeObject(items);

			handle.TriggerEvent("ShowPlayerItems", output);
		}

        public void ReloadItems() => ShowItems();

        public Vehicle GetClosestVehicle()
		{
			var vehicles = NAPI.Pools.GetAllVehicles();

			GTANetworkAPI.Vehicle returnedVehicle = null;
			float distance = 99999;

			foreach (var loopVehicle in vehicles)
			{
				var tempDistance = handle.Position.DistanceTo(loopVehicle.Position);
				if (tempDistance < distance)
				{
					distance = tempDistance;
					returnedVehicle = loopVehicle;
				}
			}

			if (returnedVehicle == null)
				return null;

			return Managers.VehicleManager.Instance().GetByHandle(returnedVehicle);
		}

		public Vehicle GetClosestVehicle(float maxDistance)
		{
			var closestVehicle = GetClosestVehicle();

			if (closestVehicle == null)
				return null;

			if (handle.Position.DistanceTo(closestVehicle.handle.Position) >= maxDistance)
				return null;

			return closestVehicle;
		}

        public Building GetClosestBuilding(float maxDistance = 3f) => Managers.BuildingManager.Instance().GetClosestBuilding(handle.Position, maxDistance);

        public Item GetClosestItem(float maxDistance = 5f) => Managers.ItemManager.Instance().GetClosestItem(handle.Position, maxDistance);

        public List<Group> GetGroups() => Managers.GroupManager.Instance().GetPlayerGroups(this);

        public bool HasSpecialPermissionInGroup(GroupSpecialPermission permission) => (groupDuty?.member.group.specialPermissions & (int)permission) == (int)permission;

        public bool IsOnDutyOfGroupType(GroupType type) => groupDuty?.member.group.type == type;

        public bool IsOnDutyOfGroupID(int UID) => groupDuty?.member.groupID == UID;

        public bool IsInBuildingOfHisGroup() => building?.ownerType == OwnerType.Group && building?.ownerID == groupDuty?.member.groupID;

        public bool IsAdminOfLevel(AdminLevel level) => globalInfo?.adminLevel >= (int)level;
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
			var command = Database.Instance().connection.CreateCommand();
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
