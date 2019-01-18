using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using Newtonsoft.Json;

namespace roleplay.Items
{
    public class PlayerItems : Script
    {
        public class ItemInfo
        {
            public int UID;
            public string name;
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

            List<ItemInfo> items = new List<ItemInfo>();

            foreach (var item in player.GetItems())
            {
                items.Add(new ItemInfo
                {
                    UID = item.UID,
                    name = item.name
                });

            }

            var output = JsonConvert.SerializeObject(items);

            NAPI.ClientEvent.TriggerClientEvent(client, "ShowPlayerItems", output);
        }

        [RemoteEvent("UsePlayerItem")]
        public void UsePlayerItem(Client client, int itemUID)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.isLogged || player.character == null)
                return;

            if (!player.CanUseItem(itemUID))
                return;

            var item = (Entities.Item)Managers.ItemManager.Instance().GetItemConverted(itemUID);

            if (item == null)
                return;

            item.Use(player);
        }
    }
}
