using GTANetworkAPI;
using MongoDB.Bson.Serialization;
using roleplay.Serializers;

namespace roleplay
{
    public class Main : Script
    {
        [ServerEvent(Event.ResourceStart)]
        public void ResourceStart()
        {
            NAPI.Util.ConsoleOutput("Trwa nawi�zywanie po��czenia z baz� danych...");

            BsonSerializer.RegisterSerializer(typeof(Vector3), new Vector3Serializer());

            Database database2 = Database.Instance();

            if(!database2.Connect())
            {
                NAPI.Util.ConsoleOutput("B��d podczas ��czenia z baz� danych.");
#warning Set password to server.
                return;
            }
            else
            {
                NAPI.Util.ConsoleOutput("Pomy�lnie po��czono z baz� danych.");
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
        public void OnClientConnect(Player client)
        {
            Managers.PlayerManager manager = Managers.PlayerManager.Instance();
            manager.CreateFromHandle(client);
        }

        [ServerEvent(Event.PlayerDisconnected)]
        public void OnClientDisconnect(Player client, DisconnectionType type, string reason)
        {
            Managers.PlayerManager manager = Managers.PlayerManager.Instance();
            manager.DeleteFromHandle(client);
        }
    }
}