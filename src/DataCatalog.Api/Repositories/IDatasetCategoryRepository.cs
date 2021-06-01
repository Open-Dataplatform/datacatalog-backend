using DataCatalog.Data.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataCatalog.Api.Repositories
{
    public interface IDatasetCategoryRepository
    {
        Task<IEnumerable<DatasetCategory>> ListAsync();
        Task<IEnumerable<DatasetCategory>> ListAsync(Guid categoryId);
    }
}
