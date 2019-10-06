using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Persistence.Entities;

namespace MessageBroker.Interfaces
{
    public interface IMessageService
    {
        Task<IEnumerable<string>> GetByTopicName(string topic);
        Task Create(Message message);
    }
}