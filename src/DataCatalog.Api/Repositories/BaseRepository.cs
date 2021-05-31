
using DataCatalog.Common.Data;
using DataCatalog.Data;

namespace DataCatalog.Api.Repositories
{
    public abstract class BaseRepository
    {
        protected readonly DataCatalogContext _context;
        protected readonly Current _current;

        protected BaseRepository(DataCatalogContext context, Current current)
        {
            _context = context;
            _current = current;
        }

        protected BaseRepository(DataCatalogContext context)
        {
            _context = context;
        }
    }
}
