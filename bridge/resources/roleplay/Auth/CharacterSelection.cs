using System.Collections.Generic;
using GTANetworkAPI;
using Newtonsoft.Json;
using MongoDB.Bson;

namespace roleplay.Auth
{
    // Remember to change it on client-side too

    public class CharacterSelection : Script
    {
        public class CharacterInfo
        {
            public ObjectId UID;
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

            var collection = Database.Instance().GetCharactersCollection();
            var builder = new MongoDB.Driver.FilterDefinitionBuilder<Entities.Character>();
            var filter = builder.Where(x => x.GID == player.globalInfo.UID);
            var cursor = collection.FindSync<Entities.Character>(filter);
            cursor.MoveNext();

            var characters = new List<CharacterInfo>();
            foreach (var character in cursor.Current)
            {
                var characterInfo = new CharacterInfo
                {
                    UID = character.UID,
                    name = character.name
                };
                characters.Add(characterInfo);
            }

            var output = JsonConvert.SerializeObject(characters);

            NAPI.ClientEvent.TriggerClientEvent(client, "OnPlayerCharactersLoaded", output);
        }

        [RemoteEvent("SelectCharacter")]
        public void SelectCharacter(Client client, string UID)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.IsLoggedIn())
            {
                player.SendNotification("~r~Nie możesz wybrać postaci, nie jesteś zalogowany!");
                return;
            }

            if (player.character != null)
            {
                player.SendNotification("~r~Masz już wybraną postać!");
                return;
            }

            var collection = Database.Instance().GetCharactersCollection();
            var builder = new MongoDB.Driver.FilterDefinitionBuilder<Entities.Character>();
            var filter = builder.Where(x => x.UID == MongoDB.Bson.ObjectId.Parse(UID) && x.GID == player.globalInfo.UID);
            var cursor = collection.FindSync<Entities.Character>(filter);

            cursor.MoveNext();

            Entities.Character character = null;

            foreach(var i in cursor.Current)
            {
                character = i;
                break;
            }

            if(character == null)
            {
                return;
            }

            player.character = character;

            if (player.HaveActivePenaltyOfType(Penalties.PenaltyType.CharacterKill))
            {
                player.SendNotification("~r~Podana postać jest zablokowana. Nie możesz jej wybrać.");
                player.character = null;
                return;
            }

            player.SetName(character.name);
            player.SetFreezed(false);
            player.SetInvicible(false);
            player.SetTransparency(255);
            player.SetHealth(character.health);
            player.SetModel(player.character.model);
            player.TriggerEvent("OnCharacterSelectionSuccessful");

            Vector3 spawnPosition = new Vector3(1398.96, 3591.61, 35);
            uint spawnDimension = 0;

            if (character.jailBuildingID != MongoDB.Bson.ObjectId.Empty)
            {
                var building = Managers.BuildingManager.Instance().GetByID(character.jailBuildingID);
                if (building != null)
                {
                    spawnPosition = character.jailPosition;
                    spawnDimension = building.exitDimension;
                }
            }


            NAPI.Player.SpawnPlayer(client, spawnPosition, 180);
            player.SetDimension(spawnDimension);
            player.LoadLook();
            player.SendNotification("~g~Witaj na serwerze Our Role Play! Życzymy miłej gry!");
            return;
        }

        [RemoteEvent("CreateCharacter")]
        public void CreateCharacter(Client client, string name, string surname)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.IsLoggedIn())
            {
                player.SendNotification("~r~Nie możesz stworzyć postaci, nie jesteś zalogowany!");
                return;
            }

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(surname))
            {
                player.SendNotification("~r~Musisz podać imię i nazwisko aby stworzyć postać!");
                return;
            }

            var filter = new MongoDB.Driver.FilterDefinitionBuilder<Entities.Character>().Where(x => x.name == name + "_" + surname);
            if(Database.Instance().GetCharactersCollection().CountDocuments(filter) != 0)
            {
                player.SendNotification("~r~Na serwerze jest już postać która się tak nazywa!");
                return;
            }

            var character = new Entities.Character
            {
                UID = ObjectId.GenerateNewId(),
                GID = player.globalInfo.UID,
                name = name + "_" + surname,
                model = 0x705E61F2,
                money = 1000,
                health = 100,
                jailBuildingID = ObjectId.Empty,
                jailPosition = new Vector3()
            };

            Database.Instance().GetCharactersCollection().InsertOne(character);

            SelectCharacter(client, character.UID.ToString());
        }
    }
}
