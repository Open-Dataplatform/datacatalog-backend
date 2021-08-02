using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using DataCatalog.Api.Infrastructure;
using DataCatalog.Api.Repositories;
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
        // Ids for initializing a local member and identity provider
        private const string LocalTesterExternalId = "aef7e6b6-a461-47c8-94ab-429151114bd7";
        private const string LocalTesterTenantId = "233c4ab6-440a-4288-bac0-e3764d71cd26";
        private const string LocalTesterName = "LocalTester";
        
        private readonly RequestDelegate _next;

        public LocalCurrentUserInitializationMiddleware(RequestDelegate next)
        {
            if (!EnvironmentUtil.IsDevelopment())
            {
                throw new InvalidOperationException("This class cannot be used unless the environment is local");
            }

            _next = next;
        }

        // This will initialize the Current object and add all roles to the HttpContext in order to make these available in controllers
        public async Task InvokeAsync(HttpContext context, Current current, IMemberService memberService, IIdentityProviderService identityProviderService, IMemberRepository memberRepository, IUnitOfWork unitOfWork)
        {
            var executingUser = context.User;

            AddIdentityToExecutingUser(executingUser);
            var identityProvider = await identityProviderService.FindByTenantIdAsync(LocalTesterTenantId);

            // If identityProvider is null then this is the first time running locally and we should create the local IdentityProvider and Member
            // Otherwise just use get the member from the database
            var memberId = identityProvider switch
            {
                null => (await CreateLocalMemberAndIdentityProvider(memberRepository, unitOfWork)).Id,
                _ => (await memberService.GetOrCreateAsync(LocalTesterExternalId, identityProvider.Id)).Id
            };

            InitializeCurrentObject(executingUser, memberId, current);

            await _next(context);
        }

        /// <summary>
        /// Add claims to the executing user even though they are not present in the token. Only for local environment
        /// </summary>
        private void AddIdentityToExecutingUser(ClaimsPrincipal executingUser)
        {
            var claimsIdentity = new ClaimsIdentity(new List<Claim>
            {
                new(ClaimsUtility.ClaimName, LocalTesterName),
                new(ClaimTypes.Role, Role.Admin.ToString()),
                new(ClaimTypes.Role, Role.User.ToString()),
                new(ClaimTypes.Role, Role.DataSteward.ToString()),
                new(ClaimsUtility.ClaimUserIdentity, LocalTesterExternalId)
            });

            if (ClaimsUtility.GetClaim(executingUser, ClaimsUtility.ClaimName) == null)
            {
                executingUser.AddIdentity(claimsIdentity);
            }
        }

        private async Task<DataCatalog.Data.Model.Member> CreateLocalMemberAndIdentityProvider(IMemberRepository memberRepository, IUnitOfWork unitOfWork)
        {
            var member = new DataCatalog.Data.Model.Member
            {
                ExternalId = LocalTesterExternalId,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                IdentityProvider = new DataCatalog.Data.Model.IdentityProvider
                {
                    Name = "LocalIdentityProvider",
                    TenantId = LocalTesterTenantId
                }
            };

            await memberRepository.AddAsync(member);
            await unitOfWork.CompleteAsync();

            return member;
        }

        private void InitializeCurrentObject(ClaimsPrincipal executingUser, Guid memberId, Current current)
        {

            current.MemberId = memberId;
            current.Name = LocalTesterName;
            
            if (executingUser.Identity != null)
            {
                current.Email = executingUser.Identity.Name;
            }

            current.Roles.Add(Role.Admin);
            current.Roles.Add(Role.DataSteward);
            current.Roles.Add(Role.User);
        }
    }
}