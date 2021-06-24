using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using DataCatalog.Api.Data.Domain;
using DataCatalog.Api.Infrastructure;
using DataCatalog.Common.Data;
using DataCatalog.Common.Enums;
using DataCatalog.Common.Utils;
using Microsoft.AspNetCore.Http;

namespace DataCatalog.Api.Services.Local
{
    /// <summary>
    /// If running in a local environment, we ignore security concerns, and give the user every role
    /// </summary>
    public class LocalCurrentUserInitializationMiddleware
    {
        private readonly RequestDelegate _next;

        public LocalCurrentUserInitializationMiddleware(RequestDelegate next)
        {
            if (!EnvironmentUtil.IsLocal())
            {
                throw new InvalidOperationException("This class cannot be used unless the environment is local");
            }

            _next = next;
        }

        // This will initialize the Current object and add all roles to the HttpContext in order to make these available in controllers
        public async Task InvokeAsync(HttpContext context, Current current)
        {
            var executingUser = context.User;
            var claimsIdentity = new ClaimsIdentity(new List<Claim>
            {
                new(ClaimsUtility.ClaimName, "LocalTester"),
                new(ClaimTypes.Role, Role.Admin.ToString()),
                new(ClaimTypes.Role, Role.User.ToString()),
                new(ClaimTypes.Role, Role.DataSteward.ToString()),
                new(ClaimsUtility.ClaimUserIdentity, Guid.NewGuid().ToString())
            });
            if (ClaimsUtility.GetClaim(executingUser, ClaimsUtility.ClaimName) == null)
            {
                executingUser.AddIdentity(claimsIdentity);
            }

            current.MemberId = Guid.NewGuid();
            current.Name = "LocalTester";
            
            if (executingUser.Identity != null)
            {
                current.Email = executingUser.Identity.Name;
            }

            current.Roles.Add(Role.Admin);
            current.Roles.Add(Role.DataSteward);
            current.Roles.Add(Role.User);
            
            await _next(context);
        }
    }
}