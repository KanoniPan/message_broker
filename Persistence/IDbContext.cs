using MongoDB.Driver;
using Persistence.Entities;

namespace Persistence
{
    public interface IDbContext
    {
        IMongoCollection<Subscription> Subscriptions { get; }
        IMongoCollection<Message> Messages { get; }
    }
}