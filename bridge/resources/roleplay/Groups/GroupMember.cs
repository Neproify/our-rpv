using System;
using System.Collections.Generic;
using System.Text;

namespace roleplay.Groups
{
    public class GroupMember
    {
        public int UID;
        public int charID;
        public int groupID;
        public int rankID;
        public int dutyTime;

        public Entities.Group group;
        public Groups.GroupRank rank;

        public void Save()
        {
            var command = Database.Instance().Connection.CreateCommand();
            command.CommandText = "UPDATE `rp_groups_members` SET `dutyTime`=@dutyTime WHERE `UID`=@UID;";
            command.Prepare();

            command.Parameters.AddWithValue("@dutyTime", dutyTime);
            command.Parameters.AddWithValue("@UID", UID);

            command.ExecuteNonQuery();
        }
    }
}
