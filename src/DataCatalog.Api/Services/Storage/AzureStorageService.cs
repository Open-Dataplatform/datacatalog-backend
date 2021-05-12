using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Storage.Files.DataLake;
using Microsoft.Extensions.Logging;

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

        public async Task<IDictionary<string,string>> GetDirectoryMetadataAsync(string path)
        {
            var fileSystemClient = _dataLakeServiceClient.GetFileSystemClient("datasets");
            var directoryClient = fileSystemClient.GetDirectoryClient(path);

            try
            {
                var directoryProperties = await directoryClient.GetPropertiesAsync();
                return directoryProperties.Value.Metadata;
            }
            catch (Azure.RequestFailedException rfe)
            {
                _logger.LogWarning(rfe, $"Failed to request metadata for directory {path}");
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Enexpected error occurred when loading metadata properties for the path {path}");
                throw;
            }
        }
    }
}
