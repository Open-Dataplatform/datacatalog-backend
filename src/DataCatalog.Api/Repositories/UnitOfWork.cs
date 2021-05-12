using DataCatalog.Api.Data;
using System.Threading.Tasks;

namespace DataCatalog.Api.Repositories
{
    public class UnitOfWork : IUnitIOfWork
    {
        protected readonly DataCatalogContext _context;

        public UnitOfWork(DataCatalogContext context)
        {
            _context = context;
        }

        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
