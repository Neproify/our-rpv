using System;
using System.Collections.Generic;
using GTANetworkAPI;

namespace roleplay
{
    public class PlayerGroups : Script
    {
        [Command("g", GreedyArg = true)]
        public void GroupCommand(Client client, string arg)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.IsReady())
                return;

            List<Entities.Group> groups = player.GetGroups();

            if (groups.Count == 0)
            {
                player.handle.SendNotification("~r~Nie należysz do żadnej grupy.");
                return;
            }

            var args = arg.Split(" ");

            if (args[0] == "lista")
            {
                player.handle.SendChatMessage("====LISTA TWOICH GRUP====");
                foreach (var group in groups)
                {
                    player.handle.SendChatMessage($"[{group.UID}]{group.name}");
                }
                player.handle.SendChatMessage("====KONIEC LISTY GRUP====");
                return;
            }

            if (!Int32.TryParse(args[0], out var groupID))
            {
                player.handle.SendNotification("Użyj: /g [lista, identyfikator grupy]");
                return;
            }

            var selectedGroup = groups.Find(x => x.UID == groupID);

            if (selectedGroup == null)
            {
                player.handle.SendNotification("~r~Podałeś identyfikator grupy do której nie należysz!");
                return;
            }

            if (args.GetLength(0) < 2)
                goto Usage;

            if (args[1] == "info")
            {
                player.handle.SendChatMessage($"Nazwa grupy: {selectedGroup.name}, typ: {selectedGroup.type}, na służbie: {selectedGroup.GetPlayersOnDuty().Count}");
                return;
            }

