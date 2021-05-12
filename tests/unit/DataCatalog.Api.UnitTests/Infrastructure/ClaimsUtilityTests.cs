using DataCatalog.Api.Infrastructure;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using Xunit;

namespace DataCatalog.Api.UnitTests.Infrastructure
{
    public class ClaimsUtilityTests
    {
        [Fact]
        public void GetClaim_NullUser_ShouldThrowException()
        {
            // ARRANGE
            Action action = () => ClaimsUtility.GetClaim(null, ClaimsUtility.ClaimTenantId);

            // ACT
            action.Should().Throw<Exception>();
        }

        [Fact]
        public void GetClaim_MissingClaim_ShouldReturnNull()
        {
            // ARRANGE
            var tenantId = Guid.NewGuid().ToString();
            var userIdentity = Guid.NewGuid().ToString();
            Thread.CurrentPrincipal = new TestPrincipal(new List<Claim>
            {
                new Claim(ClaimsUtility.ClaimTenantId, tenantId),
                new Claim(ClaimsUtility.ClaimUserIdentity, userIdentity)
            });
            var invalidClaimType = Guid.NewGuid().ToString();

            // ACT
            var claim = ClaimsUtility.GetClaim(Thread.CurrentPrincipal as ClaimsPrincipal, invalidClaimType);

            // ASSERT
            claim.Should().BeNull();
        }

        [Fact]
        public void GetClaim_ValidClaimType_ShouldReturnClaim()
        {
            // ARRANGE
            var tenantId = Guid.NewGuid().ToString();
            var userIdentity = Guid.NewGuid().ToString();
            Thread.CurrentPrincipal = new TestPrincipal(new List<Claim>
            {
                new Claim(ClaimsUtility.ClaimTenantId, tenantId),
                new Claim(ClaimsUtility.ClaimUserIdentity, userIdentity)
            });

            // ACT
            var claim = ClaimsUtility.GetClaim(Thread.CurrentPrincipal as ClaimsPrincipal, ClaimsUtility.ClaimTenantId);

            // ASSERT
            claim.Should().NotBeNull();
            claim.Should().Be(tenantId, "because we asked for that specific claim type");
        }

        [Fact]
        public void GetClaims_NullUser_ShouldThrowException()
        {
            // ARRANGE
            Action action = () => ClaimsUtility.GetClaims(null, ClaimsUtility.ClaimTenantId);

            // ACT
            action.Should().Throw<Exception>();
        }

        
        [Fact]
        public void GetClaims_MissingClaim_ShouldReturnEmptyList()
        {
            // ARRANGE
            var tenantId = Guid.NewGuid().ToString();
            var userIdentity = Guid.NewGuid().ToString();
            Thread.CurrentPrincipal = new TestPrincipal(new List<Claim>
            {
                new Claim(ClaimsUtility.ClaimTenantId, tenantId),
                new Claim(ClaimsUtility.ClaimUserIdentity, userIdentity)
            });
            var invalidClaimType = Guid.NewGuid().ToString();

            // ACT
            var claims = ClaimsUtility.GetClaims(Thread.CurrentPrincipal as ClaimsPrincipal, invalidClaimType);

            // ASSERT
            var claimsArray = claims as string[] ?? claims.ToArray();
            claimsArray.Should().NotBeNull();
            claimsArray.Count().Should().Be(0, "because we asked for a non-existing claim type");
        }

        [Fact]
        public void GetClaims_ValidClaimType_ShouldReturnList()
        {
            // ARRANGE
            var tenantId = Guid.NewGuid().ToString();
            var userIdentity = Guid.NewGuid().ToString();
            Thread.CurrentPrincipal = new TestPrincipal(new List<Claim>
            {
                new Claim(ClaimsUtility.ClaimTenantId, tenantId),
                new Claim(ClaimsUtility.ClaimUserIdentity, userIdentity)
            });

            // ACT
            var claims = ClaimsUtility.GetClaims(Thread.CurrentPrincipal as ClaimsPrincipal, ClaimsUtility.ClaimTenantId);

            // ASSERT
            var claimsArray = claims as string[] ?? claims.ToArray();
            claimsArray.Should().NotBeNull();
            claimsArray.Count().Should().Be(1, "because we have one claim of that type");
            claimsArray.First().Should().Be(tenantId, "because that is the value of the requested claim");
        }
    }
}
