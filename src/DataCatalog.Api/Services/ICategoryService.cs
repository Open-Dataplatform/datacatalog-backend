using DataCatalog.Api.Data.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataCatalog.Api.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> ListAsync(bool includeEmpty);
        Task<Category> FindByIdAsync(Guid id);
        Task<Category> SaveAsync(Category category);
        Task<Category> UpdateAsync(Category category);
        Task DeleteAsync(Guid id);
    }
}
