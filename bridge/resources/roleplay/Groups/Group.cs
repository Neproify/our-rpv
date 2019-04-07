using System.Collections.Generic;

namespace roleplay.Entities
{
    public class Group
    {
        public List<GroupRank> ranks;
        public List<GroupMember> members;

        public int UID;
        public string name;
        public int bank;
        public int leaderRank;
        public int leaderID;
        public GroupType type;
        public int specialPermissions;

        public Group()
        {
            ranks = new List<GroupRank>();
            members = new List<GroupMember>();
        }

        public void LoadRanks()
        {
            var command = Database.Instance().connection.CreateCommand();
            command.CommandText = "SELECT * FROM `rp_groups_ranks` WHERE `groupID` = @groupID;";
            command.Prepare();
            command.Parameters.AddWithValue("@groupID", UID);
            var reader = command.ExecuteReader();

            while(reader.Read())
            {
                var rank = new GroupRank
                {
                    UID = reader.GetInt32("UID"),
                    groupID = reader.GetInt32("groupID"),
                    name = reader.GetString("name"),
                    salary = reader.GetInt32("salary"),
                    skin = reader.GetUInt32("skin"),
                    permissions = reader.GetInt32("permissions"),
                    group = this
                };


                ranks.Add(rank);
            }

            reader.Close();
        }

        public void LoadMembers()
        {
            var command = Database.Instance().connection.CreateCommand();
            command.CommandText = "SELECT * FROM `rp_groups_members` WHERE `groupID` = @groupID;";
            command.Prepare();
            command.Parameters.AddWithValue("@groupID", UID);
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var member = new GroupMember
                {
                    UID = reader.GetInt32("UID"),
                    charID = reader.GetInt32("charID"),
                    groupID = reader.GetInt32("groupID"),
                    rankID = reader.GetInt32("rankID"),
                    dutyTime = reader.GetInt32("dutyTime"),
                    group = this
                };

                member.rank = ranks.Find(x => x.UID == member.rankID);

                members.Add(member);
            }

            reader.Close();
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
            var command = Database.Instance().connection.CreateCommand();
            command.CommandText = "UPDATE `rp_groups` SET `bank`=@bank WHERE `UID`=@UID;";
            command.Prepare();

            command.Parameters.AddWithValue("@bank", bank);
            command.Parameters.AddWithValue("@UID", UID);

            command.ExecuteNonQuery();

            ranks.ForEach(x => x.Save());
            members.ForEach(x => x.Save());
        }
    }
}
