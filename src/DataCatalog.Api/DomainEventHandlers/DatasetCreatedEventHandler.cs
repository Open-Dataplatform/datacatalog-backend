using System.Threading;
using System.Threading.Tasks;
using DataCatalog.Api.DomainEvents;
using DataCatalog.Api.Messages;
using MediatR;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
using Serilog.Context;

namespace DataCatalog.Api.DomainEventHandlers
{
    public class DatasetCreatedEventHandler : INotificationHandler<DatasetCreatedEvent>
    {
        private readonly ILogger<DatasetCreatedEventHandler> _logger;
        private readonly IBus _bus;

        public DatasetCreatedEventHandler(IBus bus, ILogger<DatasetCreatedEventHandler> logger)
        {
            _bus = bus;
            _logger = logger;
        }

        public Task Handle(DatasetCreatedEvent datasetCreatedEvent, CancellationToken cancellationToken)
        {
            using (LogContext.PushProperty("DatasetCreatedEvent", datasetCreatedEvent, true))
            {
                _logger.LogInformation("Publishing a new new DatasetCreatedMessage based on event on dataset id {DatasetID}", datasetCreatedEvent.DatasetId);
            }
            return _bus.Publish(new DatasetCreatedMessage
            {
                DatasetId = datasetCreatedEvent.DatasetId,
                Container = datasetCreatedEvent.Container,
                DatasetName = datasetCreatedEvent.DatasetName,
                Owner = datasetCreatedEvent.Owner,
                Hierarchy = datasetCreatedEvent.Hierarchy,
                Public = datasetCreatedEvent.Public
            });
        }
    }
}