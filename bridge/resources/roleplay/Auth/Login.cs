using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using BCrypt;

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

            //TODO: Check if another player is logged on this account.
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

            client.SendNotification("~g~Pomyślnie zalogowałeś się! ~w~ Użyj ~g~/postać [nazwa]~w~ aby wybrać postać.");
        }

        [Command("postac")]
        public void CharacterCommand(Client client, string name)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if(!player.isLogged)
            {
                client.SendNotification("~r~Nie możesz wybrać postaci, nie jesteś zalogowany!");
                return;
            }

            if(player.character != null)
            {
                client.SendNotification("~r~Masz już wybraną postać!");
                return;
            }

            var command = Database.Instance().Connection.CreateCommand();
            command.CommandText = "SELECT * FROM `rp_characters` WHERE `GID`=@GID;";
            command.Prepare();
            command.Parameters.AddWithValue("@GID", player.globalInfo.UID);
            var reader = command.ExecuteReader();
            var characters = new List<Entities.Character>();
            while(reader.Read())
            {
                var character = new Entities.Character
                {
                    UID = reader.GetInt32("UID"),
                    GID = reader.GetInt32("GID"),
                    name = reader.GetString("name")
                };
                characters.Add(character);
            }

            reader.Close();

            foreach(var character in characters)
            {
                if(character.name == name)
                {
                    player.character = character;
                    player.handle.Name = character.name;
                    client.Freeze(false);
                    client.Invincible = false;
                    client.Transparency = 255;
                    NAPI.Player.SpawnPlayer(client, new Vector3(1391.773, 3608.716, 38.942), 0);
                    player.handle.SendNotification("~g~Witaj na serwerze Our Role Play! Życzymy miłej gry!");
                    return;
                }
            }

            player.handle.SendNotification("~r~Podałeś nieprawidłową nazwę postaci!");
            return;
        }
    }
}
