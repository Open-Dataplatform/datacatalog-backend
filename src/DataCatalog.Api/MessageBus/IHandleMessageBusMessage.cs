using System.Threading.Tasks;

namespace DataCatalog.Api.MessageBus
{
    public interface IHandleMessageBusMessage
    {
        Task HandleMessage(string messageJson);
    }
}
