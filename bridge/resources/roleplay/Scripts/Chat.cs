﻿using System;
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
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.isLogged || player.character == null)
                return;

            if (player.isBrutallyWounded)
            {
                player.handle.SendNotification("~r~Jesteś nieprzytomny, nie możesz tego zrobić!");
                return;
            }

            if (player.isGagged)
            {
                player.handle.SendNotification("~r~Jesteś zakneblowany, nie możesz mówić.");
                return;
            }

            var players = NAPI.Player.GetPlayersInRadiusOfPosition(20, player.handle.Position);

            message = message.Replace("<", "!{#C2A2DA}*");
            message = message.Replace(">", "*!{#FFFFFF}");

            foreach (var nearPlayer in players)
            {
                nearPlayer.SendChatMessage($"!{{#FFFFFF}}{player.formattedName} mówi{(player.phoneCall?.active == true ? "(telefon)" : "")}: {message}");
            }

            if (player.phoneCall != null)
            {
                if (player.phoneCall.receiverPhone == 911)
                    goto AlarmPhone;

                Entities.Player receiver = null;
                if (player.phoneCall.sender == player)
                {
                    receiver = player.phoneCall.receiver;
                }
                else
                {
                    receiver = player.phoneCall.sender;
                }

                receiver.handle.SendChatMessage($"!{{#FFFFFF}}Telefon({player.formattedName}): {message}");
                return;

            AlarmPhone:
                player.phoneCall = null;
                var onDutyPlayers = Managers.PlayerManager.Instance().GetAll().FindAll(x => x.groupDuty?.member.group.type == Groups.GroupType.Police || x.groupDuty?.member.group.type == Groups.GroupType.Medical);
                onDutyPlayers.ForEach(x => x.handle.SendChatMessage($"[911]Zgłoszenie({player.activePhone.properties[0]}): {message}"));
                return;
            }
        }

        [Command("me", GreedyArg = true)]
        public void MeCommand(Client client, string action)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.isLogged || player.character == null)
                return;

            player.OutputMe(action);
        }

        [Command("do", GreedyArg = true)]
        public void DoCommand(Client client, string action)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.isLogged || player.character == null)
                return;

            player.OutputDo(action);
        }

        [Command("b", GreedyArg = true)]
        public void OOCCommand(Client client, string message)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.isLogged || player.character == null)
                return;

            var players = NAPI.Player.GetPlayersInRadiusOfPosition(20, player.handle.Position);

            foreach (var nearPlayer in players)
            {
                nearPlayer.SendChatMessage($"!{{#FFFFFF}}(({player.formattedName}({player.handle.Handle}): {message}))");
            }
        }

        [Command("sprobuj", GreedyArg = true)]
        public void TryCommand(Client client, string action)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.isLogged || player.character == null)
                return;

            if (player.isBrutallyWounded)
            {
                player.handle.SendNotification("~r~Jesteś nieprzytomny, nie możesz tego zrobić!");
                return;
            }

            var players = NAPI.Player.GetPlayersInRadiusOfPosition(20, player.handle.Position);

            var result = new Random().Next(9999);

            foreach (var nearPlayer in players)
            {
                if (result % 2 == 0)
                {
                    nearPlayer.SendChatMessage($"!{{#C2A2DA}}* {player.formattedName} poległ próbując {action}");
                }
                else
                {
                    nearPlayer.SendChatMessage($"!{{#C2A2DA}}* {player.formattedName} odniósł sukces próbując {action}");
                }
            }
        }

        [Command("k", GreedyArg = true)]
        public void ShoutCommand(Client client, string message)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.isLogged || player.character == null)
                return;

            if (player.isBrutallyWounded)
            {
                player.handle.SendNotification("~r~Jesteś nieprzytomny, nie możesz tego zrobić!");
                return;
            }

            if (player.isGagged)
            {
                player.handle.SendNotification("~r~Jesteś zakneblowany, nie możesz krzyczeć.");
                return;
            }

            var players = NAPI.Player.GetPlayersInRadiusOfPosition(30, player.handle.Position);

            foreach (var nearPlayer in players)
            {
                nearPlayer.SendChatMessage($"!{{#FFFFFF}}{player.formattedName} krzyczy{(player.phoneCall?.active == true ? "(telefon)" : "")}: {message}");
            }

            if (player.phoneCall != null)
            {
                Entities.Player receiver = null;
                if (player.phoneCall.sender == player)
                {
                    receiver = player.phoneCall.receiver;
                }
                else
                {
                    receiver = player.phoneCall.sender;
                }

                receiver.handle.SendChatMessage($"!{{#FFFFFF}}Telefon({player.formattedName}, krzyk): {message}");
            }
        }

        [Command("s", GreedyArg = true)]
        public void SilentCommand(Client client, string message)
        {

            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.isLogged || player.character == null)
                return;

            if (player.isBrutallyWounded)
            {
                player.handle.SendNotification("~r~Jesteś nieprzytomny, nie możesz tego zrobić!");
                return;
            }

            if (player.isGagged)
            {
                player.handle.SendNotification("~r~Jesteś zakneblowany, nie możesz szepczeć.");
                return;
            }

            var players = NAPI.Player.GetPlayersInRadiusOfPosition(5, player.handle.Position);

            foreach (var nearPlayer in players)
            {
                nearPlayer.SendChatMessage($"!{{#FFFFFF}}{player.formattedName} szepcze{(player.phoneCall?.active == true ? "(telefon)" : "")}: {message}");
            }

            if (player.phoneCall != null)
            {
                Entities.Player receiver = null;
                if (player.phoneCall.sender == player)
                {
                    receiver = player.phoneCall.receiver;
                }
                else
                {
                    receiver = player.phoneCall.sender;
                }

                receiver.handle.SendChatMessage($"!{{#FFFFFF}}Telefon({player.formattedName}, szept): {message}");
            }
        }

        [Command("r", GreedyArg = true)]
        public void RadioCommand(Client client, string message)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.isLogged || player.character == null)
                return;

            if (player.isBrutallyWounded)
            {
                player.handle.SendNotification("~r~Jesteś nieprzytomny, nie możesz tego zrobić!");
                return;
            }

            if (player.isGagged)
            {
                player.handle.SendNotification("~r~Jesteś zakneblowany, nie możesz mówić.");
                return;
            }

            var group = player.groupDuty?.member.rank.group;
            if (group == null)
            {
                player.handle.SendNotification("~r~Nie jesteś na służbie żadnej grupy.");
                return;
            }

            var players = NAPI.Player.GetPlayersInRadiusOfPosition(20, player.handle.Position);

            foreach (var nearPlayer in players)
            {
                nearPlayer.SendChatMessage($"!{{#FFFFFF}}{player.formattedName} mówi(radio): {message}");
            }

            var onDutyPlayers = group.GetPlayersOnDuty();
            foreach (var dutyPlayer in onDutyPlayers)
            {
                dutyPlayer.handle.SendChatMessage($"!{{#0039e6}}[RADIO]{player.formattedName}: {message}");
            }
        }

        [Command("m", GreedyArg = true)]
        public void MegaphoneCommand(Client client, string message)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.isLogged || player.character == null)
                return;

            if (player.isBrutallyWounded)
            {
                player.handle.SendNotification("~r~Jesteś nieprzytomny, nie możesz tego zrobić!");
                return;
            }

            if (player.isGagged)
            {
                player.handle.SendNotification("~r~Jesteś zakneblowany, nie możesz mówić.");
                return;
            }

            if (!player.HasSpecialPermissionInGroup(Groups.GroupSpecialPermission.Megaphone))
            {
                player.handle.SendNotification("~r~Nie masz uprawnień aby użyć megafonu!");
                return;
            }

            var players = NAPI.Player.GetPlayersInRadiusOfPosition(50, player.handle.Position);

            foreach (var nearPlayer in players)
            {
                nearPlayer.SendChatMessage($"!{{#ffff00}}>> {player.formattedName}(megafon): {message}");
            }
        }

        [Command("w", GreedyArg = true)]
        public void PrivateMessageCommand(Client client, int playerID, string message)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.isLogged || player.character == null)
                return;

            var targetPlayer = Managers.PlayerManager.Instance().GetByID(playerID);

            if (targetPlayer == null)
            {
                player.handle.SendNotification("~r~Podany gracz nie jest zalogowany!");
                return;
            }

            if (!targetPlayer.isLogged || targetPlayer.character == null)
            {
                player.handle.SendNotification("~r~Podany gracz nie jest zalogowany!");
                return;
            }

            player.handle.SendChatMessage($"!{{#99FFAA}}(( << {targetPlayer.formattedName}({targetPlayer.handle.Handle}): {message}))");
            targetPlayer.handle.SendChatMessage($"#78DEAA(( >> {player.formattedName}({player.handle.Handle}): {message}))");
        }
    }
}
