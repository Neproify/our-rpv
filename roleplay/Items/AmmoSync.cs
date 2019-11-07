using GTANetworkAPI;

namespace roleplay.Items
{
    public class AmmoSync : Script
    {
        [RemoteEvent("OnPlayerWeaponShot")]
        public void OnPlayerWeaponShot(Client client, int weaponHash)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            var weapon = player.GetWeapons()?.Find(x => x.isUsed && x.weaponHash == weaponHash);

            if (!player.IsReady())
                return;

            if (weapon == null)
                return;

            weapon.ammo -= 1;

            if(weapon.ammo <= 0)
            {
                weapon.Use(player);
            }
        }
    }
}
