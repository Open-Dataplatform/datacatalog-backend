
using DataCatalog.Common.Data;
using DataCatalog.Data.Model;
using DataCatalog.Common.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataCatalog.Data;

namespace DataCatalog.Api.Repositories
{
    public class DataSourceRepository : BaseRepository, IDataSourceRepository
    {
        public DataSourceRepository(DataCatalogContext context, Current current) : base(context, current)
        { }

        public async Task<IEnumerable<DataSource>> ListAsync()
        {
            var dataSources = await _context.DataSources.ToListAsync();

            if (_current.Roles.Contains(Role.Admin))
                return dataSources;

            return dataSources.Where(c => !c.OriginDeleted).ToList();
        }
        public async Task AddAsync(DataSource dataSource)
        {
            await _context.DataSources.AddAsync(dataSource);
        }

        public async Task<DataSource> FindByIdAsync(Guid id)
        {
            var existingDataSource = await _context.DataSources.FirstOrDefaultAsync(a => a.Id == id);

            if (_current.Roles.Contains(Role.Admin))
                return existingDataSource;

            if (existingDataSource == null)
                return null;

            return existingDataSource.OriginDeleted
                ? null
                : existingDataSource;
        }

        public async Task<bool> AnyAsync(IEnumerable<Guid> ids, IEnumerable<SourceType> sourceTypes)
        {
            return await _context.DataSources.AnyAsync(a => ids.Contains(a.Id) && sourceTypes.Contains(a.SourceType));
        }

        public void Update(DataSource dataSource)
        {
            _context.DataSources.Update(dataSource);
        }

        public void Remove(DataSource dataSource)
        {
            _context.DataSources.Remove(dataSource);
        }
    }
}
