using DataCatalog.Api.Data.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataCatalog.Api.Services
{
    public interface ITransformationService
    {
        Task<IEnumerable<Transformation>> ListAsync();
        Task<Transformation> FindByIdAsync(Guid id);
        Task<IEnumerable<Transformation>> GetByDatasetIdsAsync(Guid[] datasetIds);
        Task SaveAsync(Transformation transformation);
        Task UpdateAsync(Transformation transformation);
        Task DeleteAsync(Guid id);
    }
}
