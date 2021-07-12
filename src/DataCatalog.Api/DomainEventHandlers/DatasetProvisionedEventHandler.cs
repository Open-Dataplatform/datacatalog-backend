using System.Threading;
using System.Threading.Tasks;
using DataCatalog.Api.DomainEvents;
using DataCatalog.Api.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace DataCatalog.Api.DomainEventHandlers
{
    public class DatasetProvisionedEventHandler : INotificationHandler<DatasetProvisionedEvent>
    {
        private readonly IDatasetRepository _datasetRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DatasetProvisionedEventHandler> _logger;

        public DatasetProvisionedEventHandler(IDatasetRepository datasetRepository, IUnitOfWork unitOfWork, ILogger<DatasetProvisionedEventHandler> logger)
        {
            _datasetRepository = datasetRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(DatasetProvisionedEvent datasetProvisionedEvent, CancellationToken cancellationToken)
        {
            using (LogContext.PushProperty("DatasetProvisionedEvent", datasetProvisionedEvent, true))
            {
                _logger.LogInformation("Updating the provisionedStatus for dataset id {DatasetID} to {Status}", datasetProvisionedEvent.DatasetId, datasetProvisionedEvent.Status);
            }
            await _datasetRepository.UpdateProvisioningStatusAsync(datasetProvisionedEvent.DatasetId, datasetProvisionedEvent.Status);
            await _unitOfWork.CompleteAsync();
        }
    }
}