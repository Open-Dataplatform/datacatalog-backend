using DataCatalog.Api.Data.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataCatalog.Api.Services
{
    public interface IHierarchyService
    {
        Task<IEnumerable<Hierarchy>> ListAsync();
        Task<Hierarchy> FindByIdAsync(Guid id);
        Task SaveAsync(Hierarchy hierarchy);
        Task UpdateAsync(Hierarchy hierarchy);
        Task DeleteAsync(Guid id);
    }
}
