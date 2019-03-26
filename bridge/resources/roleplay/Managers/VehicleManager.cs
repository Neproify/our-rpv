using System.Collections.Generic;
using GTANetworkAPI;

namespace roleplay.Managers
{
    public class VehicleManager
    {
        private readonly List<Entities.Vehicle> vehicles = new List<Entities.Vehicle>();
        private readonly Dictionary<NetHandle, Entities.Vehicle> vehiclesDictionary = new Dictionary<NetHandle, Entities.Vehicle>();

        private static VehicleManager _instance;
        public static VehicleManager Instance()
        {
            return _instance ?? (_instance = new VehicleManager());
        }

        public void Add(Entities.Vehicle vehicle)
        {
            vehicles.Add(vehicle);
        }

        public void Remove(Entities.Vehicle vehicle)
        {
            vehicles.Remove(vehicle);
        }

        public void LinkWithHandle(Entities.Vehicle vehicle)
        {
            vehiclesDictionary.Add(vehicle.handle, vehicle);
        }

        public void UnlinkWithHandle(Entities.Vehicle vehicle)
        {
            vehiclesDictionary.Remove(vehicle.handle);
        }

        public Entities.Vehicle CreateFromHandle(Vehicle vehicle)
        {
            Entities.Vehicle vehicleNew = new Entities.Vehicle(vehicle);
            vehiclesDictionary.Add(vehicle.Handle, vehicleNew);
            return vehicleNew;
        }

        public void DeleteFromHandle(Vehicle vehicle)
        {
            vehiclesDictionary.Remove(vehicle.Handle);
        }

        public Entities.Vehicle GetByHandle(Vehicle vehicle)
        {
            return vehiclesDictionary[vehicle];
        }

        public Entities.Vehicle GetByID(int UID)
        {
            return vehicles.Find(x => x.vehicleData.UID == UID);
        }

        public void SaveAll()
        {
            foreach(var vehicle in vehiclesDictionary.Values)
            {
                vehicle.Save();
            }
        }

        public void LoadFromDatabase()
        {
            var command = Database.Instance().connection.CreateCommand();
            command.CommandText = "SELECT * FROM `rp_vehicles`;";
            var reader = command.ExecuteReader();

            while(reader.Read())
            {
                Load(reader);
            }

            reader.Close();
        }

        public Entities.Vehicle Load(MySql.Data.MySqlClient.MySqlDataReader reader)
        {
            var position = new Vector3();
            var rotation = new Vector3();
            position.X = reader.GetFloat("spawnPosX");
            position.Y = reader.GetFloat("spawnPosY");
            position.Z = reader.GetFloat("spawnPosZ");
            rotation.X = reader.GetFloat("spawnRotX");
            rotation.Y = reader.GetFloat("spawnRotY");
            rotation.Z = reader.GetFloat("spawnRotZ");

            var vehicleData = new Entities.VehicleData
            {
                UID = reader.GetInt32("UID"),
                model = reader.GetUInt32("model"),
                ownerType = (OwnerType)reader.GetInt32("ownerType"),
                ownerID = reader.GetInt32("ownerID"),
                color1 = reader.GetInt32("color1"),
                color2 = reader.GetInt32("color2"),
                spawnPosition = position,
                spawnRotation = rotation
            };

            var vehicle = new Entities.Vehicle {vehicleData = vehicleData};
            Add(vehicle);
            vehicle.Spawn();

            return vehicle;
        }
        
        public Entities.Vehicle Load(int UID)
        {
            var command = Database.Instance().connection.CreateCommand();
            command.CommandText = "SELECT * FROM `rp_vehicles` WHERE `UID`=@UID;";
            command.Prepare();

            command.Parameters.AddWithValue("@UID", UID);

            var reader = command.ExecuteReader();
            reader.Read();

            var vehicle = Load(reader);

            reader.Close();

            return vehicle;
        }

        public Entities.Vehicle CreateVehicle()
        {
            var command = Database.Instance().connection.CreateCommand();
            command.CommandText = "INSERT INTO `rp_vehicles` SET `model`=1119641113, `ownerType`=0, `ownerID`=-1, `color1`=1, `color2`=1, `spawnPosX`=0, `spawnPosY`=0, `spawnPosZ`=0, `spawnRotX`=0, `spawnRotY`=0, `spawnRotZ`=0";
            command.ExecuteNonQuery();

            return Load((int)command.LastInsertedId);
        }
    }
}
