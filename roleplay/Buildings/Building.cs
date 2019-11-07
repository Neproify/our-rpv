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

        [BsonElement("canbeaccessedbyvehicle")]
        public bool canBeEnteredByVehicle => false;

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
        public ColShape enterColShape;
        [BsonIgnore]
        public Marker exitMarker;
        [BsonIgnore]
        public ColShape exitColShape;

        public void Spawn()
        {
            Unspawn();

            enterMarker = NAPI.Marker.CreateMarker((uint)MarkerType.UpsideDownCone, enterPosition, new Vector3(), new Vector3(), 1f, new Color(0, 0, 0, 120), true, enterDimension);
            enterColShape = NAPI.ColShape.CreateCylinderColShape(enterPosition, 2f, 3f, enterDimension);
            exitMarker = NAPI.Marker.CreateMarker((uint)MarkerType.UpsideDownCone, exitPosition, new Vector3(), new Vector3(), 1f, new Color(0, 0, 0, 120), true, exitDimension);
            exitColShape = NAPI.ColShape.CreateCylinderColShape(exitPosition, 2f, 3f, exitDimension);

            enterColShape.OnEntityEnterColShape += OnEntityEnterColShape;
            enterColShape.OnEntityExitColShape += OnEntityExitColShape;
            exitColShape.OnEntityEnterColShape += OnEntityEnterColShape;
            exitColShape.OnEntityExitColShape += OnEntityExitColShape;
        }

        public void Unspawn()
        {
            if (enterMarker != null)
                enterMarker.Delete();

            if (enterColShape != null)
                enterColShape.Delete();

            if (exitMarker != null)
                exitMarker.Delete();

            if (exitColShape != null)
                exitColShape.Delete();
        }

        public void Save()
        {
            var collection = Database.Instance().GetBuildingsCollection();
            var filter = new MongoDB.Driver.FilterDefinitionBuilder<Building>().Where(x => x.UID == UID);
            collection.FindOneAndReplace<Building>(filter, this);
        }

        public bool CanBeAccessedBy(Player player)
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

        private void OnEntityEnterColShape(ColShape colShape, Client client)
        {
            client.SendNotification($"{(isLocked ? "~r~" : "~g~")}Drzwi: {name}{(isLocked == false ? ", użyj ~b~/drzwi ~g~ aby wejść do środka." : "")}");
        }

        private void OnEntityExitColShape(ColShape colShape, Client client)
        {
        }

    }
}
