using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace DataCatalog.Api.Infrastructure
{
    public class ClaimsUtility
    {
        public static string ClaimName => "name";
        public static string ClaimTenantId => "http://schemas.microsoft.com/identity/claims/tenantid";
        public static string ClaimUserIdentity => "http://schemas.microsoft.com/identity/claims/objectidentifier";

        public static string GetClaim(ClaimsPrincipal user, string claimType)
        {
            return user.Claims.ToList().SingleOrDefault(c => c.Type == claimType)?.Value;
        }

        public static IEnumerable<string> GetClaims(ClaimsPrincipal user, string claimType)
        {
            return user.Claims.ToList().Where(c => c.Type == claimType).Select(claim => claim.Value);
        }
    }
}
