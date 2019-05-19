using System;
using GTANetworkAPI;

namespace roleplay.Admin
{
    public class PlayerCommands : Script
    {
        [Command("agracz", GreedyArg = true)]
        public void AdminPlayerCommand(Client client, string parameters)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.IsReady())
                return;

            if (!player.IsAdminOfLevel(AdminLevel.Supporter))
            {
                player.SendNoPermissionsToCommandNotification();
                return;
            }

            string[] args = parameters.Split(" ");

            if (args.Length < 2)
                goto Usage;

            if (!Int32.TryParse(args[0], out var playerID))
                goto Usage;

            Entities.Player targetPlayer = Managers.PlayerManager.Instance().GetByID(playerID);

            if (targetPlayer == null)
            {
                player.SendPlayerNotFoundNotification();
                return;
            }

            if (args[1] == "hp")
            {
                if (args.Length < 3)
                {
                    goto HPUsage;
                }

                if (!Int32.TryParse(args[2], out var healthValue))
                {
                    goto HPUsage;
                }

                targetPlayer.health = healthValue;
                return;
            }

            if (args[1] == "unbw")
            {
                targetPlayer.SetIsBrutallyWounded(false);
                return;
            }

            if (args[1] == "model")
            {
                if (args.Length < 3)
                {
                    goto ModelUsage;
                }

                if (!UInt32.TryParse(args[2], out var modelHash))
                {
                    goto ModelUsage;
                }

                targetPlayer.SetModel(modelHash);
                return;
            }

            if (args[1] == "przedmioty")
            {
                player.SendChatMessage($"==== LISTA PRZEDMIOTÓW GRACZA {targetPlayer.formattedName} ====");

                foreach (var item in targetPlayer.GetItems())
                {
                    player.SendChatMessage($"[{item.UID}] - {item.name}");
                }

                player.SendChatMessage("==== KONIEC LISTY ====");
                return;
            }

            if (args[1] == "money")
            {
                if (args.Length < 3)
                {
                    goto MoneyUsage;
                }

                if (!Int32.TryParse(args[2], out var moneyValue))
                {
                    goto MoneyUsage;
                }

                targetPlayer.money = moneyValue;
                return;
            }

            if (args[1] == "tpto")
            {
                player.position = targetPlayer.position;
                return;
            }

            if (args[1] == "tphere")
            {
                targetPlayer.position = player.position;
                return;
            }

        Usage:
            player.SendUsageNotification("Użycie komendy: /agracz [id gracza] [hp, unbw, model, przedmioty, money, tpto, tphere]");
            return;
        HPUsage:
            player.SendUsageNotification($"Użycie komendy: /agracz {playerID} hp [ilość].");
            return;
        ModelUsage:
            player.SendUsageNotification($"Użycie komendy: /agracz {playerID} model [hash modelu].");
            return;
        MoneyUsage:
            player.SendUsageNotification($"Użycie komendy: /agracz {playerID} money [ilość].");
            return;
        }

        [Command("ado", GreedyArg = true)]
        public void AdminDoCommand(Client client, string action)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.IsReady())
                return;

            if (!player.IsAdminOfLevel(AdminLevel.Supporter))
            {
                player.SendNoPermissionsToCommandNotification();
                return;
            }

            NAPI.Chat.SendChatMessageToAll($"!{{#9A9CCD}}* {action}");
        }

        [Command("ban", GreedyArg = true)]
        public void BanCommand(Client client, int playerID, string reason)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.IsReady())
                return;

            if (!player.IsAdminOfLevel(AdminLevel.Supporter))
            {
                player.SendNoPermissionsToCommandNotification();
                return;
            }

            var targetPlayer = Managers.PlayerManager.Instance().GetByID(playerID);

            if (targetPlayer == null)
            {
                player.SendPlayerNotFoundNotification();
                return;
            }

            targetPlayer.Ban(reason, player.character.UID);
        }


        [Command("kick", GreedyArg = true)]
        public void KickCommand(Client client, int playerID, string reason)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.IsReady())
                return;

            if (!player.IsAdminOfLevel(AdminLevel.Supporter))
            {
                player.SendNoPermissionsToCommandNotification();
                return;
            }

            var targetPlayer = Managers.PlayerManager.Instance().GetByID(playerID);
            if (targetPlayer == null)
            {
                player.SendPlayerNotFoundNotification();
                return;
            }

            targetPlayer.Kick(reason, player.character.UID);
        }

        [Command("gooc", GreedyArg = true)]
        public void AdminOOCCommand(Client client, string message)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.IsReady())
                return;

            if (!player.IsAdminOfLevel(AdminLevel.Supporter))
            {
                player.SendNoPermissionsToCommandNotification();
                return;
            }

            NAPI.Chat.SendChatMessageToAll($"!{{#FFFFFF}}(({player.formattedName}: {message}))");
        }

        [Command("warn", GreedyArg = true)]
        public void WarnCommand(Client client, int playerID, string reason)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.IsReady())
                return;

            if (!player.IsAdminOfLevel(AdminLevel.Supporter))
            {
                player.SendNoPermissionsToCommandNotification();
                return;
            }

            var targetPlayer = Managers.PlayerManager.Instance().GetByID(playerID);
            if (targetPlayer == null)
            {
                player.SendPlayerNotFoundNotification();
                return;
            }

            targetPlayer.CreatePenalty(Penalties.PenaltyType.Warning, reason, player.character.UID, DateTime.Now);
        }

        [Command("reward", GreedyArg = true)]
        public void RewardCommand(Client client, int playerID, string reason)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.IsReady())
                return;

            if (!player.IsAdminOfLevel(AdminLevel.Supporter))
            {
                player.SendNoPermissionsToCommandNotification();
                return;
            }

            var targetPlayer = Managers.PlayerManager.Instance().GetByID(playerID);
            if (targetPlayer == null)
            {
                player.SendPlayerNotFoundNotification();
                return;
            }

            targetPlayer.CreatePenalty(Penalties.PenaltyType.Reward, reason, player.character.UID, DateTime.Now);
        }
    }
}
