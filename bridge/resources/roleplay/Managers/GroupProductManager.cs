using System;
using System.Collections.Generic;
using System.Text;

namespace roleplay.Managers
{
    public class GroupProductManager
    {
        private List<Groups.GroupProduct> products;
        private Dictionary<Groups.GroupType, HashSet<Groups.GroupProduct>> productsInGroupType;
        private Dictionary<Entities.Group, HashSet<Groups.GroupProduct>> productsForGroup;

        private static GroupProductManager _instance;
        public static GroupProductManager Instance()
        {
            if (_instance == null)
                _instance = new GroupProductManager();
            return _instance;
        }

        public GroupProductManager()
        {
            products = new List<Groups.GroupProduct>();
            productsInGroupType = new Dictionary<Groups.GroupType, HashSet<Groups.GroupProduct>>();

            foreach (var i in Enum.GetValues(typeof(Groups.GroupType)))
            {
                productsInGroupType[(Groups.GroupType)i] = new HashSet<Groups.GroupProduct>();
            }

            productsForGroup = new Dictionary<Entities.Group, HashSet<Groups.GroupProduct>>();
        }

        public void Add(Groups.GroupProduct product)
        {
            products.Add(product);
        }

        public void Remove(Groups.GroupProduct product)
        {
            products.Remove(product);
        }

        public List<Groups.GroupProduct> GetAll()
        {
            return products;
        }

        public Groups.GroupProduct GetByID(int ID)
        {
            return products.Find(x => x.UID == ID);
        }

        public HashSet<Groups.GroupProduct> GetProductsForGroup(Entities.Group group)
        {
            return productsForGroup[group];
        }

        public void LoadFromDatabase()
        {
            var command = Database.Instance().Connection.CreateCommand();
            command.CommandText = "SELECT * FROM `rp_products`;";

            var reader = command.ExecuteReader();

            while(reader.Read())
            {
                Load(reader);
            }

            reader.Close();

            AssignProductsToTypes();
            AssignProductsToGroups();
        }

        public Groups.GroupProduct Load(MySql.Data.MySqlClient.MySqlDataReader reader)
        {
            var product = new Groups.GroupProduct()
            {
                UID = reader.GetInt32("UID"),
                name = reader.GetString("name"),
                type = (ItemType)reader.GetInt32("type"),
                propertiesString = reader.GetString("properties"),
                price = reader.GetInt32("price")
            };

            Add(product);

            return product;
        }

        public Groups.GroupProduct Load(int UID)
        {
            var command = Database.Instance().Connection.CreateCommand();
            command.CommandText = "SELECT * FROM `rp_products` WHERE `UID`=@UID;";

            command.Prepare();
            command.Parameters.AddWithValue("@UID", UID);

            var reader = command.ExecuteReader();
            reader.Read();

            var product = Load(reader);

            reader.Close();

            return product;
        }

        public void SaveAll()
        {
            products.ForEach(x => x.Save());
        }

        public void AssignProductsToTypes()
        {
            var command = Database.Instance().Connection.CreateCommand();

            command.CommandText = "SELECT * FROM `rp_products_group_types`;";

            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                int productID = reader.GetInt32("productID");
                Groups.GroupType groupType = (Groups.GroupType)reader.GetInt32("groupType");
                var product = GetByID(productID);

                productsInGroupType[groupType].Add(product);
            }

            reader.Close();
        }

        public void AssignProductsToGroups()
        {
            var groups = Managers.GroupManager.Instance().groups;

            foreach(var group in groups)
            {
                if (!productsForGroup.ContainsKey(group))
                    productsForGroup[group] = new HashSet<Groups.GroupProduct>();

                foreach(var product in productsInGroupType[group.type])
                {
                    productsForGroup[group].Add(product);
                }

                // Now load group-specific products

                var command = Database.Instance().Connection.CreateCommand();
                command.CommandText = "SELECT * FROM `rp_products_group_ids` WHERE `groupID`=@groupID;";

                command.Prepare();
                command.Parameters.AddWithValue("@groupID", group.UID);

                var reader = command.ExecuteReader();

                while(reader.Read())
                {
                    var productID = reader.GetInt32("productID");
                    var product = GetByID(productID);

                    if (product == null)
                        continue;

                    productsForGroup[group].Add(product);
                }

                reader.Close();
            }
        }

    }
}
