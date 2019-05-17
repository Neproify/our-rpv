using System;
using GTANetworkAPI;
using MongoDB.Bson;

namespace roleplay.Admin
{
    public class ItemCommands : Script
    {
        [Command("aprzedmiot", GreedyArg = true)]
        public void AdminItemsCommand(Client client, string parameters)
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

            if (args[0] == "stworz" || args[0] == "stwórz")
            {
                Entities.Item createdItem = Managers.ItemManager.Instance().CreateItem();
                player.SendChatMessage($"ID stworzonego przedmiotu: {createdItem.UID}.");
                player.selectedEntities.selectedItem = createdItem;
                return;
            }

            if (args.Length < 2)
                goto Usage;

            var itemID = AdminHelpers.GetObjectId(args[0], EntityType.Item, player);

            if (itemID == ObjectId.Empty)
                goto Usage;

            Entities.Item item = Managers.ItemManager.Instance().GetByID(itemID);

            if (item == null)
            {
                player.SendItemNotFoundNotification();
                return;
            }

            if (args[1] == "nazwa")
            {
                if (args.Length < 3)
                {
                    goto NameUsage;
                }

                string name = string.Join(" ", args, 2, args.Length - 2);

                item.name = name;
                item.Save();
                return;
            }

            if (args[1] == "typ")
            {
                if (args.Length != 3)
                {
                    goto TypeUsage;
                }

                ItemType type = Utils.GetItemTypeByName(args[2]);

                if (type == ItemType.Invalid)
                {
                    player.SendNotification("~r~Podałeś nieprawidłowy typ przedmiotu.");
                    return;
                }

                item.type = type;

                item.Save();

                Managers.ItemManager.Instance().ReloadItem(item);
                return;
            }

            if (args[1] == "wlasciciel" || args[1] == "właściciel")
            {
                if (args.Length < 3)
                {
                    goto OwnerUsage;
                }

                OwnerType type = Utils.GetOwnerTypeByName(args[2]);

                if (type == OwnerType.Invalid)
                {
                    player.SendInvalidOwnerTypeNotification();
                    return;
                }

                ObjectId ownerID = ObjectId.Empty;

                if (type != OwnerType.None && type != OwnerType.World)
                {
                    if (args.Length != 4)
                        goto OwnerUsage;

                    ownerID = AdminHelpers.GetObjectId(args[3], Utils.GetEntityTypeFromOwnerType(type), player);
                }

                item.ChangeOwner(type, ownerID);
                item.Save();
                return;
            }

            if (args[1] == "wlasciwosc" || args[1] == "właściwość")
            {
                if (args.Length != 4)
                {
                    goto PropertyUsage;
                }

                item.properties[args[2]] = args[3];

                item.Save();

                return;
            }

            if (args[1] == "tpto")
            {
                if (item.objectHandle != null)
                {
                    player.SetPosition(item.objectHandle.Position);
                }
                return;
            }

            if (args[1] == "tphere")
            {
                if (item.objectHandle != null)
                {
                    item.position = player.GetPosition();
                    item.objectHandle.Position = item.position;
                    item.Save();
                }
                return;
            }

        Usage:
            player.SendUsageNotification("Użycie komendy: /aprzedmiot [id przedmiotu/stwórz] [nazwa, typ, właściciel, właściwość, tpto, tphere]");
            return;
        NameUsage:
            player.SendUsageNotification($"Użycie komendy: /aprzedmiot {item.UID} nazwa [wartość].");
            return;
        TypeUsage:
            player.SendUsageNotification($"Użycie komendy: /aprzedmiot {item.UID} typ [{Utils.GetItemTypes()}].");
            return;
        OwnerUsage:
            player.SendUsageNotification($"Użycie komendy: /aprzedmiot {item.UID} właściciel [{Utils.GetOwnerTypes()}] [identyfikator własciciela].");
            return;
        PropertyUsage:
            player.SendUsageNotification($"Użycie komendy: /aprzedmiot {item.UID} właściwość [numer] [wartość].");
            return;
        }
    }
}
