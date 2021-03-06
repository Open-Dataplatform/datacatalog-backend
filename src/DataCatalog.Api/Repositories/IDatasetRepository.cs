using DataCatalog.Data.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataCatalog.Common.Enums;

namespace DataCatalog.Api.Repositories
{
    public interface IDatasetRepository
    {
        Task<Dataset> FindByIdAsync(Guid id);
        Task<IEnumerable<Dataset>> ListAllNonDeletedDatasets();
        Task<IEnumerable<Dataset>> ListSummariesAsync();
        Task<IEnumerable<Dataset>> GetDatasetByCategoryAsync(Guid categoryId, SortType sortType, int take, int pageSize, int pageIndex);
        Task<IEnumerable<Dataset>> GetDatasetsBySearchTermQueryAsync(string searchTerm, SortType sortType, int take, int pageSize, int pageIndex);
        Task AddAsync(Dataset dataset);
        void Remove(Dataset dataset);
        Task UpdateProvisioningStatusAsync(Guid id, ProvisionDatasetStatusEnum status);
        Task<ProvisionDatasetStatusEnum?> GetProvisioningStatusAsync(Guid id);
    }
}
