using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace roleplay.Entities
{
    public class Vehicle
    {
        public GTANetworkAPI.Vehicle handle;

        public VehicleData vehicleData;

        private bool _engineStatus;
        public bool engineStatus
        {
            get
            {
                return _engineStatus;
            }
            set
            {
                _engineStatus = value;
                handle.EngineStatus = value;
                handle.SetSharedData("engineStatus", value);
            }
        }

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
#warning Use vector for rotation in future. Crashes in current(0.3.7.2) release
            var vehicle = NAPI.Vehicle.CreateVehicle((VehicleHash)vehicleData.model, vehicleData.spawnPosition, 0f, vehicleData.color1, vehicleData.color2, "SA " + vehicleData.UID, 255, true, false, 0);
            handle = vehicle;
            Managers.VehicleManager.Instance().LinkWithHandle(this);
            engineStatus = false;
        }

        public bool CanBeAccessedBy(Entities.Player player)
        {
            if (vehicleData.ownerType == (int)OwnerType.Character && vehicleData.ownerID == player.character.UID)
                return true;

            if (vehicleData.ownerType == (int)OwnerType.Group && player.GetGroups().Find(x => x.UID == vehicleData.ownerID) != null)
                return true;

            return false;
        }
    }

    public class VehicleData
    {
        public int UID;
        public uint model;
        public int ownerType;
        public int ownerID;
        public int color1;
        public int color2;
        public Vector3 spawnPosition;
        public Vector3 spawnRotation;

        public void Save()
        {
            var command = Database.Instance().Connection.CreateCommand();
            command.CommandText = "UPDATE `rp_vehicles` SET `model`=@model, `ownerType`=@ownerType, `ownerID`=@ownerID, `color1`=@color1, `color2`=@color2, `spawnPosX`=@spawnPosX, `spawnPosY`=@spawnPosY, `spawnPosZ`=@spawnPosZ, `spawnRotX`=@spawnRotX, `spawnRotY`=@spawnRotY, `spawnRotZ`=@spawnRotZ WHERE `UID`=@UID";
            command.Prepare();
            command.Parameters.AddWithValue("@model", model);
            command.Parameters.AddWithValue("@ownerType", ownerType);
            command.Parameters.AddWithValue("@ownerID", ownerID);
            command.Parameters.AddWithValue("@color1", color1);
            command.Parameters.AddWithValue("@color2", color2);
            command.Parameters.AddWithValue("@spawnPosX", spawnPosition.X);
            command.Parameters.AddWithValue("@spawnPosY", spawnPosition.Y);
            command.Parameters.AddWithValue("@spawnPosZ", spawnPosition.Z);
            command.Parameters.AddWithValue("@spawnRotX", spawnRotation.X);
            command.Parameters.AddWithValue("@spawnRotY", spawnRotation.Y);
            command.Parameters.AddWithValue("@spawnRotZ", spawnRotation.Z);
            command.Parameters.AddWithValue("@UID", UID);
            command.ExecuteNonQuery();
        }
    }
}
