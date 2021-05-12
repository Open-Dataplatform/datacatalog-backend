using DataCatalog.Api.Data.Common;
using DataCatalog.Api.Data.Domain;
using DataCatalog.Api.Enums;
using DataCatalog.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DataCatalog.Api.Infrastructure
{
    public class AuthorizeRolesAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        readonly List<Role> _allowedRoles = null;

        public AuthorizeRolesAttribute(params Role[] allowedRoles) : base()
        {
            _allowedRoles = allowedRoles.ToList();
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // Get calling user and return if user do not carry an authentication
            var executingUser = context.HttpContext.User;
            if (!executingUser.Identity.IsAuthenticated)
            {
                context.Result = new StatusCodeResult((int)System.Net.HttpStatusCode.Unauthorized);
                return;
            }

            // Get claims for IdP and user id
            var tenantId = ClaimsUtility.GetClaim(executingUser, ClaimsUtility.ClaimTenantId);
            var externalId = ClaimsUtility.GetClaim(executingUser, ClaimsUtility.ClaimUserIdentity);

            // Lookup IdP in list of accepted IdP's and return if not found
            var services = context.HttpContext.RequestServices;
            var identityProviderService = services.GetService<IIdentityProviderService>();
            var identityProvider = await identityProviderService.FindByTenantIdAsync(tenantId);
            if (identityProvider == null)
            {
                context.Result = new StatusCodeResult((int)System.Net.HttpStatusCode.Unauthorized);
                return;
            }

            // Create or lookup this user
            var memberService = services.GetService<IMemberService>();
            var member = await memberService.GetOrCreateAsync(externalId, identityProvider.Id);
            
            // Initialize the Current object
            var settings = services.GetService<AzureAd>();
            InitializeCurrentObject(services, executingUser, settings, member);

            // Check user roles against endpoint requested roles
            if ((_allowedRoles.Contains(Role.Admin) && executingUser.IsInRole(settings.Roles.Admin)) || 
                (_allowedRoles.Contains(Role.DataSteward) && executingUser.IsInRole(settings.Roles.DataSteward)) ||
                (_allowedRoles.Contains(Role.User) && executingUser.IsInRole(settings.Roles.User)))
                return;

            context.Result = new StatusCodeResult((int)System.Net.HttpStatusCode.Forbidden);
        }

        private void InitializeCurrentObject(IServiceProvider services, ClaimsPrincipal executingUser, AzureAd settings, Member member)
        {
            var current = services.GetService<Current>();
            current.MemberId = member.Id;
            current.Name = ClaimsUtility.GetClaim(executingUser, ClaimsUtility.ClaimName);
            current.Email = executingUser.Identity.Name;
            if (executingUser.IsInRole(settings.Roles.Admin))
                current.Roles.Add(Role.Admin);
            if (executingUser.IsInRole(settings.Roles.DataSteward))
                current.Roles.Add(Role.DataSteward);
            if (executingUser.IsInRole(settings.Roles.User))
                current.Roles.Add(Role.User);
        }
    }
}
