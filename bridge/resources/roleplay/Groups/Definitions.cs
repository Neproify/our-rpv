using System;
using System.Collections.Generic;
using System.Text;

namespace roleplay.Groups
{
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
