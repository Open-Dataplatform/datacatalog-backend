using System;
using DataCatalog.Data.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataCatalog.Common.Enums;
using DataCatalog.Data;

namespace DataCatalog.Api.Repositories
{
    public class DatasetDurationRepository : BaseRepository, IDatasetDurationRepository
    {
        public DatasetDurationRepository(DataCatalogContext context) : base(context)
        { }

        public async Task<IEnumerable<DatasetDuration>> ListAsync()
        {
            return await _context.DatasetDurations.ToListAsync();
        }
        
        public async Task<DatasetDuration> FindByDatasetAndTypeAsync(Guid datasetId, DurationType durationType)
        {
            return await _context.DatasetDurations.Include(a => a.Duration).SingleOrDefaultAsync(a => a.DatasetId == datasetId && a.DurationType == durationType);
        }

        public async Task AddAsync(DatasetDuration datasetDuration)
        {
            await _context.DatasetDurations.AddAsync(datasetDuration);
        }

        public void Remove(DatasetDuration datasetDuration)
        {
            _context.DatasetDurations.Remove(datasetDuration);
        }
    }
}
