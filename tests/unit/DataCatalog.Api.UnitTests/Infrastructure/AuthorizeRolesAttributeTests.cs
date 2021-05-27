using DataCatalog.Api.Data.Common;
using DataCatalog.Api.Enums;
using DataCatalog.Api.Infrastructure;
using DataCatalog.Api.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using DataCatalog.Api.Data.Domain;
using Xunit;
using Roles = DataCatalog.Api.Infrastructure.Roles;

namespace DataCatalog.Api.UnitTests.Infrastructure
{
    public class AuthorizeRolesAttributeTests
    {
        private TestPrincipal CreateUser(Roles settings, string tenantId, string userId, bool isAuthenticated, List<Role> roles)
        {
            var claims = new List<Claim>
            { 
                new Claim(ClaimsUtility.ClaimTenantId, tenantId), 
                new Claim(ClaimsUtility.ClaimUserIdentity, userId)
            };
            foreach (var role in roles)
            {
                if (role == Role.Admin)
                    claims.Add(new Claim(ClaimTypes.Role, settings.Admin));
                if (role == Role.DataSteward)
                    claims.Add(new Claim(ClaimTypes.Role, settings.DataSteward));
                if (role == Role.User)
                    claims.Add(new Claim(ClaimTypes.Role, settings.User));
            }

            return isAuthenticated ?
                new TestPrincipal("bearer_from_azure_ad", claims) :
                new TestPrincipal(claims);
        }

        private Mock<IServiceProvider> CreateMockedServiceProvider(Mock<IIdentityProviderService> identityProviderServiceMock, Mock<IMemberService> memberServiceMock, Roles settings, Current current)
        {
            var serviceProviderMock = new Mock<IServiceProvider>();

            if (identityProviderServiceMock != null)
                serviceProviderMock
                    .Setup(provider => provider.GetService(typeof(IIdentityProviderService)))
                    .Returns(identityProviderServiceMock.Object);

            if (memberServiceMock != null)
                serviceProviderMock
                    .Setup(provider => provider.GetService(typeof(IMemberService)))
                .Returns(memberServiceMock.Object);

            if (settings != null)
                serviceProviderMock
                    .Setup(provider => provider.GetService(typeof(Roles)))
                .Returns(settings);

            if (current != null)
                serviceProviderMock
                    .Setup(provider => provider.GetService(typeof(Current)))
                .Returns(current);

            return serviceProviderMock;
        }

        private AuthorizationFilterContext CreateAuthorizationFilterContext(Mock<HttpContext> httpContextMock)
        {
            ActionContext fakeActionContext =
                new ActionContext(httpContextMock.Object,
                                  new Microsoft.AspNetCore.Routing.RouteData(),
                                  new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());

            return new AuthorizationFilterContext(fakeActionContext, new List<IFilterMetadata>());
        }

        [Fact]
        public void NullContext_ShouldThrowException()
        {
            // ARRANGE
            var attribute = new AuthorizeRolesAttribute(Role.Admin);
            Func<Task> f = async () => await attribute.OnAuthorizationAsync(null);

            // ACT
            f.Should().Throw<Exception>();

            // ASSERT
        }

        [Fact]
        public async Task UnauthorizedUser_ShouldReturnUnauthorized()
        {
            // ARRANGE
            var tenantId = Guid.NewGuid().ToString();
            var httpContextMock = new Mock<HttpContext>();
            httpContextMock
                .Setup(a => a.User)
                .Returns(CreateUser(null, tenantId, Guid.NewGuid().ToString(), false, new List<Role>()));
            var fakeAuthFilterContext = CreateAuthorizationFilterContext(httpContextMock);

            // ACT
            var attribute = new AuthorizeRolesAttribute();
            await attribute.OnAuthorizationAsync(fakeAuthFilterContext);
            var result = ((StatusCodeResult)fakeAuthFilterContext.Result);

            // ASSERT
            result.StatusCode.Should().Be(StatusCodes.Status401Unauthorized, "because the caller is not authorized");
        }

        [Fact]
        public async Task Unknown_Tenant_ShouldReturnForbidden()
        {
            // ARRANGE
            var tenantId = Guid.NewGuid().ToString();
            var invalidTenantId = Guid.NewGuid().ToString();
            var identityProviderServiceMock = new Mock<IIdentityProviderService>();
            identityProviderServiceMock
                .Setup(a => a.FindByTenantIdAsync(invalidTenantId))
                .Returns(Task.FromResult((IdentityProvider) null));
            var serviceProviderMock = CreateMockedServiceProvider(identityProviderServiceMock, null, null, null);
            var httpContextMock = new Mock<HttpContext>();
            httpContextMock
                .Setup(a => a.User)
                .Returns(CreateUser(null, tenantId, Guid.NewGuid().ToString(), true, new List<Role>()));
            httpContextMock.SetupGet(context => context.RequestServices)
                .Returns(serviceProviderMock.Object);
            var fakeAuthFilterContext = CreateAuthorizationFilterContext(httpContextMock);

            // ACT
            var attribute = new AuthorizeRolesAttribute(Role.Admin, Role.DataSteward);
            await attribute.OnAuthorizationAsync(fakeAuthFilterContext);
            var result = ((StatusCodeResult)fakeAuthFilterContext.Result);

            // ASSERT
            result.StatusCode.Should().Be(StatusCodes.Status401Unauthorized, "because the tenant id is not registered as valid");
        }

