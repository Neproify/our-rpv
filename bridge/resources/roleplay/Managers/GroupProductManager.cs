using System;
using System.Collections.Generic;

namespace roleplay.Managers
{
    public class GroupProductManager
    {
        private readonly List<GroupProduct> products;
        private readonly Dictionary<GroupType, HashSet<GroupProduct>> productsInGroupType;
        private readonly Dictionary<Entities.Group, HashSet<GroupProduct>> productsForGroup;

        private static GroupProductManager _instance;
        public static GroupProductManager Instance()
        {
            return _instance ?? (_instance = new GroupProductManager());
        }

        public GroupProductManager()
        {
            products = new List<GroupProduct>();
            productsInGroupType = new Dictionary<GroupType, HashSet<GroupProduct>>();

            foreach (var i in Enum.GetValues(typeof(GroupType)))
            {
                productsInGroupType[(GroupType)i] = new HashSet<GroupProduct>();
            }

            productsForGroup = new Dictionary<Entities.Group, HashSet<GroupProduct>>();
        }

        public void Add(GroupProduct product)
        {
            products.Add(product);
        }

        public void Remove(GroupProduct product)
        {
            products.Remove(product);
        }

        public List<GroupProduct> GetAll()
        {
            return products;
        }

        public GroupProduct GetByID(int ID)
        {
            return products.Find(x => x.UID == ID);
        }

        public HashSet<GroupProduct> GetProductsForGroup(Entities.Group group)
        {
            return productsForGroup[group];
        }

        public void LoadFromDatabase()
        {
            var command = Database.Instance().connection.CreateCommand();
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

        public GroupProduct Load(MySql.Data.MySqlClient.MySqlDataReader reader)
        {
            var product = new GroupProduct()
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

        public GroupProduct Load(int UID)
        {
            var command = Database.Instance().connection.CreateCommand();
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
            var command = Database.Instance().connection.CreateCommand();

            command.CommandText = "SELECT * FROM `rp_products_group_types`;";

            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                int productID = reader.GetInt32("productID");
                GroupType groupType = (GroupType)reader.GetInt32("groupType");
                var product = GetByID(productID);

                productsInGroupType[groupType].Add(product);
            }

            reader.Close();
        }

        public void AssignProductsToGroups()
        {
            var groups = GroupManager.Instance().groups;

            foreach(var group in groups)
            {
                if (!productsForGroup.ContainsKey(group))
                    productsForGroup[group] = new HashSet<GroupProduct>();

                foreach(var product in productsInGroupType[group.type])
                {
                    productsForGroup[group].Add(product);
                }

                // Now load group-specific products

                var command = Database.Instance().connection.CreateCommand();
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
