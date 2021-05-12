using DataCatalog.Api.Data;
using DataCatalog.Api.Data.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataCatalog.Api.Repositories
{
    public class DatasetGroupRepository : BaseRepository, IDatasetGroupRepository
    {
        public DatasetGroupRepository(DataCatalogContext context) : base(context)
        { }

        public async Task<IEnumerable<DatasetGroup>> ListAsync()
        {
            return await _context.DatasetGroups.Include(a => a.DatasetGroupDatasets).ThenInclude(a => a.Dataset).ToListAsync();
        }
        public async Task AddAsync(DatasetGroup datasetGroup)
        {
            await _context.DatasetGroups.AddAsync(datasetGroup);
        }

        public async Task<DatasetGroup> FindByIdAsync(Guid id)
        {
            return await _context.DatasetGroups.Include(a => a.DatasetGroupDatasets).FirstOrDefaultAsync(a => a.Id == id);
        }

        public void Update(DatasetGroup datasetGroup)
        {
            _context.DatasetGroups.Update(datasetGroup);
        }

        public void Remove(DatasetGroup datasetGroup)
        {
            _context.DatasetGroups.Remove(datasetGroup);
        }
    }
}
