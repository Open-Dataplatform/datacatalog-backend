using DataCatalog.Api.Data.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataCatalog.Api.Services
{
    public interface IDurationService
    {
        Task<IEnumerable<Duration>> ListAsync();
        Task<Duration> FindByIdAsync(Guid id);
        Task SaveAsync(Duration duration);
        Task UpdateAsync(Duration duration);
        Task DeleteAsync(Guid id);
    }
}
