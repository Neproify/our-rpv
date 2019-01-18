using System;
using System.Collections.Generic;
using System.Text;

namespace roleplay.Groups
{
    public class GroupRank
    {
        public int UID;
        public int groupID;
        public string name;
        public int salary;
        public int skin; // it should be uint probably
        public int permissions;

        public Entities.Group group;

        public void Save()
        {
#warning Implement this.
        }
    }
}
