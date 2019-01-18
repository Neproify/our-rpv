using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace roleplay.Managers
{
    public class VehicleManager
    {
        private List<Entities.Vehicle> vehicles = new List<Entities.Vehicle>();
        private Dictionary<GTANetworkAPI.NetHandle, Entities.Vehicle> vehiclesDictionary = new Dictionary<GTANetworkAPI.NetHandle, Entities.Vehicle>();

        private static VehicleManager _instance;
        public static VehicleManager Instance()
        {
            if (_instance == null)
                _instance = new VehicleManager();
            return _instance;
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

        public void SaveAll()
        {
            foreach(var vehicle in vehiclesDictionary.Values)
            {
                vehicle.Save();
            }
        }

        public void LoadFromDatabase()
        {
            var command = Database.Instance().Connection.CreateCommand();
            command.CommandText = "SELECT * FROM `rp_vehicles`;";
            var reader = command.ExecuteReader();

            while(reader.Read())
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
                    ownerType = reader.GetInt32("ownerType"),
                    ownerID = reader.GetInt32("ownerID"),
                    color1 = reader.GetInt32("color1"),
                    color2 = reader.GetInt32("color2"),
                    spawnPosition = position,
                    spawnRotation = rotation
                };

                var vehicle = new Entities.Vehicle();
                vehicle.vehicleData = vehicleData;
                Add(vehicle);
                vehicle.Spawn();
            }

            reader.Close();
        }
    }
}
