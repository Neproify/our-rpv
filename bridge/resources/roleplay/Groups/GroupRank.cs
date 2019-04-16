using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace roleplay
{
    public class GroupRank
    {
        [BsonId]
        [BsonElement("_id")]
        public ObjectId UID;

        [BsonElement("name")]
        public string name;

        [BsonElement("salary")]
        public int salary;

        [BsonElement("skin")]
        public uint skin;

        [BsonElement("permissions")]
        public int permissions;

        [BsonIgnore]
        public Entities.Group group;

        public void Save()
        {
            // Nothing to save, left it here.
        }
    }
}