        [Fact]
        public async Task ValidTenantAndUser_MissingRole_ShouldReturnForbidden()
        {
            // ARRANGE
            var tenantId = Guid.NewGuid().ToString();
            var identityProviderServiceMock = new Mock<IIdentityProviderService>();
            var identityProvider = new IdentityProvider { Id = Guid.NewGuid(), TenantId = tenantId };
            identityProviderServiceMock
                .Setup(a => a.FindByTenantIdAsync(tenantId))
                .Returns(Task.FromResult(identityProvider));
            var memberServiceMock = new Mock<IMemberService>();
            var member = new Member { Id = Guid.NewGuid(), IdentityProviderId = identityProvider.Id, ExternalId = Guid.NewGuid().ToString() };
            memberServiceMock
                .Setup(a => a.GetOrCreateAsync(member.ExternalId, identityProvider.Id))
                .Returns(Task.FromResult(member));
            Roles settings = new Roles
            {
                Admin = Guid.NewGuid().ToString(),
                DataSteward = Guid.NewGuid().ToString(),
                User = Guid.NewGuid().ToString()
            };
            var current = new Current
            {
                Roles = new List<Role>()
            };
            var serviceProviderMock = CreateMockedServiceProvider(identityProviderServiceMock, memberServiceMock, settings, current);
            var httpContextMock = new Mock<HttpContext>();
            httpContextMock
                .Setup(a => a.User)
                .Returns(CreateUser(settings, tenantId, member.ExternalId, true, new List<Role> { Role.User }));
            httpContextMock.SetupGet(context => context.RequestServices)
                .Returns(serviceProviderMock.Object);
            var fakeAuthFilterContext = CreateAuthorizationFilterContext(httpContextMock);

            // ACT
            var attribute = new AuthorizeRolesAttribute(Role.Admin, Role.DataSteward);
            await attribute.OnAuthorizationAsync(fakeAuthFilterContext);

            // ASSERT
            var result = ((StatusCodeResult)fakeAuthFilterContext.Result);
            result.StatusCode.Should().Be(StatusCodes.Status403Forbidden, "because the required role is not present in user claim");
        }

        [Fact]
        public async Task ValidTenantAndUser_HaveRole_ShouldReturn()
        {
            // ARRANGE
            var tenantId = Guid.NewGuid().ToString();
            var identityProviderServiceMock = new Mock<IIdentityProviderService>();
            var identityProvider = new IdentityProvider { Id = Guid.NewGuid(), TenantId = tenantId };
            identityProviderServiceMock
                .Setup(a => a.FindByTenantIdAsync(tenantId))
                .Returns(Task.FromResult(identityProvider));
            var memberServiceMock = new Mock<IMemberService>();
            var member = new Member { Id = Guid.NewGuid(), IdentityProviderId = identityProvider.Id, ExternalId = Guid.NewGuid().ToString() };
            memberServiceMock
                .Setup(a => a.GetOrCreateAsync(member.ExternalId, identityProvider.Id))
                .Returns(Task.FromResult(member));
            var settings = new Roles
            {
                Admin = Guid.NewGuid().ToString(),
                DataSteward = Guid.NewGuid().ToString(),
                User = Guid.NewGuid().ToString()
            };
            var current = new Current
            {
                Roles = new List<Role>()
            };
            var serviceProviderMock = CreateMockedServiceProvider(identityProviderServiceMock, memberServiceMock, settings, current);
            var httpContextMock = new Mock<HttpContext>();
            httpContextMock
                .Setup(a => a.User)
                .Returns(CreateUser(settings, tenantId, member.ExternalId, true, new List<Role> { Role.Admin, Role.DataSteward }));
            httpContextMock.SetupGet(context => context.RequestServices)
                .Returns(serviceProviderMock.Object);
            var fakeAuthFilterContext = CreateAuthorizationFilterContext(httpContextMock);

            // ACT
            var attribute = new AuthorizeRolesAttribute(Role.Admin, Role.DataSteward);
            await attribute.OnAuthorizationAsync(fakeAuthFilterContext);
            var result = ((StatusCodeResult)fakeAuthFilterContext.Result);

            // ASSERT
            result.Should().BeNull("because the attribute do not set the Result upon success");
        }
    }
}
