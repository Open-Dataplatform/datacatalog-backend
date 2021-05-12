using System.Threading.Tasks;

namespace DataCatalog.Api.Services
{
    public interface IHandleMessageBusMessage
    {
        Task HandleMessage(string messageJson);
    }
}
