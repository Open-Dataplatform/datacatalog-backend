using DataCatalog.Api.Data;
using DataCatalog.Api.Data.Model;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DataCatalog.Api.Repositories
{
    public class IdentityProviderRepository : BaseRepository, IIdentityProviderRepository
    {
        public IdentityProviderRepository(DataCatalogContext context) : base(context)
        { }

        public async Task<IdentityProvider> FindByTenantIdAsync(string tenantId)
        {
            return await _context.IdentityProvider.FirstOrDefaultAsync(a => a.TenantId == tenantId);
        }
    }
}
