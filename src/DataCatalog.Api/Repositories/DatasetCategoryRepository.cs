using DataCatalog.Data;
using DataCatalog.Data.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataCatalog.Api.Repositories
{
    public class DatasetCategoryRepository : BaseRepository, IDatasetCategoryRepository
    {
        public DatasetCategoryRepository(DataCatalogContext context) : base(context)
        { }

        public async Task<IEnumerable<DatasetCategory>> ListAsync()
        {
            return await _context.DatasetCategories.ToListAsync();
        }

        public async Task<IEnumerable<DatasetCategory>> ListAsync(Guid categoryId)
        {
            return await _context.DatasetCategories.Where(d => d.CategoryId == categoryId).ToListAsync();
        }
    }
}
