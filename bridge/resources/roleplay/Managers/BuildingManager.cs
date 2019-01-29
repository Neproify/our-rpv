using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace roleplay.Managers
{
    public class BuildingManager
    {
        public List<Entities.Building> buildings = new List<Entities.Building>();

        private static BuildingManager _instance;
        public static BuildingManager Instance()
        {
            if (_instance == null)
                _instance = new BuildingManager();
            return _instance;
        }

        public void Add(Entities.Building building)
        {
            buildings.Add(building);
        }

        public void Remove(Entities.Building building)
        {
            buildings.Remove(building);
        }

        public List<Entities.Building> GetAll()
        {
            return buildings;
        }

        public Entities.Building GetByID(int ID)
        {
            return buildings.Find(x => x.UID == ID);
        }

        public Entities.Building GetClosestBuilding(Vector3 position, float maxDistance = 999999f)
        {
            Entities.Building building = null;
            float distance = maxDistance;

            foreach(var buildingTemp in buildings)
            {
                if(buildingTemp.enterPosition.DistanceTo(position) <= distance)
                {
                    building = buildingTemp;
                }
            }

            return building;
        }

        public void LoadFromDatabase()
        {
            var command = Database.Instance().Connection.CreateCommand();
            command.CommandText = "SELECT * FROM `rp_buildings`";
            command.Prepare();

            var reader = command.ExecuteReader();

            while(reader.Read())
            {
                Vector3 enterPosition = new Vector3();
                enterPosition.X = reader.GetFloat("enterPosX");
                enterPosition.Y = reader.GetFloat("enterPosY");
                enterPosition.Z = reader.GetFloat("enterPosZ");
                Vector3 exitPosition = new Vector3();
                exitPosition.X = reader.GetFloat("exitPosX");
                exitPosition.Y = reader.GetFloat("exitPosY");
                exitPosition.Z = reader.GetFloat("exitPosZ");

                var building = new Entities.Building
                {
                    UID = reader.GetInt32("UID"),
                    name = reader.GetString("name"),
                    description = reader.GetString("description"),
                    enterPosition = enterPosition,
                    enterDimension = reader.GetUInt32("enterDimension"),
                    exitPosition = exitPosition,
                    ownerType = reader.GetInt32("ownerType"),
                    ownerID = reader.GetInt32("ownerID")
                };

                Add(building);

                building.Spawn();
            }

            reader.Close();
        }

        public void SaveAll()
        {
            buildings.ForEach(x => x.Save());
        }
    }
}
