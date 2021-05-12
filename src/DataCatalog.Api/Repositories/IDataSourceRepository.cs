using DataCatalog.Api.Data.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataCatalog.Api.Enums;

namespace DataCatalog.Api.Repositories
{
    public interface IDataSourceRepository
    {
        Task<IEnumerable<DataSource>> ListAsync();
        Task AddAsync(DataSource dataSource);
        Task<DataSource> FindByIdAsync(Guid id);
        Task<bool> AnyAsync(IEnumerable<Guid> ids, IEnumerable<SourceType> sourceTypes);
        void Update(DataSource dataSource);
        void Remove(DataSource dataSource);
    }
}
