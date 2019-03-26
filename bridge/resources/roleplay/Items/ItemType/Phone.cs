using roleplay.Entities;

namespace roleplay.Items.ItemType
{
    public class Phone : Item
    {
        public override void Use(Player player)
        {
            base.Use(player);

            if(player.phoneCall != null)
            {
                player.handle.SendNotification("~r~Posiadasz aktywną rozmowę telefoniczną.");
                return;
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
