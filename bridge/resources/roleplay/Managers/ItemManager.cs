using System;
using System.Collections.Generic;
using System.Text;

namespace roleplay.Managers
{
    public class ItemManager
    {
        private List<Entities.Item> items;
        private Dictionary<OwnerType, Dictionary<int, List<Entities.Item>>> itemsOfOwner;

        public ItemManager()
        {
            // Messy but works.
            items = new List<Entities.Item>();
            itemsOfOwner = new Dictionary<OwnerType, Dictionary<int, List<Entities.Item>>>();
            foreach (var i in Enum.GetValues(typeof(OwnerType)))
            {
                itemsOfOwner[(OwnerType)i] = new Dictionary<int, List<Entities.Item>>();
            }
        }

        private static ItemManager _instance;
        public static ItemManager Instance()
        {
            if (_instance == null)
                _instance = new ItemManager();
            return _instance;
        }

        public void Add(Entities.Item item)
        {
            items.Add(item);

            if(!itemsOfOwner[item.ownerType].ContainsKey(item.ownerID))
            {
                itemsOfOwner[item.ownerType].Add(item.ownerID, new List<Entities.Item>());
            }

            itemsOfOwner[item.ownerType][item.ownerID].Add(item);
        }

        public List<Entities.Item> GetItemsOf(OwnerType ownerType, int ownerID)
        {
            if (!itemsOfOwner[ownerType].ContainsKey(ownerID))
                return null;

            return itemsOfOwner[ownerType][ownerID];
        }

        public void LoadFromDatabase()
        {
            var command = Database.Instance().Connection.CreateCommand();
            command.CommandText = "SELECT * FROM `rp_items`;";
            var reader = command.ExecuteReader();

            while(reader.Read())
            {
                var item = new Entities.Item
                {
                    UID = reader.GetInt32("UID"),
                    name = reader.GetString("name"),
                    type = reader.GetInt32("type"),
                    properties = reader.GetString("properties"),
                    ownerType = (OwnerType)reader.GetInt32("ownerType"),
                    ownerID = reader.GetInt32("ownerID")
                };

                Add(item);
            }

            reader.Close();
        }
    }
}
