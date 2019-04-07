using roleplay.Entities;

namespace roleplay.Items.ItemType
{
    public class Phone : Item
    {
        public override bool Use(Player player)
        {
            if (!base.Use(player))
                return false;

            if (player.phoneCall != null)
            {
                player.SendNotification("~r~Posiadasz aktywną rozmowę telefoniczną.");
                return false;
            }


            if(player.activePhone == this)
            {
                player.activePhone = null;
                isUsed = false;
            }
            else
            {
                player.activePhone?.Use(player);

                player.activePhone = this;
                isUsed = true;
            }

            return true;
        }
    }

    public class PhoneCall
    {
        public int senderPhone = 0;
        public Player sender = null;
        public int receiverPhone = 0;
        public Player receiver = null;
        public bool active = false;
    }
}
