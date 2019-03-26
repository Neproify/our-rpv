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
        ItemsManagement = 16
    }
}
