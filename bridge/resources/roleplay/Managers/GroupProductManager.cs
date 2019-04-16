using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace roleplay.Managers
{
    public class GroupProductManager
    {
        private List<GroupProduct> products;

        private static GroupProductManager _instance;
        public static GroupProductManager Instance()
        {
            return _instance ?? (_instance = new GroupProductManager());
        }

        public GroupProductManager()
        {
            products = new List<GroupProduct>();
        }

        public void Add(GroupProduct product)
        {
            products.Add(product);
        }

        public void Remove(GroupProduct product)
        {
            products.Remove(product);
        }

        public List<GroupProduct> GetAll()
        {
            return products;
        }

        public GroupProduct GetByID(ObjectId ID)
        {
            return products.Find(x => x.UID == ID);
        }

        public List<GroupProduct> GetProductsForGroup(Entities.Group group)
        {
            return products.FindAll(x => x.CanBeBoughtByGroup(group));
        }

        public void LoadFromDatabase()
        {
            var collection = Database.Instance().GetGameDatabase().GetCollection<GroupProduct>("groupproducts");
            var cursor = collection.FindSync<GroupProduct>(new BsonDocument());
            cursor.MoveNext();

            foreach (var product in cursor.Current)
            {
                Add(product);
            }
        }

        public GroupProduct Load(ObjectId UID)
        {
            var collection = Database.Instance().GetGameDatabase().GetCollection<GroupProduct>("groupproducts");
            var filter = new MongoDB.Driver.FilterDefinitionBuilder<GroupProduct>().Where(x => x.UID == UID);
            var cursor = collection.FindSync<GroupProduct>(filter);
            cursor.MoveNext();

            foreach(var product in cursor.Current)
            {
                Add(product);
                return product;
            }

            return null;
        }

        public void SaveAll()
        {
            products.ForEach(x => x.Save());
        }

    }
}
