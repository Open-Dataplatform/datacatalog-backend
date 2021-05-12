using System.Threading.Tasks;

namespace DataCatalog.Api.Services
{
    public interface IIdentityProviderService
    {
        Task<Data.Domain.IdentityProvider> FindByTenantIdAsync(string id);
    }
}
