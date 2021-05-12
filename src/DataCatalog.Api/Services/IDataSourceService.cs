using DataCatalog.Api.Data.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataCatalog.Api.Services
{
    public interface IDataSourceService
    {
        Task<IEnumerable<DataSource>> ListAsync();
        Task<DataSource> FindByIdAsync(Guid id);
        Task SaveAsync(DataSource dataSource);
        Task UpdateAsync(DataSource dataSource);
        Task DeleteAsync(Guid id);
    }
}
