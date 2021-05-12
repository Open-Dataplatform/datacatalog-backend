using DataCatalog.Api.Data.Model;
using System.Threading.Tasks;

namespace DataCatalog.Api.Repositories
{
    public interface IIdentityProviderRepository
    {
        Task<IdentityProvider> FindByTenantIdAsync(string id);
    }
}
