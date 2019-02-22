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
            var command = Database.Instance().Connection.CreateCommand();
            command.CommandText = "UPDATE `rp_products` SET `name`=@name, `type`=@type, `properties`=@properties, `price`=@price WHERE `UID`=@UID;";
            command.Prepare();

            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@type", type);
            command.Parameters.AddWithValue("@properties", propertiesString);
            command.Parameters.AddWithValue("@price", price);
            command.Parameters.AddWithValue("@UID", UID);

            command.ExecuteNonQuery();
        }
    }
}
