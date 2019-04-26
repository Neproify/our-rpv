using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace roleplay.Items.ItemType
{
    public class Document : Entities.Item
    {
        [BsonIgnore]
        public DocumentType documentType
        {
            get
            {
                return (DocumentType)properties["documenttype"];
            }
            set
            {
                properties["documenttype"] = value;
            }
        }

        [BsonIgnore]
        public ObjectId personID
        {
            get
            {
                return (ObjectId)properties["personid"];
            }
            set
            {
                properties["personid"] = value;
            }
        }

        public override bool Use(Entities.Player player)
        {
            if (!base.Use(player))
                return false;

            if(documentType == DocumentType.Personal)
            {
                player.OutputMe("pokazuje dowód osobisty.");
            }

            if(documentType == DocumentType.VehicleLicense)
            {
                player.OutputMe("pokazuje prawo jazdy.");
            }

            return true;
        }
    }
}
