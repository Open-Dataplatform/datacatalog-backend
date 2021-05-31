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
    public class DataContractRepository : BaseRepository, IDataContractRepository
    {
        public DataContractRepository(DataCatalogContext context, Current current) : base(context, current)
        { }

        public async Task<IEnumerable<DataContract>> ListAsync()
        {
            var query = _context.DataContracts.Include(a => a.Dataset).AsQueryable();

            if (!_current.Roles.Contains(Role.Admin))
                query = query.Where(a => !a.OriginDeleted);

            return await query.ToListAsync();
        }
        public async Task AddAsync(DataContract category)
        {
            await _context.DataContracts.AddAsync(category);
        }

        public async Task<DataContract> FindByIdAsync(Guid id)
        {
            var query = _context.DataContracts.Include(a => a.Dataset).AsQueryable();

            if (!_current.Roles.Contains(Role.Admin))
                query = query.Where(a => !a.OriginDeleted);
            
            return await query.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<DataContract>> GetByDatasetIdAsync(Guid datasetId)
        {
            var query = _context.DataContracts.Include(a => a.Dataset).AsQueryable();

            if (!_current.Roles.Contains(Role.Admin))
                query = query.Where(a => !a.OriginDeleted);

            return await query.Where(a => a.DatasetId == datasetId).ToListAsync();
        }

        public async Task<IEnumerable<DataContract>> GetByDataSourceIdAsync(Guid dataSourceId)
        {
            var query = _context.DataContracts.Include(a => a.Dataset).AsQueryable();

            if (!_current.Roles.Contains(Role.Admin))
                query = query.Where(a => !a.OriginDeleted);

            return await query.Where(a => a.DataSourceId == dataSourceId).ToListAsync();
        }

        public void Update(DataContract category)
        {
            _context.DataContracts.Update(category);
        }

        public void Remove(DataContract category)
        {
            _context.DataContracts.Remove(category);
        }
    }
}
