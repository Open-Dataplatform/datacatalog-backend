using System;
using System.Threading.Tasks;
using DataCatalog.Api.Repositories;
using DataCatalog.Common.Enums;
using DataCatalog.Common.Rebus;
using DataCatalog.DatasetResourceManagement.Messages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rebus.Bus;
using Rebus.Handlers;

namespace DataCatalog.Api.MessageHandlers
{
    public class DatasetProvisionedHandler : AbstractMessageHandler<DatasetProvisionedMessage>
    {
        private readonly IDatasetRepository _datasetRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DatasetProvisionedHandler(IDatasetRepository datasetRepository, IUnitOfWork unitOfWork, IBus bus, IOptions<RebusOptions> rebusOptions, ILogger<DatasetProvisionedHandler> logger)
            :base(logger, rebusOptions, bus)
        {
            _datasetRepository = datasetRepository;
            _unitOfWork = unitOfWork;
        }

        public override async Task Handle(DatasetProvisionedMessage message)
        {
            var parseSuccess = Enum.TryParse(message.Status, out ProvisionDatasetStatusEnum status);
            if (!parseSuccess)
            {
                throw new Exception($"Could not parse status to a ProvisionDatasetStatusEnum in field 'Status' with the value: {message.Status}");
            }
            await _datasetRepository.UpdateProvisioningStatusAsync(message.DatasetId, status);
            await _unitOfWork.CompleteAsync();
        }
    }
}