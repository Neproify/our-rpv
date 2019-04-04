namespace roleplay.Items.ItemType
{
    public class Document : Entities.Item
    {
        public override bool Use(Entities.Player player)
        {
            if (!base.Use(player))
                return false;

            if(properties[0] == (int)DocumentType.Personal)
            {
                player.OutputMe("pokazuje dowód osobisty.");
            }

            if(properties[0] == (int)DocumentType.VehicleLicense)
            {
                player.OutputMe("pokazuje prawo jazdy.");
            }

            return true;
        }
    }
}
