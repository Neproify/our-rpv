using MongoDB.Driver;

namespace roleplay
{
    public class Database
    {
        public MongoClient client;

        public bool Connect()
        {
            client = new MongoClient("mongodb://127.0.0.1:27017");

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
    }
}
