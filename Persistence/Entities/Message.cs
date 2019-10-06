using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Persistence.Entities
{
    public class Message
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string MessageId { get; set; }
        public string Topic { get; set; }
        public string Text { get; set; }
    }
}