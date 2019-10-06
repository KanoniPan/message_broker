using System.Collections.Generic;

namespace MessageBroker.Interfaces
{
    public interface IContentBasedRouter
    {
        IEnumerable<string> AdditionalRoutes(string message);
    }
}