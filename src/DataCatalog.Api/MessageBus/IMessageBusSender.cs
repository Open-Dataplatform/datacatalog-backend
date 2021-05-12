using System.Threading.Tasks;
using DataCatalog.Api.Data.Dto;

namespace DataCatalog.Api.MessageBus
{
    public interface IMessageBusSender<in T> where T : MessageBusPublishMessage
    {
        Task PublishAsync(T message, string topicName);
    }
}
