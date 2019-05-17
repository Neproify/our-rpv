using MongoDB.Bson.Serialization.Attributes;
using roleplay.Entities;

namespace roleplay.Items.ItemType
{
    public class Phone : Item
    {
        [BsonIgnore]
        public int phoneNumber
        {
            get
            {
                if (!properties.ContainsKey("phonenumber"))
                    return 0;

                if (properties["phonenumber"] is int)
                    return (int)properties["phonenumber"];

                return int.Parse((string)properties["phonenumber"]);
            }
            set
            {
                properties["phonenumber"] = value;
            }
        }

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
