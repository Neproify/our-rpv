using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace roleplay
{
    /*
     *  None,
        Character,
        World,
        Building,
        Group
        */
    public static class Utils
    {
        public static OwnerType GetOwnerTypeByName(string name)
        {
            if (name == "brak")
                return OwnerType.None;

            if (name == "gracz")
                return OwnerType.Character;

            if (name == "swiat" || name == "świat")
                return OwnerType.World;

            if (name == "budynek")
                return OwnerType.Building;

            if (name == "grupa")
                return OwnerType.Group;

            return OwnerType.None;
        }

        public static string GetNameFromOwnerType(OwnerType type)
        {
            switch(type)
            {
                case OwnerType.None:
                    return "Brak";
                case OwnerType.Character:
                    return "Gracz";
                case OwnerType.World:
                    return "Świat";
                case OwnerType.Building:
                    return "Budynek";
                case OwnerType.Group:
                    return "Grupa";
                default:
                    NAPI.Util.ConsoleOutput($"GETNAMEFROMOWNERTYPE ERROR, TYPE: {type}");
                    return "Brak";
            }
        }
    }
}
