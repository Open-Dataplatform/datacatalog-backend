using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Storage.Files.DataLake;
using DataCatalog.Api.Repositories;
using DataCatalog.Common.Enums;
using Microsoft.Extensions.Logging;
using Polly;
using Serilog.Context;

namespace DataCatalog.Api.Services.Storage
{
    public class AzureStorageService : IStorageService
    {
        private readonly DataLakeServiceClient _dataLakeServiceClient;
        private readonly ILogger<AzureStorageService> _logger;
        private readonly IDatasetRepository _datasetRepository;
        private const string FileSystemName = "datasets";

        public AzureStorageService(
            DataLakeServiceClient dataLakeServiceClient,
            ILogger<AzureStorageService> logger,
            IDatasetRepository datasetRepository)
        {
            _dataLakeServiceClient = dataLakeServiceClient;
            _logger = logger;
            _datasetRepository = datasetRepository;
        }

        public async Task<IDictionary<string,string>> GetDirectoryMetadataWithRetry(Guid datasetId)
        {
            try
            {
                // Use Polly to wait and retry if dataset provisioning status is Pending.
                // This is relevant because dataset creation happens asynchronously and the meta data will not be available until this process is finished
                var provisionStatus = await Policy
                    .HandleResult<ProvisionDatasetStatusEnum?>(status => status is ProvisionDatasetStatusEnum.Pending)
                    .WaitAndRetryAsync(4, _ => TimeSpan.FromSeconds(1))
                    .ExecuteAsync(() =>
                    {
                        _logger.LogDebug("Trying to fetch provisioning status for the datasetId {DatasetId}", datasetId);
                        return _datasetRepository.GetProvisioningStatusAsync(datasetId);
                    });

                return provisionStatus switch
                {
                    ProvisionDatasetStatusEnum.Pending => null,
                    ProvisionDatasetStatusEnum.Failed => throw new Exception("Dataset was not provisioned correctly"),
                    ProvisionDatasetStatusEnum.Succeeded or null => await GetDirectoryMetadataAsync(datasetId.ToString()),
                    _ => throw new Exception("Dataset has unknown provisioning status")
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
            var fileSystemClient = _dataLakeServiceClient.GetFileSystemClient(FileSystemName);
            var directoryClient = fileSystemClient.GetDirectoryClient(path);

            var directoryProperties = await directoryClient.GetPropertiesAsync();

            using (LogContext.PushProperty("DirectoryProperties", directoryProperties, true))
            {
                _logger.LogDebug("Fetched directory properties from the datalake using the path {Path}", path);
            }
            return directoryProperties.Value.Metadata;
        }
    }
}
