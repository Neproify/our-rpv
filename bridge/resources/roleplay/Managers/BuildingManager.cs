﻿using System.Collections.Generic;
using GTANetworkAPI;

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
            var command = Database.Instance().connection.CreateCommand();
            command.CommandText = "SELECT * FROM `rp_buildings`";
            command.Prepare();

            var reader = command.ExecuteReader();

            while(reader.Read())
            {
                Load(reader);
            }

            reader.Close();
        }

        public Entities.Building Load(MySql.Data.MySqlClient.MySqlDataReader reader)
        {
            Vector3 enterPosition = new Vector3
            {
                X = reader.GetFloat("enterPosX"), Y = reader.GetFloat("enterPosY"), Z = reader.GetFloat("enterPosZ")
            };
            Vector3 exitPosition = new Vector3
            {
                X = reader.GetFloat("exitPosX"), Y = reader.GetFloat("exitPosY"), Z = reader.GetFloat("exitPosZ")
            };

            var building = new Entities.Building
            {
                UID = reader.GetInt32("UID"),
                name = reader.GetString("name"),
                description = reader.GetString("description"),
                enterPosition = enterPosition,
                enterDimension = reader.GetUInt32("enterDimension"),
                exitPosition = exitPosition,
                ownerType = (OwnerType)reader.GetInt32("ownerType"),
                ownerID = reader.GetInt32("ownerID")
            };

            Add(building);

            building.Spawn();

            return building;
        }

        public Entities.Building Load(int UID)
        {
            var command = Database.Instance().connection.CreateCommand();
            command.CommandText = "SELECT * FROM `rp_buildings` WHERE `UID`=@UID";
            command.Prepare();

            command.Parameters.AddWithValue("@UID", UID);

            var reader = command.ExecuteReader();
            reader.Read();

            var building = Load(reader);

            reader.Close();

            return building;
        }

        public void SaveAll()
        {
            buildings.ForEach(x => x.Save());
        }

        public Entities.Building CreateBuilding()
        {
            var command = Database.Instance().connection.CreateCommand();
            command.CommandText = "INSERT INTO `rp_buildings` SET `name`='', `description`='', `enterPosX`=0, `enterPosY`=0, `enterPosZ`=0, " +
                "`enterDimension`=0, `exitPosX`=0, `exitPosY`=0, `exitPosZ`=0, `ownerType`=0, `ownerID`=-1";

            command.ExecuteNonQuery();

            return Load((int)command.LastInsertedId);
        }
    }
}
