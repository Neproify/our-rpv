using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace roleplay.Groups
{
    public class PlayerGroups : Script
    {
        [Command("g", GreedyArg = true)]
        public void GroupCommand(Client client, string arg)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.isLogged || player.character == null)
                return;

            List<Entities.Group> groups = player.GetGroups();

            if(groups.Count == 0)
            {
                player.handle.SendNotification("~r~Nie należysz do żadnej grupy.");
                return;
            }

            var args = arg.Split(" ");

            if(args[0] == "lista")
            {
                player.handle.SendChatMessage("====LISTA TWOICH GRUP====");
                foreach (var group in groups)
                {
                    player.handle.SendChatMessage($"[{group.UID}]{group.name}");
                }
                player.handle.SendChatMessage("====KONIEC LISTY GRUP====");
                return;
            }

            Entities.Group selectedGroup = null;
            int groupID;
            bool result = Int32.TryParse(args[0], out groupID);

            if(!result)
            {
                player.handle.SendNotification("Użyj: /g [lista, identyfikator grupy]");
                return;
            }

            selectedGroup = groups.Find(x => x.UID == groupID);
            
            if(selectedGroup == null)
            {
                player.handle.SendNotification("~r~Podałeś identyfikator grupy do której nie należysz!");
                return;
            }

            if (args.GetLength(0) < 2)
                goto Usage;

            if(args[1] == "info")
            {
#warning Implement this.
                return;
            }
            if (args[1] == "duty")
            {
                if(player.groupDuty == null)
                {
                    var member = selectedGroup.GetMember(player);

                    if (member == null)
                        return;

                    player.groupDuty = new GroupDuty();
                    player.groupDuty.member = member;
                    player.handle.SendNotification($"~g~Rozpocząłeś pracę w grupie {selectedGroup.name}.");
                }
                else
                {
                    if (player.groupDuty.member.group == selectedGroup)
                    {
                        player.groupDuty = null;
                        player.handle.SendNotification($"~g~Skończyłeś pracę w grupie {selectedGroup.name}");
                    }
                    else
                    {
                        player.handle.SendNotification($"~r~Nie możesz rozpocząć pracy w tej grupie({selectedGroup.UID}), ponieważ pracujesz obecnie w innej({player.groupDuty.member.group.UID}).");
                    }
                }
                return;
            }
            if (args[1] == "przebierz")
            {
                var member = selectedGroup.GetMember(player);

                if (member == null)
                    return;
            
                if(player.handle.Model == member.rank.skin)
                {
                    NAPI.Entity.SetEntityModel(player.handle, player.character.model);
                    player.handle.SendNotification("~g~Przebrałeś się w codzienne ubranie.");
                }
                else
                {
                    if (member.rank.skin == 0)
                    {
                        player.handle.SendNotification("~r~Nie masz przypisanego ubrania grupowego!");
                        return;
                    }

                    NAPI.Entity.SetEntityModel(player.handle, member.rank.skin);
                    player.handle.SendNotification("~g~Przebrałeś się w ubranie służbowe.");
                }

                return;
            }
            if (args[1] == "online")
            {
#warning Implement this.
                return;
            }

            Usage:

            player.handle.SendNotification($"Użyj: /g {groupID} [info, duty, przebierz, online]");
            return;
        }
    }
}
