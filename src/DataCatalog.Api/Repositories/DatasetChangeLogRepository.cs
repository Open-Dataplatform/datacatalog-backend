using System;
using DataCatalog.Data.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataCatalog.Data;

namespace DataCatalog.Api.Repositories
{
    public class DatasetChangeLogRepository : BaseRepository, IDatasetChangeLogRepository
    {
        public DatasetChangeLogRepository(DataCatalogContext context) : base(context)
        { }

        public async Task<IEnumerable<DatasetChangeLog>> ListAsync()
        {
            return await _context.DatasetChangeLogs.ToListAsync();
        }

        public async Task<DatasetChangeLog> FindByIdAsync(Guid id)
        {
            return await _context.DatasetChangeLogs.FirstOrDefaultAsync(a => a.Id == id);
        }

        public void Add(DatasetChangeLog datasetChangeLog)
        {
            _context.DatasetChangeLogs.Add(datasetChangeLog);
        }

        public void Update(DatasetChangeLog datasetChangeLog)
        {
            _context.DatasetChangeLogs.Update(datasetChangeLog);
        }

        public void Remove(DatasetChangeLog datasetChangeLog)
        {
            _context.DatasetChangeLogs.Remove(datasetChangeLog);
        }
    }
}
