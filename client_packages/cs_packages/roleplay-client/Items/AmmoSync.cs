using System;
using System.Collections.Generic;
using System.Text;
using RAGE;
using RAGE.Elements;

namespace roleplay_client.Items
{
    public class AmmoSync : Events.Script
    {
        public AmmoSync()
        {
            Events.OnPlayerWeaponShot += OnPlayerWeaponShot;
        }

        private void OnPlayerWeaponShot(Vector3 targetPos, Player target, Events.CancelEventArgs cancel)
        {
            int weaponHash = 0;
            Player.LocalPlayer.GetCurrentWeapon(ref weaponHash, true);
            Events.CallRemote("OnPlayerWeaponShot", weaponHash);
        }
    }
}
