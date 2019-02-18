using System;
using System.Collections.Generic;
using System.Text;
using roleplay.Entities;

namespace roleplay.Items.ItemType
{
    public class Phone : Entities.Item
    {
        public override void Use(Player player)
        {
            base.Use(player);


            if(player.activePhone == this)
            {
                player.activePhone = null;
                this.isUsed = false;
            }
            else
            {
                if(player.activePhone != null)
                    player.activePhone.Use(player);

                player.activePhone = this;
                this.isUsed = true;
            }
        }
    }

    public class PhoneCall
    {
        public int senderPhone = 0;
        public Entities.Player sender = null;
        public int receiverPhone = 0;
        public Entities.Player receiver = null;
        public bool active = false;
    }
}
