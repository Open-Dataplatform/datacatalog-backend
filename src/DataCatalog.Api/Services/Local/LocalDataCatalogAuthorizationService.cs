using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using DataCatalog.Api.Data.Domain;
using DataCatalog.Api.Infrastructure;
using DataCatalog.Common.Data;
using DataCatalog.Common.Enums;
using DataCatalog.Common.Utils;
using Roles = DataCatalog.Api.Infrastructure.Roles;

namespace DataCatalog.Api.Services.Local
{
    /// <summary>
    /// If running in a local environment, we ignore security concerns, and give the user every role
    /// </summary>
    public class LocalDataCatalogAuthorizationService : IDataCatalogAuthorizationService
    {
        private readonly IMemberService _memberService;
        private readonly Roles _roles;
        private readonly Current _current;

        public LocalDataCatalogAuthorizationService(IMemberService memberService, Roles roles, Current current)
        {
            if (!EnvironmentUtil.IsLocal())
            {
                throw new InvalidOperationException("This class cannot be used unless the environment is local");
            }
            _memberService = memberService;
            _roles = roles;
            _current = current;
        }

        public bool IsUserAuthenticated(ClaimsPrincipal executingUser)
        {
            // Locally we are always authenticated
            return true;
        }

        public Task<IdentityProvider> GetIdentityProviderForUser(ClaimsPrincipal executingUser)
        {
            // Locally we never use this provider for anything, so an empty object is fine.
            return Task.FromResult(new IdentityProvider());
        }

        public async Task<bool> IsUserAuthorized(ClaimsPrincipal executingUser, IdentityProvider identityProvider, List<Role> allowedRoles)
        {
            var member = await _memberService.GetOrCreateAsync(Guid.NewGuid().ToString(), Guid.Parse("75030760-f7f8-40d8-a1ab-718bcb7327b7"));

            // Initialize the Current object
            var claimsIdentity = new ClaimsIdentity(new List<Claim>
            {
                new(ClaimsUtility.ClaimName, "LocalTester"),
                new(ClaimTypes.Role, _roles.Admin),
                new(ClaimTypes.Role, _roles.User),
                new(ClaimTypes.Role, _roles.DataSteward),
                new(ClaimsUtility.ClaimUserIdentity, Guid.NewGuid().ToString())
            });
            if (ClaimsUtility.GetClaim(executingUser, ClaimsUtility.ClaimName) == null)
            {
                executingUser.AddIdentity(claimsIdentity);
            }
            InitializeCurrentObject(executingUser, _roles, member);
            return true;
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