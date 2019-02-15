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
        public OwnerType ownerType;
        public int ownerID;

        public GTANetworkAPI.Object handle;

        public Object()
        {
        }

        public void Spawn()
        {
            if (IsSpawned())
                Unspawn();

            handle = NAPI.Object.CreateObject(model, position, rotation, 255, GetDimension());
        }

        public void Unspawn()
        {
            if (!IsSpawned())
                return;

            handle.Delete();
            handle = null;
        }

        public bool IsSpawned()
        {
            return handle != null;
        }

        public void Save()
        {
            var command = Database.Instance().Connection.CreateCommand();
            command.CommandText = "UPDATE `rp_objects` SET `model`=@model, `positionX`=@positionX, `positionY`=@positionY, `positionZ`=@positionZ, " +
                "`rotationX`=@rotationX, `rotationY`=@rotationY, `rotationZ`=@rotationZ, " +
                "`ownerType`=@ownerType, `ownerID`=@ownerID WHERE `UID`=@UID;";
            command.Prepare();

            command.Parameters.AddWithValue("@model", model);
            command.Parameters.AddWithValue("positionX", position.X);
            command.Parameters.AddWithValue("positionY", position.Y);
            command.Parameters.AddWithValue("positionZ", position.Z);
            command.Parameters.AddWithValue("rotationX", rotation.X);
            command.Parameters.AddWithValue("rotationY", rotation.Y);
            command.Parameters.AddWithValue("rotationZ", rotation.Z);
            command.Parameters.AddWithValue("ownerType", ownerType);
            command.Parameters.AddWithValue("ownerID", ownerID);
            command.Parameters.AddWithValue("UID", UID);

            command.ExecuteNonQuery();
        }

        public uint GetDimension()
        {
            if (ownerType == OwnerType.World)
                return 0;

            if(ownerType == OwnerType.Building)
            {
                var building = Managers.BuildingManager.Instance().GetByID(ownerID);

                if (building == null)
                    return 0;

                return building.exitDimension;
            }

            return 0;
        }
    }
}
