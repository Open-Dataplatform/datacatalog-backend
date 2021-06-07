using DataCatalog.Common.Enums;
using DataCatalog.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            var services = context.HttpContext.RequestServices;
            var authService = services.GetService<IDataCatalogAuthorizationService>();
            var authenticated = authService.IsUserAuthenticated(executingUser);
            if (!authenticated)
            {
                context.Result = new StatusCodeResult((int)System.Net.HttpStatusCode.Unauthorized);
                return;
            }

            // Lookup IdP in list of accepted IdP's and return if not found
            var identityProvider = await authService.GetIdentityProviderForUser(executingUser);
            if (identityProvider == null)
            {
                context.Result = new StatusCodeResult((int)System.Net.HttpStatusCode.Unauthorized);
                return;
            }
            
            // Ensure user is authorized access with the given roles
            var authorized = await authService.IsUserAuthorized(executingUser, identityProvider, _allowedRoles);
            if (!authorized)
            {
                context.Result = new StatusCodeResult((int)System.Net.HttpStatusCode.Forbidden);
            }
        }
    }
}
