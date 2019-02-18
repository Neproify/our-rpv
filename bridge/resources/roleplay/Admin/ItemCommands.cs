using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace roleplay.Admin
{
    public class ItemCommands : Script
    {
        [Command("aprzedmiot", GreedyArg = true)]
        public void AdminItemsCommand(Client client, string parameters)
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
                Entities.Item createdItem = Managers.ItemManager.Instance().CreateItem();
                player.handle.SendNotification($"ID stworzonego przedmiotu: {createdItem.UID}.");
                return;
            }

            int itemID;

            if (args.Length < 2)
                goto Usage;

            if (!Int32.TryParse(args[0], out itemID))
                goto Usage;

            Entities.Item item = Managers.ItemManager.Instance().GetByID(itemID);

            if (item == null)
            {
                player.handle.SendNotification("~r~Nie znaleziono przedmiotu o podanym identyfikatorze.");
                return;
            }

            if (args[1] == "nazwa")
            {
                if(args.Length < 3)
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
                if(args.Length != 3)
                {
                    goto TypeUsage;
                }

                ItemType type = Utils.GetItemTypeByName(args[2]);

                if(type == ItemType.Invalid)
                {
                    player.handle.SendNotification("~r~Podałeś nieprawidłowy typ przedmiotu.");
                    return;
                }

                item.type = type;

                item.Save();

                Managers.ItemManager.Instance().ReloadItem(item);
                return;
            }

            if (args[1] == "wlasciciel" || args[1] == "właściciel")
            {
                if (args.Length != 4)
                {
                    goto OwnerUsage;
                }

                OwnerType type = Utils.GetOwnerTypeByName(args[2]);

                if (type == OwnerType.Invalid)
                {
                    player.handle.SendNotification("~r~Podałeś nieprawidłowy typ właściciela.");
                    return;
                }

                int ownerID;

                if(!Int32.TryParse(args[3], out ownerID))
                {
                    goto OwnerUsage;
                }

                item.ChangeOwner(type, ownerID);
                item.Save();
                return;
            }

            if(args[1] == "wlasciwosci" || args[1] == "właściwości")
            {
                if (args.Length != 3)
                {
                    goto PropertiesUsage;
                }

                item.propertiesString = args[2];

                item.Save();

                return;
            }

            if(args[1] == "wlasciwosc" || args[1] == "właściwość")
            {
                if(args.Length != 4)
                {
                    goto PropertyUsage;
                }

                int propertyNumber, propertyValue;

                if(!Int32.TryParse(args[2], out propertyNumber) || !Int32.TryParse(args[3], out propertyValue))
                {
                    goto PropertyUsage;
                }

                item.properties[propertyNumber] = propertyValue;

                item.Save();

                return;
            }

            if(args[1] == "tpto")
            {
                if(item.objectHandle != null)
                {
                    player.handle.Position = item.objectHandle.Position;
                }
                return;
            }

            if(args[1] == "tphere")
            {
                if(item.objectHandle != null)
                {
                    item.position = player.handle.Position;
                    item.objectHandle.Position = item.position;
                    item.Save();
                }
                return;
            }

        Usage:
            player.handle.SendNotification("Użycie komendy: /aprzedmiot [id przedmiotu/stwórz] [nazwa, typ, właściciel, właściwości, właściwość, tpto, tphere]");
            return;
        NameUsage:
            player.handle.SendNotification($"Użycie komendy: /aprzedmiot {item.UID} nazwa [wartość].");
            return;
        TypeUsage:
            player.handle.SendNotification($"Użycie komendy: /aprzedmiot {item.UID} typ [{Utils.GetItemTypes()}].");
            return;
        OwnerUsage:
            player.handle.SendNotification($"Użycie komendy: /aprzedmiot {item.UID} właściciel [{Utils.GetOwnerTypes()}] [identyfikator własciciela].");
            return;
        PropertiesUsage:
            player.handle.SendNotification($"Użycie komendy: /aprzedmiot {item.UID} właściwości [lista oddzielona znakiem |].");
            return;
        PropertyUsage:
            player.handle.SendNotification($"Użycie komendy: /aprzedmiot {item.UID} właściwość [numer] [wartość].");
            return;
        }
    }
}
