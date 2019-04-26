using System.Collections.Generic;
using GTANetworkAPI;
using MongoDB.Bson;

namespace roleplay.Items
{
    public class PlayerItems : Script
    {
        [Command("podnies")]
        public void ItemPickupCommand(Client client)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.IsReady())
                return;

            var item = player.GetClosestItem();

            if(item == null)
            {
                player.SendItemNotFoundNotification();
                return;
            }

            item.ChangeOwner(OwnerType.Character, player.character.UID);
            item.Save();

            player.SendNotification($"~g~Podniosłeś przedmiot {item.name}.");
        }

        [RemoteEvent("ShowPlayerItems")]
        public void ShowPlayerItems(Client client)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.IsReady())
                return;

            if (player.GetItems() == null)
            {
                player.SendNotification("~r~Nie posiadasz żadnych przedmiotów!");
                return;
            }

			player.ShowItems();
        }

        [RemoteEvent("UsePlayerItem")]
        public void UsePlayerItem(Client client, string itemUID)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.IsReady())
                return;

            var item = Managers.ItemManager.Instance().GetByID(ObjectId.Parse(itemUID));

            item?.Use(player);
        }

        [RemoteEvent("DropPlayerItem")]
        public void DropPlayerItem(Client client, string itemUID)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.IsReady())
                return;

            var item = Managers.ItemManager.Instance().GetByID(ObjectId.Parse(itemUID));

            if (!player.CanUseItem(item))
                return;

            item.position = player.GetPosition();
            item.position.Z -= 0.5f;
            item.ChangeOwner(OwnerType.World, ObjectId.Empty);
            item.Save();

            player.SendNotification($"~g~Wyrzuciłeś przedmiot {item.name}");
        }

        [Command("przeszukaj")]
        public void SearchItemsCommand(Client client, int ID)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.IsReady())
                return;

            if (!player.IsOnDutyOfGroupType(GroupType.Police))
            {
                player.SendNoPermissionsToCommandNotification();
                return;
            }

            var searchedPlayer = Managers.PlayerManager.Instance().GetByID(ID);

            if(searchedPlayer?.IsReady() == false)
            {
                player.SendPlayerNotFoundNotification();
                return;
            }

            if(player.GetPosition().DistanceTo(searchedPlayer.GetPosition()) > 5)
            {
                player.SendNotification("~r~Znajdujesz się za daleko od tego gracza.");
                return;
            }

            player.SendChatMessage($"====LISTA PRZEDMIOTÓW GRACZA {searchedPlayer.formattedName}");
            foreach(var item in searchedPlayer.GetItems())
            {
                player.SendChatMessage($"Nazwa: {item.name}, typ: {item.type}");
            }
            player.SendChatMessage("====KONIEC LISTY====");

            player.OutputMe($"przeszukuje {searchedPlayer.formattedName}.");
        }
    }
}
