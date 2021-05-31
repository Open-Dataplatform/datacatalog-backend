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
    public class CategoryRepository : BaseRepository, ICategoryRepository
    {
        public CategoryRepository(DataCatalogContext context, Current current) : base(context, current)
        { }

        public async Task<IEnumerable<Category>> ListAsync()
        {
            var categories = await _context.Categories.ToListAsync();

            if (_current.Roles.Contains(Role.Admin))
                return categories;

            return categories.Where(c => !c.OriginDeleted).ToList();
        }
        public async Task AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
        }

        public async Task<Category> FindByIdAsync(Guid id)
        {
            var existingCategory = await _context.Categories.FirstOrDefaultAsync(a => a.Id == id);

            if (_current.Roles.Contains(Role.Admin))
                return existingCategory;

            if (existingCategory == null)
                return null;

            return existingCategory.OriginDeleted
                ? null
                : existingCategory;
        }

        public void Update(Category category)
        {
            _context.Categories.Update(category);
        }

        public void Remove(Category category)
        {
            _context.Categories.Remove(category);
        }
    }
}
