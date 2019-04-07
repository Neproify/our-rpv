using GTANetworkAPI;

namespace roleplay.Auth
{
    public class Login : Script
    {
        [ServerEvent(Event.PlayerConnected)]
        public void OnPlayerConnect(Client client)
        {
            NAPI.Util.ConsoleOutput("Player " + client.Name + " joined.");
            client.Freeze(true);
            client.Invincible = true;
            client.Transparency = 0;
        }

        [RemoteEvent("OnLoginRequest")]
        public void OnLoginRequest(Client client, string login, string password)
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

            var command = Database.Instance().connection.CreateCommand();
            command.CommandText = "SELECT * FROM `ipb_core_members` WHERE `name`=@name LIMIT 1;";
            command.Prepare();

            command.Parameters.AddWithValue("@name", login);
            var reader = command.ExecuteReader();
            if (!reader.HasRows)
            {
                reader.Close();
                player.SendNotification("~r~Nie znaleźliśmy konta o podanej nazwie.");
                return;
            }
            reader.Read();

            var passwordToCheck = reader.GetString("members_pass_hash");
            if (!BCrypt.BCryptHelper.CheckPassword(password, passwordToCheck))
            {
                reader.Close();
                player.SendNotification("~r~Wpisałeś błędne hasło!");
                return;
            }

            var globalInfo = new Entities.GlobalInfo
            {
                UID = reader.GetInt32("member_id"),
                name = reader.GetString("name"),
                score = reader.GetInt32("game_score"),
                adminLevel = reader.GetInt32("game_admin_level"),
                adminPermissions = reader.GetInt32("game_admin_permissions")
            };

            reader.Close();

            if (Managers.PlayerManager.Instance().GetAll().Exists(x => x.globalInfo?.UID == globalInfo.UID))
            {
                //Tried to login when other player is logged.
                return;
            }

            player.SetIsLoggedIn(true);
            player.globalInfo = globalInfo;
            player.LoadPenalties();

            if(player.HaveActivePenaltyOfType(Penalties.PenaltyType.Ban))
            {
                player.SilentKick("Posiadasz aktywną karę administracyjną.");
                return;
            }

            NAPI.ClientEvent.TriggerClientEvent(client, "LoginSuccessful");

            player.SendNotification("~g~Pomyślnie zalogowałeś się!");
        }
    }
}
