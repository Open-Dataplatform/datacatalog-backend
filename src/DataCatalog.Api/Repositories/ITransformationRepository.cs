using DataCatalog.Api.Data.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataCatalog.Api.Repositories
{
    public interface ITransformationRepository
    {
        Task<IEnumerable<Transformation>> ListAsync();
        Task AddAsync(Transformation transformation);
        Task<Transformation> FindByIdAsync(Guid id);
        Task<IEnumerable<Transformation>> GetByDatasetIdsAsync(Guid[] datasetIds);
        void Update(Transformation transformation);
        Task RemoveAsync(Transformation transformation);
    }
}
