using System;
using System.Linq;
using System.Threading.Tasks;
using DataCatalog.Common.Utils;
using Microsoft.AspNetCore.Authorization;

namespace DataCatalog.Api.Services.Local
{
    /// <summary>
    /// Ignore all security requirements such as no anonymous user access.
    /// Can ONLY be used in the local environment context.
    /// </summary>
    public class AllowAnonymousAuthorizationHandler : IAuthorizationHandler
    {
        public AllowAnonymousAuthorizationHandler()
        {
            if (!EnvironmentUtil.IsDevelopment())
            {
                throw new InvalidOperationException("This class cannot be used unless the environment is local");
            }
        }
        
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            foreach (var requirement in context.PendingRequirements.ToList())
                context.Succeed(requirement);
        
            return Task.CompletedTask;
        }
    }
}