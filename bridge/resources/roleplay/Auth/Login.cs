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
            if(string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                client.SendNotification("~r~Musisz podać login i hasło aby się zalogować!");
                return;
            }

#warning Check if another player is logged on this account
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

            player.isLogged = true;

            var globalInfo = new Entities.GlobalInfo
            {
                UID = reader.GetInt32("member_id"),
                name = reader.GetString("name")
            };

            reader.Close();

            player.globalInfo = globalInfo;

            NAPI.ClientEvent.TriggerClientEvent(client, "LoginSuccessful");

            client.SendNotification("~g~Pomyślnie zalogowałeś się!");
        }
    }
}
