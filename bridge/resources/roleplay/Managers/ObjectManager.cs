using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace roleplay.Managers
{
    public class ObjectManager
    {
        public List<Entities.Object> objects = new List<Entities.Object>();

        private static ObjectManager _instance;
        public static ObjectManager Instance()
        {
            if (_instance == null)
                _instance = new ObjectManager();
            return _instance;
        }

        public void Add(Entities.Object @object)
        {
            objects.Add(@object);
        }

        public void Remove(Entities.Object @object)
        {
            objects.Remove(@object);
        }

        public Entities.Object GetByID(int ID)
        {
            return objects.Find(x => x.UID == ID);
        }

        public void SaveAll()
        {
            objects.ForEach(x => x.Save());
        }

        public void LoadFromDatabase()
        {
            var command = Database.Instance().Connection.CreateCommand();
            command.CommandText = "SELECT * FROM `rp_objects`";
            command.Prepare();

            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var position = new Vector3();
                var rotation = new Vector3();
                position.X = reader.GetFloat("positionX");
                position.Y = reader.GetFloat("positionY");
                position.Z = reader.GetFloat("positionZ");
                rotation.X = reader.GetFloat("rotationX");
                rotation.Y = reader.GetFloat("rotationY");
                rotation.Z = reader.GetFloat("rotationZ");

                var @object = new Entities.Object
                {
                    UID = reader.GetInt32("UID"),
                    model = reader.GetUInt32("model"),
                    position = position,
                    rotation = rotation,
                    ownerType = reader.GetInt32("ownerType"),
                    ownerID = reader.GetInt32("ownerID")
                };
            }

            reader.Close();
        }
    }
}
