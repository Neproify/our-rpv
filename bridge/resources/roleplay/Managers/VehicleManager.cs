using System.Collections.Generic;
using GTANetworkAPI;
using MongoDB.Bson;

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

        public Entities.Vehicle GetByID(ObjectId UID)
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
            var collection = Database.Instance().GetVehiclesCollection();
            var cursor = collection.FindSync<Entities.VehicleData>(new BsonDocument());
            cursor.MoveNext();
            
            foreach(var vehicleData in cursor.Current)
            {
                var vehicle = new Entities.Vehicle { vehicleData = vehicleData };
                Add(vehicle);
                vehicle.Spawn();
            }
        }
        
        public Entities.Vehicle Load(ObjectId UID)
        {
            var builder = new MongoDB.Driver.FilterDefinitionBuilder<Entities.VehicleData>();
            var filter = builder.Where(x => x.UID == UID);
            var cursor = Database.Instance().GetVehiclesCollection().FindSync<Entities.VehicleData>(filter);
            cursor.MoveNext();

            foreach(var vehicleData in cursor.Current)
            {
                var vehicle = new Entities.Vehicle { vehicleData = vehicleData };
                Add(vehicle);
                vehicle.Spawn();
                return vehicle;
            }

            return null;
        }

        public Entities.Vehicle CreateVehicle()
        {
            var vehicleData = new Entities.VehicleData
            {
                UID = ObjectId.GenerateNewId(),
                model = 1119641113,
                ownerType = OwnerType.None,
                ownerID = ObjectId.Empty,
                primaryColor = 1,
                secondaryColor = 1,
                spawnPosition = new Vector3(),
                spawnRotation = new Vector3()
            };

            Database.Instance().GetVehiclesCollection().InsertOne(vehicleData);

            var vehicle = new Entities.Vehicle { vehicleData = vehicleData };
            Add(vehicle);
            vehicle.Spawn();

            return vehicle;
        }
    }
}
