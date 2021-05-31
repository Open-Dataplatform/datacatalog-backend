using DataCatalog.Data.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataCatalog.Api.Repositories
{
    public interface IDataContractRepository
    {
        Task<IEnumerable<DataContract>> ListAsync();
        Task AddAsync(DataContract category);
        Task<DataContract> FindByIdAsync(Guid id);
        Task<IEnumerable<DataContract>> GetByDatasetIdAsync(Guid datasetId);
        Task<IEnumerable<DataContract>> GetByDataSourceIdAsync(Guid dataSourceId);
        void Update(DataContract category);
        void Remove(DataContract category);
    }
}
