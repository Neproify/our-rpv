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
                Load(reader);
            }

            reader.Close();
        }

        public Entities.Object Load(MySql.Data.MySqlClient.MySqlDataReader reader)
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
                ownerType = (OwnerType)reader.GetInt32("ownerType"),
                ownerID = reader.GetInt32("ownerID")
            };

            Add(@object);

            @object.Spawn();

            return @object;
        }

        public Entities.Object Load(int UID)
        {
            var command = Database.Instance().Connection.CreateCommand();
            command.CommandText = "SELECT * FROM `rp_objects` WHERE `UID`=@UID;";
            command.Prepare();

            command.Parameters.AddWithValue("@UID", UID);

            var reader = command.ExecuteReader();
            reader.Read();

            var @object = Load(reader);

            reader.Close();

            return @object;
        }

        public Entities.Object CreateObject()
        {
            var command = Database.Instance().Connection.CreateCommand();
            command.CommandText = "INSERT INTO `rp_objects` SET `model`=579156093, `ownerType`=0, `ownerID`=-1, `postionX`=0, `positionY`=0, `positionZ`=0, `rotationX`=0, `rotationY`=0, `rotationZ`=0";
            command.ExecuteNonQuery();

            return Load((int)command.LastInsertedId);
        }
    }
}
