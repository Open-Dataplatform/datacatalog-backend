using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Storage.Files.DataLake;
using Microsoft.Extensions.Logging;
using Polly;

namespace DataCatalog.Api.Services.Storage
{
    public class AzureStorageService : IStorageService
    {
        private readonly DataLakeServiceClient _dataLakeServiceClient;
        private readonly ILogger<AzureStorageService> _logger;

        public AzureStorageService(
            DataLakeServiceClient dataLakeServiceClient, 
            ILogger<AzureStorageService> logger)
        {
            _dataLakeServiceClient =
                dataLakeServiceClient ?? throw new ArgumentNullException(nameof(dataLakeServiceClient));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IDictionary<string,string>> GetDirectoryMetadataWithRetry(string path)
        {
            try
            {
                // Use Polly to wait and retry on RequestFailedExceptions.
                // This is relevant because the meta data is unavailable immediately after creating a new dataset 
                return await Policy
                    .Handle<Azure.RequestFailedException>()
                    .WaitAndRetryAsync(4, retryAttempt => TimeSpan.FromSeconds(1))
                    .ExecuteAsync(() => GetDirectoryMetadataAsync(path));
            }
            catch (Azure.RequestFailedException rfe)
            {
                _logger.LogWarning(rfe, "Failed to request metadata for directory {Path}", path);
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected error occurred when loading metadata properties for the path {Path}", path);
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
