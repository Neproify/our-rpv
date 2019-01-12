using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace roleplay.Entities
{
    public class Player
    {
        public bool isLogged;

        public Client handle;
        public GlobalInfo globalInfo;
        public Character character;

        public List<Entities.Item> GetItems()
        {
            if (!isLogged)
                return null;

            return Managers.ItemManager.Instance().GetItemsOf(OwnerType.Character, character.UID);
        }

        public bool CanUseItem(int itemUID)
        {
            var item = Managers.ItemManager.Instance().GetItem(itemUID);

            if (item == null)
                return false;

            if (item.ownerType == OwnerType.Character && item.ownerID == character.UID)
                return true;

            return false;
        }
    }

    public class GlobalInfo
    {
        public int UID;
        public string name;
    }

    public class Character
    {
        public int UID;
        public int GID;
        public string name;
    }
}
