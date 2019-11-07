using System.Collections.Generic;
using GTANetworkAPI;
using MongoDB.Bson;

namespace roleplay.Managers
{
    public class BuildingManager
    {
        public List<Entities.Building> buildings = new List<Entities.Building>();

        private static BuildingManager _instance;
        public static BuildingManager Instance()
        {
            return _instance ?? (_instance = new BuildingManager());
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

        public Entities.Building GetByID(ObjectId ID)
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
            var collection = Database.Instance().GetBuildingsCollection();
            var cursor = collection.FindSync<Entities.Building>(new BsonDocument());
            cursor.MoveNext();
            foreach(var building in cursor.Current)
            {
                Add(building);
                building.Spawn();
            }
        }

        public Entities.Building Load(ObjectId UID)
        {
            var collection = Database.Instance().GetBuildingsCollection();
            var filter = new MongoDB.Driver.FilterDefinitionBuilder<Entities.Building>().Where(x => x.UID == UID);
            var cursor = collection.FindSync<Entities.Building>(filter);
            cursor.MoveNext();

            foreach(var building in cursor.Current)
            {
                Add(building);
                building.Spawn();
                return building;
            }

            return null;
        }

        public void SaveAll()
        {
            buildings.ForEach(x => x.Save());
        }

        public Entities.Building CreateBuilding()
        {
            var building = new Entities.Building
            {
                UID = ObjectId.GenerateNewId(),
                name = "Nowy budynek",
                description = "",
                enterPosition = new Vector3(),
                enterDimension = 0,
                exitPosition = new Vector3(),
                ownerType = OwnerType.None,
                ownerID = ObjectId.Empty
            };

            Database.Instance().GetBuildingsCollection().InsertOne(building);

            Add(building);

            return building;
        }
    }
}
