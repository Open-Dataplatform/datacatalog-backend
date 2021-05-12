using DataCatalog.Api.Data.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataCatalog.Api.Repositories
{
    public interface IDurationRepository
    {
        Task<IEnumerable<Duration>> ListAsync();
        Task AddAsync(Duration duration);
        Task<Duration> FindByIdAsync(Guid id);
        Task<Duration> FindByCodeAsync(string code);
        void Update(Duration duration);
        void Remove(Duration duration);
    }
}
