using GTANetworkAPI;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace roleplay.Entities
{
    public class Building
    {
        [BsonId]
        [BsonElement("_id")]
        public ObjectId UID;

        [BsonElement("name")]
        public string name;
        
        [BsonElement("description")]
        public string description;

        [BsonElement("enterposition")]
        public Vector3 enterPosition;

        [BsonElement("enterdimension")]
        public uint enterDimension;

        [BsonElement("exitposition")]
        public Vector3 exitPosition;

        [BsonIgnore]
        public bool isLocked = false;

        [BsonIgnore]
        public uint exitDimension => enterDimension + 10000;

        [BsonElement("ownertype")]
        public OwnerType ownerType;

        [BsonElement("ownerid")]
        public ObjectId ownerID;

        [BsonIgnore]
        public Marker enterMarker;
        [BsonIgnore]
        public Marker exitMarker;

        public Building()
        {
        }

        public void Spawn()
        {
            if (enterMarker != null || exitMarker != null)
                Unspawn();
            enterMarker = NAPI.Marker.CreateMarker((uint)MarkerType.UpsideDownCone, enterPosition, new Vector3(), new Vector3(), 1f, new Color(0, 0, 0, 120), true, enterDimension);
            exitMarker = NAPI.Marker.CreateMarker((uint)MarkerType.UpsideDownCone, exitPosition, new Vector3(), new Vector3(), 1f, new Color(0, 0, 0, 120), true, exitDimension);
        }

        public void Unspawn()
        {
            if (enterMarker != null)
                enterMarker.Delete();

            if (exitMarker != null)
                exitMarker.Delete();
        }

        public void Save()
        {
            var collection = Database.Instance().GetGameDatabase().GetCollection<Building>("buildings");
            var filter = new MongoDB.Driver.FilterDefinitionBuilder<Building>().Where(x => x.UID == UID);
            collection.FindOneAndReplace<Building>(filter, this);
        }

        public bool CanBeAccessedBy(Entities.Player player)
        {
            switch (ownerType)
            {
                case OwnerType.Character when ownerID == player.character.UID:
                case OwnerType.Group when ownerID == player.groupDuty?.member.group.UID:
                    return true;
                default:
                    return false;
            }
        }
    }
}