            if (args[1] == "duty")
            {
                if (player.groupDuty == null)
                {
                    var member = selectedGroup.GetMember(player);

                    if (member == null)
                        return;

                    player.groupDuty = new GroupDuty {member = member};
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

                if (player.handle.Model == member.rank.skin)
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
                player.handle.SendChatMessage($"====OSOBY ONLINE W GRUPIE {selectedGroup.name}====");
                foreach (var groupPlayer in selectedGroup.GetPlayersOnDuty())
                {
                    player.handle.SendChatMessage($"{groupPlayer.formattedName}(ID: {groupPlayer.handle.Handle})");
                }
                player.handle.SendChatMessage("====KONIEC LISTY====");
                return;
            }

            if (args[1] == "zamow" || args[1] == "zamów")
            {
                var member = selectedGroup.GetMember(player);

                if (member == null)
                    return;

                if ((member.rank.permissions & (int)GroupMemberPermission.OrdersManagement) == 0)
                {
                    player.handle.SendNotification("~r~Nie masz uprawnień do zamawiania przedmiotów w tej grupie!");
                    return;
                }

                if (args.Length < 3)
                    goto OrderUsage;

                if (args[2] == "lista")
                {
                    var products = Managers.GroupProductManager.Instance().GetProductsForGroup(selectedGroup);

                    player.handle.SendChatMessage($"====LISTA PRZEDMIOTÓW DO ZAMÓWIENIA W GRUPIE {selectedGroup.name}");
                    foreach (var productToList in products)
                    {
                        player.handle.SendChatMessage($"[{productToList.UID}] {productToList.name}, typ: {Utils.GetNameFromItemType(productToList.type)}, właściwości: {productToList.propertiesString}, cena: ${productToList.price}");
                    }
                    player.handle.SendChatMessage("====KONIEC LISTY====");
                    return;
                }

                if (args.Length < 4)
                    goto OrderUsage;

                if (!Int32.TryParse(args[2], out var productID))
                    goto OrderUsage;

                if (!Int32.TryParse(args[3], out var quantity))
                    quantity = 1;

                var product = Managers.GroupProductManager.Instance().GetByID(productID);

                if (product == null)
                {
                    player.handle.SendNotification("~r~Podałeś nieprawidłowy identyfikator produktu");
                    return;
                }

                if (!product.CanBeBoughtByGroup(selectedGroup))
                {
                    player.handle.SendNotification("~r~Podałeś nieprawidłowy identyfikator produktu");
                    return;
                }

                int finalPrice = product.price * quantity;

                if (selectedGroup.bank < finalPrice)
                {
                    player.handle.SendNotification("~r~Grupa nie ma wystarczającej ilości środków na koncie.");
                    return;
                }

                selectedGroup.bank -= finalPrice;
                selectedGroup.Save();

                for (int i = 1; i <= quantity; i++)
                {
                    var createdItem = Managers.ItemManager.Instance().CreateItem();
                    createdItem.name = product.name;
                    createdItem.type = product.type;
                    createdItem.propertiesString = product.propertiesString.Replace("*group*", selectedGroup.UID.ToString());
                    createdItem.ChangeOwner(OwnerType.Group, selectedGroup.UID);
                    createdItem.Save();
                    Managers.ItemManager.Instance().ReloadItem(createdItem);
                }

                player.handle.SendNotification($"~g~Zakupiłeś {quantity} sztuk {product.name} za ${finalPrice}.");

                return;
            }

            if (args[1] == "magazyn")
            {
                var member = selectedGroup.GetMember(player);

                if ((member.rank.permissions & (int)GroupMemberPermission.ItemsManagement) == 0)
                {
                    player.handle.SendNotification("~r~Nie masz dostępu do magazynu grupy.");
                    return;
                }

                if (args.Length < 3)
                    goto StorageUsage;

                if (args[2] == "lista")
                {
                    var items = selectedGroup.GetItems();

                    if (items == null)
                    {
                        player.handle.SendNotification("~r~Magazyn grupy jest pusty.");
                        return;
                    }

                    player.handle.SendChatMessage($"====MAGAZYN GRUPY {selectedGroup.name}====");

                    foreach (var item in items)
                    {
                        player.handle.SendChatMessage($"[{item.UID}] {item.name}, typ: {item.type}");
                    }

                    player.handle.SendChatMessage("====KONIEC LISTY====");

                    return;
                }

                if (args[2] == "wloz" || args[2] == "włóż")
                {
                    if (args.Length < 4)
                        goto StorageUsage;

                    if (!Int32.TryParse(args[3], out var itemID))
                        goto StorageUsage;

                    var item = Managers.ItemManager.Instance().GetByID(itemID);
                    if (item == null)
                    {
                        player.handle.SendNotification("~r~Podałeś nieprawidłowy identyfikator przedmiotu.");
                        return;
                    }

                    if (!player.CanUseItem(item))
                    {
                        player.handle.SendNotification("~r~Podałeś nieprawidłowy identyfikator przedmiotu.");
                        return;
                    }

                    item.ChangeOwner(OwnerType.Group, selectedGroup.UID);
                    item.Save();
                    player.handle.SendNotification("Umiesciłeś przedmiot w magazynie grupowym.");

                    return;
                }

                if (args[2] == "wyciagnij" || args[2] == "wyciągnij")
                {
                    if (args.Length < 4)
                        goto StorageUsage;

                    if (!Int32.TryParse(args[3], out var itemID))
                        goto StorageUsage;

                    var item = Managers.ItemManager.Instance().GetByID(itemID);
                    if (item == null)
                    {
                        player.handle.SendNotification("~r~Podałeś nieprawidłowy identyfikator przedmiotu.");
                        return;
                    }

                    if (item.ownerType != OwnerType.Group || item.ownerID != selectedGroup.UID)
                    {
                        player.handle.SendNotification("~r~Podałeś nieprawidłowy identyfikator przedmiotu.");
                        return;
                    }

                    item.ChangeOwner(OwnerType.Character, player.character.UID);
                    item.Save();
                    player.handle.SendNotification("Zabrałeś przedmiot z magazynu grupowego.");

                    return;
                }

                goto StorageUsage;
            }

        Usage:
            player.handle.SendNotification($"Użyj: /g {groupID} [info, duty, przebierz, online, zamów, magazyn]");
            return;
        OrderUsage:
            player.handle.SendNotification($"Użyj: /g {groupID} zamów [lista/identyfikator produktu] [ilość]");
            return;
        StorageUsage:
            player.handle.SendNotification($"Użyj: /g {groupID} magazyn [lista/włóż/wyciągnij] [identyfikator przedmiotu]");
            return;
        }

    }
}
