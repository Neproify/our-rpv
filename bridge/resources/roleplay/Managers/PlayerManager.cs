using System.Collections.Generic;
using GTANetworkAPI;

namespace roleplay.Managers
{
    public class PlayerManager
    {
        private readonly Dictionary<Client, Entities.Player> playersDictionary = new Dictionary<Client, Entities.Player>();

        private static PlayerManager _instance;
        public static PlayerManager Instance()
        {
            return _instance ?? (_instance = new PlayerManager());
        }

        public Entities.Player CreateFromHandle(Client client)
        {
            Entities.Player player = new Entities.Player(client);
            playersDictionary.Add(client, player);
            return player;
        }

        public void DeleteFromHandle(Client client)
        {
            playersDictionary.Remove(client);
        }

        public Entities.Player GetByHandle(Client client)
        {
            return playersDictionary[client];
        }

        public void SaveAll()
        {
            foreach(var player in playersDictionary.Values)
            {
                player.Save();
            }
        }

        public List<Entities.Player> GetAll()
        {
            List<Entities.Player> players = new List<Entities.Player>();
            foreach(var pair in playersDictionary)
            {
                players.Add(pair.Value);
            }

            return players;
        }

        public Entities.Player GetByCharacterID(int ID)
        {
            return GetAll().Find(x => x.character?.UID == ID);
        }

        public Entities.Player GetByID(int ID)
        {
            var client = NAPI.Pools.GetAllPlayers().Find(x => x.Handle.Value == ID);

            if (client == null)
                return null;

            return GetByHandle(client);
        }
    }
}
