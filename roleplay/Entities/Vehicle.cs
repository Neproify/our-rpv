﻿using GTANetworkAPI;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace roleplay.Entities
{
    public class Vehicle
    {
        private GTANetworkAPI.Vehicle handle;

        public VehicleData vehicleData;

        private bool _engineStatus;
        public bool engineStatus
        {
            get => _engineStatus;
            set
            {
                _engineStatus = value;
                handle.EngineStatus = value;
                handle.SetSharedData("engineStatus", value);
            }
        }

        public float health
        {
            set => handle.Health = value;
            get => handle.Health;
        }

        public Vector3 position
        {
            set => handle.Position = value;
            get => handle.Position;
        }

        public Vector3 rotation
        {
            set => handle.Rotation = value;
            get => handle.Rotation;
        }

        public bool locked
        {
            set => handle.Locked = value;
            get => handle.Locked;
        }

        public int primaryColor
        {
            set
            {
                vehicleData.primaryColor = value;
                handle.PrimaryColor = value;
            }
            get => handle.PrimaryColor;
        }

        public int secondaryColor
        {
            set
            {
                vehicleData.secondaryColor = value;
                handle.SecondaryColor = value;
            }
            get => handle.SecondaryColor;
        }

        public uint model
        {
            set
            {
                NAPI.Entity.SetEntityModel(handle, value);
                vehicleData.model = value;
            }
            get => NAPI.Entity.GetEntityModel(handle);
        }

        public string displayName => handle.DisplayName;

        public Vehicle()
        {

        }

        public void Save()
        {
            vehicleData.Save();
        }

        public Vehicle(GTANetworkAPI.Vehicle handle)
        {
            this.handle = handle;
            handle.SetSharedData("engineStatus", engineStatus);
        }

        public void Spawn()
        {
            if (handle != null)
                return;

            var vehicle = NAPI.Vehicle.CreateVehicle((VehicleHash)vehicleData.model, vehicleData.spawnPosition, vehicleData.spawnRotation, vehicleData.primaryColor, vehicleData.secondaryColor, "SA " + vehicleData.UID, 255, true, false);
            handle = vehicle;
            Managers.VehicleManager.Instance().LinkWithHandle(this);
            engineStatus = false;
        }

        public void Unspawn()
        {
            if (handle == null)
                return;

            Managers.VehicleManager.Instance().UnlinkWithHandle(this);
            handle.Delete();
            handle = null;
        }

        public bool IsSpawned()
        {
            if (handle != null)
                return true;

            return false;
        }

        public bool CanBeAccessedBy(Player player)
        {
            if (vehicleData.ownerType == OwnerType.Character && vehicleData.ownerID == player.character.UID)
                return true;

            if (vehicleData.ownerType == OwnerType.Group && player.GetGroups().Find(x => x.UID == vehicleData.ownerID) != null)
                return true;

            return false;
        }

        public void Repair() => handle.Repair();

        public NetHandle GetNetHandle() => handle;
    }

    public class VehicleData
    {
        [BsonId]
        [BsonElement("_id")]
        public ObjectId UID;

        [BsonRepresentation(BsonType.Int64)]
        [BsonElement("model")]
        public uint model;

        [BsonElement("ownertype")]
        public OwnerType ownerType;

        [BsonElement("ownerid")]
        public ObjectId ownerID;

        [BsonElement("primarycolor")]
        public int primaryColor;

        [BsonElement("secondarycolor")]
        public int secondaryColor;

        [BsonElement("spawnposition")]
        public Vector3 spawnPosition;

        [BsonElement("spawnrotation")]
        public Vector3 spawnRotation;

        public void Save()
        {
            var collection = Database.Instance().GetVehiclesCollection();
            var builder = new MongoDB.Driver.FilterDefinitionBuilder<VehicleData>();
            var filter = builder.Where(x => x.UID == UID);
            collection.FindOneAndReplace<VehicleData>(filter, this);
        }
    }
}
