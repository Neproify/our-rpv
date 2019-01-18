﻿using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

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
            itemsOfOwner[OwnerType.World][0] = new List<Entities.Item>();
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

        public Entities.Item GetItem(int UID)
        {
            return items.Find(x => x.UID == UID);
        }

        public List<Entities.Item> GetItemsOf(OwnerType ownerType, int ownerID)
        {
            if (!itemsOfOwner[ownerType].ContainsKey(ownerID))
                return null;

            return itemsOfOwner[ownerType][ownerID];
        }

        /*public object GetItemConverted(int UID)
        {
            var item = GetItem(UID);

            if (item == null)
                return null;

            object convertedItem = null;

            switch (item.type)
            {
                case (int)ItemType.None:
                    convertedItem = item;
                    break;
                case (int)ItemType.Weapon:
                    convertedItem = item as Items.ItemType.Weapon;
                    break;
            }

            return convertedItem;
        }*/

        public Entities.Item GetClosestItem(Vector3 position, float maxDistance = 5f)
        {
            var items = itemsOfOwner[OwnerType.World][0];
            Entities.Item closestItem = null;
            float distance = maxDistance;
            foreach(var item in items)
            {
                var distanceFromPosition = item.position.DistanceTo(position);
                if (distanceFromPosition <= distance)
                {
                    closestItem = item;
                    distance = distanceFromPosition;
                }
            }

            return closestItem;
        }

        public void Remove(Entities.Item item)
        {
            items.Remove(item);

            itemsOfOwner[item.ownerType][item.ownerID].Remove(item);
        }

        public void LoadFromDatabase()
        {
            var command = Database.Instance().Connection.CreateCommand();
            command.CommandText = "SELECT * FROM `rp_items`;";
            var reader = command.ExecuteReader();

            while(reader.Read())
            {
                var item = CreateItem(reader);

                Add(item);
            }

            reader.Close();
        }

        public void SaveAll()
        {
            items.ForEach(x => x.Save());
        }

        public dynamic CreateItem(MySql.Data.MySqlClient.MySqlDataReader reader)
        {
            dynamic item;

            var UID = reader.GetInt32("UID");
            var name = reader.GetString("name");
            var type = reader.GetInt32("type");
            var properties = reader.GetString("properties");
            var ownerType = (OwnerType)reader.GetInt32("ownerType");
            var ownerID = reader.GetInt32("ownerID");
            var position = new Vector3();
            position.X = reader.GetFloat("positionX");
            position.Y = reader.GetFloat("positionY");
            position.Z = reader.GetFloat("positionZ");

            switch(type)
            {
                case (int)ItemType.None:
                    item = new Entities.Item();
                    break;
                case (int)ItemType.Weapon:
                    item = new Items.ItemType.Weapon();
                    break;
                default:
                    NAPI.Util.ConsoleOutput("[WARNING]Used default on creating item, type: " + type);
                    item = new Entities.Item();
                    break;
            }

            item.UID = UID;
            item.name = name;
            item.type = type;
            item.propertiesString = properties;
            item.ownerType = ownerType;
            item.ownerID = ownerID;
            item.position = position;

            return item;
        }
    }
}
