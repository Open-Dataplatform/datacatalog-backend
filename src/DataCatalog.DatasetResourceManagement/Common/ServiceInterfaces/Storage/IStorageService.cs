using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataCatalog.DatasetResourceManagement.Common.ServiceInterfaces.Storage
{
    public interface IStorageService
    {
        Task CreateDirectoryIfNeeded(string storageContainer, string path);
        Task SetDirectoryMetadata(string storageContainer, string path, IDictionary<string, string> metadata, string leaseId);
        Task<ILease> AcquireLeaseAsync(string storageContainer, string path = null);
    }
}
