using System;
using DataCatalog.Data.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataCatalog.Api.Repositories
{
    public interface IDatasetGroupRepository
    {
        Task<IEnumerable<DatasetGroup>> ListAsync();
        Task AddAsync(DatasetGroup datasetGroup);
        Task<DatasetGroup> FindByIdAsync(Guid id);
        void Update(DatasetGroup datasetGroup);
        void Remove(DatasetGroup datasetGroup);
    }
}
