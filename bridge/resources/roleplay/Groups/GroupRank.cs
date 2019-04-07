namespace roleplay
{
    public class GroupRank
    {
        public int UID;
        public int groupID;
        public string name;
        public int salary;
        public uint skin; // it should be uint probably
        public int permissions;

        public Entities.Group group;

        public void Save()
        {
            // Nothing to save, left it here.
        }
    }
}
