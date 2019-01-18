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
#warning Implement this.
        }
    }
}
