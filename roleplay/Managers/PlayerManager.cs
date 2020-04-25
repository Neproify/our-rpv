using System.Collections.Generic;
using GTANetworkAPI;

namespace roleplay.Managers
{
    public class PlayerManager
    {
        private readonly Dictionary<Player, Entities.Player> playersDictionary = new Dictionary<Player, Entities.Player>();

        private static PlayerManager _instance;
        public static PlayerManager Instance()
        {
            return _instance ?? (_instance = new PlayerManager());
        }

        public Entities.Player CreateFromHandle(Player client)
        {
            Entities.Player player = new Entities.Player(client);
            playersDictionary.Add(client, player);
            return player;
        }

        public void DeleteFromHandle(Player client)
        {
            playersDictionary.Remove(client);
        }

        public Entities.Player GetByHandle(Player client)
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

        public Entities.Player GetByCharacterID(MongoDB.Bson.ObjectId ID)
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
