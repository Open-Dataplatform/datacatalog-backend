using DataCatalog.Data.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataCatalog.Api.Repositories
{
    public interface IDatasetChangeLogRepository
    {
        Task<IEnumerable<DatasetChangeLog>> ListAsync();
        void Add(DatasetChangeLog datasetChangeLog);
        Task<DatasetChangeLog> FindByIdAsync(Guid id);
        void Update(DatasetChangeLog datasetChangeLog);
        void Remove(DatasetChangeLog datasetChangeLog);
    }
}