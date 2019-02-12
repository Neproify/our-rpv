using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace roleplay.Admin
{
    public class PlayerCommands : Script
    {
        [Command("agracz")]
        public void AdminPlayerCommand(Client client, string parameters)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.isLogged || player.character == null)
                return;

            if (!player.IsAdminOfLevel(AdminLevel.Supporter))
            {
                player.handle.SendNotification("~r~Nie masz uprawnień do użycia tej komendy!");
                return;
            }

            string[] args = parameters.Split(" ");
            int playerID;

            if (args.Length < 2)
                goto Usage;

            if (!Int32.TryParse(args[0], out playerID))
                goto Usage;

            Entities.Player targetPlayer = Managers.PlayerManager.Instance().GetByID(playerID);

            if (targetPlayer == null)
            {
                player.handle.SendNotification("~r~Nie znaleziono gracza o podanym identyfikatorze.");
                return;
            }

            if (args[1] == "hp")
            {
                if (args.Length < 3)
                {
                    goto HPUsage;
                }

                int healthValue;

                if (!Int32.TryParse(args[2], out healthValue))
                {
                    goto HPUsage;
                }

                targetPlayer.handle.Health = healthValue;
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

                uint modelHash;

                if (!UInt32.TryParse(args[2], out modelHash))
                {
                    goto ModelUsage;
                }

                NAPI.Entity.SetEntityModel(targetPlayer.handle, modelHash);
                return;
            }

            if (args[1] == "przedmioty")
            {
                player.handle.SendChatMessage($"==== LISTA PRZEDMIOTÓW GRACZA {targetPlayer.formattedName} ====");

                foreach (var item in targetPlayer.GetItems())
                {
                    player.handle.SendChatMessage($"[{item.UID}] - {item.name}");
                }

                player.handle.SendChatMessage("==== KONIEC LISTY ====");
                return;
            }

            if (args[1] == "money")
            {
                if (args.Length < 3)
                {
                    goto MoneyUsage;
                }

                int moneyValue;

                if (!Int32.TryParse(args[2], out moneyValue))
                {
                    goto MoneyUsage;
                }

                targetPlayer.money = moneyValue;
                return;
            }

            if (args[1] == "tpto")
            {
                player.handle.Position = targetPlayer.handle.Position;
                return;
            }

            if (args[1] == "tphere")
            {
                targetPlayer.handle.Position = player.handle.Position;
                return;
            }

        Usage:
            player.handle.SendNotification("Użycie komendy: /agracz [id gracza] [hp, unbw, model, przedmioty, money, tpto, tphere]");
            return;
        HPUsage:
            player.handle.SendNotification($"Użycie komendy: /agracz {targetPlayer} hp [ilość].");
            return;
        ModelUsage:
            player.handle.SendNotification($"Użycie komendy: /agracz {targetPlayer} model [hash modelu].");
            return;
        MoneyUsage:
            player.handle.SendNotification($"Użycie komendy: /agracz {targetPlayer} money [ilość].");
            return;
        }

        [Command("ado", GreedyArg = true)]
        public void AdminDoCommand(Client client, string action)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.isLogged || player.character == null)
                return;

            if (!player.IsAdminOfLevel(AdminLevel.Supporter))
            {
                player.handle.SendNotification("~r~Nie masz uprawnień do użycia tej komendy!");
                return;
            }

            NAPI.Chat.SendChatMessageToAll($"!{{#9A9CCD}}* {action}");
        }

        [Command("kick", GreedyArg = true)]
        public void KickCommand(Client client, int playerID, string reason)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.isLogged || player.character == null)
                return;

            if (!player.IsAdminOfLevel(AdminLevel.Supporter))
            {
                player.handle.SendNotification("~r~Nie masz uprawnień do użycia tej komendy!");
                return;
            }

            var targetPlayer = Managers.PlayerManager.Instance().GetByID(playerID);
            if (targetPlayer == null)
            {
                player.handle.SendNotification("~r~Podany gracz nie jest w grze!");
                return;
            }

            targetPlayer.CreatePenalty(Penalties.PenaltyType.Kick, reason, player.character.UID, DateTime.Now);
            targetPlayer.handle.Kick(reason);
        }

        [Command("gooc", GreedyArg = true)]
        public void AdminOOCCommand(Client client, string message)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.isLogged || player.character == null)
                return;

            if (!player.IsAdminOfLevel(AdminLevel.Supporter))
            {
                player.handle.SendNotification("~r~Nie masz uprawnień do użycia tej komendy!");
                return;
            }

            NAPI.Chat.SendChatMessageToAll($"!{{#FFFFFF}}(({player.formattedName}: {message}))");
        }

        [Command("warn", GreedyArg = true)]
        public void WarnCommand(Client client, int playerID, string reason)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.isLogged || player.character == null)
                return;

            if (!player.IsAdminOfLevel(AdminLevel.Supporter))
            {
                player.handle.SendNotification("~r~Nie masz uprawnień do użycia tej komendy!");
                return;
            }

            var targetPlayer = Managers.PlayerManager.Instance().GetByID(playerID);
            if (targetPlayer == null)
            {
                player.handle.SendNotification("~r~Podany gracz nie jest w grze!");
                return;
            }

            targetPlayer.CreatePenalty(Penalties.PenaltyType.Warning, reason, player.character.UID, DateTime.Now);
        }

        [Command("reward", GreedyArg = true)]
        public void RewardCommand(Client client, int playerID, string reason)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.isLogged || player.character == null)
                return;

            if (!player.IsAdminOfLevel(AdminLevel.Supporter))
            {
                player.handle.SendNotification("~r~Nie masz uprawnień do użycia tej komendy!");
                return;
            }

            var targetPlayer = Managers.PlayerManager.Instance().GetByID(playerID);
            if (targetPlayer == null)
            {
                player.handle.SendNotification("~r~Podany gracz nie jest w grze!");
                return;
            }

            targetPlayer.CreatePenalty(Penalties.PenaltyType.Reward, reason, player.character.UID, DateTime.Now);
        }
    }
}
