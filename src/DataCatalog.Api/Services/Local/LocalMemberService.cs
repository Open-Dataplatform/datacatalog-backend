using System;
using System.Threading.Tasks;
using DataCatalog.Api.Data.Domain;
using DataCatalog.Common.Enums;
using DataCatalog.Common.Utils;

namespace DataCatalog.Api.Services.Local
{
    /// <summary>
    /// Dummy member service implementation used ONLY for the local environment runtime.
    /// DO NOT use this in any other context! 
    /// </summary>
    public class LocalMemberService : IMemberService
    {
        public Task<Member> GetOrCreateAsync(string externalId, Guid identityProviderId)
        {
            if (!EnvironmentUtil.IsLocal())
            {
                throw new InvalidOperationException("This class cannot be used unless the environment is local");
            }
            return Task.FromResult(new Member
            {
                CreatedDate = DateTime.UtcNow,
                Id = Guid.Empty,
                ExternalId = "ExternalId",
                MemberRole = Role.Admin,
                IdentityProviderId = Guid.Empty
            });
        }
    }
}