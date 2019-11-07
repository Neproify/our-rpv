using System;
using System.Collections.Generic;
using GTANetworkAPI;
using Newtonsoft.Json;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Linq;

namespace roleplay.Entities
{
	public class Player
	{
        private bool isLogged = false;
		public string formattedName => handle.Name.Replace("_", " ");

		public bool isBrutallyWounded = false;
		public int secondsToEndOfBrutallyWounded = 0;
		public bool isGagged = false;

        private Client handle;
		public GlobalInfo globalInfo;
		public Character character;
		public GroupDuty groupDuty;
		public Offers.OfferInfo offerInfo;
		public List<Penalties.Penalty> penalties = new List<Penalties.Penalty>();

        //Admin only

        public Admin.SelectedEntities selectedEntities = new Admin.SelectedEntities();

        public Player(Client handle)
        {
            this.handle = handle;
        }

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

        public Vector3 position
        {
            get => handle.Position;
            set => handle.Position = value;
        }

        public int health
        {
            get => handle.Health;
            set => handle.Health = value;
        }

        public bool isInAnyVehicle => handle.IsInVehicle;
        public bool isDriver => handle.VehicleSeat == (int)VehicleSeat.Driver;

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
                SetFreezed(true);
				handle.PlayAnimation("combat@death@from_writhe", "death_a", 9);
				handle.SendNotification("~r~Straciłeś przytomność. Poczekaj aby ją odzyskać.");
			}
			else
			{
				isBrutallyWounded = false;
				secondsToEndOfBrutallyWounded = 0;
				SetFreezed(false);
				handle.StopAnimation();
				handle.SendNotification("~g~Odzyskałeś przytomność. Pamiętaj o odegraniu obrażeń.");
			}
		}

		public void KillCharacter(string reason)
		{
			CreatePenalty(Penalties.PenaltyType.CharacterKill, reason, ObjectId.Empty, DateTime.Now.AddYears(50));
			handle.Kick("Character Kill");
		}

		public Penalties.Penalty CreatePenalty(Penalties.PenaltyType type, string reason, ObjectId penaltiedBy, DateTime expireDate)
		{
            var penalty = new Penalties.Penalty
            {
                UID = ObjectId.GenerateNewId(),
                globalID = globalInfo.UID,
                characterID = character.UID,
                type = type,
                reason = reason,
                penaltiedBy = penaltiedBy,
                expireDate = expireDate
            };

            Database.Instance().GetPenaltiesCollection().InsertOne(penalty);

			penalties.Add(penalty);

			return penalty;
		}

		public void LoadPenalties()
		{
			penalties.Clear();

            var collection = Database.Instance().GetPenaltiesCollection();
            var filter = new MongoDB.Driver.FilterDefinitionBuilder<Penalties.Penalty>().Where(x => x.globalID == globalInfo.UID && x.expireDate > DateTime.Now);
            var cursor = collection.FindSync<Penalties.Penalty>(filter);
            cursor.MoveNext();

            foreach(var penalty in cursor.Current)
            {
                penalties.Add(penalty);
            }
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

        public List<Items.ItemType.Weapon> GetWeapons()
        {
            var weapons = GetItems()?.FindAll(x => x.type == ItemType.Weapon);

            return weapons?.ConvertAll(new Converter<Item, Items.ItemType.Weapon>(x => x as Items.ItemType.Weapon));
        }

        public bool CanUseItem(Item item) => item?.ownerType == OwnerType.Character && item?.ownerID == character.UID;

        public bool CanUseItem(ObjectId itemUID) => CanUseItem(Managers.ItemManager.Instance().GetByID(itemUID));

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
            var vehicles = Managers.VehicleManager.Instance().GetAll();

            return (from veh in vehicles orderby veh.position.DistanceTo(position) ascending select veh).FirstOrDefault();
        }

		public Vehicle GetClosestVehicle(float maxDistance)
		{
            var vehicles = Managers.VehicleManager.Instance().GetAll();

            return (from veh in vehicles where veh.position.DistanceTo(position) <= maxDistance orderby veh.position.DistanceTo(position) ascending select veh).FirstOrDefault();
		}

        public Building GetClosestBuilding(float maxDistance = 3f) => Managers.BuildingManager.Instance().GetClosestBuilding(handle.Position, maxDistance);

        public Player GetClosestPlayer(float maxDistance = 10f)
        {
            var players = Managers.PlayerManager.Instance().GetAll();

            return (from player in players where player.IsReady() && player.position.DistanceTo(position) <= maxDistance orderby player.position.DistanceTo(position) ascending select player).FirstOrDefault();
        }

        public Item GetClosestItem(float maxDistance = 5f) => Managers.ItemManager.Instance().GetClosestItem(handle.Position, maxDistance);

        public Object GetClosestObject(float maxDistance = 10f)
        {
            var objects = Managers.ObjectManager.Instance().GetAll();

            return (from @object in objects where @object.position.DistanceTo(position) <= maxDistance orderby @object.position.DistanceTo(position) ascending select @object).FirstOrDefault();
        }

        public List<Group> GetGroups() => Managers.GroupManager.Instance().GetPlayerGroups(this);

        public bool HasSpecialPermissionInGroup(GroupSpecialPermission permission) => (groupDuty?.member.group.specialPermissions & (int)permission) == (int)permission;

        public bool IsOnDutyOfGroupType(GroupType type) => groupDuty?.member.group.type == type;

        public bool IsOnDutyOfGroupID(ObjectId UID) => groupDuty?.member.group.UID == UID;

        public bool IsInBuildingOfHisGroup() => building?.ownerType == OwnerType.Group && building?.ownerID == groupDuty?.member.group.UID;

        public bool IsAdminOfLevel(AdminLevel level) => globalInfo?.adminLevel >= (int)level;

        public void SendNoPermissionsToCommandNotification() => SendNotification("~r~Nie masz uprawnień do użycia tej komendy!");

        public void SendBrutallyWoundedNoPermissionNotification() => SendNotification("~r~Jesteś nieprzytomny, nie możesz tego zrobić!");

        public void SendGaggedNoPermissionNotification() => SendNotification("~r~Jesteś zakneblowany, nie możesz krzyczeć.");

        public void SendInvalidOwnerTypeNotification() => SendNotification("~r~Podałeś nieprawidłowy typ właściciela.");

        public void SendItemNotFoundNotification() => SendNotification("~r~Nie znaleziono przedmiotu o podanym identyfikatorze.");

        public void SendNotADriverNotification() => SendNotification("~r~Nie siedzisz w żadnym pojeździe lub nie jesteś kierowcą!");

        public void SendPhoneIsNotRespondingNotification() => SendNotification("~r~Telefon nie odpowiada.");

        public void SendPlayerNotFoundNotification() => SendNotification("~r~Nie znaleziono gracza o podanym identyfikatorze.");

        public void SendVehicleNotFoundNotification() => SendNotification("~r~Nie znaleziono pojazdu o podanym identyfikatorze.");

        public void SendGroupNotFoundNotification() => SendNotification("~r~Nie znaleziono grupy o podanym identyfikatorze!");
        public void SendNotification(string message) => handle.SendNotification(message, true);

        public void SendUsageNotification(string message) => SendNotification(message);

        public void SendChatMessage(string message) => handle.SendChatMessage(message);

        public uint GetDimension() => handle.Dimension;

        public void SetDimension(uint dimension) => handle.Dimension = dimension;

        public void SetModel(uint model) => NAPI.Entity.SetEntityModel(GetGameID(), model);

        public uint GetModel() => NAPI.Entity.GetEntityModel(GetGameID());

        public void SetFreezed(bool isFreezed)
        {
#warning NOT IMPLEMENTED, FREEZED
        }

        public void SetInvicible(bool isInvicible)
        {
#warning NOT IMPLEMENTED, INVICIBLE
        }

        public void SetTransparency(int transparency) => handle.Transparency = transparency;

        public Entities.Vehicle GetVehicle() => Managers.VehicleManager.Instance().GetByHandle(handle.Vehicle);

        public int GetVehicleSeat() => handle.VehicleSeat;

        public void TriggerEvent(string eventName, params object[] args) => handle.TriggerEvent(eventName, args);

        public void SetName(string name) => handle.Name = name;

        public string GetName() => handle.Name;

        public void GiveWeapon(WeaponHash weapon, int ammo) => handle.GiveWeapon(weapon, ammo);

        public void RemoveWeapon(WeaponHash weapon) => handle.RemoveWeapon(weapon);

        public void Kick(string reason, ObjectId penaltiedBy)
        {
            CreatePenalty(Penalties.PenaltyType.Kick, reason, penaltiedBy, DateTime.Now);
            handle.Kick(reason);
        }

        public void SilentKick(string reason)
        {
            handle.Kick(reason);
        }
        
        public void Ban(string reason, ObjectId penaltiedBy)
        {
            CreatePenalty(Penalties.PenaltyType.Ban, reason, penaltiedBy, DateTime.Now.AddYears(50));
            handle.Kick(reason);
        }

        public NetHandle GetGameID() => handle.Handle;

        public bool IsOwnerOfVehicle(Entities.Vehicle vehicle) => vehicle.vehicleData.ownerType == OwnerType.Character && vehicle.vehicleData.ownerID == character?.UID;

        public void LoadLook()
        {
            if (character == null)
                return;

            switch(character.gender)
            {
                case "male":
                    SetModel(0x705E61F2);
                    break;
                case "female":
                    SetModel(0x9C9EFFD8);
                    break;
                default:
                    SetModel(0x705E61F2);
                    break;
            }

            character.faceFeatures?.ForEach(x => handle.SetFaceFeature(x.index, x.value));
            character.clothOptions?.ForEach(x => handle.SetClothes(x.index, x.value, 0));
            character.propOptions?.ForEach(x => handle.SetAccessories(x.index, x.value, 0));
        }
    }

	public class GlobalInfo
	{
		public ObjectId UID;
		public string name;
		public int score;
		public int adminLevel;
		public int adminPermissions;
	}

	public class Character
	{
        [BsonId]
        [BsonElement("_id")]
		public ObjectId UID;

        [BsonElement("globalid")]
		public ObjectId GID;

        [BsonElement("name")]
		public string name;

        [BsonRepresentation(BsonType.Int64)]
        [BsonElement("model")]
		public uint model;

        [BsonElement("money")]
		public int money;

        [BsonElement("health")]
		public int health;

        [BsonElement("jailbuildingid")]
		public ObjectId jailBuildingID;

        [BsonElement("jailposition")]
        public Vector3 jailPosition;

        [BsonElement("gender")]
        public string gender;

        [BsonElement("facefeatures")]
        public List<FaceCustomizationPacket> faceFeatures;

        [BsonElement("clothoptions")]
        public List<ClothCustomizationPacket> clothOptions;

        [BsonElement("propoptions")]
        public List<PropCustomizationPacket> propOptions;

		public void Save()
		{
            var collection = Database.Instance().GetCharactersCollection();
            var builder = new MongoDB.Driver.FilterDefinitionBuilder<Character>();
            var filter = builder.Where(x => x.UID == this.UID);
            collection.FindOneAndReplace<Character>(filter, this);
		}
	}
}
