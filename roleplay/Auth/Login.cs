using GTANetworkAPI;
using MongoDB.Bson;

namespace roleplay.Auth
{
    public class Login : Script
    {
        [ServerEvent(Event.PlayerConnected)]
        public void OnPlayerConnect(Player client)
        {
            NAPI.Util.ConsoleOutput("Player " + client.Name + " joined.");
#warning FREEZE AND INVICIBLE NOT IMPLEMENTED
            //client.Freeze(true);
            //client.Invincible = true;
            client.Transparency = 0;
        }

        [RemoteEvent("OnLoginRequest")]
        public void OnLoginRequest(Player client, string login, string password)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (player.IsLoggedIn())
            {
                player.SendNotification("~r~Jesteś już zalogowany.");
                return;
            }

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                player.SendNotification("~r~Musisz podać login i hasło aby się zalogować!");
                return;
            }

            var collection = Database.Instance().GetForumDatabase().GetCollection<BsonDocument>("objects");

            MongoDB.Driver.FilterDefinition<BsonDocument> filter = new BsonDocument
            {
                {"_key", new BsonRegularExpression("/user/")},
                {"userslug", login}
            };
            long count = collection.CountDocuments(filter);

            if (count == 0)
            {
                player.SendNotification("~r~Nie znaleźliśmy konta o podanej nazwie.");
                return;
            }

            var cursor = collection.FindSync<BsonDocument>(filter);
            cursor.MoveNext();
            BsonDocument document = null;
            foreach (var i in cursor.Current)
            {
                document = i;
                break;
            }

            if (!BCrypt.BCryptHelper.CheckPassword(password, document.GetValue("password").AsString))
            {
                player.SendNotification("~r~Wpisałeś błędne hasło!");
                return;
            }

            // Make sure we have all fields in database. :)
            if(!document.Contains(("gamescore")))
            {
                document.Set("gamescore", 0);
                document.Set("gameadminlevel", 0);
                document.Set("gameadminpermissions", 0);
                collection.FindOneAndUpdate<BsonDocument>(filter, document);
            }

            var globalInfo = new Entities.GlobalInfo
            {
                UID = document.GetValue("_id").AsObjectId,
                name = document.GetValue("username").AsString,
                score = document.GetValue("gamescore").AsInt32,
                adminLevel = document.GetValue("gameadminlevel").AsInt32,
                adminPermissions = document.GetValue("gameadminpermissions").AsInt32
            };

            if (Managers.PlayerManager.Instance().GetAll().Exists(x => x.globalInfo?.UID == globalInfo.UID))
            {
                //Tried to login when other player is logged.
                return;
            }

            player.SetIsLoggedIn(true);
            player.globalInfo = globalInfo;
            player.LoadPenalties();

            if (player.HaveActivePenaltyOfType(Penalties.PenaltyType.Ban))
            {
                player.SilentKick("Posiadasz aktywną karę administracyjną.");
                return;
            }

            NAPI.ClientEvent.TriggerClientEvent(client, "LoginSuccessful");

            player.SendNotification("~g~Pomyślnie zalogowałeś się!");
        }
    }
}
