using System.Threading;
using System.Threading.Tasks;
using DataCatalog.Api.DomainEvents;
using DataCatalog.Common.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace DataCatalog.Api.Services.Local
{
    public class LocalDatasetCreatedEventHandler : INotificationHandler<DatasetCreatedEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<LocalDatasetCreatedEventHandler> _logger;

        public LocalDatasetCreatedEventHandler(IMediator mediator, ILogger<LocalDatasetCreatedEventHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public Task Handle(DatasetCreatedEvent datasetCreatedEvent, CancellationToken cancellationToken)
        {
            using (LogContext.PushProperty("DatasetCreatedEvent", datasetCreatedEvent, true))
            {
                _logger.LogInformation(
                    "Skipping DRM and assume success on provisioning for demo purposes. Creating new provisioned event");
            }

            return _mediator.Publish(new DatasetProvisionedEvent
            {
                DatasetId = datasetCreatedEvent.DatasetId,
                Status = ProvisionDatasetStatusEnum.Succeeded
            }, cancellationToken);
        }
    }
}