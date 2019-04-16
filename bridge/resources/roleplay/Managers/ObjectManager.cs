using System.Collections.Generic;
using GTANetworkAPI;
using MongoDB.Bson;

namespace roleplay.Managers
{
    public class ObjectManager
    {
        public List<Entities.Object> objects = new List<Entities.Object>();

        private static ObjectManager _instance;
        public static ObjectManager Instance()
        {
            return _instance ?? (_instance = new ObjectManager());
        }

        public void Add(Entities.Object @object)
        {
            objects.Add(@object);
        }

        public void Remove(Entities.Object @object)
        {
            objects.Remove(@object);
        }

        public Entities.Object GetByID(ObjectId ID)
        {
            return objects.Find(x => x.UID == ID);
        }

        public void SaveAll()
        {
            objects.ForEach(x => x.Save());
        }

        public void LoadFromDatabase()
        {
            var collection = Database.Instance().GetGameDatabase().GetCollection<Entities.Object>("objects");
            var cursor = collection.FindSync<Entities.Object>(new BsonDocument());
            cursor.MoveNext();

            foreach(var @object in cursor.Current)
            {
                Add(@object);
                @object.Spawn();
            }
        }

        public Entities.Object Load(ObjectId UID)
        {
            var collection = Database.Instance().GetGameDatabase().GetCollection<Entities.Object>("objects");
            var filter = new MongoDB.Driver.FilterDefinitionBuilder<Entities.Object>().Where(x => x.UID == UID);
            var cursor = collection.FindSync<Entities.Object>(filter);
            cursor.MoveNext();

            foreach(var @object in cursor.Current)
            {
                Add(@object);
                @object.Spawn();
                return @object;
            }

            return null;
        }

        public Entities.Object CreateObject()
        {
            var @object = new Entities.Object
            {
                UID = ObjectId.GenerateNewId(),
                model = 579156093,
                ownerType = OwnerType.None,
                ownerID = ObjectId.Empty,
                position = new Vector3(),
                rotation = new Vector3()
            };

            var collection = Database.Instance().GetGameDatabase().GetCollection<Entities.Object>("objects");
            collection.InsertOne(@object);

            Add(@object);
            @object.Spawn();

            return @object;
        }
    }
}
