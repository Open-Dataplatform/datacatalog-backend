using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Storage.Files.DataLake;
using DataCatalog.Api.Repositories;
using DataCatalog.Common.Enums;
using Microsoft.Extensions.Logging;
using Polly;

namespace DataCatalog.Api.Services.Storage
{
    public class AzureStorageService : IStorageService
    {
        private readonly DataLakeServiceClient _dataLakeServiceClient;
        private readonly ILogger<AzureStorageService> _logger;
        private readonly IDatasetRepository _datasetRepository;

        public AzureStorageService(
            DataLakeServiceClient dataLakeServiceClient,
            ILogger<AzureStorageService> logger,
            IDatasetRepository datasetRepository)
        {
            _dataLakeServiceClient =
                dataLakeServiceClient ?? throw new ArgumentNullException(nameof(dataLakeServiceClient));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _datasetRepository = datasetRepository;
        }

        public async Task<IDictionary<string,string>> GetDirectoryMetadataWithRetry(Guid datasetId)
        {
            try
            {
                // Use Polly to wait and retry if dataset provisioning status is Pending.
                // This is relevant because dataset creation happens asynchronously and the meta data will not be availble until this process is finished
                var provisionStatus = await Policy
                    .HandleResult<ProvisionDatasetStatusEnum?>((status) => status.HasValue && status.Value == ProvisionDatasetStatusEnum.Pending)
                    .WaitAndRetryAsync(4, retryAttempt => TimeSpan.FromSeconds(1))
                    .ExecuteAsync(() => _datasetRepository.GetProvisioningStatusAsync(datasetId));

                return provisionStatus switch
                {
                    ProvisionDatasetStatusEnum.Pending => null,
                    ProvisionDatasetStatusEnum.Failed => throw new Exception("Dataset was not provisioned correctly"),
                    _ => await GetDirectoryMetadataAsync(datasetId.ToString())
                };
            }
            catch (Azure.RequestFailedException rfe)
            {
                _logger.LogWarning(rfe, "Failed to request metadata for directory {Path}", datasetId);
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected error occurred when loading metadata properties for the path {Path}", datasetId);
                throw;
            }
        }

        private async Task<IDictionary<string,string>> GetDirectoryMetadataAsync(string path)
        {
            var fileSystemClient = _dataLakeServiceClient.GetFileSystemClient("datasets");
            var directoryClient = fileSystemClient.GetDirectoryClient(path);

            var directoryProperties = await directoryClient.GetPropertiesAsync();
            
            return directoryProperties.Value.Metadata;
        }
    }
}
