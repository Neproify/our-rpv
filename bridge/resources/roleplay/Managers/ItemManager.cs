using System;
using System.Collections.Generic;
using GTANetworkAPI;
using MongoDB.Bson;

namespace roleplay.Managers
{
    public class ItemManager
    {
        private readonly List<Entities.Item> items;
        private readonly Dictionary<OwnerType, Dictionary<ObjectId, List<Entities.Item>>> itemsOfOwner;

        public ItemManager()
        {
            items = new List<Entities.Item>();
            itemsOfOwner = new Dictionary<OwnerType, Dictionary<ObjectId, List<Entities.Item>>>();
            foreach (var i in Enum.GetValues(typeof(OwnerType)))
            {
                itemsOfOwner[(OwnerType)i] = new Dictionary<ObjectId, List<Entities.Item>>();
            }
            itemsOfOwner[OwnerType.None][ObjectId.Empty] = new List<Entities.Item>();
            itemsOfOwner[OwnerType.World][ObjectId.Empty] = new List<Entities.Item>();
        }

        private static ItemManager _instance;
        public static ItemManager Instance()
        {
            return _instance ?? (_instance = new ItemManager());
        }

        public void Add(Entities.Item item)
        {
            items.Add(item);

            if (!itemsOfOwner[item.ownerType].ContainsKey(item.ownerID))
            {
                itemsOfOwner[item.ownerType].Add(item.ownerID, new List<Entities.Item>());
            }

            itemsOfOwner[item.ownerType][item.ownerID].Add(item);
        }

        public Entities.Item GetByID(ObjectId UID)
        {
            return items.Find(x => x.UID == UID);
        }

        public List<Entities.Item> GetItemsOf(OwnerType ownerType, ObjectId ownerID)
        {
            if (!itemsOfOwner[ownerType].ContainsKey(ownerID))
                return null;

            return itemsOfOwner[ownerType][ownerID];
        }

        public Entities.Item GetClosestItem(Vector3 position, float maxDistance = 5f)
        {
            var worldItems = itemsOfOwner[OwnerType.World][ObjectId.Empty];
            Entities.Item closestItem = null;
            float distance = maxDistance;
            foreach (var item in worldItems)
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
            var collection = Database.Instance().GetItemsCollection();
            var cursor = collection.FindSync<Entities.Item>(new BsonDocument());
            cursor.MoveNext();

            foreach(var item in cursor.Current)
            {
                var itemTemp = CreateAndGetCorrectType(item);

                Add(itemTemp);
            }
        }

        public void SaveAll()
        {
            items.ForEach(x => x.Save());
        }

        public Entities.Item Load(ObjectId UID)
        {
            var collection = Database.Instance().GetItemsCollection();
            var builder = new MongoDB.Driver.FilterDefinitionBuilder<Entities.Item>();
            var filter = builder.Where(x => x.UID == UID);
            var cursor = collection.FindSync<Entities.Item>(filter);
            cursor.MoveNext();

            foreach (var item in cursor.Current)
            {
                var itemTemp = CreateAndGetCorrectType(item);

                Add(itemTemp);

                return itemTemp;
            }

            return null;
        }

        public Entities.Item CreateItem(string name = "", ItemType type = ItemType.None)
        {
            var item = new Entities.Item
            {
                UID = ObjectId.GenerateNewId(),
                name = name,
                type = type,
                properties = new Dictionary<string, object>(),
                ownerType = OwnerType.None,
                ownerID = ObjectId.Empty,
                position = new Vector3()
            };

            Database.Instance().GetItemsCollection().InsertOne(item);

            var correctItem = CreateAndGetCorrectType(item);

            return correctItem;
        }

        public Entities.Item ReloadItem(Entities.Item item)
        {
            Remove(item);

            return Load(item.UID);
        }

        public List<Items.ItemType.Phone> GetPhones()
        {
            return items.FindAll(x => x.type == ItemType.Phone).ConvertAll(new Converter<Entities.Item, Items.ItemType.Phone>(x => x as Items.ItemType.Phone));
        }

        public Entities.Item CreateAndGetCorrectType(Entities.Item item)
        {
            Entities.Item correct = null;

            switch (item.type)
            {
                case ItemType.None:
                    correct = new Entities.Item();
                    break;
                case ItemType.Weapon:
                    correct = new Items.ItemType.Weapon();
                    break;
                case ItemType.Document:
                    correct = new Items.ItemType.Document();
                    break;
                case ItemType.Phone:
                    correct = new Items.ItemType.Phone();
                    break;
                case ItemType.Balaclava:
                    correct = new Items.ItemType.Balaclava();
                    break;
                default:
                    NAPI.Util.ConsoleOutput("[WARNING]Used default on creating item, type: " + item.type);
                    correct = new Entities.Item();
                    break;
            }

            correct.UID = item.UID;
            correct.name = item.name;
            correct.type = item.type;
            correct.properties = item.properties;
            correct.ownerType = item.ownerType;
            correct.ownerID = item.ownerID;
            correct.position = item.position;

            return correct;
        }
    }
}
