using System;
using System.Collections.Generic;
using System.Text;

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
        Document
    }

    public enum DocumentType
    {
        None,
        Personal,
        VehicleLicense
    }
}
