using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace roleplay
{
    public class GroupMember
    {
        [BsonId]
        [BsonElement("_id")]
        public ObjectId UID;

        [BsonElement("characterid")]
        public ObjectId charID;

        [BsonElement("dutytime")]
        public int dutyTime;

        [BsonIgnore]
        public Entities.Group group;
        [BsonIgnore]
        public GroupRank rank;

        public void Save()
        {
        }
    }
}
