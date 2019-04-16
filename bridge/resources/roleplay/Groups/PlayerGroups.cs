using System;
using System.Collections.Generic;
using GTANetworkAPI;
using MongoDB.Bson;

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
                player.SendNotification("~r~Nie należysz do żadnej grupy.");
                return;
            }

            var args = arg.Split(" ");

            if (args[0] == "lista")
            {
                player.SendChatMessage("====LISTA TWOICH GRUP====");
                foreach (var group in groups)
                {
                    player.SendChatMessage($"[{group.UID}]{group.name}");
                }
                player.SendChatMessage("====KONIEC LISTY GRUP====");
                return;
            }

            if(!ObjectId.TryParse(args[0], out var groupID))
            {
                player.SendUsageNotification("Użyj: /g [lista, identyfikator grupy]");
                return;
            }

            var selectedGroup = groups.Find(x => x.UID == groupID);

            if (selectedGroup == null)
            {
                player.SendGroupNotFoundNotification();
                return;
            }

            if (args.GetLength(0) < 2)
                goto Usage;

            if (args[1] == "info")
            {
                player.SendChatMessage($"Nazwa grupy: {selectedGroup.name}, typ: {selectedGroup.type}, na służbie: {selectedGroup.GetPlayersOnDuty().Count}");
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
                    player.SendNotification($"~g~Rozpocząłeś pracę w grupie {selectedGroup.name}.");
                }
                else
                {
                    if (player.groupDuty.member.group == selectedGroup)
                    {
                        player.groupDuty = null;
                        player.SendNotification($"~g~Skończyłeś pracę w grupie {selectedGroup.name}");
                    }
                    else
                    {
                        player.SendNotification($"~r~Nie możesz rozpocząć pracy w tej grupie({selectedGroup.UID}), ponieważ pracujesz obecnie w innej({player.groupDuty.member.group.UID}).");
                    }
                }
                return;
            }

            if (args[1] == "przebierz")
            {
                var member = selectedGroup.GetMember(player);

                if (member == null)
                    return;

                if (player.GetModel() == member.rank.skin)
                {
                    player.SetModel(player.character.model);
                    player.SendNotification("~g~Przebrałeś się w codzienne ubranie.");
                }
                else
                {
                    if (member.rank.skin == 0)
                    {
                        player.SendNotification("~r~Nie masz przypisanego ubrania grupowego!");
                        return;
                    }

                    player.SetModel(member.rank.skin);
                    player.SendNotification("~g~Przebrałeś się w ubranie służbowe.");
                }

                return;
            }

            if (args[1] == "online")
            {
                player.SendChatMessage($"====OSOBY ONLINE W GRUPIE {selectedGroup.name}====");
                foreach (var groupPlayer in selectedGroup.GetPlayersOnDuty())
                {
                    player.SendChatMessage($"{groupPlayer.formattedName}(ID: {groupPlayer.GetGameID()})");
                }
                player.SendChatMessage("====KONIEC LISTY====");
                return;
            }

            if (args[1] == "zamow" || args[1] == "zamów")
            {
                var member = selectedGroup.GetMember(player);

                if (member == null)
                    return;

                if ((member.rank.permissions & (int)GroupMemberPermission.OrdersManagement) == 0)
                {
                    player.SendNoPermissionsToCommandNotification();
                    return;
                }

                if (args.Length < 3)
                    goto OrderUsage;

                if (args[2] == "lista")
                {
                    var products = Managers.GroupProductManager.Instance().GetProductsForGroup(selectedGroup);

                    player.SendChatMessage($"====LISTA PRZEDMIOTÓW DO ZAMÓWIENIA W GRUPIE {selectedGroup.name}");
                    foreach (var productToList in products)
                    {
                        player.SendChatMessage($"[{productToList.UID}] {productToList.name}, typ: {Utils.GetNameFromItemType(productToList.type)}, właściwości: {productToList.properties.ToString()}, cena: ${productToList.price}");
                    }
                    player.SendChatMessage("====KONIEC LISTY====");
                    return;
                }

                if (args.Length < 4)
                    goto OrderUsage;

                if (!ObjectId.TryParse(args[2], out var productID))
                    goto OrderUsage;

                if (!Int32.TryParse(args[3], out var quantity))
                    quantity = 1;

                var product = Managers.GroupProductManager.Instance().GetByID(productID);

                if (product?.CanBeBoughtByGroup(selectedGroup) == false)
                {
                    player.SendNotification("~r~Podałeś nieprawidłowy identyfikator produktu");
                    return;
                }

                int finalPrice = product.price * quantity;

                if (selectedGroup.bank < finalPrice)
                {
                    player.SendNotification("~r~Grupa nie ma wystarczającej ilości środków na koncie.");
                    return;
                }

                selectedGroup.bank -= finalPrice;
                selectedGroup.Save();

                for (int i = 1; i <= quantity; i++)
                {
                    var createdItem = Managers.ItemManager.Instance().CreateItem(product.name, product.type);

                    createdItem.properties = new Dictionary<string, object>(product.properties);

                    if(createdItem.properties.ContainsValue("*group*"))
                    {
                        foreach(var key in createdItem.properties.Keys)
                        {
                            if((string)createdItem.properties[key] == "*group*")
                            {
                                createdItem.properties[key] = selectedGroup.UID;
                            }
                        }
                    }

                    createdItem.ChangeOwner(OwnerType.Group, selectedGroup.UID);
                    createdItem.Save();
                    Managers.ItemManager.Instance().ReloadItem(createdItem);
                }

                player.SendNotification($"~g~Zakupiłeś {quantity} sztuk {product.name} za ${finalPrice}.");

                return;
            }

            if (args[1] == "magazyn")
            {
                var member = selectedGroup.GetMember(player);

                if ((member.rank.permissions & (int)GroupMemberPermission.ItemsManagement) == 0)
                {
                    player.SendNoPermissionsToCommandNotification();
                    return;
                }

                if (args.Length < 3)
                    goto StorageUsage;

                if (args[2] == "lista")
                {
                    var items = selectedGroup.GetItems();

                    if (items == null)
                    {
                        player.SendNotification("~r~Magazyn grupy jest pusty.");
                        return;
                    }

                    player.SendChatMessage($"====MAGAZYN GRUPY {selectedGroup.name}====");

                    foreach (var item in items)
                    {
                        player.SendChatMessage($"[{item.UID}] {item.name}, typ: {item.type}");
                    }

                    player.SendChatMessage("====KONIEC LISTY====");

                    return;
                }

                if (args[2] == "wloz" || args[2] == "włóż")
                {
                    if (args.Length < 4)
                        goto StorageUsage;

                    if (!ObjectId.TryParse(args[3], out var itemID))
                        goto StorageUsage;

                    var item = Managers.ItemManager.Instance().GetByID(itemID);

                    if (!player.CanUseItem(item))
                    {
                        player.SendNotification("~r~Podałeś nieprawidłowy identyfikator przedmiotu.");
                        return;
                    }

                    item.ChangeOwner(OwnerType.Group, selectedGroup.UID);
                    item.Save();
                    player.SendNotification("Umiesciłeś przedmiot w magazynie grupowym.");

                    return;
                }

                if (args[2] == "wyciagnij" || args[2] == "wyciągnij")
                {
                    if (args.Length < 4)
                        goto StorageUsage;

                    if (!ObjectId.TryParse(args[3], out var itemID))
                        goto StorageUsage;

                    var item = Managers.ItemManager.Instance().GetByID(itemID);

                    if (item?.ownerType != OwnerType.Group || item?.ownerID != selectedGroup.UID)
                    {
                        player.SendNotification("~r~Podałeś nieprawidłowy identyfikator przedmiotu.");
                        return;
                    }

                    item.ChangeOwner(OwnerType.Character, player.character.UID);
                    item.Save();
                    player.SendNotification("Zabrałeś przedmiot z magazynu grupowego.");

                    return;
                }

                goto StorageUsage;
            }

        Usage:
            player.SendUsageNotification($"Użyj: /g {groupID} [info, duty, przebierz, online, zamów, magazyn]");
            return;
        OrderUsage:
            player.SendUsageNotification($"Użyj: /g {groupID} zamów [lista/identyfikator produktu] [ilość]");
            return;
        StorageUsage:
            player.SendUsageNotification($"Użyj: /g {groupID} magazyn [lista/włóż/wyciągnij] [identyfikator przedmiotu]");
            return;
        }

    }
}
