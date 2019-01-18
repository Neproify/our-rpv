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
        Workshop
    }

    public enum MemberPermission
    {
        None,
        MembersManagement = 1,
        VehicleManagement = 2,
        BuildingsManagement = 4
    }
}
