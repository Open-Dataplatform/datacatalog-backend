
using DataCatalog.Data;
using DataCatalog.Data.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataCatalog.Api.Repositories
{
    public class HierarchyRepository : BaseRepository, IHierarchyRepository
    {
        public HierarchyRepository(DataCatalogContext context) : base(context)
        { }

        public async Task<IEnumerable<Hierarchy>> ListAsync()
        {
            return await _context.Hierarchies.Where(h => h.ParentHierarchyId == null).Include(a => a.ChildHierarchies).ToListAsync();
        }
        public async Task AddAsync(Hierarchy hierarchy)
        {
            await _context.Hierarchies.AddAsync(hierarchy);
        }

        public async Task<Hierarchy> FindByIdAsync(Guid id)
        {
            return await _context.Hierarchies.Include(a => a.ParentHierarchy).Include(a => a.ChildHierarchies).SingleOrDefaultAsync(a => a.Id == id);
        }

        public void Update(Hierarchy hierarchy)
        {
            _context.Hierarchies.Update(hierarchy);
        }

        public void Remove(Hierarchy hierarchy)
        {
            _context.Hierarchies.Remove(hierarchy);
        }
    }
}
