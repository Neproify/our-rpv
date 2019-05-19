using MongoDB.Bson;

namespace roleplay
{
    public enum OwnerType
    {
        Invalid = -1,
        None,
        Character,
        World,
        Building,
        Group
    }

    public enum EntityType
    {
        None,
        Character,
        Vehicle,
        Building,
        Group,
        Item,
        Object
    }

    public enum ItemType
    {
        Invalid = -1,
        None,
        Weapon,
        Document,
        Phone,
        Balaclava,
        Food
    }

    public enum DocumentType
    {
        None,
        Personal,
        VehicleLicense
    }

    public enum GroupType
    {
        None,
        Government,
        Police,
        Medical,
        Radio,
        Workshop,
        Gang,
        Mafia,
        Restaurant,
        Club
    }

    public enum GroupSpecialPermission
    {
        None,
        Megaphone = 1
    }

    public enum GroupMemberPermission
    {
        None,
        MembersManagement = 1,
        VehicleManagement = 2,
        BuildingsManagement = 4,
        OrdersManagement = 8,
        ItemsManagement = 16,
		All = 31
    }

    public enum AdminLevel
    {
        None,
        Supporter,
        GameMaster,
        Admin,
        HeadAdmin
    }

    public enum AdminPermissions
    {
        None
    }

    public enum OfferType
    {
        None,
        Healing,
        Item,
        Ticket,
        VehicleRepair,
        PersonalDocument,
        VehicleLicenseDocument,
        VehicleSell
    }
	public class ItemInfo
	{
		public ObjectId UID;
		public string name;
	}

    public class FaceCustomizationPacket
    {
        public int index;
        public float value;
    }

    public class ClothCustomizationPacket
    {
        public int index;
        public int value;
    }

    public class PropCustomizationPacket
    {
        public int index;
        public int value;
    }
}
