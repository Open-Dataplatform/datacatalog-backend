using System;
using DataCatalog.Api.Data.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataCatalog.Api.Enums;

namespace DataCatalog.Api.Repositories
{
    public interface IDatasetDurationRepository
    {
        Task<IEnumerable<DatasetDuration>> ListAsync();
        Task<DatasetDuration> FindByDatasetAndTypeAsync(Guid datasetId, DurationType durationType);
        Task AddAsync(DatasetDuration datasetDuration);
        void Remove(DatasetDuration datasetDuration);
    }
}
