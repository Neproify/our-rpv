using System;
using System.Collections.Generic;
using System.Text;

namespace roleplay.Entities
{
    public class Group
    {
        public List<Groups.GroupRank> ranks;
        public List<Groups.GroupMember> members;

        public int UID;
        public string name;
        public int bank;
        public int leaderRank;
        public int leaderID;
        public int type;
        public int specialPermissions;

        public Group()
        {
            ranks = new List<Groups.GroupRank>();
            members = new List<Groups.GroupMember>();
        }

        public void LoadRanks()
        {
            var command = Database.Instance().Connection.CreateCommand();
            command.CommandText = "SELECT * FROM `rp_groups_ranks` WHERE `groupID` = @groupID;";
            command.Prepare();
            command.Parameters.AddWithValue("@groupID", UID);
            var reader = command.ExecuteReader();

            while(reader.Read())
            {
                var rank = new Groups.GroupRank
                {
                    UID = reader.GetInt32("UID"),
                    groupID = reader.GetInt32("groupID"),
                    name = reader.GetString("name"),
                    salary = reader.GetInt32("salary"),
                    skin = reader.GetInt32("skin"),
                    permissions = reader.GetInt32("permissions")
                };

                rank.group = this;

                ranks.Add(rank);
            }

            reader.Close();
        }

        public void LoadMembers()
        {
            var command = Database.Instance().Connection.CreateCommand();
            command.CommandText = "SELECT * FROM `rp_groups_members` WHERE `groupID` = @groupID;";
            command.Prepare();
            command.Parameters.AddWithValue("@groupID", UID);
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var member = new Groups.GroupMember
                {
                    UID = reader.GetInt32("UID"),
                    charID = reader.GetInt32("charID"),
                    groupID = reader.GetInt32("charID"),
                    rankID = reader.GetInt32("rankID"),
                    dutyTime = reader.GetInt32("dutyTime")
                };

                member.group = this;
                member.rank = ranks.Find(x => x.UID == member.rankID);

                members.Add(member);
            }

            reader.Close();
        }

        public Groups.GroupMember GetMember(Entities.Player player)
        {
            Groups.GroupMember member = null;

            member = members.Find(x => x.charID == player.character.UID);

            return member;
        }

        public void Save()
        {
#warning Save group
            ranks.ForEach(x => x.Save());
            members.ForEach(x => x.Save());
        }
    }
}
