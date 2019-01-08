using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace roleplay.Scripts
{
    public class Chat : Script
    {

        [ServerEvent(Event.ChatMessage)]
        public void OnPlayerChat(Client client, string message)
        {
            if (!Managers.PlayerManager.Instance().GetByHandle(client).isLogged)
                return;

            var players = NAPI.Player.GetPlayersInRadiusOfPosition(20, client.Position);

            foreach (var player in players)
            {
                player.SendChatMessage(string.Format("!{{#FFFFFF}}{0} mówi: {1}", client.Name, message));
            }
        }

        [Command("me", GreedyArg = true)]
        public void MeCommand(Client client, string action)
        {
            if(!Managers.PlayerManager.Instance().GetByHandle(client).isLogged)
                return;

            var players = NAPI.Player.GetPlayersInRadiusOfPosition(20, client.Position);

            foreach(var player in players)
            {
                player.SendChatMessage(string.Format("!{{#C2A2DA}}*{0} {1}", client.Name, action));
            }
        }

        [Command("do", GreedyArg = true)]
        public void DoCommand(Client client, string action)
        {
            if (!Managers.PlayerManager.Instance().GetByHandle(client).isLogged)
                return;

            var players = NAPI.Player.GetPlayersInRadiusOfPosition(20, client.Position);

            foreach (var player in players)
            {
                player.SendChatMessage(string.Format("!{{#9A9CCD}}* {0} (({1}))", action, client.Name));
            }
        }

        [Command("b", GreedyArg = true)]
        public void OOCCommand(Client client, string action)
        {
            if (!Managers.PlayerManager.Instance().GetByHandle(client).isLogged)
                return;

            var players = NAPI.Player.GetPlayersInRadiusOfPosition(20, client.Position);

            foreach (var player in players)
            {
                player.SendChatMessage(string.Format("!{{#FFFFFF}}(({0}({1}): {2}))", client.Name, client.Handle, action));
            }
        }

        [Command("sprobuj", GreedyArg = true)]
        public void TryCommand(Client client, string action)
        {
            if (!Managers.PlayerManager.Instance().GetByHandle(client).isLogged)
                return;

            var players = NAPI.Player.GetPlayersInRadiusOfPosition(20, client.Position);

            var result = new Random().Next(9999);

            foreach (var player in players)
            {
                if (result % 2 == 0)
                {
                    player.SendChatMessage(string.Format("!{{#C2A2DA}}* {0} poległ próbując {1}", client.Name, action));
                }
                else
                {
                    player.SendChatMessage(string.Format("!{{#C2A2DA}}* {0} odniósł sukces próbując {1}", client.Name, action));
                }
            }
        }

        [Command("k", GreedyArg = true)]
        public void ShoutCommand(Client client, string action)
        {
            if (!Managers.PlayerManager.Instance().GetByHandle(client).isLogged)
                return;

            var players = NAPI.Player.GetPlayersInRadiusOfPosition(30, client.Position);

            foreach (var player in players)
            {
                player.SendChatMessage(string.Format("!{{#FFFFFF}}{0} krzyczy: {1}", client.Name, action));
            }
        }

        [Command("s", GreedyArg = true)]
        public void SilentCommand(Client client, string action)
        {
            if (!Managers.PlayerManager.Instance().GetByHandle(client).isLogged)
                return;

            var players = NAPI.Player.GetPlayersInRadiusOfPosition(5, client.Position);

            foreach (var player in players)
            {
                player.SendChatMessage(string.Format("!{{#FFFFFF}}{0} szepcze: {1}", client.Name, action));
            }
        }
    }
}
