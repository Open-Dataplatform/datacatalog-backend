using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using DataCatalog.Api.Data.Dto;
using DataCatalog.Common.Extensions;
using DataCatalog.Common.Utils;
using Microsoft.Extensions.Configuration;

namespace DataCatalog.Api.MessageBus
{
    public class MessageBusSender<T> : IMessageBusSender<T> where T : MessageBusPublishMessage
    {
        private readonly ServiceBusClient _serviceBusClient;

        public MessageBusSender(IConfiguration configuration)
        {
            var conn = configuration.GetConnectionString("ServiceBus");
            conn.ValidateConfiguration("ConnectionStrings:ServiceBus");

            _serviceBusClient = new ServiceBusClient(conn);
        }

        public async Task PublishAsync(T message, string topicName)
        {
            var sender = _serviceBusClient.CreateSender(topicName);

            await sender.SendMessageAsync(new ServiceBusMessage(JsonSerializer.Serialize(message))
            {
                ContentType = "application/json",
                MessageId = Guid.NewGuid().ToString()
            });
        }
    }
}
