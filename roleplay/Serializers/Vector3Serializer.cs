using GTANetworkAPI;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace roleplay.Serializers
{
    class Vector3Serializer : SerializerBase<Vector3>
    {
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Vector3 value)
        {
            context.Writer.WriteStartDocument();

            context.Writer.WriteName("x");
            context.Writer.WriteDouble((double)value.X);
            context.Writer.WriteName("y");
            context.Writer.WriteDouble((double)value.Y);
            context.Writer.WriteName("z");
            context.Writer.WriteDouble((double) value.Z);

            context.Writer.WriteEndDocument();
        }

        public override Vector3 Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var serializer = BsonSerializer.LookupSerializer(typeof(BsonDocument));
            var document = serializer.Deserialize(context, args).ToBsonDocument();

            return new Vector3(document["x"].ToDouble(), document["y"].ToDouble(), document["z"].ToDouble());
        }
    }
}
