using System;
using System.Collections.Generic;
using System.Text;
using roleplay.Entities;

namespace roleplay.Items.ItemType
{
    public class Balaclava : Entities.Item
    {
        public override bool Use(Player player)
        {
            if(!base.Use(player))
            {
                return false;
            }

            if(player.IsUsingItemOfType(roleplay.ItemType.Balaclava))
            {
                player.SendNotification("~r~Masz już na sobie kominiarkę!");
                return false;
            }

            if(isUsed)
            {
                player.SetName(player.character.name);
                isUsed = false;
            }
            else
            {
                player.SetName($"Nieznajomy_{player.character.UID}");
                isUsed = true;
            }

            return true;
        }
    }
}
