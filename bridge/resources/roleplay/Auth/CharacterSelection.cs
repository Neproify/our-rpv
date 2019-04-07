using System.Collections.Generic;
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

            if (!player.IsLoggedIn())
            {
                return;
            }

            var command = Database.Instance().connection.CreateCommand();
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

            if (!player.IsLoggedIn())
            {
                client.SendNotification("~r~Nie możesz wybrać postaci, nie jesteś zalogowany!");
                return;
            }

            if (player.character != null)
            {
                client.SendNotification("~r~Masz już wybraną postać!");
                return;
            }

            var command = Database.Instance().connection.CreateCommand();
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

            Vector3 jailPosition = new Vector3
            {
                X = reader.GetFloat("jailPositionX"),
                Y = reader.GetFloat("jailPositionY"),
                Z = reader.GetFloat("jailPositionZ")
            };

            var character = new Entities.Character
            {
                UID = reader.GetInt32("UID"),
                GID = reader.GetInt32("GID"),
                name = reader.GetString("name"),
                model = reader.GetUInt32("model"),
                money = reader.GetInt32("money"),
                health = reader.GetInt32("health"),
                jailBuildingID = reader.GetInt32("jailBuilding"),
                jailPosition = jailPosition
            };

            reader.Close();

            player.character = character;

            if (player.HaveActivePenaltyOfType(Penalties.PenaltyType.CharacterKill))
            {
                player.handle.SendNotification("~r~Podana postać jest zablokowana. Nie możesz jej wybrać.");
                player.character = null;
                return;
            }

            player.handle.Name = character.name;
            player.handle.Freeze(false);
            player.handle.Invincible = false;
            player.handle.Transparency = 255;
            player.handle.Health = character.health;
            NAPI.Entity.SetEntityModel(player.handle, player.character.model);
            NAPI.ClientEvent.TriggerClientEvent(client, "OnCharacterSelectionSuccessful");

            Vector3 spawnPosition = new Vector3(1398.96, 3591.61, 35);
            uint spawnDimension = 0;

            if (character.jailBuildingID != -1)
            {
                var building = Managers.BuildingManager.Instance().GetByID(character.jailBuildingID);
                if (building != null)
                {
                    spawnPosition = character.jailPosition;
                    spawnDimension = building.exitDimension;
                }
            }


            NAPI.Player.SpawnPlayer(client, spawnPosition, 180);
            player.handle.Dimension = spawnDimension;
            player.handle.SendNotification("~g~Witaj na serwerze Our Role Play! Życzymy miłej gry!");
            return;
        }

        public void CustomizationCommand(Client client)
        {
            client.TriggerEvent("ShowCharacterCustomization");
        }
    }
}
