using System;
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
            if(!isUsed)
            {
#warning Enable after update, GetWeapons is not implemented yet.
                /*foreach(var weapon in player.handle.Weapons)
                {
                    if(weapon == (WeaponHash)properties[0])
                    {
                        player.handle.SendNotification("~r~Posiadasz już wyjętą broń tego typu.");
                        return;
                    }
                }*/
#warning TODO: save ammo on shot

                player.handle.GiveWeapon((WeaponHash)properties[0], properties[1]);
                isUsed = true;
            }
            else
            {
                player.handle.RemoveWeapon((WeaponHash)properties[0]);
                isUsed = false;
            }
        }
    }
}
