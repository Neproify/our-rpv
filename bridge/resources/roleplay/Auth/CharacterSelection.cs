using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using Newtonsoft.Json;

namespace roleplay.Auth
{
    // Remember to change it on client-side too

    public class CharacterSelection : Script
    {
        public class CharacterInfo
        {
            public int UID;
            public string name;
        }

        [RemoteEvent("LoadPlayerCharacters")]
        public void LoadPlayerCharacters(Client client)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.isLogged)
            {
                return;
            }

            var command = Database.Instance().Connection.CreateCommand();
            command.CommandText = "SELECT * FROM `rp_characters` WHERE `GID`=@GID;";
            command.Prepare();
            command.Parameters.AddWithValue("@GID", player.globalInfo.UID);
            var reader = command.ExecuteReader();
            var characters = new List<CharacterInfo>();
            while (reader.Read())
            {
                var characterInfo = new CharacterInfo
                {
                    UID = reader.GetInt32("UID"),
                    name = reader.GetString("name")
                };

                characters.Add(characterInfo);
            }

            reader.Close();

            var output = JsonConvert.SerializeObject(characters);

            NAPI.ClientEvent.TriggerClientEvent(client, "OnPlayerCharactersLoaded", output);
        }

        [RemoteEvent("SelectCharacter")]
        public void SelectCharacter(Client client, int UID)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.isLogged)
            {
                client.SendNotification("~r~Nie możesz wybrać postaci, nie jesteś zalogowany!");
                return;
            }

            if (player.character != null)
            {
                client.SendNotification("~r~Masz już wybraną postać!");
                return;
            }

            var command = Database.Instance().Connection.CreateCommand();
            command.CommandText = "SELECT * FROM `rp_characters` WHERE `GID`=@GID AND `UID`=@UID LIMIT 1;";
            command.Prepare();
            command.Parameters.AddWithValue("@GID", player.globalInfo.UID);
            command.Parameters.AddWithValue("@UID", UID);
            var reader = command.ExecuteReader();

            if (!reader.Read())
            {
                reader.Close();
                return;
            }

            var character = new Entities.Character
            {
                UID = reader.GetInt32("UID"),
                GID = reader.GetInt32("GID"),
                name = reader.GetString("name"),
                model = reader.GetUInt32("model")
            };

            reader.Close();

            NAPI.ClientEvent.TriggerClientEvent(client, "OnCharacterSelectionSuccessful");

            player.character = character;
            player.handle.Name = character.name;
            player.handle.Freeze(false);
            player.handle.Invincible = false;
            player.handle.Transparency = 255;
            NAPI.Entity.SetEntityModel(player.handle, player.character.model);
            NAPI.Player.SpawnPlayer(client, new Vector3(1398.96, 3591.61, 35), 0);
            player.handle.SendNotification("~g~Witaj na serwerze Our Role Play! Życzymy miłej gry!");
            return;
        }
    }
}
