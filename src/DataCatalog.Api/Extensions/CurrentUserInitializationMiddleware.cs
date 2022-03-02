using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DataCatalog.Api.Data.Domain;
using DataCatalog.Api.Infrastructure;
using DataCatalog.Api.Services;
using DataCatalog.Common.Data;
using DataCatalog.Common.Enums;
using Microsoft.AspNetCore.Http;

namespace DataCatalog.Api.Extensions
{
    /// <summary>
    /// This middleware will initialize the scoped 'Current' object and create a Member the first time a user logs on
    /// </summary>
    public class CurrentUserInitializationMiddleware
    {
        private readonly RequestDelegate _next;

        public CurrentUserInitializationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // current, memberService and identityProviderService is populated by DI
        public async Task InvokeAsync(HttpContext context, Current current, IMemberService memberService, IIdentityProviderService identityProviderService)
        {
            var executingUser = context.User;

            if (!executingUser.Claims.Any())
            {
                // Call must have been made by an allowAnonymous call such as /health
                await _next(context);
                return;
            }
            var identityProvider = await GetIdentityProviderForUser(executingUser, identityProviderService);

            // Get claims for IdP and user id
            var externalId = ClaimsUtility.GetClaim(executingUser, ClaimsUtility.ClaimUserIdentity);

            // Create or lookup this user
            var member = await memberService.GetOrCreateAsync(externalId, identityProvider.Id);

            InitializeCurrentObject(executingUser, member, current);
            
            await _next(context);
        }

        private async Task<IdentityProvider> GetIdentityProviderForUser(ClaimsPrincipal executingUser, IIdentityProviderService identityProviderService)
        {
            var tenantId = ClaimsUtility.GetClaim(executingUser, ClaimsUtility.ClaimTenantId);
            
            return await identityProviderService.FindByTenantIdAsync(tenantId);
        }

        private void InitializeCurrentObject(ClaimsPrincipal executingUser, Member member, Current _current)
        {
            _current.MemberId = member.Id;
            _current.Name = ClaimsUtility.GetClaim(executingUser, ClaimsUtility.ClaimName);

            if (executingUser.Identity != null)
            {
                _current.Email = executingUser.Identity.Name;
            }
            
            if (executingUser.IsInRole(Role.Admin.ToString()))
                _current.Roles.Add(Role.Admin);
            if (executingUser.IsInRole(Role.DataSteward.ToString()))
                _current.Roles.Add(Role.DataSteward);
            if (executingUser.IsInRole(Role.User.ToString()))
                _current.Roles.Add(Role.User);
            if (executingUser.IsInRole(Role.MetadataProvider.ToString()))
                _current.Roles.Add(Role.MetadataProvider);
            if (executingUser.IsInRole(Role.ServiceReader.ToString()))
                _current.Roles.Add(Role.ServiceReader);
        }
    }
}