using System.Collections.Generic;
using System.Security.Claims;

namespace DataCatalog.Api.UnitTests.Infrastructure
{
    public class TestPrincipal : ClaimsPrincipal
    {
        public TestPrincipal(List<Claim> claims) : base(new TestIdentity(claims))
        { }

        public TestPrincipal(string authenticationType, List<Claim> claims) : base(new TestIdentity(authenticationType, claims))
        { }
    }

    public class TestIdentity : ClaimsIdentity
    {
        public TestIdentity(List<Claim> claims) : base(claims)
        { }

        public TestIdentity(string authenticationType, List<Claim> claims) : base(claims, authenticationType)
        { }
    }
}
