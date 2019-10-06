using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MessageBroker.Interfaces;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;
using Persistence.Entities;
using Serilog;

namespace MessageBroker.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IMessageService _messageService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IMessageEnricher _messageEnricher;
        private readonly IContentBasedRouter _contentBasedRouter;
        public ChatHub(IMessageService messageService, ISubscriptionService subscriptionService, IContentBasedRouter contentBasedRouter, IMessageEnricher messageEnricher)
        {
            _messageService = messageService;
            _subscriptionService = subscriptionService;
            _contentBasedRouter = contentBasedRouter;
            _messageEnricher = messageEnricher;
        }
        
        
        
        public async Task SendMessage(string message, string topic)
        {
            var translatedMessage = _messageEnricher.ProcessMessage(message);
            Log.Information("{0} send message was \"{1}\", it was translated into \"{2}\" to topic \"{3}\"", Context.ConnectionId, message, translatedMessage, topic);

            await _messageService.Create(new Message()
            {
                Text = translatedMessage,
                Topic = topic
            });
            
            var topicList = new List<string> { topic };
            var additionalTopics = _contentBasedRouter.AdditionalRoutes(message);

            topicList.AddRange(additionalTopics);
            Task.WaitAll(
                topicList
                    .Select(t => Clients.Group(t).SendAsync("message", translatedMessage))
                    .ToArray()
            );
        }

        public async Task Subscribe(string topic)
        {
            var subscription = await _subscriptionService.Get(Context.ConnectionId);

            if (subscription != null)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, subscription.Topic);
                await _subscriptionService.Delete(Context.ConnectionId);
            }

            await _subscriptionService.Add(new Subscription
            {
                Topic = topic,
                SubscriptionId = Context.ConnectionId
            });

            await Groups.AddToGroupAsync(Context.ConnectionId, topic);

            var messages = await _messageService.GetByTopicName(topic);
            await Clients.Caller.SendAsync("connect", messages.ToJson());
        }
        
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var subscription = await _subscriptionService.Get(Context.ConnectionId);

            await _subscriptionService.Delete(Context.ConnectionId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, subscription.Topic);
        }
        
    }
}