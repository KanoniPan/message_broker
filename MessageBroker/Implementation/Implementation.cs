using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MessageBroker.Interfaces;
using MongoDB.Driver;
using Persistence;
using Persistence.Entities;

namespace MessageBroker.Implementation
{
    public class Implementation : IMessageService, ISubscriptionService, IMessageEnricher, IContentBasedRouter
    {
        private readonly IDbContext _dbContext;
        private readonly Regex _punctuationRegex = new Regex(@"[\s.,!?\-]+");

        private readonly IDictionary<string, string> _dictionary = new Dictionary<string, string>
        {
            {"rus", "russian"},
            {"eng", "english"},
            {"ro", "romanian"},
            {"ok", "okay"},
            {"wp", "well played"}
        };
        
        private readonly IDictionary<string, IEnumerable<string>> _topics = new Dictionary<string, IEnumerable<string>>
        {
            {
                "anime", new List<string>
                {
                    "anime",
                    "one piece",
                    "naruto"
                }
            }
        };


        public Implementation(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<string>> GetByTopicName(string topic)
        {
            return await _dbContext.Messages
                .Find(message => message.Topic == topic)
                .Project(message => message.Text)
                .ToListAsync();
        }

        public async Task Create(Message message)
        {
            await _dbContext.Messages.InsertOneAsync(message);
        }

        public async Task<Subscription> Get(string id)
        {
            return await _dbContext.Subscriptions.Find(c => c.SubscriptionId == id).FirstOrDefaultAsync();
        }

        public async Task Add(Subscription subscription)
        {
            await _dbContext.Subscriptions.InsertOneAsync(subscription);
        }

        public async Task Delete(string id)
        {
            await _dbContext.Subscriptions.DeleteOneAsync(c => c.SubscriptionId == id);
        }



        public string ProcessMessage(string message)
        {
            var words = _punctuationRegex.Split(message)
                .Select(it => it.ToLowerInvariant())
                .Distinct()
                .Where(word => _dictionary.ContainsKey(word));

            foreach (var word in words)
            {
                message = message.Replace( word, _dictionary[word]);
            }

            return message;
        }

        public IEnumerable<string> AdditionalRoutes(string message)
        {
            var words = _punctuationRegex
                .Split(message)
                .Select(it => it)
                .Distinct();

            return _topics
                .Where(topic => words.Any(word => topic.Value.Contains(word)))
                .Select(topic => topic.Key);
        }
    }
}