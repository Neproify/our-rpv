using roleplay.Entities;
using GTANetworkAPI;

namespace roleplay.Items.ItemType
{
    public class Weapon : Item
    {
        public override bool Use(Player player)
        {
            if (!base.Use(player))
                return false;

            if (!isUsed)
            {
                if(player.GetItems().Find(x => x.properties[0] == properties[0] && x.isUsed) != null)
                {
                    player.handle.SendNotification("~r~Posiadasz już wyjętą broń tego typu.");
                    return false;
                }

                if(properties[1] <= 0)
                {
                    player.handle.SendNotification("~r~Wybrana broń nie posiada amunicji.");
                    return false;
                }

                if(properties[1] != 0)
                {
                    if(!player.IsOnDutyOfGroupID(properties[1]))
                    {
                        player.handle.SendNotification("~r~Nie masz uprawnień do użycia tej broni. Jest ona podpisana pod grupę.");
                        return false;
                    }
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
                Save();
            }

            NAPI.ClientEvent.TriggerClientEvent(player.handle, "HidePlayerItems");

            return true;
        }
    }
}
