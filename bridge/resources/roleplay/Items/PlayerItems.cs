﻿using System.Collections.Generic;
using GTANetworkAPI;

namespace roleplay.Items
{
    public class PlayerItems : Script
    {
        [Command("podnies")]
        public void ItemPickupCommand(Client client)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.isLogged && player.character == null)
                return;

            var item = player.GetClosestItem();

            item.ChangeOwner(OwnerType.Character, player.character.UID);
            item.Save();

            player.handle.SendNotification($"~g~Podniosłeś przedmiot {item.name}.");
        }

        [RemoteEvent("ShowPlayerItems")]
        public void ShowPlayerItems(Client client)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.isLogged && player.character == null)
                return;

            if (player.GetItems() == null)
            {
                client.SendNotification("~r~Nie posiadasz żadnych przedmiotów!");
                return;
            }

			player.ShowItems();
        }

        [RemoteEvent("UsePlayerItem")]
        public void UsePlayerItem(Client client, int itemUID)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.isLogged || player.character == null)
                return;

            var item = Managers.ItemManager.Instance().GetByID(itemUID);

            item?.Use(player);
        }

        [RemoteEvent("DropPlayerItem")]
        public void DropPlayerItem(Client client, int itemUID)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.isLogged || player.character == null)
                return;

            var item = Managers.ItemManager.Instance().GetByID(itemUID);

            if (!player.CanUseItem(item))
                return;

            item.position = player.handle.Position;
            item.position.Z -= 0.5f;
            item.ChangeOwner(OwnerType.World, 0);
            item.Save();

            player.handle.SendNotification($"~g~Wyrzuciłeś przedmiot {item.name}");
        }

        [Command("przeszukaj")]
        public void SearchItemsCommand(Client client, int ID)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.isLogged || player.character == null)
                return;

            if (!player.IsOnDutyOfGroupType(GroupType.Police))
            {
                player.handle.SendNotification("~r~Nie posiadasz uprawnień do przeszukiwania.");
                return;
            }

            var searchedPlayer = Managers.PlayerManager.Instance().GetByID(ID);

            if(searchedPlayer == null || !searchedPlayer.isLogged || searchedPlayer.character == null)
            {
                player.handle.SendNotification("~r~Podałeś zły identyfikator gracza!");
                return;
            }

            if(player.handle.Position.DistanceTo(searchedPlayer.handle.Position) > 5)
            {
                player.handle.SendNotification("~r~Znajdujesz się za daleko od tego gracza.");
                return;
            }

            player.handle.SendChatMessage($"====LISTA PRZEDMIOTÓW GRACZA {searchedPlayer.formattedName}");
            foreach(var item in searchedPlayer.GetItems())
            {
                player.handle.SendChatMessage($"Nazwa: {item.name}, typ: {item.type}");
            }
            player.handle.SendChatMessage("====KONIEC LISTY====");

            player.OutputMe($"przeszukuje {searchedPlayer.formattedName}.");
        }
    }
}
