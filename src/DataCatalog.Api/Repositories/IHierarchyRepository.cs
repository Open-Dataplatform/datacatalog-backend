using DataCatalog.Api.Data.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataCatalog.Api.Repositories
{
    public interface IHierarchyRepository
    {
        Task<IEnumerable<Hierarchy>> ListAsync();
        Task AddAsync(Hierarchy hierarchy);
        Task<Hierarchy> FindByIdAsync(Guid id);
        void Update(Hierarchy hierarchy);
        void Remove(Hierarchy hierarchy);
    }
}
