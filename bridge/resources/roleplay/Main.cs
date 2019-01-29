using System;
using GTANetworkAPI;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace roleplay
{
    public class Main : Script
    {
        [ServerEvent(Event.ResourceStart)]
        public void ResourceStart()
        {
            NAPI.Util.ConsoleOutput("Trwa nawi¹zywanie po³¹czenia z baz¹ danych...");

            Database database = Database.Instance();
            database.Address = "51.15.119.59";
            database.Login = "vrp";
            database.Password = "STdyt125";
            database.DatabaseName = "vrp";
            if(!database.Connect())
            {
                NAPI.Util.ConsoleOutput("B³¹d podczas ³¹czenia z baz¹ danych.");
                NAPI.Server.SetServerPassword("niewiadomoco");
                return;
            }
            else
            {
                NAPI.Util.ConsoleOutput("Pomyœlnie po³¹czono z baz¹ danych.");
            }

            Managers.VehicleManager.Instance().LoadFromDatabase();
            Managers.ItemManager.Instance().LoadFromDatabase();
            Managers.GroupManager.Instance().LoadFromDatabase();
            Managers.BuildingManager.Instance().LoadFromDatabase();
            Managers.ObjectManager.Instance().LoadFromDatabase();

            NAPI.Server.SetAutoSpawnOnConnect(false);
            NAPI.Server.SetDefaultSpawnLocation(new Vector3(1398.96, 3591.61, 35), 180);
            NAPI.Server.SetGlobalServerChat(false);
            NAPI.Util.ConsoleOutput("Resource RolePlay started.");
        }
        
        [ServerEvent(Event.ResourceStop)]
        public void ResourceStop()
        {
            Managers.ItemManager.Instance().SaveAll();
            Managers.PlayerManager.Instance().SaveAll();
            Managers.VehicleManager.Instance().SaveAll();
            Managers.GroupManager.Instance().SaveAll();
            Managers.BuildingManager.Instance().SaveAll();
            Managers.ObjectManager.Instance().SaveAll();
        }

        [ServerEvent(Event.PlayerConnected)]
        public void OnClientConnect(Client client)
        {
            Managers.PlayerManager manager = Managers.PlayerManager.Instance();
            manager.CreateFromHandle(client);
        }

        [ServerEvent(Event.PlayerDisconnected)]
        public void OnClientDisconnect(Client client, DisconnectionType type, string reason)
        {
            Managers.PlayerManager manager = Managers.PlayerManager.Instance();
            manager.DeleteFromHandle(client);
        }
    }
}