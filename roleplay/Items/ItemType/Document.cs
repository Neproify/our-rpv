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
                if (!properties.ContainsKey("documenttype"))
                    return DocumentType.None;

                if (properties["documenttype"] is int)
                    return (DocumentType)properties["documenttype"];

                return DocumentType.None;
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
                if (!properties.ContainsKey("personid"))
                    return ObjectId.Empty;

                if (properties["personid"] is ObjectId)
                    return (ObjectId)properties["personid"];

                return ObjectId.Parse((string)properties["personid"]);
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
