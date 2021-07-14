using System;
using System.Threading.Tasks;
using Azure.Storage.Files.DataLake;
using DataCatalog.DatasetResourceManagement.Common.ServiceInterfaces.Storage;

namespace DataCatalog.DatasetResourceManagement.Services.Storage
{
    public class Lease : ILease
    {
        private readonly DataLakeLeaseClient _dataLakeLeaseClient;

        public Lease(DataLakeLeaseClient dataLakeLeaseClient)
        {
            _dataLakeLeaseClient = dataLakeLeaseClient ?? throw new ArgumentNullException(nameof(dataLakeLeaseClient));
        }
        
        public async ValueTask DisposeAsync()
        {
            await _dataLakeLeaseClient.BreakAsync();
            await _dataLakeLeaseClient.ReleaseAsync();
        }

        public string LeaseId => _dataLakeLeaseClient.LeaseId;
    }
}