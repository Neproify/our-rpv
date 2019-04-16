using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace roleplay.Penalties
{
    public enum PenaltyType
    {
        Warning,
        Reward,
        CharacterKill,
        Kick,
        Ban
    }

    public class Penalty
    {
        [BsonId]
        [BsonElement("_id")]
        public ObjectId UID;

        [BsonElement("globalid")]
        public ObjectId globalID;

        [BsonElement("characterid")]
        public ObjectId characterID;

        [BsonElement("type")]
        public PenaltyType type;

        [BsonElement("reason")]
        public string reason;

        [BsonElement("penaltiedby")]
        public ObjectId penaltiedBy;

        [BsonElement("expiredate")]
        public DateTime expireDate;

        public void Save()
        {
            var collection = Database.Instance().GetGameDatabase().GetCollection<Penalty>("penalties");
            var filter = new MongoDB.Driver.FilterDefinitionBuilder<Penalty>().Where(x => x.UID == UID);
            collection.FindOneAndReplace<Penalty>(filter, this);
        }
    }
}
