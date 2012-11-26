using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Configuration;

namespace WebSitesWithOnPremIntegration.Core
{
    public class ServiceBusHelper
    {
        const string TOPIC_NAME = "Customers";
        const string SUBSCRIPTION_NAME = "BasicSubscription";
        const string SERVICE_BUS_CONNECTION_STRING = "Microsoft.ServiceBus.ConnectionString";

        string _connectionString;
        TopicClient _client;
        NamespaceManager _namespaceManger;

        private ServiceBusHelper()
        {
        }

        private ServiceBusHelper CreateNamespaceManager()
        {
            if (_namespaceManger == null)
                _namespaceManger = NamespaceManager.CreateFromConnectionString(_connectionString);

            return this;
        }

        private ServiceBusHelper CreateTopicIfNotExists()
        {
            if (!_namespaceManger.TopicExists(TOPIC_NAME))
                _namespaceManger.CreateTopic(TOPIC_NAME);

            return this;
        }

        public static ServiceBusHelper Setup()
        {
            return new ServiceBusHelper
            {
                _connectionString =
                    ConfigurationManager.AppSettings[SERVICE_BUS_CONNECTION_STRING]
            }
            .CreateNamespaceManager()
            .CreateTopicIfNotExists();
        }

        public ServiceBusHelper Publish<T>(T target)
        {
            if (_client == null)
                _client = TopicClient.CreateFromConnectionString(_connectionString, TOPIC_NAME);

            _client.Send(new BrokeredMessage(target));

            return this;
        }

        public void Subscribe<T>(Action<T> callback)
        {
            if (!_namespaceManger.SubscriptionExists(TOPIC_NAME, SUBSCRIPTION_NAME))
                _namespaceManger.CreateSubscription(TOPIC_NAME, SUBSCRIPTION_NAME);

            var subscriptionClient = SubscriptionClient.CreateFromConnectionString(
                _connectionString,
                TOPIC_NAME,
                SUBSCRIPTION_NAME);

            while (true)
            {
                var message = 
                    subscriptionClient.Receive(TimeSpan.FromSeconds(3));

                if (message != null)
                {
                    try
                    {
                        if (callback != null)
                            callback(message.GetBody<T>());

                        message.Complete();
                    }
                    catch
                    {
                        message.Abandon();
                    }
                }
            }
        }
    }
}
