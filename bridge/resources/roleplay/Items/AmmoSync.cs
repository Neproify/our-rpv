using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace roleplay.Items
{
    public class AmmoSync : Script
    {
        [RemoteEvent("OnPlayerWeaponShot")]
        public void OnPlayerWeaponShot(Client client, int weaponHash)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.isLogged || player.character == null)
                return;

            var weapon = player.GetItems().Find(x => x.properties[0] == weaponHash && x.isUsed == true);

            if (weapon == null)
                return;

            weapon.properties[1] -= 1;

            if(weapon.properties[1] <= 0)
            {
                weapon.Use(player);
            }
        }
    }
}
