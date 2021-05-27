using System;
using System.Threading;
using System.Threading.Tasks;
using DataCatalog.Api.Data.Domain;
using DataCatalog.Api.Data.Dto;
using DataCatalog.Api.Enums;
using DataCatalog.Api.MessageBus;
using DataCatalog.Api.Repositories;
using Microsoft.Extensions.Hosting;

namespace DataCatalog.Api.Services.Local
{
    public class LocalMessageHandler<TMessage> : IHostedService, IMessageBusSender<TMessage> where TMessage : MessageBusPublishMessage
    {
        private readonly IDatasetRepository _datasetRepository;
        private readonly IUnitIOfWork _unitOfWork;

        public LocalMessageHandler(IDatasetRepository datasetRepository, IUnitIOfWork unitOfWork)
        {
            _datasetRepository = datasetRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task PublishAsync(TMessage message, string topicName)
        {
            // Just assume the data is provisioned correctly - locally we have no clue anyways about data.
            if (message is DatasetCreated datasetCreatedMessage)
            {
                await _datasetRepository.UpdateProvisioningStatusAsync(Guid.Parse(datasetCreatedMessage.DatasetId), ProvisionDatasetStatusEnum.Succeeded);
                await _unitOfWork.CompleteAsync();    
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}