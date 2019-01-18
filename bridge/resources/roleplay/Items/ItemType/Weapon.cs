﻿using System;
using System.Collections.Generic;
using System.Text;
using roleplay.Entities;
using GTANetworkAPI;

namespace roleplay.Items.ItemType
{
    public class Weapon : Entities.Item
    {
        public override void Use(Player player)
        {
            base.Use(player);
            if(!isUsed)
            {
#warning Check for slots, not exact weapon. Why? Because most of weapons have shared ammo. Needs a lot of code, but can't be synchronised right now(without using client-side).
                if(player.GetItems().Find(x => x.properties[0] == properties[0] && x.isUsed == true) != null)
                {
                    player.handle.SendNotification("~r~Posiadasz już wyjętą broń tego typu.");
                    return;
                }

                if(properties[1] <= 0)
                {
                    player.handle.SendNotification("~r~Wybrana broń nie posiada amunicji.");
                    return;
                }

                player.handle.GiveWeapon((WeaponHash)properties[0], properties[1]);
                isUsed = true;
                player.OutputMe($"wyciąga {name}.");
            }
            else
            {
                player.handle.RemoveWeapon((WeaponHash)properties[0]);
                isUsed = false;
                player.OutputMe($"chowa {name}.");
            }

            NAPI.ClientEvent.TriggerClientEvent(player.handle, "HidePlayerItems");
        }
    }
}
