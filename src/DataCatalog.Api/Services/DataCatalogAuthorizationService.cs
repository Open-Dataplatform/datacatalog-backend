using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using DataCatalog.Api.Data.Domain;
using DataCatalog.Api.Infrastructure;
using DataCatalog.Common.Data;
using DataCatalog.Common.Enums;

namespace DataCatalog.Api.Services
{
    public class DataCatalogAuthorizationService : IDataCatalogAuthorizationService
    {
        private readonly IMemberService _memberService;
        private readonly Roles _roles;
        private readonly Current _current;
        private readonly IIdentityProviderService _identityProviderService;

        public DataCatalogAuthorizationService(IMemberService memberService, Roles roles, Current current, IIdentityProviderService identityProviderService)
        {
            _memberService = memberService;
            _roles = roles;
            _current = current;
            _identityProviderService = identityProviderService;
        }

        public bool IsUserAuthenticated(ClaimsPrincipal executingUser)
        {
            return executingUser.Identity is {IsAuthenticated: true};
        }

        public async Task<IdentityProvider> GetIdentityProviderForUser(ClaimsPrincipal executingUser)
        {
            var tenantId = ClaimsUtility.GetClaim(executingUser, ClaimsUtility.ClaimTenantId);
            
            return await _identityProviderService.FindByTenantIdAsync(tenantId);
        }

        public async Task<bool> IsUserAuthorized(ClaimsPrincipal executingUser, IdentityProvider identityProvider, List<Role> allowedRoles)
        {
            // Get claims for IdP and user id
            var externalId = ClaimsUtility.GetClaim(executingUser, ClaimsUtility.ClaimUserIdentity);

            // Create or lookup this user
            var member = await _memberService.GetOrCreateAsync(externalId, identityProvider.Id);

            // Initialize the Current object
            InitializeCurrentObject(executingUser, _roles, member);

            // Check user roles against endpoint requested roles
            return allowedRoles.Contains(Role.Admin) && executingUser.IsInRole(_roles.Admin) ||
                   allowedRoles.Contains(Role.DataSteward) && executingUser.IsInRole(_roles.DataSteward) ||
                   allowedRoles.Contains(Role.User) && executingUser.IsInRole(_roles.User);
        }
        
        private void InitializeCurrentObject(ClaimsPrincipal executingUser, Roles settings, Member member)
        {
            _current.MemberId = member.Id;
            _current.Name = ClaimsUtility.GetClaim(executingUser, ClaimsUtility.ClaimName);
            if (executingUser.Identity != null)
            {
                _current.Email = executingUser.Identity.Name;
            }

            if (executingUser.IsInRole(settings.Admin))
                _current.Roles.Add(Role.Admin);
            if (executingUser.IsInRole(settings.DataSteward))
                _current.Roles.Add(Role.DataSteward);
            if (executingUser.IsInRole(settings.User))
                _current.Roles.Add(Role.User);
        }
    }
}