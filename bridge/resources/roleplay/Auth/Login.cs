using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using Newtonsoft.Json;

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
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                client.SendNotification("~r~Musisz podać login i hasło aby się zalogować!");
                return;
            }

            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (player.isLogged)
            {
                client.SendNotification("~r~Jesteś już zalogowany.");
                return;
            }

            var command = Database.Instance().Connection.CreateCommand();
            command.CommandText = "SELECT * FROM `ipb_core_members` WHERE `name`=@name LIMIT 1;";
            command.Prepare();

            command.Parameters.AddWithValue("@name", login);
            var reader = command.ExecuteReader();
            if (!reader.HasRows)
            {
                reader.Close();
                client.SendNotification("~r~Nie znaleźliśmy konta o podanej nazwie.");
                return;
            }
            reader.Read();

            var passwordToCheck = reader.GetString("members_pass_hash");
            if (!BCrypt.BCryptHelper.CheckPassword(password, passwordToCheck))
            {
                reader.Close();
                client.SendNotification("~r~Wpisałeś błędne hasło!");
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

            player.isLogged = true;
            player.globalInfo = globalInfo;

            NAPI.ClientEvent.TriggerClientEvent(client, "LoginSuccessful");

            client.SendNotification("~g~Pomyślnie zalogowałeś się!");
        }
    }
}
