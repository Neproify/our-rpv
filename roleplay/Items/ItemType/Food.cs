using roleplay.Entities;

namespace roleplay.Items.ItemType
{
    public class Food : Item
    {
        public int healthToRegenerate
        {
            get
            {
                if (!properties.ContainsKey("healthtoregenerate"))
                    return 0;

                if(properties["healthtoregenerate"] is int)
                {
                    return (int)properties["healthtoregenerate"];
                }

                return int.Parse((string)properties["healthtoregenerate"]);
            }
            set
            {
                properties["healthtoregenerate"] = value;
            }
        }

        public override bool Use(Player player)
        {
            if (!base.Use(player))
                return false;

            player.health = player.health + healthToRegenerate;

            return true;
        }
    }
}
