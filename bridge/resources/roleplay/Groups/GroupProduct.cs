using System;
using System.Collections.Generic;
using System.Text;

namespace roleplay.Groups
{
    public class GroupProduct
    {
        public int UID;
        public string name;
        public ItemType type;
        public string propertiesString;
        public int price;

        public bool CanBeBoughtByGroup(Entities.Group group)
        {
            var products = Managers.GroupProductManager.Instance().GetProductsForGroup(group);

            if (products.Contains(this))
                return true;

            return false;
        }

        public void Save()
        {
#warning Implement this(GroupProduct).
        }
    }
}
