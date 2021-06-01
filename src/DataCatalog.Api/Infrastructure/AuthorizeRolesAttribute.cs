using DataCatalog.Common.Data;
using DataCatalog.Api.Data.Domain;
using DataCatalog.Common.Enums;
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
using DataCatalog.Common.Utils;

namespace DataCatalog.Api.Infrastructure
{
    public class AuthorizeRolesAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        private readonly List<Role> _allowedRoles;

        public AuthorizeRolesAttribute(params Role[] allowedRoles)
        {
            _allowedRoles = allowedRoles.ToList();
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // Get calling user and return if user do not carry an authentication
            var executingUser = context.HttpContext.User;
            if (EnvironmentUtil.IsLocal())
            {
                await HandleLocalEnvironment(context, executingUser);
                return;
            }

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
            var settings = services.GetService<Roles>();
            InitializeCurrentObject(services, executingUser, settings, member);

            // Check user roles against endpoint requested roles
            if ((_allowedRoles.Contains(Role.Admin) && executingUser.IsInRole(settings.Admin)) || 
                (_allowedRoles.Contains(Role.DataSteward) && executingUser.IsInRole(settings.DataSteward)) ||
                (_allowedRoles.Contains(Role.User) && executingUser.IsInRole(settings.User)))
                return;

            context.Result = new StatusCodeResult((int)System.Net.HttpStatusCode.Forbidden);
        }

        /// <summary>
        /// If running in a local environment, we ignore security concerns, and give the user every role
        /// </summary>
        /// <param name="context">The context of the http request</param>
        /// <param name="executingUser">The principal calling user</param>
        private async Task HandleLocalEnvironment(ActionContext context, ClaimsPrincipal executingUser)
        {
            var services = context.HttpContext.RequestServices;
            var memberService = services.GetService<IMemberService>();
            if (memberService != null)
            {
                var member = await memberService.GetOrCreateAsync(Guid.NewGuid().ToString(), Guid.Parse("75030760-f7f8-40d8-a1ab-718bcb7327b7"));

                // Initialize the Current object
                var settings = services.GetService<Roles>();
                var claimsIdentity = new ClaimsIdentity(new List<Claim>
                {
                    new(ClaimsUtility.ClaimName, "LocalTester"),
                    new(ClaimTypes.Role, settings.Admin),
                    new(ClaimTypes.Role, settings.User),
                    new(ClaimTypes.Role, settings.DataSteward),
                    new(ClaimsUtility.ClaimUserIdentity, Guid.NewGuid().ToString())
                });
                if (ClaimsUtility.GetClaim(executingUser, ClaimsUtility.ClaimName) == null)
                {
                    executingUser.AddIdentity(claimsIdentity);
                }
                InitializeCurrentObject(services, executingUser, settings, member);
            }
        }

        private void InitializeCurrentObject(IServiceProvider services, ClaimsPrincipal executingUser, Roles settings, Member member)
        {
            var current = services.GetService<Current>();
            current.MemberId = member.Id;
            current.Name = ClaimsUtility.GetClaim(executingUser, ClaimsUtility.ClaimName);
            current.Email = executingUser.Identity.Name;
            if (executingUser.IsInRole(settings.Admin))
                current.Roles.Add(Role.Admin);
            if (executingUser.IsInRole(settings.DataSteward))
                current.Roles.Add(Role.DataSteward);
            if (executingUser.IsInRole(settings.User))
                current.Roles.Add(Role.User);
        }
    }
}
