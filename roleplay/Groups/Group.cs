using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace roleplay.Entities
{
    public class Group
    {
        [BsonId]
        [BsonElement("_id")]
        public ObjectId UID;

        [BsonElement("name")]
        public string name;

        [BsonElement("bank")]
        public int bank;

        [BsonElement("leaderrank")]
        public ObjectId leaderRank;

        [BsonElement("leaderid")]
        public ObjectId leaderID;

        [BsonElement("type")]
        public GroupType type;

        [BsonElement("specialpermissions")]
        public int specialPermissions;

        [BsonElement("ranks")]
        public List<GroupRank> ranks;

        [BsonElement("members")]
        public List<GroupMember> members;

        public Group()
        {
            ranks = new List<GroupRank>();
            members = new List<GroupMember>();
        }

        public GroupMember GetMember(Player player)
        {
            var member = members.Find(x => x.charID == player.character.UID);

            return member;
        }

        public List<Player> GetPlayersOnDuty()
        {
            return Managers.PlayerManager.Instance().GetAll().FindAll(x => x.groupDuty?.member.group == this);
        }

        public List<Item> GetItems()
        {
            return Managers.ItemManager.Instance().GetItemsOf(OwnerType.Group, UID);
        }

        public void Save()
        {
            var collection = Database.Instance().GetGroupsCollection();
            var builder = new MongoDB.Driver.FilterDefinitionBuilder<Group>();
            var filter = builder.Where(x => x.UID == UID);
            collection.FindOneAndReplace<Group>(filter, this);
        }
    }
}
