using System;
using System.Linq.Expressions;
using MongoDB.Driver;

namespace roleplay
{
    public class Database
    {
        public MongoClient client;

        public bool Connect()
        {
            try
            {
                client = new MongoClient("mongodb://127.0.0.1:27017");
            }
            catch (Exception e)
            {
#warning Add some feedback about connection problems.
                return false;
            }

            return true;
        }

        private static Database _instance;
        public static Database Instance()
        {
            return _instance ?? (_instance = new Database());
        }

        public IMongoDatabase GetForumDatabase()
        {
            return client.GetDatabase("nodebb");
        }

        public IMongoDatabase GetGameDatabase()
        {
            return client.GetDatabase("game");
        }

        public IMongoCollection<Entities.Character> GetCharactersCollection()
        {
            return GetGameDatabase().GetCollection<Entities.Character>("characters");
        }

        public IMongoCollection<Entities.Building> GetBuildingsCollection()
        {
            return GetGameDatabase().GetCollection<Entities.Building>("buildings");
        }

        public IMongoCollection<Penalties.Penalty> GetPenaltiesCollection()
        {
            return GetGameDatabase().GetCollection<Penalties.Penalty>("penalties");
        }

        public IMongoCollection<Entities.VehicleData> GetVehiclesCollection()
        {
            return GetGameDatabase().GetCollection<Entities.VehicleData>("vehicles");
        }

        public IMongoCollection<Entities.Item> GetItemsCollection()
        {
            return GetGameDatabase().GetCollection<Entities.Item>("items");
        }

        public IMongoCollection<Entities.Object> GetObjectsCollection()
        {
            return GetGameDatabase().GetCollection<Entities.Object>("objects");
        }

        public IMongoCollection<Entities.Group> GetGroupsCollection()
        {
            return GetGameDatabase().GetCollection<Entities.Group>("groups");
        }

        public IMongoCollection<GroupProduct> GetGroupProductsCollection()
        {
            return GetGameDatabase().GetCollection<GroupProduct>("groupproducts");
        }
    }
}
