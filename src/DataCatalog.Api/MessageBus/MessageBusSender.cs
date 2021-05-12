using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DataCatalog.Api.Data.Dto;
using Energinet.DataPlatform.Shared.Environments;
using Microsoft.Azure.ServiceBus;

namespace DataCatalog.Api.MessageBus
{
    public class MessageBusSender<T> : IMessageBusSender<T> where T : MessageBusPublishMessage
    {
        readonly ITopicClient _topicClient;

        public MessageBusSender(IEnvironment environment)
        {
            var serviceBusNamespace = $"dpdomainevents-servicebus-{environment.Name.ToLower()}";
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
