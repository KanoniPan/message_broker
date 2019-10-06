using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Persistence.Entities
{
    public class Subscription
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string SubscriptionId { get; set; }
        public string Topic { get; set; }
    }
}