using System.Threading.Tasks;
using Persistence.Entities;

namespace MessageBroker.Interfaces
{
    public interface ISubscriptionService
    {
        Task<Subscription> Get(string id);
        Task Add(Subscription subscription);
        Task Delete(string id);
    }
}