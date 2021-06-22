using System.Threading.Tasks;
using DataCatalog.Api.Data.Messages;
using DataCatalog.Api.Repositories;
using Rebus.Handlers;

namespace DataCatalog.Api.MessageHandlers
{
    public class DatasetProvisionedHandler : IHandleMessages<DatasetProvisioned>
    {
        private readonly IDatasetRepository _datasetRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DatasetProvisionedHandler(IDatasetRepository datasetRepository, IUnitOfWork unitOfWork)
        {
            _datasetRepository = datasetRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DatasetProvisioned message)
        {
            await _datasetRepository.UpdateProvisioningStatusAsync(message.DatasetId, message.Status);
            await _unitOfWork.CompleteAsync();
        }
    }
}