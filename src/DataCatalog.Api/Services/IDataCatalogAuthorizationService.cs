using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using DataCatalog.Api.Data.Domain;
using DataCatalog.Common.Enums;

namespace DataCatalog.Api.Services
{
    public interface IDataCatalogAuthorizationService
    {
        public bool IsUserAuthenticated(ClaimsPrincipal executingUser);
        public Task<IdentityProvider> GetIdentityProviderForUser(ClaimsPrincipal executingUser);
        public Task<bool> IsUserAuthorized(ClaimsPrincipal executingUser, IdentityProvider identityProvider, List<Role> allowedRoles);
    }
}