using System;
using System.Collections.Generic;
using System.Text;

namespace roleplay.Entities
{
    public class Item
    {
        public int UID;
        public string name;
        public int type;
        public string properties;//make get and set, do it table-way
        public OwnerType ownerType;
        public int ownerID;
    }
}
