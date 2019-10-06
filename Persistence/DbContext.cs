using MongoDB.Driver;
using Persistence.DBSettings;
using Persistence.Entities;

namespace Persistence
{
    public class DbContext : IDbContext
    {
        public IMongoCollection<Subscription> Subscriptions { get; }
        public IMongoCollection<Message> Messages { get; }

        public DbContext(IDBSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var db = client.GetDatabase(settings.DatabaseName);

            Subscriptions = db.GetCollection<Subscription>(settings.SubscriptionName);
            Messages = db.GetCollection<Message>(settings.MessagesName);
        }
    }
}