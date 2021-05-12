using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DataCatalog.Api.Data.Dto;
using DataCatalog.Api.Utils;
using Microsoft.Azure.ServiceBus;

namespace DataCatalog.Api.MessageBus
{
    public class MessageBusSender<T> : IMessageBusSender<T> where T : MessageBusPublishMessage
    {
        private readonly ITopicClient _topicClient;

        public MessageBusSender()
        {
            var serviceBusNamespace = $"dpdomainevents-servicebus-{EnvironmentUtil.GetCurrentEnvironment().ToLower()}";
            var serviceBusEndpoint = $"Endpoint=sb://{serviceBusNamespace}.servicebus.windows.net/;Authentication=Managed Identity;";
            _topicClient = new TopicClient(serviceBusEndpoint, typeof(T).Name, RetryPolicy.Default);
        }

        public async Task PublishAsync(T message, string topicName)
        {
            await _topicClient.SendAsync(new Message(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message)))
            {
                ContentType = "application/json",
                MessageId = Guid.NewGuid().ToString()
            });
        }
    }
}
