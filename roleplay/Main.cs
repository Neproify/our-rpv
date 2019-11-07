using GTANetworkAPI;

namespace roleplay
{
    public class Main : Script
    {
        [ServerEvent(Event.ResourceStart)]
        public void ResourceStart()
        {
            NAPI.Util.ConsoleOutput("Trwa nawi¹zywanie po³¹czenia z baz¹ danych...");

            Database database2 = Database.Instance();

            if(!database2.Connect())
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
            Managers.GroupProductManager.Instance().LoadFromDatabase();
            Managers.BuildingManager.Instance().LoadFromDatabase();
            Managers.ObjectManager.Instance().LoadFromDatabase();

            NAPI.Server.SetAutoSpawnOnConnect(false);
            NAPI.Server.SetDefaultSpawnLocation(new Vector3(1398.96, 3591.61, 35), 180);
            NAPI.Server.SetGlobalServerChat(false);
            NAPI.Server.SetAutoRespawnAfterDeath(false);
            NAPI.Util.ConsoleOutput("Resource RolePlay started.");
        }

        [ServerEvent(Event.ResourceStop)]
        public void ResourceStop()
        {
            Managers.ItemManager.Instance().SaveAll();
            Managers.PlayerManager.Instance().SaveAll();
            Managers.VehicleManager.Instance().SaveAll();
            Managers.GroupManager.Instance().SaveAll();
            Managers.GroupProductManager.Instance().SaveAll();
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