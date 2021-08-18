using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataCatalog.Api.Services.Storage
{
    public interface IStorageService
    {
        Task<IDictionary<string, string>> GetDirectoryMetadataWithRetry(Guid datasetId);
    }
}