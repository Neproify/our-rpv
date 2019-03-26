using System;
using GTANetworkAPI;

namespace roleplay.Admin
{
    public class GroupCommands : Script
    {
        [Command("agrupa", GreedyArg = true)]
        public void AdminGroupsCommand(Client client, string parameters)
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

            if (args[0] == "stworz" || args[0] == "stwórz")
            {
                Entities.Group createdGroup = Managers.GroupManager.Instance().CreateGroup();
                player.handle.SendNotification($"ID stworzonej grupy: {createdGroup.UID}.");
                return;
            }

            if (args.Length < 2)
                goto Usage;

            if (!Int32.TryParse(args[0], out var groupID))
                goto Usage;

            Entities.Group group = Managers.GroupManager.Instance().GetByID(groupID);

            if (group == null)
            {
                player.handle.SendNotification("~r~Nie znaleziono budynku o podanym identyfikatorze.");
                return;
            }

            if(args[1] == "lider")
            {
                if (args.Length < 3)
                    goto LeaderUsage;

                if (!Int32.TryParse(args[2], out var leaderID))
                    goto LeaderUsage;

                group.leaderID = leaderID;

                group.Save();

                return;
            }

            if(args[1] == "nazwa")
            {
                if (args.Length < 3)
                    goto NameUsage;

                string name = string.Join(" ", args, 2, args.Length - 2);

                group.name = name;
                group.Save();

                return;
            }

            if (args[1] == "typ")
            {
                if (args.Length != 3)
                {
                    goto TypeUsage;
                }

                GroupType type = Utils.GetGroupTypeByName(args[2]);

                if (type == GroupType.None)
                {
                    player.handle.SendNotification("~r~Podałeś nieprawidłowy typ grupy.");
                    return;
                }

                group.type = type;

                group.Save();
                return;
            }

            if(args[1] == "suprawnienia")
            {
                if(args.Length != 3)
                {
                    goto SpecialPermissionsUsage;
                }

                if (!Int32.TryParse(args[2], out var permissions))
                {
                    goto SpecialPermissionsUsage;
                }

                group.specialPermissions = permissions;
                group.Save();

                return;
            }

            if(args[1] == "bank")
            {
                if(args.Length != 3)
                {
                    goto BankUsage;
                }

                if(!Int32.TryParse(args[2], out var bank))
                {
                    goto BankUsage;
                }

                group.bank = bank;
                group.Save();
                return;
            }

        Usage:
            player.handle.SendNotification("Użycie komendy: /agrupa [id grupy/stwórz] [lider, nazwa, typ, suprawnienia]");
            return;
        LeaderUsage:
            player.handle.SendNotification($"Użycie komendy: /agrupa {groupID} lider [UID gracza]");
            return;
        NameUsage:
            player.handle.SendNotification($"Użycie komendy: /agrupa {groupID} nazwa [nazwa grupy]");
            return;
        TypeUsage:
            player.handle.SendNotification($"Użycie komendy: /agrupa {groupID} typ [{Utils.GetGroupTypes()}]");
            return;
        SpecialPermissionsUsage:
            player.handle.SendNotification($"Użycie komendy: /agrupa {groupID} suprawnienia [uprawnienia]");
            return;
        BankUsage:
            player.handle.SendNotification($"Użycie komendy: /agrupa {groupID} bank [stan]");
        }
    }
}
