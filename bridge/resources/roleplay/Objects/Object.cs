using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace roleplay.Entities
{
    public class Object
    {
        public int UID;
        public uint model;
        public Vector3 position;
        public Vector3 rotation;
        public int ownerType;
        public int ownerID;

        public GTANetworkAPI.Object handle;

        public Object()
        {
            Spawn();
        }

        public void Spawn()
        {
            if (handle != null)
                Unspawn();

            handle = NAPI.Object.CreateObject(model, position, rotation, 255, GetDimension());
        }

        public void Unspawn()
        {
            handle.Delete();
        }

        public void Save()
        {
#warning Implement this.
        }

        public uint GetDimension()
        {
            if (ownerType == (int)OwnerType.World)
                return NAPI.GlobalDimension;

            if(ownerType == (int)OwnerType.Building)
            {
                var building = Managers.BuildingManager.Instance().GetByID(ownerID);

                if (building == null)
                    return NAPI.GlobalDimension;

                return building.exitDimension;
            }

            return NAPI.GlobalDimension;
        }
    }
}
