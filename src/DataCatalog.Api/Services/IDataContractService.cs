using DataCatalog.Api.Data.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataCatalog.Api.Services
{
    public interface IDataContractService
    {
        Task<IEnumerable<DataContract>> ListAsync();
        Task<DataContract> FindByIdAsync(Guid id);
        Task<DataContract[]> GetByDatasetIdAsync(Guid datasetId);
        Task<DataContract[]> GetByDataSourceIdAsync(Guid dataSourceId);
        Task SaveAsync(DataContract category);
        Task UpdateAsync(DataContract category);
        Task DeleteAsync(Guid id);
    }
}
