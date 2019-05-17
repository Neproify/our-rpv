using System.Collections.Generic;
using MongoDB.Bson;

namespace roleplay.Managers
{
    public class GroupManager
    {
        public List<Entities.Group> groups = new List<Entities.Group>();

        private static GroupManager _instance;
        public static GroupManager Instance()
        {
            return _instance ?? (_instance = new GroupManager());
        }

        public void Add(Entities.Group group)
        {
            groups.Add(group);
        }

        public void SaveAll()
        {
            groups.ForEach(x => x.Save());
        }

        public Entities.Group GetByID(ObjectId ID)
        {
            return groups.Find(x => x.UID == ID);
        }

        public List<Entities.Group> GetAll()
        {
            return groups;
        }

        public List<Entities.Group> GetPlayerGroups(Entities.Player player)
        {
            return groups.FindAll(x => x.members.Find(y => y.charID == player.character.UID) != null);
        }

        public void LoadFromDatabase()
        {
            var collection = Database.Instance().GetGroupsCollection();
            var cursor = collection.FindSync<Entities.Group>(new BsonDocument());
            cursor.MoveNext();

            foreach(var group in cursor.Current)
            {
                Add(group);
            }
        }

		public Entities.Group Load(ObjectId UID)
		{
            var collection = Database.Instance().GetGroupsCollection();
            var filter = new MongoDB.Driver.FilterDefinitionBuilder<Entities.Group>().Where(x => x.UID == UID);
            var cursor = collection.FindSync<Entities.Group>(filter);
            cursor.MoveNext();

            foreach(var group in cursor.Current)
            {
                Add(group);
                return group;
            }

            return null;
		}

        public Entities.Group CreateGroup()
        {
            var group = new Entities.Group
            {
                UID = MongoDB.Bson.ObjectId.GenerateNewId(),
                name = "Nowa groupa",
                bank = 0,
                leaderRank = MongoDB.Bson.ObjectId.GenerateNewId(),
                leaderID = MongoDB.Bson.ObjectId.Empty,
                type = GroupType.None,
                specialPermissions = 0
            };

            var leaderRank = new GroupRank
            {
                UID = group.leaderRank,
                name = "Lider",
                salary = 0,
                skin = 0,
                permissions = 0
            };

            group.ranks.Add(leaderRank);
            leaderRank.group = group;

            var collection = Database.Instance().GetGroupsCollection();
            collection.InsertOne(group);

            Add(group);

            return group;
        }
    }
}
