using GTANetworkAPI;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace roleplay.Entities
{
    public class Object
    {
        [BsonId]
        [BsonElement("_id")]
        public ObjectId UID;

        [BsonRepresentation(BsonType.Int64)]
        [BsonElement("model")]
        public uint model;

        [BsonElement("position")]
        public Vector3 position;

        [BsonElement("rotation")]
        public Vector3 rotation;

        [BsonElement("ownertype")]
        public OwnerType ownerType;

        [BsonElement("ownerid")]
        public ObjectId ownerID;

        [BsonIgnore]
        public GTANetworkAPI.Object handle;

        public Object()
        {
        }

        public void Spawn()
        {
            if (IsSpawned())
                UnSpawn();

            handle = NAPI.Object.CreateObject(model, position, rotation, 255, GetDimension());
        }

        public void UnSpawn()
        {
            if (!IsSpawned())
                return;

            handle.Delete();
            handle = null;
        }

        public bool IsSpawned()
        {
            return handle != null;
        }

        public void Save()
        {
            var collection = Database.Instance().GetObjectsCollection();
            var filter = new MongoDB.Driver.FilterDefinitionBuilder<Object>().Where(x => x.UID == UID);
            collection.FindOneAndReplace<Building>(filter, this);
        }

        public uint GetDimension()
        {
            if (ownerType == OwnerType.World)
                return 0;

            if(ownerType == OwnerType.Building)
            {
                var building = Managers.BuildingManager.Instance().GetByID(ownerID);

                if (building == null)
                    return 0;

                return building.exitDimension;
            }

            return 0;
        }
    }
}
