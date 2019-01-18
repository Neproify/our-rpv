using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace roleplay.Managers
{
    public class PlayerManager
    {
        private Dictionary<Client, Entities.Player> playersDictionary = new Dictionary<Client, Entities.Player>();

        private static PlayerManager _instance;
        public static PlayerManager Instance()
        {
            if (_instance == null)
                _instance = new PlayerManager();
            return _instance;
        }

        public Entities.Player CreateFromHandle(Client client)
        {
            Entities.Player player = new Entities.Player();
            player.handle = client;
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
    }
}
