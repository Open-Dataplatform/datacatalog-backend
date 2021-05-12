using DataCatalog.Api.Data.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataCatalog.Api.Services
{
    public interface IDatasetGroupService
    {
        Task<IEnumerable<DatasetGroup>> ListAsync();
        Task<DatasetGroup> FindByIdAsync(Guid id);
        Task<DatasetGroup> SaveAsync(DatasetGroup datasetGroup);
        Task<DatasetGroup> UpdateAsync(DatasetGroup datasetGroup);
        Task DeleteAsync(Guid id);
    }
}
