using System;
using GTANetworkAPI;

namespace roleplay.Scripts
{
    public class Chat : Script
    {

        [ServerEvent(Event.ChatMessage)]
        public void OnPlayerChat(Player client, string message)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.IsReady())
                return;

            if (player.isBrutallyWounded)
            {
                player.SendBrutallyWoundedNoPermissionNotification();
                return;
            }

            if (player.isGagged)
            {
                player.SendGaggedNoPermissionNotification();
                return;
            }

            var players = NAPI.Player.GetPlayersInRadiusOfPosition(20, player.position);

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

                var receiver = player.phoneCall.sender == player ? player.phoneCall.receiver : player.phoneCall.sender;

                receiver.SendChatMessage($"!{{#FFFFFF}}Telefon({player.formattedName}): {message}");
                return;

            AlarmPhone:
                player.phoneCall = null;
                var onDutyPlayers = Managers.PlayerManager.Instance().GetAll().FindAll(x => x.groupDuty?.member.group.type == GroupType.Police || x.groupDuty?.member.group.type == GroupType.Medical);
                onDutyPlayers.ForEach(x => x.SendChatMessage($"[911]Zgłoszenie({player.activePhone.phoneNumber}): {message}"));
                return;
            }
        }

        [Command("me", GreedyArg = true)]
        public void MeCommand(Player client, string action)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.IsReady())
                return;

            player.OutputMe(action);
        }

        [Command("do", GreedyArg = true)]
        public void DoCommand(Player client, string action)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.IsReady())
                return;

            player.OutputDo(action);
        }

        [Command("b", GreedyArg = true)]
        public void OOCCommand(Player client, string message)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.IsReady())
                return;

            var players = NAPI.Player.GetPlayersInRadiusOfPosition(20, player.position);

            foreach (var nearPlayer in players)
            {
                nearPlayer.SendChatMessage($"!{{#FFFFFF}}(({player.formattedName}({player.GetGameID()}): {message}))");
            }
        }

        [Command("sprobuj", GreedyArg = true)]
        public void TryCommand(Player client, string action)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.IsReady())
                return;

            if (player.isBrutallyWounded)
            {
                player.SendBrutallyWoundedNoPermissionNotification();
                return;
            }

            var players = NAPI.Player.GetPlayersInRadiusOfPosition(20, player.position);

            var result = new Random().Next(9999);

            foreach (var nearPlayer in players)
            {
                nearPlayer.SendChatMessage(result % 2 == 0
                    ? $"!{{#C2A2DA}}* {player.formattedName} poległ próbując {action}"
                    : $"!{{#C2A2DA}}* {player.formattedName} odniósł sukces próbując {action}");
            }
        }

        [Command("k", GreedyArg = true)]
        public void ShoutCommand(Player client, string message)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.IsReady())
                return;

            if (player.isBrutallyWounded)
            {
                player.SendBrutallyWoundedNoPermissionNotification();
                return;
            }

            if (player.isGagged)
            {
                player.SendGaggedNoPermissionNotification();
                return;
            }

            var players = NAPI.Player.GetPlayersInRadiusOfPosition(30, player.position);

            foreach (var nearPlayer in players)
            {
                nearPlayer.SendChatMessage($"!{{#FFFFFF}}{player.formattedName} krzyczy{(player.phoneCall?.active == true ? "(telefon)" : "")}: {message}");
            }

            if (player.phoneCall != null)
            {
                var receiver = player.phoneCall.sender == player ? player.phoneCall.receiver : player.phoneCall.sender;

                receiver.SendChatMessage($"!{{#FFFFFF}}Telefon({player.formattedName}, krzyk): {message}");
            }
        }

        [Command("s", GreedyArg = true)]
        public void SilentCommand(Player client, string message)
        {

            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.IsReady())
                return;

            if (player.isBrutallyWounded)
            {
                player.SendBrutallyWoundedNoPermissionNotification();
                return;
            }

            if (player.isGagged)
            {
                player.SendGaggedNoPermissionNotification();
                return;
            }

            var players = NAPI.Player.GetPlayersInRadiusOfPosition(5, player.position);

            foreach (var nearPlayer in players)
            {
                nearPlayer.SendChatMessage($"!{{#FFFFFF}}{player.formattedName} szepcze{(player.phoneCall?.active == true ? "(telefon)" : "")}: {message}");
            }

            if (player.phoneCall != null)
            {
                var receiver = player.phoneCall.sender == player ? player.phoneCall.receiver : player.phoneCall.sender;

                receiver.SendChatMessage($"!{{#FFFFFF}}Telefon({player.formattedName}, szept): {message}");
            }
        }

        [Command("r", GreedyArg = true)]
        public void RadioCommand(Player client, string message)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.IsReady())
                return;

            if (player.isBrutallyWounded)
            {
                player.SendBrutallyWoundedNoPermissionNotification();
                return;
            }

            if (player.isGagged)
            {
                player.SendGaggedNoPermissionNotification();
                return;
            }

            var group = player.groupDuty?.member.rank.group;
            if (group == null)
            {
                player.SendNotification("~r~Nie jesteś na służbie żadnej grupy.");
                return;
            }

            var players = NAPI.Player.GetPlayersInRadiusOfPosition(20, player.position);

            foreach (var nearPlayer in players)
            {
                nearPlayer.SendChatMessage($"!{{#FFFFFF}}{player.formattedName} mówi(radio): {message}");
            }

            var onDutyPlayers = group.GetPlayersOnDuty();
            foreach (var dutyPlayer in onDutyPlayers)
            {
                dutyPlayer.SendChatMessage($"!{{#0039e6}}[RADIO]{player.formattedName}: {message}");
            }
        }

        [Command("m", GreedyArg = true)]
        public void MegaphoneCommand(Player client, string message)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.IsReady())
                return;

            if (player.isBrutallyWounded)
            {
                player.SendBrutallyWoundedNoPermissionNotification();
                return;
            }

            if (player.isGagged)
            {
                player.SendGaggedNoPermissionNotification();
                return;
            }

            if (!player.HasSpecialPermissionInGroup(GroupSpecialPermission.Megaphone))
            {
                player.SendNotification("~r~Nie masz uprawnień aby użyć megafonu!");
                return;
            }

            var players = NAPI.Player.GetPlayersInRadiusOfPosition(50, player.position);

            foreach (var nearPlayer in players)
            {
                nearPlayer.SendChatMessage($"!{{#ffff00}}>> {player.formattedName}(megafon): {message}");
            }
        }

        [Command("w", GreedyArg = true)]
        public void PrivateMessageCommand(Player client, int playerID, string message)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.IsReady())
                return;

            var targetPlayer = Managers.PlayerManager.Instance().GetByID(playerID);

            if (targetPlayer?.IsReady() == null)
            {
                player.SendPlayerNotFoundNotification();
                return;
            }

            player.SendChatMessage($"!{{#99FFAA}}(( << {targetPlayer.formattedName}({targetPlayer.GetGameID()}): {message}))");
            targetPlayer.SendChatMessage($"#78DEAA(( >> {player.formattedName}({player.GetGameID()}): {message}))");
        }
    }
}
