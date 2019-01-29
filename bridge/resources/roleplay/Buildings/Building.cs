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

        public int ownerType;
        public int ownerID;

        public Marker enterMarker;
        public Marker exitMarker;

        public Building()
        {
            Spawn();
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

        }

        public void Save()
        {
#warning Implement this.
        }
    }
}
