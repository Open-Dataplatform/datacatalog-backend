using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Storage.Files.DataLake;
using Azure.Storage.Files.DataLake.Models;
using DataCatalog.DatasetResourceManagement.Common.ServiceInterfaces.Storage;
using Microsoft.Extensions.Logging;

namespace DataCatalog.DatasetResourceManagement.Services.Storage
{
    public class StorageService : IStorageService
    {
        private readonly DataLakeServiceClient _dataLakeServiceClient;
        private readonly ILeaseClientProvider _leaseClientProvider;
        private readonly ILogger<StorageService> _logger;

        public StorageService(
            ILogger<StorageService> logger, 
            DataLakeServiceClient dataLakeServiceClient,
            ILeaseClientProvider leaseClientProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dataLakeServiceClient = dataLakeServiceClient ?? throw new ArgumentNullException(nameof(dataLakeServiceClient));
            _leaseClientProvider = leaseClientProvider ?? throw new ArgumentNullException(nameof(dataLakeServiceClient)); ;
        }

        public async Task CreateDirectoryIfNeeded(string storageContainer, string path)
        {
            var fileSystemClient = _dataLakeServiceClient.GetFileSystemClient(storageContainer.ToLower());
            var dirClient = fileSystemClient.GetDirectoryClient(path);

            _logger.LogInformation("Creating: {StorageContainer}/{Path} if needed", storageContainer, path);
            await dirClient.CreateIfNotExistsAsync();
        }

        public async Task<ILease> AcquireLeaseAsync(string storageContainer, string path = null)
        {
            var fileSystemClient = _dataLakeServiceClient.GetFileSystemClient(storageContainer.ToLower());

            DataLakeLeaseClient leaseClient;
            
            if (path == null)
                leaseClient = _leaseClientProvider.ProvideDataLakeLeaseClient(fileSystemClient);

            else
            {
                var directoryClient = fileSystemClient.GetDirectoryClient(path);
                leaseClient = _leaseClientProvider.ProvideDataLakeLeaseClient(directoryClient);
            }

            await leaseClient.AcquireAsync(TimeSpan.FromSeconds(50));

            return new Lease(leaseClient);
        }

        public Task SetDirectoryMetadata(string storageContainer, string path, IDictionary<string, string> metadata, string leaseId)
        {
            var fileSystemClient = _dataLakeServiceClient.GetFileSystemClient(storageContainer.ToLower());
            var directoryClient = fileSystemClient.GetDirectoryClient(path);

            return directoryClient.SetMetadataAsync(metadata,new DataLakeRequestConditions {LeaseId = leaseId });
        }
    }
}
