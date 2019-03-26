using GTANetworkAPI;

namespace roleplay
{
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

            return OwnerType.Invalid;
        }

        public static string GetNameFromOwnerType(OwnerType type)
        {
            switch (type)
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
                    return "Niezdefiniowany";
            }
        }

        public static string GetOwnerTypes()
        {
            string[] types = new string[sizeof(OwnerType)];
            for (int i = 0; i < sizeof(OwnerType); i++)
            {
                types[i] = GetNameFromOwnerType((OwnerType)i);
            }

            return string.Join(",", types);
        }

        public static ItemType GetItemTypeByName(string name)
        {
            if (name == "brak")
                return ItemType.None;

            if (name == "bron" || name == "broń")
                return ItemType.Weapon;

            if (name == "dokument")
                return ItemType.Document;

            if (name == "telefon")
                return ItemType.Phone;

            return ItemType.Invalid;
        }

        public static string GetNameFromItemType(ItemType type)
        {
            switch (type)
            {
                case ItemType.None:
                    return "Brak";
                case ItemType.Weapon:
                    return "Broń";
                case ItemType.Document:
                    return "Dokument";
                case ItemType.Phone:
                    return "Telefon";
                default:
                    NAPI.Util.ConsoleOutput($"GETNAMEFROMITEMTYPE ERROR, TYPE: {type}");
                    return "Brak";
            }
        }

        public static string GetItemTypes()
        {
            string[] types = new string[sizeof(ItemType)];
            for (int i = 0; i < sizeof(ItemType); i++)
            {
                types[i] = GetNameFromItemType((ItemType)i);
            }

            return string.Join(",", types);
        }

        public static GroupType GetGroupTypeByName(string name)
        {
            if (name == "rzad" || name == "rząd" || name == "urzad" || name == "urząd")
                return GroupType.Government;

            if (name == "policja")
                return GroupType.Police;

            if (name == "szpital")
                return GroupType.Medical;

            if (name == "radio")
                return GroupType.Radio;

            if (name == "warsztat")
                return GroupType.Workshop;

            if (name == "gang")
                return GroupType.Gang;

            if (name == "mafia")
                return GroupType.Mafia;

            return GroupType.None;
        }

        public static string GetNameFromGroupType(GroupType name)
        {
            switch (name)
            {
                case GroupType.None:
                    return "Brak";
                case GroupType.Government:
                    return "Urząd";
                case GroupType.Police:
                    return "Policja";
                case GroupType.Medical:
                    return "Szpital";
                case GroupType.Radio:
                    return "Radio";
                case GroupType.Workshop:
                    return "Warsztat";
                case GroupType.Gang:
                    return "Gang";
                case GroupType.Mafia:
                    return "Mafia";

                default:
                    return "Brak";
            }
        }

        public static string GetGroupTypes()
        {
            string[] types = new string[sizeof(GroupType)];
            for (int i = 0; i < sizeof(GroupType); i++)
            {
                types[i] = GetNameFromGroupType((GroupType)i);
            }

            return string.Join(",", types);
        }
    }
}
