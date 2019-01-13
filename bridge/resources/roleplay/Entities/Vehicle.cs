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
            }
        }

        public void Spawn()
        {
#warning Use vector for rotation in future. Crashes in current(0.3.7.2) release
            var vehicle = NAPI.Vehicle.CreateVehicle((VehicleHash)vehicleData.model, new Vector3(vehicleData.spawnPosX, vehicleData.spawnPosY, vehicleData.spawnPosZ), 0f, vehicleData.color1, vehicleData.color2, "SA " + vehicleData.UID, 255, true, false, 0);
            handle = vehicle;
            Managers.VehicleManager.Instance().LinkWithHandle(this);
        }

        public bool CanBeAccessedBy(Entities.Player player)
        {
            if (vehicleData.ownerType == (int)OwnerType.Character && vehicleData.ownerID == player.character.UID)
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
        public float spawnPosX;
        public float spawnPosY;
        public float spawnPosZ;
        public float spawnRotX;
        public float spawnRotY;
        public float spawnRotZ;
    }
}
