namespace roleplay.Items.ItemType
{
    public class Document : Entities.Item
    {
        public override void Use(Entities.Player player)
        {
            base.Use(player);

            if(properties[0] == (int)DocumentType.Personal)
            {
                player.OutputMe("pokazuje dowód osobisty.");
            }
            if(properties[0] == (int)DocumentType.VehicleLicense)
            {
                player.OutputMe("pokazuje prawo jazdy.");
            }
        }
    }
}
