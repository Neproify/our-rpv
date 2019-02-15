using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace roleplay.Entities
{
    public class Building
    {
        public int UID;
        public string name;
        public string description;
        public Vector3 enterPosition;
        public uint enterDimension;
        public Vector3 exitPosition;

        public uint exitDimension
        {
            get
            {
                return enterDimension + 10000;
            }
        }

        public OwnerType ownerType;
        public int ownerID;

        public Marker enterMarker;
        public Marker exitMarker;

        public Building()
        {
        }

        public void Spawn()
        {
            if (enterMarker != null || exitMarker != null)
                Unspawn();
            enterMarker = NAPI.Marker.CreateMarker((uint)MarkerType.UpsideDownCone, enterPosition, new Vector3(), new Vector3(), 1f, new Color(0, 0, 0, 120), true, enterDimension);
            exitMarker = NAPI.Marker.CreateMarker((uint)MarkerType.UpsideDownCone, exitPosition, new Vector3(), new Vector3(), 1f, new Color(0, 0, 0, 120), true, exitDimension);
        }

        public void Unspawn()
        {
            if (enterMarker != null)
                enterMarker.Delete();

            if (exitMarker != null)
                exitMarker.Delete();
        }

        public void Save()
        {
            var command = Database.Instance().Connection.CreateCommand();
            command.CommandText = "UPDATE `rp_buildings` SET `name`=@name, `description`=@description, `enterPosX`=@enterPosX, " +
                "`enterPosY`=@enterPosY, `enterPosZ`=@enterPosZ, `enterDimension`=@enterDimension, " +
                "`exitPosX`=@exitPosX, `exitPosY`=@exitPosY, `exitPosZ`=@exitPosZ, `ownerType`=@ownerType, `ownerID`=@ownerID " +
                "WHERE `UID`=@UID;";
            command.Prepare();

            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@description", description);
            command.Parameters.AddWithValue("@enterPosX", enterPosition.X);
            command.Parameters.AddWithValue("@enterPosY", enterPosition.Y);
            command.Parameters.AddWithValue("@enterPosZ", enterPosition.Z);
            command.Parameters.AddWithValue("@enterDimension", enterDimension);
            command.Parameters.AddWithValue("@exitPosX", exitPosition.X);
            command.Parameters.AddWithValue("@exitPosY", exitPosition.Y);
            command.Parameters.AddWithValue("@exitPosZ", exitPosition.Z);
            command.Parameters.AddWithValue("@ownerType", ownerType);
            command.Parameters.AddWithValue("@ownerID", ownerID);
            command.Parameters.AddWithValue("@UID", UID);

            command.ExecuteNonQuery();
        }
    }
}
