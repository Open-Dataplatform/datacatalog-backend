using System;
using System.Threading;
using System.Threading.Tasks;
using DataCatalog.Api.Data.Domain;
using DataCatalog.Api.Data.Dto;
using DataCatalog.Api.Enums;
using DataCatalog.Api.MessageBus;
using DataCatalog.Api.Repositories;
using DataCatalog.Api.Utils;
using Microsoft.Extensions.Hosting;

namespace DataCatalog.Api.Services.Local
{
    /// <summary>
    /// Dummy Group service implementation used ONLY for the local environment runtime.
    /// DO NOT use this in any other context! 
    /// </summary>
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
            if (!EnvironmentUtil.IsLocal())
            {
                throw new InvalidOperationException("This class cannot be used unless the environment is local");
            }
            // Just assume the data is provisioned correctly - locally we have no clue anyways about data.
            if (message is DatasetCreated datasetCreatedMessage)
            {
                await _datasetRepository.UpdateProvisioningStatusAsync(Guid.Parse(datasetCreatedMessage.DatasetId), ProvisionDatasetStatusEnum.Succeeded);
                await _unitOfWork.CompleteAsync();    
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (!EnvironmentUtil.IsLocal())
            {
                throw new InvalidOperationException("This class cannot be used unless the environment is local");
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (!EnvironmentUtil.IsLocal())
            {
                throw new InvalidOperationException("This class cannot be used unless the environment is local");
            }
            return Task.CompletedTask;
        }
    }
}