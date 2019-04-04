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

    public enum ItemType
    {
        Invalid = -1,
        None,
        Weapon,
        Document,
        Phone
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
        Mafia
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
        VehicleLicenseDocument
    }
	public class ItemInfo
	{
		public int UID;
		public string name;
	}
}
