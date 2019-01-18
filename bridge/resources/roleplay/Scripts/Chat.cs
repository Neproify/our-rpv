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

            message = message.Replace("<", "!{#C2A2DA}*");
            message = message.Replace(">", "*!{#FFFFFF}");

            foreach (var player in players)
            {
                player.SendChatMessage($"!{{#FFFFFF}}{Managers.PlayerManager.Instance().GetByHandle(client).formattedName} mówi: {message}");
            }
        }

        [Command("me", GreedyArg = true)]
        public void MeCommand(Client client, string action)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.isLogged)
                return;

            player.OutputMe(action);
        }

        [Command("do", GreedyArg = true)]
        public void DoCommand(Client client, string action)
        {
            if (!Managers.PlayerManager.Instance().GetByHandle(client).isLogged)
                return;

            var players = NAPI.Player.GetPlayersInRadiusOfPosition(20, client.Position);

            foreach (var player in players)
            {
                player.SendChatMessage($"!{{#9A9CCD}}* {action} (({Managers.PlayerManager.Instance().GetByHandle(client).formattedName}))");
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
                player.SendChatMessage($"!{{#FFFFFF}}(({Managers.PlayerManager.Instance().GetByHandle(client).formattedName}({client.Handle}): {action}))");
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
                    player.SendChatMessage($"!{{#C2A2DA}}* {Managers.PlayerManager.Instance().GetByHandle(client).formattedName} poległ próbując {action}");
                }
                else
                {
                    player.SendChatMessage($"!{{#C2A2DA}}* {Managers.PlayerManager.Instance().GetByHandle(client).formattedName} odniósł sukces próbując {action}");
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
                player.SendChatMessage($"!{{#FFFFFF}}{Managers.PlayerManager.Instance().GetByHandle(client).formattedName} krzyczy: {action}");
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
                player.SendChatMessage($"!{{#FFFFFF}}{Managers.PlayerManager.Instance().GetByHandle(client).formattedName} szepcze: {action}");
            }
        }
    }
}
