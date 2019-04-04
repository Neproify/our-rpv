using System.Collections.Generic;

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

        public Entities.Group GetByID(int ID)
        {
            return groups.Find(x => x.UID == ID);
        }

        public List<Entities.Group> GetPlayerGroups(Entities.Player player)
        {
            return groups.FindAll(x => x.members.Find(y => y.charID == player.character.UID) != null);
        }

        public void LoadFromDatabase()
        {
            var command = Database.Instance().connection.CreateCommand();
            command.CommandText = "SELECT * FROM `rp_groups`";
            var reader = command.ExecuteReader();

            while(reader.Read())
            {
                Load(reader);
            }

            reader.Close();

            groups.ForEach(x => x.LoadRanks());
            groups.ForEach(x => x.LoadMembers());
        }

        public Entities.Group Load(MySql.Data.MySqlClient.MySqlDataReader reader)
        {
            var group = new Entities.Group
            {
                UID = reader.GetInt32("UID"),
                name = reader.GetString("name"),
                bank = reader.GetInt32("bank"),
                leaderRank = reader.GetInt32("leaderRank"),
                leaderID = reader.GetInt32("leaderID"),
                type = (GroupType)reader.GetInt32("type"),
                specialPermissions = reader.GetInt32("specialPermissions")
            };

            Add(group);

            return group;
        }

		public Entities.Group Load(int UID)
		{
			var command = Database.Instance().connection.CreateCommand();
			command.CommandText = "SELECT * FROM `rp_groups` WHERE `UID` = @UID";

			var reader = command.ExecuteReader();

			reader.Read();

			var group = Load(reader);

			reader.Close();

			return group;
		}

        public Entities.Group CreateGroup()
        {
			var command = Database.Instance().connection.CreateCommand();

			command.CommandText = "INSERT INTO `rp_groups` SET `name` = 'Nowa grupa', `bank` = 0, `leaderRank` = -1, `leaderID` = -1, `type` = 0, `specialPermissions` = 0";

			command.ExecuteNonQuery();

			int createdGroupID = (int)command.LastInsertedId;

			command.CommandText = "INSERT INTO `rp_groups_ranks` SET `groupID` = @groupID, `name` = 'Lider', `salary` = 0, `skin` = 0, `permissions` = @permissions";
			command.Prepare();

			command.Parameters.AddWithValue("@groupID", createdGroupID);
			command.Parameters.AddWithValue("@permissions", GroupMemberPermission.All);

			command.ExecuteNonQuery();

			int createdRankID = (int)command.LastInsertedId;

			command.CommandText = "UPDATE `rp_groups` SET `leaderRank` = @rankID WHERE `UID` = @UID";
			command.Prepare();

			command.Parameters.AddWithValue("@rankID", createdRankID);
			command.Parameters.AddWithValue("@UID", createdGroupID);

			command.ExecuteNonQuery();

			return Load(createdGroupID);
        }
    }
}
