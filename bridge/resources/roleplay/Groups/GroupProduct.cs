using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace roleplay
{
    public class GroupProduct
    {
        [BsonId]
        [BsonElement("_id")]
        public ObjectId UID;

        [BsonElement("name")]
        public string name;

        [BsonElement("type")]
        public ItemType type;

        [BsonElement("propertiesstring")]
        public string propertiesString;

        [BsonElement("price")]
        public int price;

        [BsonElement("grouptypes")]
        public List<GroupType> groupTypes;

        [BsonElement("groupids")]
        public List<ObjectId> groupIds;

        public bool CanBeBoughtByGroup(Entities.Group group)
        {
            if (groupTypes.Contains(group.type))
                return true;

            if (groupIds.Contains(group.UID))
                return true;

            return false;
        }

        public void Save()
        {
            var collection = Database.Instance().GetGameDatabase().GetCollection<GroupProduct>("groupproducts");
            var filter = new MongoDB.Driver.FilterDefinitionBuilder<GroupProduct>().Where(x => x.UID == UID);
            collection.FindOneAndReplace<GroupProduct>(filter, this);
        }
    }
}
